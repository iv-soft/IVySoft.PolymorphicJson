namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Tests for options caching behavior
/// </summary>
public partial class OptionsCachingTests
{
    /// <summary>
    /// Test that same options instance returns cached result
    /// </summary>
    [Fact]
    public void OptionsCachingTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<OptionsCachingContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        var options1 = new JsonSerializerOptions();
        var result1 = serializer.CreateOptions(options1);
        var result2 = serializer.CreateOptions(options1);

        // Should be the same cached instance
        Assert.Same(result1, result2);
    }

    /// <summary>
    /// Test that different options create different cached results
    /// </summary>
    [Fact]
    public void DifferentOptionsCreateDifferentCacheEntriesTest()
    {
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<OptionsCachingContext>()
            .BuildServiceProvider();

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        var options1 = new JsonSerializerOptions { WriteIndented = true };
        var options2 = new JsonSerializerOptions { WriteIndented = false };

        var result1 = serializer.CreateOptions(options1);
        var result2 = serializer.CreateOptions(options2);

        // Different options should produce different results
        Assert.NotSame(result1, result2);
        Assert.NotEqual(result1.WriteIndented, result2.WriteIndented);
    }

    private interface ITest { }
    [JsonTypeId("class1")]
    private sealed class Class1 : ITest { }
    [JsonSerializable(typeof(Class1))]
    private sealed partial class OptionsCachingContext : JsonSerializerContext { }
}
