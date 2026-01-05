namespace IVySoft.PolymorphicJson;

/// <summary>
/// Attribute used to specify type discriminator values for polymorphic JSON serialization.
/// When applied to a class, this attribute defines the identifier that will be used
/// in the JSON "$type" property to distinguish between different derived types.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class JsonTypeIdAttribute : Attribute
{
#pragma warning disable IDE0060 // Remove unused parameter
    /// <summary>
    /// Initializes a new instance of the JsonTypeIdAttribute with a string discriminator.
    /// </summary>
    /// <param name="typeDiscriminator">The string value used to identify this type in JSON.</param>
    public JsonTypeIdAttribute(string typeDiscriminator) { }

    /// <summary>
    /// Initializes a new instance of the JsonTypeIdAttribute with an integer discriminator.
    /// </summary>
    /// <param name="typeDiscriminator">The integer value used to identify this type in JSON.</param>
    public JsonTypeIdAttribute(int typeDiscriminator) { }
#pragma warning restore IDE0060 // Remove unused parameter
}
