namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Simple json serialize tests
/// </summary>
public partial class SimpleTests
{
    /// <summary>
    /// Test simple serialize
    /// </summary>
    [Fact]
    public void SerializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<SimpleContext1>()
            .AddJsonSerializerContext<SimpleContext2>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class1""}", serializer.Serialize(new Class1()));
        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class2""}", serializer.Serialize(new Class2()));
    }
    /// <summary>
    /// Test simple deserialize
    /// </summary>
    [Fact]
    public void DeserializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<SimpleContext1>()
            .AddJsonSerializerContext<SimpleContext2>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        Assert.IsType<Class1>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class1""}"));
        Assert.IsType<Class2>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class2""}"));
    }
    /// <summary>
    /// Test simple serialize with missed json context
    /// </summary>
    [Fact]
    public void OneContextSerializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<SimpleContext1>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class1""}", serializer.Serialize(new Class1()));
        Assert.Throws<NotSupportedException>(() => serializer.Serialize(new Class2()));
    }
    /// <summary>
    /// Test simple deserialize with missed json context
    /// </summary>
    [Fact]
    public void OneContextDeserializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<SimpleContext1>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        Assert.IsType<Class1>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class1""}"));
        Assert.Throws<System.Text.Json.JsonException>(() => serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class2""}"));
    }

    private interface ITest { }
    [JsonTypeId("class1")]
    private sealed class Class1 : ITest { }
    [JsonTypeId("class2")]
    private sealed class Class2 : ITest { }
    [JsonSerializable(typeof(Class1))]
    private sealed partial class SimpleContext1 : JsonSerializerContext { }
    [JsonSerializable(typeof(Class2))]
    private sealed partial class SimpleContext2 : JsonSerializerContext { }
}
