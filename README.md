# IVySoft.PolymorphicJson

A .NET library that provides polymorphic JSON serialization capabilities using System.Text.Json with type discriminator support.

## Overview

This library enables serialization and deserialization of polymorphic types in JSON format by including a type discriminator property (`$type`) in the JSON output. It integrates seamlessly with the Microsoft.Extensions.DependencyInjection container and leverages the built-in polymorphic serialization features of System.Text.Json.

## Features

- **Polymorphic Serialization**: Automatically includes a `$type` property in JSON to identify derived types.
- **DI Integration**: Easy registration with Microsoft.Extensions.DependencyInjection.
- **Multiple Context Support**: Combine multiple JsonSerializerContext types for comprehensive type coverage.
- **Type-Safe API**: Generic and non-generic interfaces for type-safe operations.
- **Flexible Type Discriminators**: Supports both string and integer type discriminator values.
- **Caching**: Optimized option caching for improved serialization performance.

## Installation

Add the package to your project via NuGet:

```bash
dotnet add package IVySoft.PolymorphicJson
```

Or via Package Manager Console:

```powershell
Install-Package IVySoft.PolymorphicJson
```

## Requirements

- .NET 8.0 or .NET 10.0
- Microsoft.Extensions.DependencyInjection.Abstractions

## Quick Start

### 1. Define Your Types

Mark your polymorphic types with the `JsonTypeId` attribute to specify the type discriminator:

```csharp
using IVySoft.PolymorphicJson;
using System.Text.Json.Serialization;

public interface IShape { }

[JsonTypeId("circle")]
public sealed class Circle : IShape
{
    public double Radius { get; set; }
}

[JsonTypeId("rectangle")]
public sealed class Rectangle : IShape
{
    public double Width { get; set; }
    public double Height { get; set; }
}
```

### 2. Create JsonSerializerContext

Generate a JsonSerializerContext for your types:

```csharp
using System.Text.Json.Serialization;

[JsonSerializable(typeof(Circle))]
[JsonSerializable(typeof(Rectangle))]
public partial class ShapeContext : JsonSerializerContext { }
```

### 3. Register Services

Add the polymorphic serializer and your context to the DI container:

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddPolymorphicJsonSerializer()
    .AddJsonSerializerContext<ShapeContext>()
    .BuildServiceProvider();
```

### 4. Use the Serializer

Retrieve the serializer from DI and use it for serialization:

```csharp
using var scope = services.CreateScope();
var serializer = scope.ServiceProvider.GetRequiredService<IPolymorphicJsonSerializer<IShape>>();

// Serialize
var circle = new Circle { Radius = 5.0 };
var json = serializer.Serialize(circle);
// Result: {"$type":"circle","Radius":5.0}

// Deserialize
var shape = serializer.Deserialize(json);
// Returns the correct derived type (Circle or Rectangle)
```

## Advanced Usage

### Multiple Contexts

You can register multiple JsonSerializerContext types to support a broader range of polymorphic types:

```csharp
var services = new ServiceCollection()
    .AddPolymorphicJsonSerializer()
    .AddJsonSerializerContext<ShapeContext>()
    .AddJsonSerializerContext<AnotherContext>()
    .BuildServiceProvider();
```

### Integer Type Discriminators

The library also supports integer type discriminators:

```csharp
[JsonTypeId(1)]  // Integer discriminator
public sealed class Circle : IShape { }

[JsonTypeId(2)]  // Integer discriminator
public sealed class Rectangle : IShape { }
```

### Non-Generic Serializer

For scenarios where the base type is not known at compile time, use the non-generic interface:

```csharp
var serializer = sp.GetRequiredService<IPolymorphicJsonSerializer>();

var options = serializer.CreateOptions(typeof(IShape));
var json = JsonSerializer.Serialize(shape, options);
var deserialized = serializer.Deserialize(json, typeof(IShape));
```

## How It Works

1. **Type Discovery**: The library scans registered JsonSerializerContext types for classes marked with `[JsonSerializable]` and `[JsonTypeId]` attributes.
2. **Type Mapping**: It builds a map of base types to their derived types using the type discriminators.
3. **Resolver Combination**: Multiple type info resolvers are combined to provide comprehensive type information.
4. **Polymorphic Options**: The library configures `JsonPolymorphismOptions` on the JsonSerializerOptions to enable polymorphic serialization with the `$type` property.

## API Reference

### JsonTypeIdAttribute

Attribute used to specify type discriminator values for polymorphic JSON serialization.

**Usage:**

```csharp
[JsonTypeId("circle")]           // String discriminator
public sealed class Circle : IShape { }

[JsonTypeId(1)]                   // Integer discriminator
public sealed class Circle : IShape { }
```

**Constructor Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `typeDiscriminator` | `string` | The string value used to identify this type in JSON. |
| `typeDiscriminator` | `int` | The integer value used to identify this type in JSON. |

### ServiceCollectionExtensions

Provides extension methods for registering polymorphic JSON serialization services with DI.

| Method | Description |
|--------|-------------|
| `AddPolymorphicJsonSerializer()` | Registers the polymorphic JSON serializer services in the service collection. Sets up infrastructure for polymorphic serialization. |
| `AddJsonSerializerContext<TSerializerContext>(IServiceCollection)` | Registers a `JsonSerializerContext`-derived type in the service collection for use in polymorphic JSON serialization. |
| `AddJsonSerializerContext(IServiceCollection, Type)` | Registers a `JsonSerializerContext` by Type instance in the service collection. |

### IPolymorphicJsonSerializer

Defines the contract for polymorphic JSON serialization without generic type constraints. Provides methods for serializing and deserializing objects that may have derived types, using a type discriminator property (`$type`) in the JSON representation.

| Method | Description |
|--------|-------------|
| `CreateOptions(Type baseType, JsonSerializerOptions? baseOptions)` | Creates a `JsonSerializerOptions` instance configured for polymorphic serialization of the specified base type. |
| `Serialize(object? value, Type inputType, JsonSerializerOptions? options)` | Serializes an object to a JSON string using polymorphic type handling. Returns JSON with `$type` property. |
| `Serialize(Utf8JsonWriter writer, object? value, Type inputType, JsonSerializerOptions? options)` | Writes one JSON value to the provided writer using polymorphic type handling. |
| `SerializeAsync(Stream utf8Json, object? value, Type inputType, JsonSerializerOptions? options, CancellationToken cancellationToken)` | Asynchronously writes one JSON value to the provided stream using polymorphic type handling. |
| `Deserialize(string json, Type targetType, JsonSerializerOptions? options)` | Deserializes a JSON string to an object of the specified target type using the type discriminator. |
| `DeserializeAsync(Stream utf8Json, Type targetType, JsonSerializerOptions? options, CancellationToken cancellationToken)` | Asynchronously deserializes a JSON stream to an object of the specified target type. |

### IPolymorphicJsonSerializer<TBaseType>

Defines a strongly-typed contract for polymorphic JSON serialization for a specific base type. Provides type-safe serialization and deserialization, enabling compile-time type safety while maintaining polymorphic behavior.

| Method | Description |
|--------|-------------|
| `CreateOptions(JsonSerializerOptions? baseOptions)` | Creates a `JsonSerializerOptions` instance configured for polymorphic serialization of TBaseType. |
| `Serialize(TBaseType? value, JsonSerializerOptions? options)` | Serializes an instance of TBaseType or its derived types to a JSON string with `$type` property. |
| `Serialize(Utf8JsonWriter writer, TBaseType? value, JsonSerializerOptions? options)` | Writes one JSON value to the provided writer using polymorphic type handling. |
| `SerializeAsync(Stream utf8Json, TBaseType? value, JsonSerializerOptions? options, CancellationToken cancellationToken)` | Asynchronously writes one JSON value to the provided stream using polymorphic type handling. |
| `Deserialize(string json, JsonSerializerOptions? options)` | Deserializes a JSON string to an instance of TBaseType or its derived types. |
| `DeserializeAsync(Stream utf8Json, JsonSerializerOptions? options, CancellationToken cancellationToken)` | Asynchronously deserializes a JSON stream to an instance of TBaseType or its derived types. |
| `DeserializeAsyncEnumerable(Stream utf8Json, JsonSerializerOptions? options, CancellationToken cancellationToken)` | Wraps UTF-8 encoded text into an `IAsyncEnumerable<TValue>` for streaming deserialization of root-level JSON arrays. |

## License

This project is licensed under the MIT License.
