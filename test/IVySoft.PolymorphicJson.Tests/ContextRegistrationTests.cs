namespace IVySoft.PolymorphicJson.Tests;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Tests for context registration by type
/// </summary>
public partial class ContextRegistrationTests
{
    /// <summary>
    /// Test registering context by Type instance
    /// </summary>
    [Fact]
    public void RegisterContextByTypeTest()
    {
#pragma warning disable CA2263
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext(typeof(ContextRegistrationContext1))
            .BuildServiceProvider();
#pragma warning restore CA2263

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        var json = serializer.Serialize(new Class1());
        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class1""}", json);
    }

    /// <summary>
    /// Test that both Type and generic registration work together
    /// </summary>
    [Fact]
    public void MixedContextRegistrationTest()
    {
#pragma warning disable CA2263
        using var sp = new ServiceCollection()
            .AddPolymorphicJsonSerializer()
            .AddJsonSerializerContext<ContextRegistrationContext1>()
            .AddJsonSerializerContext(typeof(ContextRegistrationContext2))
            .BuildServiceProvider();
#pragma warning restore CA2263

        var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer<ITest>>();

        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class1""}", serializer.Serialize(new Class1()));
        Assert.Equal(/*lang=json,strict*/ @"{""$type"":""class2""}", serializer.Serialize(new Class2()));
    }

    private interface ITest { }
    [JsonTypeId("class1")]
    private sealed class Class1 : ITest { }
    [JsonTypeId("class2")]
    private sealed class Class2 : ITest { }
    [JsonSerializable(typeof(Class1))]
    private sealed partial class ContextRegistrationContext1 : JsonSerializerContext { }
    [JsonSerializable(typeof(Class2))]
    private sealed partial class ContextRegistrationContext2 : JsonSerializerContext { }
}
