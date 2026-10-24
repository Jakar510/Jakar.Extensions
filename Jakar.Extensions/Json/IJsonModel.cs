// Jakar.Extensions :: Jakar.Extensions
// 10/23/2026

namespace Jakar.Extensions;


/// <summary> Carries JSON members the type doesn't declare, so they survive a round trip. </summary>
/// <remarks> Implementations mark the property <c> [JsonOverflow] </c>; Jakar.Json reads the attribute from the implementing property, not from this interface. </remarks>
public interface IJsonModel
{
    public JObjectNode? AdditionalData { get; set; }
}



/// <summary> Same bag as <see cref="IJsonModel"/>, stored as a JSON string (e.g. a database column). </summary>
public interface IJsonStringModel
{
    public string? AdditionalData { get; set; }
}
