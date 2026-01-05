namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Test property serialize
/// </summary>
public partial class PropertyTests
{
    /// <summary>
    /// Test simple property serialize
    /// </summary>
    [Fact]
    public void SerializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<PropertyContext1>()
            .AddJsonSerializerContext<PropertyContext2>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class1"",""Prop1"":null}", serializer.Serialize(new Class1()));
        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class2"",""Prop2"":null}", serializer.Serialize(new Class2()));
        Assert.Equal(
            /*lang=json,strict*/ @"{""$type"":""class1"",""Prop1"":{""$type"":""class1"",""Prop1"":null}}",
            serializer.Serialize(new Class1()
            {
                Prop1 = new Class1()
            }));
        Assert.Equal(
            /*lang=json,strict*/ @"{""$type"":""class1"",""Prop1"":{""$type"":""class2"",""Prop2"":null}}",
            serializer.Serialize(new Class1()
            {
                Prop1 = new Class2()
            }));
    }
    /// <summary>
    /// Test simple property deserialize
    /// </summary>
    [Fact]
    public void DeserializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<PropertyContext1>()
            .AddJsonSerializerContext<PropertyContext2>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        var instance1 = Assert.IsType<Class1>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class1""}"));
        Assert.Null(instance1.Prop1);

        var instance2 = Assert.IsType<Class2>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class2""}"));
        Assert.Null(instance2.Prop2);

        instance1 = Assert.IsType<Class1>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class1"",""Prop1"":{""$type"":""class1""}}"));
        instance1 = Assert.IsType<Class1>(instance1.Prop1);
        Assert.Null(instance1.Prop1);

        instance1 = Assert.IsType<Class1>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class1"",""Prop1"":{""$type"":""class2""}}"));
        instance2 = Assert.IsType<Class2>(instance1.Prop1);
        Assert.Null(instance2.Prop2);
    }
    /// <summary>
    /// Test simple property serialize with missed json context
    /// </summary>
    [Fact]
    public void OneContextSerializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<PropertyContext1>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class1"",""Prop1"":null}", serializer.Serialize(new Class1()));
        Assert.Throws<NotSupportedException>(() => serializer.Serialize(new Class1() { Prop1 = new Class2() }));
    }
    /// <summary>
    /// Test simple property deserialize with missed json context
    /// </summary>
    [Fact]
    public void OneContextDeserializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<PropertyContext1>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        Assert.IsType<Class1>(serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class1""}"));
        Assert.Throws<System.Text.Json.JsonException>(() => serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class1"",""Prop1"":{""$type"":""class2""}}"));
    }

    private interface ITest { }
    [JsonTypeId("class1")]
    private sealed class Class1 : ITest
    {
        public ITest? Prop1 { get; set; }
    }
    [JsonTypeId("class2")]
    private sealed class Class2 : ITest
    {
        public ITest? Prop2 { get; set; }
    }
    [JsonSerializable(typeof(Class1))]
    private sealed partial class PropertyContext1 : JsonSerializerContext { }
    [JsonSerializable(typeof(Class2))]
    private sealed partial class PropertyContext2 : JsonSerializerContext { }
}
