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

[JsonSerializable(typeof(IShape))]
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

## Configuration

### Custom Type Discriminator Property Name

The default type discriminator property name is `$type`. To customize this, you would need to modify the `JsonPolymorphismOptions` configuration in your code.

### Error Handling

By default, the library uses `JsonUnknownDerivedTypeHandling.FailSerialization`, which throws an exception when encountering an unknown derived type during serialization. Deserialization with an unrecognized type discriminator will throw a `JsonException`.

## API Reference

### ServiceCollectionExtensions

| Method | Description |
|--------|-------------|
| `AddPolymorphicJsonSerializer()` | Registers the polymorphic JSON serializer services. |
| `AddJsonSerializerContext<TContext>()` | Registers a JsonSerializerContext type. |
| `AddJsonSerializerContext(Type)` | Registers a JsonSerializerContext by type. |

### IPolymorphicJsonSerializer

| Method | Description |
|--------|-------------|
| `CreateOptions(Type, JsonSerializerOptions?)` | Creates options for polymorphic serialization of a specific type. |
| `Serialize(object)` | Serializes an object to JSON with type discriminator. |
| `Deserialize(string, Type)` | Deserializes JSON to an object of the specified type. |

### IPolymorphicJsonSerializer<TBaseType>

| Method | Description |
|--------|-------------|
| `CreateOptions(JsonSerializerOptions?)` | Creates options for polymorphic serialization of TBaseType. |
| `Serialize(TBaseType)` | Serializes a TBaseType instance to JSON. |
| `Deserialize(string)` | Deserializes JSON to a TBaseType instance. |

## License

This project is licensed under the MIT License.
