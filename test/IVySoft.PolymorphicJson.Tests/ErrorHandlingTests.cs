namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Tests for error handling scenarios
/// </summary>
public partial class ErrorHandlingTests
{
    /// <summary>
    /// Test error when deserializing unknown type discriminator with string
    /// </summary>
    [Fact]
    public void UnknownTypeDiscriminatorErrorTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<ErrorHandlingContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        Assert.Throws<JsonException>(() =>
            serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""unknown""}"));
    }

    /// <summary>
    /// Test error when serializing type not registered in any context
    /// </summary>
    [Fact]
    public void UnregisteredTypeSerializeErrorTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<ErrorHandlingContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        // UnregisteredClass is not in any context, should fail
        Assert.Throws<NotSupportedException>(() =>
            serializer.Serialize(new UnregisteredClass()));
    }

    /// <summary>
    /// Test error when missing required property in JSON
    /// </summary>
    [Fact]
    public void MissingRequiredPropertyErrorTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<RequiredPropertyContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        // Missing required 'Name' property should throw
        Assert.Throws<JsonException>(() =>
            serializer.Deserialize(/*lang=json,strict*/ @"{""$type"":""required""}"));
    }

    private interface ITest { }
    [JsonTypeId("class1")]
    private sealed class Class1 : ITest { }
    [JsonSerializable(typeof(Class1))]
    private sealed partial class ErrorHandlingContext : JsonSerializerContext { }

    private sealed class UnregisteredClass : ITest { }

    [JsonTypeId("required")]
    private sealed class RequiredClass : ITest
    {
        public required string Name { get; set; }
    }
    [JsonSerializable(typeof(RequiredClass))]
    private sealed partial class RequiredPropertyContext : JsonSerializerContext { }
}
