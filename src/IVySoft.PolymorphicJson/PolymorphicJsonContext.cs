namespace IVySoft.PolymorphicJson;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// Implementation of IPolymorphicJsonContext that provides polymorphic serialization
/// capabilities using a JsonSerializerContext-derived type.
/// This class is responsible for creating type info resolvers and building type maps
/// based on JsonTypeIdAttribute declarations.
/// </summary>
internal sealed class PolymorphicJsonContext(Type serializerContext) : IPolymorphicJsonContext
{
    /// <summary>
    /// Creates a JsonSerializerContext instance for the specified options.
    /// Uses reflection to access the Default static property when no options are provided,
    /// or creates a new instance with the specified options otherwise.
    /// </summary>
    /// <param name="options">The JSON serializer options to configure the context with.</param>
    /// <returns>An IJsonTypeInfoResolver implementation for polymorphic serialization.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the Default property cannot be accessed or the type cannot be instantiated.</exception>
    public IJsonTypeInfoResolver CreateContext(JsonSerializerOptions? options)
    {
        IJsonTypeInfoResolver result;
        if (options == null)
        {
            // Access the static Default property of the JsonSerializerContext-derived type
            var prop = serializerContext.GetProperty(
                "Default",
                System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.GetProperty
                | System.Reflection.BindingFlags.Public)
                ?? throw new InvalidOperationException($"Unable to get static property Default of type  {serializerContext.FullName}");
            result = (IJsonTypeInfoResolver)(prop.GetValue(null)
                ?? throw new InvalidOperationException($"Unable to get static property Default value of type  {serializerContext.FullName}"));
        }
        else
        {
            // Create a new instance with the provided options
            result = (IJsonTypeInfoResolver)(Activator.CreateInstance(serializerContext, options)
                ?? throw new InvalidOperationException($"Unable to create instance of type {serializerContext.FullName}"));
        }
        return result;
    }

    /// <summary>
    /// Builds a collection of JsonDerivedType mappings for the specified base type.
    /// Searches for types marked with JsonSerializableAttribute that are assignable
    /// to the base type and have a JsonTypeIdAttribute defined.
    /// </summary>
    /// <param name="baseType">The base type to find derived types for.</param>
    /// <returns>A collection of JsonDerivedType entries representing the polymorphic type hierarchy.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a type discriminator cannot be determined.</exception>
    public IEnumerable<JsonDerivedType> GetTypeMap(Type baseType)
    {
        var result = new List<JsonDerivedType>();
        // Find all types in the serializer context marked with JsonSerializableAttribute
        foreach (var attr in serializerContext.CustomAttributes.Where(x => x.AttributeType == typeof(JsonSerializableAttribute)))
        {
            var type = (Type)attr.ConstructorArguments.Single().Value!;
            // Check if this type is assignable to the base type
            if (baseType.IsAssignableFrom(type))
            {
                // Find the JsonTypeIdAttribute to determine the type discriminator
                foreach (var typeIdAttr in type.CustomAttributes.Where(x => x.AttributeType == typeof(JsonTypeIdAttribute)))
                {
                    var typeIdValue = typeIdAttr.ConstructorArguments.Single().Value;
                    switch (typeIdValue)
                    {
                        case string stringDiscriminator:
                            result.Add(new JsonDerivedType(type, stringDiscriminator));
                            break;
                        case int intDiscriminator:
                            result.Add(new JsonDerivedType(type, intDiscriminator));
                            break;
                        default:
                            throw new InvalidOperationException($"Unable to get constructor argument of attribute JsonTypeIdAttribute on type {type.FullName}");
                    }
                    break;
                }
            }
        }
        return result;
    }
}
