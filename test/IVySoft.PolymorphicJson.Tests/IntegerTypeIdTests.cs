namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Tests for integer type discriminator support
/// </summary>
public partial class IntegerTypeIdTests
{
    /// <summary>
    /// Test serialization with integer type discriminators
    /// </summary>
    [Fact]
    public void SerializeWithIntegerDiscriminatorTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<IntegerContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<IIntegerTest>>();

        Assert.Equal(/*lang=json,strict*/ @"{""$type"":1}", serializer.Serialize(new IntegerClass1()));
        Assert.Equal(/*lang=json,strict*/ @"{""$type"":2}", serializer.Serialize(new IntegerClass2()));
    }

    /// <summary>
    /// Test deserialization with integer type discriminators
    /// </summary>
    [Fact]
    public void DeserializeWithIntegerDiscriminatorTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<IntegerContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<IIntegerTest>>();

        Assert.IsType<IntegerClass1>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":1}"));
        Assert.IsType<IntegerClass2>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":2}"));
    }

    private interface IIntegerTest { }
    [JsonTypeId(1)]
    private sealed class IntegerClass1 : IIntegerTest { }
    [JsonTypeId(2)]
    private sealed class IntegerClass2 : IIntegerTest { }
    [JsonSerializable(typeof(IntegerClass1))]
    [JsonSerializable(typeof(IntegerClass2))]
    private sealed partial class IntegerContext : JsonSerializerContext { }
}
