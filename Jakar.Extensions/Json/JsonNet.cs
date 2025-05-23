namespace Jakar.Extensions;


public static class JsonNet
{
    private static readonly JsonConverter[]         _converters = [];
    private static          JsonSerializer?         _serializer;
    private static          JsonSerializerSettings? _settings;


    public static JsonLoadSettings LoadSettings { get;                                                      set; } = new();
    public static JsonSerializer   Serializer   { get => _serializer ??= JsonSerializer.Create( Settings ); set => _serializer = value; }
    public static JsonSerializerSettings Settings
    {
        get => _settings ??= new JsonSerializerSettings();
        set
        {
            _settings  = value;
            Serializer = JsonSerializer.Create( value );
        }
    }

    static JsonNet() => Settings = new JsonSerializerSettings();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this ReadOnlySpan<char> value )                            => Validate.ThrowIfNull( FromJson( value.ToString() ) ); // TODO: optimize?
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this string             value )                            => Validate.ThrowIfNull( FromJson( value, LoadSettings ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this string             value, JsonLoadSettings settings ) => Validate.ThrowIfNull( JToken.Parse( value, settings ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this object             value )                            => Validate.ThrowIfNull( JToken.FromObject( value, Serializer ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static JToken FromJson( this object             value, JsonSerializer serializer ) => Validate.ThrowIfNull( JToken.FromObject( value, serializer ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken value )                                                                       => ToJson( value, Formatting.None, _converters );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken value, params JsonConverter[] converters )                                    => ToJson( value, Formatting.None, converters );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken value, Formatting             formatting )                                    => ToJson( value, formatting,      _converters );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this JToken value, Formatting             formatting, params JsonConverter[] converters ) => value.ToString( formatting, converters );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value )                                                                     => ToJson( value, Formatting.None, Settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, Formatting             formatting )                                  => ToJson( value, formatting,      Settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, JsonSerializerSettings settings )                                    => ToJson( value, Formatting.None, settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, Formatting             formatting, JsonSerializerSettings settings ) => JsonConvert.SerializeObject( value, formatting, settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, params JsonConverter[] converters )                                    => value.ToJson( Formatting.Indented, converters );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToJson( this object value, Formatting             formatting, params JsonConverter[] converters ) => JsonConvert.SerializeObject( value, formatting, converters );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToPrettyJson( this JToken value ) => value.ToJson( Formatting.Indented );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string ToPrettyJson( this object value ) => value.ToJson( Formatting.Indented );


    public static async ValueTask<JToken> FromJson( this Stream jsonStream, Encoding encoding, CancellationToken token )
    {
        using StreamReader reader = new(jsonStream, encoding);
        return await reader.FromJson( token );
    }
    public static async ValueTask<JToken> FromJson( this StreamReader reader, CancellationToken token )
    {
        // ReSharper disable once ConvertToUsingDeclaration
        using ( JsonTextReader jsonReader = new(reader)
                                            {
                                                ArrayPool = JsonArrayPool<char>.Shared,
                                                Culture   = CultureInfo.CurrentCulture
                                            } ) { return await JToken.ReadFromAsync( jsonReader, LoadSettings, token ); }
    }
    public static TResult FromJson<TResult>( this Stream jsonStream, Encoding encoding )
    {
        using StreamReader reader = new(jsonStream, encoding);
        return reader.FromJson<TResult>();
    }
    public static TResult FromJson<TResult>( this StreamReader reader ) => Serializer.FromJson<TResult>( reader );
    public static TResult FromJson<TResult>( this JsonSerializer serializer, StreamReader reader )
    {
        // ReSharper disable once ConvertToUsingDeclaration
        using ( JsonTextReader jsonReader = new(reader)
                                            {
                                                ArrayPool = JsonArrayPool<char>.Shared,
                                                Culture   = CultureInfo.CurrentCulture
                                            } )
        {
            TResult? result = serializer.Deserialize<TResult>( jsonReader );
            return Validate.ThrowIfNull( result );
        }
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TResult FromJson<TResult>( this ReadOnlySpan<char> value )                                     => FromJson<TResult>( value.ToString() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TResult FromJson<TResult>( this string             value )                                     => FromJson<TResult>( value, Settings );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TResult FromJson<TResult>( this string             value, JsonSerializerSettings? settings )   => Validate.ThrowIfNull( JsonConvert.DeserializeObject<TResult>( value, settings ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TResult FromJson<TResult>( this string             value, params JsonConverter[]  converters ) => Validate.ThrowIfNull( JsonConvert.DeserializeObject<TResult>( value, converters ) );


    public static string ToJson<TValue>( this ReadOnlySpan<TValue> values )
    {
        TValue[] array = ArrayPool<TValue>.Shared.Rent( values.Length );

        try
        {
            values.CopyTo( array );
            return array.ToJson();
        }
        finally { ArrayPool<TValue>.Shared.Return( array ); }
    }
    public static string ToPrettyJson<TValue>( this ReadOnlySpan<TValue> values )
    {
        TValue[] array = ArrayPool<TValue>.Shared.Rent( values.Length );

        try
        {
            values.CopyTo( array );
            return array.ToPrettyJson();
        }
        finally { ArrayPool<TValue>.Shared.Return( array ); }
    }


    [Conditional( "DEBUG" )]
    public static void SaveDebug<TValue>( this TValue value, [CallerMemberName] string? caller = null, [CallerArgumentExpression( "value" )] string? variableName = null )
        where TValue : notnull =>
        Task.Run( async () =>
                  {
                      using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
                      LocalFile           file          = LocalDirectory.Create( "DEBUG" ).Join( $"{caller}__{variableName}.value" );
                      await file.WriteAsync( value.ToPrettyJson() );
                  } );



    public sealed class JsonArrayPool<T>() : IArrayPool<T>
    {
        public static readonly JsonArrayPool<T> Shared = new();
        private readonly       ArrayPool<T>     _inner = Validate.ThrowIfNull( ArrayPool<T>.Shared );
        public                 T[]              Rent( int    minimumLength ) => _inner.Rent( minimumLength );
        public                 void             Return( T[]? array )         => _inner.Return( Validate.ThrowIfNull( array ) );
    }
}
