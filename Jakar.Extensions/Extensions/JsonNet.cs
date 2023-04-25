#nullable enable
namespace Jakar.Extensions;


public static class JsonNet
{
    private static JsonSerializerSettings _settings = new();
    public static  JsonLoadSettings       LoadSettings { get; set; } = new();
    public static  JsonSerializer         Serializer   { get; set; } = new();
    public static JsonSerializerSettings Settings
    {
        get => _settings;
        set
        {
            _settings  = value;
            Serializer = JsonSerializer.Create( value );
        }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this ReadOnlySpan<char> value ) => FromJson( value.ToString() ) ?? throw new NullReferenceException( nameof(JToken.Parse) ); // TODO: optimize?
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this string             value ) => FromJson( value, LoadSettings ) ?? throw new NullReferenceException( nameof(JToken.Parse) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this string             value, JsonLoadSettings settings ) => JToken.Parse( value, settings ) ?? throw new NullReferenceException( nameof(JToken.Parse) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this object value ) => JToken.FromObject( value,                            Serializer ) ?? throw new NullReferenceException( nameof(JToken.Parse) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this object value, JsonSerializer serializer ) => JToken.FromObject( value, serializer ) ?? throw new NullReferenceException( nameof(JToken.Parse) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken value ) => ToJson( value,                                    Formatting.None, Array.Empty<JsonConverter>() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken value, params JsonConverter[] converters ) => ToJson( value, Formatting.None, converters );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken value, Formatting             formatting ) => ToJson( value, formatting,      Array.Empty<JsonConverter>() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken value, Formatting             formatting, params JsonConverter[] converters ) => value.ToString( formatting, converters );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value ) => ToJson( value,                                    Formatting.None, Settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, Formatting             formatting ) => ToJson( value, formatting,      Settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, JsonSerializerSettings settings ) => ToJson( value,   Formatting.None, settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, Formatting             formatting, JsonSerializerSettings settings ) => JsonConvert.SerializeObject( value, formatting, settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, params JsonConverter[] converters ) => value.ToJson( Formatting.Indented, converters );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, Formatting             formatting, params JsonConverter[] converters ) => JsonConvert.SerializeObject( value, formatting, converters );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToPrettyJson( this JToken value ) => value.ToJson( Formatting.Indented );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToPrettyJson( this object value ) => value.ToJson( Formatting.Indented );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TResult FromJson<TResult>( this ReadOnlySpan<char> value ) => FromJson<TResult>( value.ToString() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TResult FromJson<TResult>( this string value ) => FromJson<TResult>( value, Settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TResult FromJson<TResult>( this string value, JsonSerializerSettings? settings ) => Validate.ThrowIfNull( JsonConvert.DeserializeObject<TResult>( value, settings ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TResult FromJson<TResult>( this string value, params JsonConverter[] converters ) => Validate.ThrowIfNull( JsonConvert.DeserializeObject<TResult>( value, converters ) );


    [Conditional( "DEBUG" )]
    public static void SaveDebug<T>( this T value, [CallerMemberName] string? caller = default, [CallerArgumentExpression( "value" )] string? variableName = default ) where T : notnull =>
        Task.Run( async () =>
                  {
                      LocalFile file = LocalDirectory.Create( "DEBUG" )
                                                     .Join( $"{caller}__{variableName}.value" );

                      await file.WriteAsync( value.ToPrettyJson() );
                  } );
}
