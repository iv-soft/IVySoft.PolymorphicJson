namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Tests for mixed string and integer discriminators
/// </summary>
public partial class MixedDiscriminatorTests
{
    /// <summary>
    /// Test using both string and integer discriminators in same hierarchy
    /// </summary>
    [Fact]
    public void MixedStringIntegerDiscriminatorsTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<MixedContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        var json1 = serializer.Serialize(new StringClass());
        var json2 = serializer.Serialize(new IntClass());

        Assert.Contains("string_type", json1);
        Assert.Contains("1", json2);
    }

    /// <summary>
    /// Test deserialization with mixed discriminator types
    /// </summary>
    [Fact]
    public void MixedDiscriminatorDeserializationTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<MixedContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        var result1 = serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""string_type""}");
        var result2 = serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":1}");

        Assert.IsType<StringClass>(result1);
        Assert.IsType<IntClass>(result2);
    }

    private interface ITest { }
    [JsonTypeId("string_type")]
    private sealed class StringClass : ITest { }
    [JsonTypeId(1)]
    private sealed class IntClass : ITest { }
    [JsonSerializable(typeof(StringClass))]
    [JsonSerializable(typeof(IntClass))]
    private sealed partial class MixedContext : JsonSerializerContext { }
}
