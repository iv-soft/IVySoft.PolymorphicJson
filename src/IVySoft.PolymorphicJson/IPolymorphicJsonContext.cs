namespace IVySoft.PolymorphicJson;

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// Defines the contract for providing polymorphic JSON serialization context.
/// Implementations of this interface are responsible for creating JSON type info resolvers
/// and providing type mappings for polymorphic serialization scenarios.
/// </summary>
internal interface IPolymorphicJsonContext
{
    /// <summary>
    /// Creates a JSON type info resolver for the specified serialization options.
    /// </summary>
    /// <param name="options">The base JSON serializer options to configure the resolver with.</param>
    /// <returns>An IJsonTypeInfoResolver that can handle polymorphic type resolution.</returns>
    public IJsonTypeInfoResolver CreateContext(JsonSerializerOptions? options);

    /// <summary>
    /// Gets the mapping of derived types for a given base type.
    /// </summary>
    /// <param name="baseType">The base type for which to retrieve derived type mappings.</param>
    /// <returns>An enumerable collection of JsonDerivedType entries representing the type hierarchy.</returns>
    public IEnumerable<JsonDerivedType> GetTypeMap(Type baseType);
}
