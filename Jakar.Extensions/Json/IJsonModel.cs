// Jakar.Extensions :: Jakar.Extensions
// 09/19/2025  14:22

namespace Jakar.Extensions;


public interface IJsonModel<TSelf> : IEqualComparable<TSelf>
    where TSelf : IJsonModel<TSelf>
{
    public abstract static JsonSerializerContext JsonContext   { get; }
    public abstract static JsonTypeInfo<TSelf>   JsonTypeInfo  { get; }
    public abstract static JsonTypeInfo<TSelf[]> JsonArrayInfo { get; }


    // public                 void   Serialize( ref    Utf8JsonWriter writer );
    // public abstract static TSelf Parse( scoped ref Utf8JsonReader reader );


    public abstract static bool  TryFromJson( string? json, [NotNullWhen(true)] out TSelf? result );
    public abstract static TSelf FromJson( string     json );
}
