// Jakar.Extensions :: Jakar.Extensions
// 09/19/2025  14:22

namespace Jakar.Extensions;


public interface IJsonModel<TValue> 
    where TValue : IJsonModel<TValue>
{
    public abstract static JsonSerializerContext  JsonContext   { get; }
    public abstract static JsonTypeInfo<TValue>   JsonTypeInfo  { get; }
    public abstract static JsonTypeInfo<TValue[]> JsonArrayInfo { get; }


    public JsonNode ToJsonNode();
    public string   ToJson();


    // public                 void   Serialize( ref    Utf8JsonWriter writer );
    // public abstract static TValue Parse( scoped ref Utf8JsonReader reader );


    public abstract static bool   TryFromJson( string? json, [NotNullWhen(true)] out TValue? result );
    public abstract static TValue FromJson( string     json );
}
