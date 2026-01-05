namespace IVySoft.PolymorphicJson;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines the contract for polymorphic JSON serialization without generic type constraints.
/// This interface provides methods for serializing and deserializing objects that may have
/// derived types, using a type discriminator property ("$type") in the JSON representation.
/// </summary>
public interface IPolymorphicJsonSerializer
{
    /// <summary>
    /// Creates a JsonSerializerOptions instance configured for polymorphic serialization
    /// of the specified base type.
    /// </summary>
    /// <param name="baseType">The base type for which to create serialization options.</param>
    /// <param name="baseOptions">Optional existing options to inherit settings from.</param>
    /// <returns>A configured JsonSerializerOptions instance.</returns>
    public JsonSerializerOptions CreateOptions(Type baseType, JsonSerializerOptions? baseOptions = null);

    /// <summary>
    /// Serializes an object to a JSON string using polymorphic type handling.
    /// The resulting JSON includes a "$type" property for type identification.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="inputType">The type of the value to convert.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <returns>A JSON string representation with type discriminator.</returns>
    public string Serialize(object? value, Type inputType, JsonSerializerOptions? options = null);

    /// <summary>
    /// Writes one JSON value to the provided writer using polymorphic type handling.
    /// The resulting JSON includes a "$type" property for type identification.
    /// </summary>
    /// <param name="writer">The writer to write.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="inputType">The type of the value to convert.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    public void Serialize(Utf8JsonWriter writer, object? value, Type inputType, JsonSerializerOptions? options = null);

    /// <summary>
    /// Writes one JSON value to the provided writer using polymorphic type handling.
    /// The resulting JSON includes a "$type" property for type identification.
    /// </summary>
    /// <param name="utf8Json">The UTF-8 <see cref="Stream"/> to write to.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="inputType">The type of the value to convert.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public Task SerializeAsync(Stream utf8Json, object? value, Type inputType, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deserializes a JSON string to an object of the specified target type.
    /// The type discriminator in the JSON is used to instantiate the correct derived type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="targetType">The target type for deserialization.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <returns>The deserialized object, or null if the JSON represents a nullable type.</returns>
    public object? Deserialize(string json, Type targetType, JsonSerializerOptions? options = null);
    /// <summary>
    /// Deserializes a JSON string to an object of the specified target type.
    /// The type discriminator in the JSON is used to instantiate the correct derived type.
    /// </summary>
    /// <param name="utf8Json">JSON data to parse.</param>
    /// <param name="targetType">The target type for deserialization.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
    /// <returns>A <paramref name="targetType"/> representation of the JSON value.</returns>
    public ValueTask<object?> DeserializeAsync(Stream utf8Json, Type targetType, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a strongly-typed contract for polymorphic JSON serialization.
/// This interface provides type-safe serialization and deserialization for a specific base type,
/// enabling compile-time type safety while maintaining polymorphic behavior.
/// </summary>
/// <typeparam name="TBaseType">The base type that serves as the contract for serialization.</typeparam>
public interface IPolymorphicJsonSerializer<TBaseType>
{
    /// <summary>
    /// Creates a JsonSerializerOptions instance configured for polymorphic serialization
    /// of the generic base type TBaseType.
    /// </summary>
    /// <param name="baseOptions">Optional existing options to inherit settings from.</param>
    /// <returns>A configured JsonSerializerOptions instance.</returns>
    public JsonSerializerOptions CreateOptions(JsonSerializerOptions? baseOptions = null);


    /// <summary>
    /// Serializes an instance of TBaseType or its derived types to a JSON string.
    /// The resulting JSON includes a "$type" property for type identification.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public string Serialize(TBaseType? value, JsonSerializerOptions? options = null);

    /// <summary>
    /// Writes one JSON value to the provided writer using polymorphic type handling.
    /// The resulting JSON includes a "$type" property for type identification.
    /// </summary>
    /// <param name="writer">The writer to write.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    public void Serialize(Utf8JsonWriter writer, TBaseType? value, JsonSerializerOptions? options = null);

    /// <summary>
    /// Writes one JSON value to the provided writer using polymorphic type handling.
    /// The resulting JSON includes a "$type" property for type identification.
    /// </summary>
    /// <param name="utf8Json">The UTF-8 <see cref="Stream"/> to write to.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public Task SerializeAsync(Stream utf8Json, TBaseType? value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deserializes a JSON string to an instance of TBaseType or its derived types.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <returns>The deserialized object, or null if the JSON represents a nullable type.</returns>
    public TBaseType? Deserialize(string json, JsonSerializerOptions? options = null);
    /// <summary>
    /// Deserializes a JSON string to an object of the specified target type.
    /// The type discriminator in the JSON is used to instantiate the correct derived type.
    /// </summary>
    /// <param name="utf8Json">JSON data to parse.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
    /// <returns>A <typeparamref name="TBaseType"/> representation of the JSON value.</returns>
    public ValueTask<TBaseType?> DeserializeAsync(Stream utf8Json, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default);
    /// <summary>
    /// Wraps the UTF-8 encoded text into an <see cref="IAsyncEnumerable{TValue}" />
    /// that can be used to deserialize root-level JSON arrays in a streaming manner.
    /// The type discriminator in the JSON is used to instantiate the correct derived type.
    /// </summary>
    /// <param name="utf8Json">JSON data to parse.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the write operation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{TValue}" /> representation of the provided JSON array.</returns>
    public IAsyncEnumerable<TBaseType?> DeserializeAsyncEnumerable(Stream utf8Json, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default);
}
