namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Tests for custom JsonSerializerOptions integration
/// </summary>
public partial class CustomOptionsTests
{
    /// <summary>
    /// Test serialization with custom JsonSerializerOptions
    /// </summary>
    [Fact]
    public void SerializeWithCustomOptionsTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<CustomOptionsContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        var customOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var options = serializer.CreateOptions(customOptions);
        Assert.True(options.WriteIndented);
    }

    /// <summary>
    /// Test that custom options are applied to serialization
    /// </summary>
    [Fact]
    public void CustomOptionsAppliedTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<CustomOptionsContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        var customOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var options = serializer.CreateOptions(customOptions);
        var obj = new Class1 { Name = "Test" };

        var json = JsonSerializer.Serialize(obj, options);
        Assert.Contains("\n", json);
    }

    private interface ITest { }
    [JsonTypeId("class1")]
    private sealed class Class1 : ITest
    {
        public string? Name { get; set; }
    }
    [JsonSerializable(typeof(Class1))]
    private sealed partial class CustomOptionsContext : JsonSerializerContext { }
}
