namespace Jakar.Extensions.Telemetry;


[DefaultValue( nameof(Default) ), JsonConverter( typeof(Converter) )]
public readonly record struct ActivityID( string HexString ) : ISpanParsable<ActivityID>
{
    public static   ActivityID Default    { get; } = new(string.Empty);
    public override string     ToString() => HexString;


    public static ActivityID Create()          => Create( Guid.CreateVersion7() );
    public static ActivityID Create( Guid id ) => new(id.ToBase64());
    public static ActivityID? Create( [NotNullIfNotNull( nameof(id) )] Guid? id ) => id.HasValue
                                                                                         ? new ActivityID( id.Value.ToBase64() )
                                                                                         : null;
    public static ActivityID Parse( string s, IFormatProvider? provider ) => Parse( s.AsSpan(), provider );
    public static bool TryParse( string? s, IFormatProvider? provider, out ActivityID result )
    {
        ReadOnlySpan<char> span = s;
        return TryParse( span, provider, out result );
    }
    public static ActivityID Parse( ReadOnlySpan<char> s, IFormatProvider? provider ) => default;
    public static bool TryParse( ReadOnlySpan<char> s, IFormatProvider? provider, out ActivityID result )
    {
        if ( s.TryAsGuid( out Guid? id ) )
        {
            result = Create( id.Value );
            return true;
        }

        result = Default;
        return false;
    }



    public sealed class Converter : JsonConverter<ActivityID>
    {
        public override void WriteJson( JsonWriter writer, ActivityID value, JsonSerializer serializer ) { writer.WriteValue( value.HexString ); }
        public override ActivityID ReadJson( JsonReader reader, Type objectType, ActivityID existingValue, bool hasExistingValue, JsonSerializer serializer ) => reader.Value switch
                                                                                                                                                                 {
                                                                                                                                                                     Guid id  => Create( id ),
                                                                                                                                                                     string s => Create( s.AsGuid() ) ?? existingValue,
                                                                                                                                                                     _        => throw new ExpectedValueTypeException( nameof(reader.Value), reader.Value, [typeof(Guid), typeof(string)] )
                                                                                                                                                                 };
    }



    public sealed class NullableConverter : JsonConverter<ActivityID?>
    {
        public override void WriteJson( JsonWriter writer, ActivityID? value, JsonSerializer serializer ) { writer.WriteValue( value?.HexString ); }
        public override ActivityID? ReadJson( JsonReader reader, Type objectType, ActivityID? existingValue, bool hasExistingValue, JsonSerializer serializer ) => reader.Value switch
                                                                                                                                                                   {
                                                                                                                                                                       Guid id  => Create( id ),
                                                                                                                                                                       string s => Create( s.AsGuid() ) ?? existingValue,
                                                                                                                                                                       _        => throw new ExpectedValueTypeException( nameof(reader.Value), reader.Value, [typeof(Guid), typeof(string)] )
                                                                                                                                                                   };
    }
}
