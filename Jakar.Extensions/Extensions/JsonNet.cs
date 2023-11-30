﻿namespace Jakar.Extensions;


public static class JsonNet
{
    private static JsonNetSerializer?         _serializer;
    private static JsonSerializerSettings? _settings;


    public static JsonLoadSettings LoadSettings { get; set; } = new();
    public static JsonNetSerializer Serializer
    {
        get => _serializer ??= JsonNetSerializer.Create( Settings );
        set => _serializer = value;
    }
    public static JsonSerializerSettings Settings
    {
        get => _settings ??= new JsonSerializerSettings();
        set
        {
            _settings  = value;
            Serializer = JsonNetSerializer.Create( value );
        }
    }

    static JsonNet() => Settings = new JsonSerializerSettings();


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static JToken FromJson( this ReadOnlySpan<char> value ) => FromJson( value.ToString() ) ?? throw new NullReferenceException( nameof(JToken.Parse) ); // TODO: optimize?
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static JToken FromJson( this string             value ) => FromJson( value, LoadSettings ) ?? throw new NullReferenceException( nameof(JToken.Parse) );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static JToken FromJson( this string             value, JsonLoadSettings settings ) => JToken.Parse( value, settings ) ?? throw new NullReferenceException( nameof(JToken.Parse) );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static JToken FromJson( this object value )                            => JToken.FromObject( value, Serializer ) ?? throw new NullReferenceException( nameof(JToken.Parse) );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static JToken FromJson( this object value, JsonNetSerializer serializer ) => JToken.FromObject( value, serializer ) ?? throw new NullReferenceException( nameof(JToken.Parse) );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this JToken value )                                                                       => ToJson( value, Formatting.None, Array.Empty<JsonNetConverter>() );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this JToken value, params JsonNetConverter[] converters )                                    => ToJson( value, Formatting.None, converters );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this JToken value, Formatting             formatting )                                    => ToJson( value, formatting,      Array.Empty<JsonNetConverter>() );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this JToken value, Formatting             formatting, params JsonNetConverter[] converters ) => value.ToString( formatting, converters );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this object value )                                                                     => ToJson( value, Formatting.None, Settings );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this object value, Formatting             formatting )                                  => ToJson( value, formatting,      Settings );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this object value, JsonSerializerSettings settings )                                    => ToJson( value, Formatting.None, settings );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this object value, Formatting             formatting, JsonSerializerSettings settings ) => JsonConvert.SerializeObject( value, formatting, settings );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this object value, params JsonNetConverter[] converters )                                    => value.ToJson( Formatting.Indented, converters );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToJson( this object value, Formatting             formatting, params JsonNetConverter[] converters ) => JsonConvert.SerializeObject( value, formatting, converters );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToPrettyJson( this JToken value ) => value.ToJson( Formatting.Indented );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string ToPrettyJson( this object value ) => value.ToJson( Formatting.Indented );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static TResult FromJson<TResult>( this ReadOnlySpan<char> value ) => FromJson<TResult>( value.ToString() );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static TResult FromJson<TResult>( this string value ) => FromJson<TResult>( value, Settings );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static TResult FromJson<TResult>( this string value, JsonSerializerSettings? settings ) => Validate.ThrowIfNull( JsonConvert.DeserializeObject<TResult>( value, settings ) );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static TResult FromJson<TResult>( this string value, params JsonNetConverter[] converters ) => Validate.ThrowIfNull( JsonConvert.DeserializeObject<TResult>( value, converters ) );


    [ Conditional( "DEBUG" ) ]
    public static void SaveDebug<T>( this T value, [ CallerMemberName ] string? caller = default, [ CallerArgumentExpression( "value" ) ] string? variableName = default ) where T : notnull =>
        Task.Run( async () =>
                  {
                      LocalFile file = LocalDirectory.Create( "DEBUG" ).Join( $"{caller}__{variableName}.value" );

                      await file.WriteAsync( value.ToPrettyJson() );
                  } );
}