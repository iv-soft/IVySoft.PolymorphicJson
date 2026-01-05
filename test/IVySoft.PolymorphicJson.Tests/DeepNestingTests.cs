namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Tests for deeply nested polymorphic structures
/// </summary>
public partial class DeepNestingTests
{
    /// <summary>
    /// Test serialization of deeply nested polymorphic types
    /// </summary>
    [Fact]
    public void DeepNestingSerializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<DeepNestingContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<INode>>();

        var nested = new NodeA
        {
            Child = new NodeB
            {
                Child = new NodeA
                {
                    Child = new NodeB { Child = null }
                }
            }
        };

        var json = serializer.Serialize(nested);
        Assert.Contains("$type", json);
        Assert.Contains("NodeA", json);
        Assert.Contains("NodeB", json);
    }

    /// <summary>
    /// Test deserialization of deeply nested polymorphic types
    /// </summary>
    [Fact]
    public void DeepNestingDeserializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<DeepNestingContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<INode>>();

        var json = /*lang=json,strict*/ @"{""$type"":""NodeA"",""Child"":{""$type"":""NodeB"",""Child"":{""$type"":""NodeA""}}}";
        var result = serializer.Deserialize(json);

        Assert.IsType<NodeA>(result);
        var nodeA = (NodeA)result;
        Assert.IsType<NodeB>(nodeA.Child);
        var nodeB = (NodeB)nodeA.Child!;
        Assert.IsType<NodeA>(nodeB.Child);
    }

    private interface INode
    {
        public INode? Child { get; set; }
    }
    [JsonTypeId("NodeA")]
    private sealed class NodeA : INode
    {
        public INode? Child { get; set; }
    }
    [JsonTypeId("NodeB")]
    private sealed class NodeB : INode
    {
        public INode? Child { get; set; }
    }
    [JsonSerializable(typeof(NodeA))]
    [JsonSerializable(typeof(NodeB))]
    private sealed partial class DeepNestingContext : JsonSerializerContext { }
}
