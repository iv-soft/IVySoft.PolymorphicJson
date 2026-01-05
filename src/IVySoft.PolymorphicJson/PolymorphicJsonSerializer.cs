namespace IVySoft.PolymorphicJson;

using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;

/// <summary>
/// Default implementation of IPolymorphicJsonSerializer that provides polymorphic JSON
/// serialization capabilities using System.Text.Json with type discriminator support.
/// This implementation supports multiple context factories and combines their type info resolvers.
/// </summary>
internal sealed class PolymorphicJsonSerializer(IEnumerable<IPolymorphicJsonContext> contextFactories) : IPolymorphicJsonSerializer
{
    /// <summary>
    /// Creates a JsonSerializerOptions instance configured for polymorphic serialization
    /// of the specified base type. Combines type info resolvers from all registered context factories.
    /// </summary>
    /// <param name="baseType">The base type for which to configure polymorphic serialization.</param>
    /// <param name="baseOptions">Optional existing options to inherit settings from.</param>
    /// <returns>A configured JsonSerializerOptions instance with combined type resolution.</returns>
    public JsonSerializerOptions CreateOptions(Type baseType, JsonSerializerOptions? baseOptions = null)
    {
        var options = baseOptions is null ? new JsonSerializerOptions() : new JsonSerializerOptions(baseOptions);
        var resolvers = new List<IJsonTypeInfoResolver>
        {
            new PolymorphicTypeResolver(baseType, contextFactories)
        };
        // Add resolvers from all context factories, passing inherited options if available
        resolvers.AddRange(contextFactories.Select(x => x.CreateContext(
            baseOptions is null
            ? null
            : new JsonSerializerOptions(baseOptions))));

        options.TypeInfoResolver = JsonTypeInfoResolver.Combine([.. resolvers]);
        return options;
    }

    /// <inheritdoc/>
    public string Serialize(object? value, JsonSerializerOptions? options = null)
        => JsonSerializer.Serialize(value, this.CreateOptions(typeof(object), options));
    /// <inheritdoc/>
    public string Serialize(object? value, Type inputType, JsonSerializerOptions? options = null)
        => JsonSerializer.Serialize(value, inputType, this.CreateOptions(typeof(object), options));
    /// <inheritdoc/>
    public void Serialize(Utf8JsonWriter writer, object? value, JsonSerializerOptions? options = null)
        => JsonSerializer.Serialize(writer, value, this.CreateOptions(typeof(object), options));
    /// <inheritdoc/>
    public void Serialize(Utf8JsonWriter writer, object? value, Type inputType, JsonSerializerOptions? options = null)
        => JsonSerializer.Serialize(writer, value, inputType, this.CreateOptions(typeof(object), options));
    /// <inheritdoc/>
    public Task SerializeAsync(Stream utf8Json, object? value, Type inputType, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        => JsonSerializer.SerializeAsync(utf8Json, value, inputType, this.CreateOptions(typeof(object), options), cancellationToken);
    /// <inheritdoc/>
    public object? Deserialize(string json, Type targetType, JsonSerializerOptions? options = null)
        => JsonSerializer.Deserialize(json, targetType, this.CreateOptions(typeof(object), options));
    /// <inheritdoc/>
    public ValueTask<object?> DeserializeAsync(Stream utf8Json, Type targetType, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync(utf8Json, targetType, this.CreateOptions(typeof(object), options), cancellationToken);

    /// <summary>
    /// Nested class that implements IJsonTypeInfoResolver to provide polymorphic
    /// type resolution for a specific base type.
    /// </summary>
    private sealed class PolymorphicTypeResolver : IJsonTypeInfoResolver
    {
        private readonly JsonPolymorphismOptions polymorphismOptions;
        private readonly Type baseType;

        public PolymorphicTypeResolver(Type baseType, IEnumerable<IPolymorphicJsonContext> contextFactories)
        {
            this.baseType = baseType ?? throw new ArgumentNullException(nameof(baseType));

            // Configure polymorphic serialization options
            this.polymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = false,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
            };
            // Collect derived types from all context factories
            foreach (var factory in contextFactories)
            {
                foreach (var derived in factory.GetTypeMap(baseType))
                {
                    this.polymorphismOptions.DerivedTypes.Add(derived);
                }
            }
        }
        /// <summary>
        /// Gets JsonTypeInfo for the configured base type, applying polymorphism options.
        /// Returns null for other types, allowing fallthrough to other resolvers.
        /// </summary>
        /// <param name="type">The type to get type info for.</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>JsonTypeInfo with polymorphism configured, or null for non-base types.</returns>
        public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            if (this.baseType == type)
            {
                var jsonTypeInfo = new DefaultJsonTypeInfoResolver().GetTypeInfo(type, options);
                jsonTypeInfo.PolymorphismOptions = this.polymorphismOptions;
                return jsonTypeInfo;
            }
            return null;
        }
    }
}

/// <summary>
/// Strongly-typed wrapper of IPolymorphicJsonSerializer that provides type-safe
/// serialization for a specific base type TBaseType.
/// </summary>
/// <typeparam name="TBaseType">The base type for serialization operations.</typeparam>
internal sealed class PolymorphicJsonSerializer<TBaseType>(IPolymorphicJsonSerializer polymorphicJsonSerializer)
    : IPolymorphicJsonSerializer<TBaseType>
{
    private readonly IPolymorphicJsonSerializer polymorphicJsonSerializer = polymorphicJsonSerializer ?? throw new ArgumentNullException(nameof(polymorphicJsonSerializer));
    private readonly JsonSerializerOptions defaultOptions = polymorphicJsonSerializer.CreateOptions(typeof(TBaseType), null);
    private readonly ConcurrentDictionary<JsonSerializerOptions, JsonSerializerOptions> cache = [];

    /// <summary>
    /// Creates a JsonSerializerOptions instance for TBaseType, using cached options
    /// when the same baseOptions are provided multiple times.
    /// </summary>
    /// <param name="baseOptions">Optional existing options to inherit settings from.</param>
    /// <returns>A configured JsonSerializerOptions instance.</returns>
    public JsonSerializerOptions CreateOptions(JsonSerializerOptions? baseOptions = null)
        => baseOptions == null
        ? this.defaultOptions
        : this.cache.GetOrAdd(baseOptions, (baseOptions) => this.polymorphicJsonSerializer.CreateOptions(typeof(TBaseType), baseOptions));

    /// <inheritdoc/>
    public string Serialize(TBaseType? value, JsonSerializerOptions? options = null)
        => JsonSerializer.Serialize(value, this.CreateOptions(options));
    /// <inheritdoc/>
    public void Serialize(Utf8JsonWriter writer, TBaseType? value, JsonSerializerOptions? options = null)
        => JsonSerializer.Serialize(writer, value, this.CreateOptions(options));
    /// <inheritdoc/>
    public Task SerializeAsync(Stream utf8Json, TBaseType? value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        => JsonSerializer.SerializeAsync(utf8Json, value, this.CreateOptions(options), cancellationToken);
    /// <inheritdoc/>
    public TBaseType? Deserialize(string json, JsonSerializerOptions? options = null)
        => JsonSerializer.Deserialize<TBaseType>(json, this.CreateOptions(options));
    /// <inheritdoc/>
    public ValueTask<TBaseType?> DeserializeAsync(Stream utf8Json, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync<TBaseType>(utf8Json, this.CreateOptions(options), cancellationToken);
    /// <inheritdoc/>
    public IAsyncEnumerable<TBaseType?> DeserializeAsyncEnumerable(Stream utf8Json, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsyncEnumerable<TBaseType>(utf8Json, this.CreateOptions(options), cancellationToken);
}
