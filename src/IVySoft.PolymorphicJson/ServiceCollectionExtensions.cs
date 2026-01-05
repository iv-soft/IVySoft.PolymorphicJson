namespace IVySoft.PolymorphicJson;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering polymorphic JSON serialization services
/// with the Microsoft.Extensions.DependencyInjection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a JsonSerializerContext-derived type to the service collection
    /// for use in polymorphic JSON serialization.
    /// </summary>
    /// <typeparam name="TSerializerContext">The JsonSerializerContext-derived type to register.</typeparam>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The same service collection for method chaining.</returns>
    public static IServiceCollection AddJsonSerializerContext<TSerializerContext>(this IServiceCollection services)
        where TSerializerContext : JsonSerializerContext
        => AddJsonSerializerContext(services, typeof(TSerializerContext));

    /// <summary>
    /// Adds a JsonSerializerContext by type to the service collection
    /// for use in polymorphic JSON serialization.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="serializerContext">The Type of JsonSerializerContext to register.</param>
    /// <returns>The same service collection for method chaining.</returns>
    public static IServiceCollection AddJsonSerializerContext(this IServiceCollection services, Type serializerContext)
        => services.AddTransient<IPolymorphicJsonContext>(sp => new PolymorphicJsonContext(serializerContext));

    /// <summary>
    /// Registers the polymorphic JSON serializer services in the service collection.
    /// This method sets up the infrastructure for polymorphic serialization by registering
    /// both the base IPolymorphicJsonSerializer and the generic IPolymorphicJsonSerializer&lt;TBaseType&gt;.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The same service collection for method chaining.</returns>
    public static IServiceCollection AddPolymorphicJsonSerializer(this IServiceCollection services)
        => services
            .AddSingleton<IPolymorphicJsonSerializer, PolymorphicJsonSerializer>()
            .AddSingleton(typeof(IPolymorphicJsonSerializer<>), typeof(PolymorphicJsonSerializer<>))
            ;
}
