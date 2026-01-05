namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Tests for the non-generic IPolymorphicJsonSerializer interface
/// </summary>
public partial class NonGenericInterfaceTests
{
    /// <summary>
    /// Test serialization via non-generic interface
    /// </summary>
    [Fact]
    public void NonGenericSerializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<NonGenericContext1>()
            .AddJsonSerializerContext<NonGenericContext2>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer>();

        var json = serializer.Serialize(new Class1(), typeof(ITest));
        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class1""}", json);
    }

    /// <summary>
    /// Test deserialization via non-generic interface
    /// </summary>
    [Fact]
    public void NonGenericDeserializeTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<NonGenericContext1>()
            .AddJsonSerializerContext<NonGenericContext2>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer>();

        var result = serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""class1""}", typeof(ITest));
        Assert.IsType<Class1>(result);
    }

    /// <summary>
    /// Test CreateOptions via non-generic interface
    /// </summary>
    [Fact]
    public void NonGenericCreateOptionsTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<NonGenericContext1>()
            .AddJsonSerializerContext<NonGenericContext2>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer>();

        var options = serializer.CreateOptions(typeof(ITest));
        Assert.NotNull(options);
        Assert.NotNull(options.TypeInfoResolver);
    }

    private interface ITest { }
    [JsonTypeId("class1")]
    private sealed class Class1 : ITest { }
    [JsonTypeId("class2")]
    private sealed class Class2 : ITest { }
    [JsonSerializable(typeof(Class1))]
    private sealed partial class NonGenericContext1 : JsonSerializerContext { }
    [JsonSerializable(typeof(Class2))]
    private sealed partial class NonGenericContext2 : JsonSerializerContext { }
}
