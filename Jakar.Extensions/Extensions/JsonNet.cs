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


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TResult FromJson<TResult>( this ReadOnlySpan<char> json ) => json.ToString()
                                                                                   .FromJson<TResult>(); // TODO: 
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TResult FromJson<TResult>( this string json ) => JsonConvert.DeserializeObject<TResult>( json ) ?? throw new NullReferenceException( nameof(JsonConvert.DeserializeObject) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TResult FromJson<TResult>( this string json, JsonSerializerSettings? settings ) => JsonConvert.DeserializeObject<TResult>( json, settings ) ?? throw new NullReferenceException( nameof(JsonConvert.DeserializeObject) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TResult FromJson<TResult>( this string json, params JsonConverter[] converters ) => JsonConvert.DeserializeObject<TResult>( json, converters ) ?? throw new NullReferenceException( nameof(JsonConvert.DeserializeObject) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this ReadOnlySpan<char> json ) => JToken.Parse( json.ToString() ) ?? throw new NullReferenceException( nameof(JToken.Parse) ); // TODO: optimize?
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this string             json ) => JToken.Parse( json ) ?? throw new NullReferenceException( nameof(JToken.Parse) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this string             json, JsonLoadSettings settings ) => JToken.Parse( json, settings ) ?? throw new NullReferenceException( nameof(JToken.Parse) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this object json ) => JToken.FromObject( json ) ?? throw new NullReferenceException( nameof(JToken.Parse) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this object json, JsonSerializer serializer ) => JToken.FromObject( json, serializer ) ?? throw new NullReferenceException( nameof(JToken.Parse) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken json ) => json.ToJson( Formatting.Indented );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken json, params JsonConverter[] converters ) => json.ToJson( Formatting.Indented, converters );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken json, Formatting             formatting ) => json.ToString( formatting );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken json, Formatting             formatting, params JsonConverter[] converters ) => json.ToString( formatting, converters );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object item ) => JsonConvert.SerializeObject( item );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object item, Formatting             formatting ) => JsonConvert.SerializeObject( item, formatting );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object item, JsonSerializerSettings settings ) => item.ToJson( Formatting.Indented, settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object item, Formatting             formatting, JsonSerializerSettings settings ) => JsonConvert.SerializeObject( item, formatting, settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object item, params JsonConverter[] converters ) => item.ToJson( Formatting.Indented, converters );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object item, Formatting             formatting, params JsonConverter[] converters ) => JsonConvert.SerializeObject( item, formatting, converters );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToPrettyJson( this object item ) => item.ToJson( Formatting.Indented );


    [Conditional( "DEBUG" )]
    public static void SaveDebug<T>( this T value, [CallerMemberName] string? caller = default, [CallerArgumentExpression( "value" )] string? variableName = default ) where T : notnull
    {
        Task.Run( async () =>
                  {
                      LocalFile file = LocalDirectory.Create( "DEBUG" )
                                                     .Join( $"{caller}__{variableName}.json" );

                      await file.WriteAsync( value.ToPrettyJson() );
                  } );
    }
}
