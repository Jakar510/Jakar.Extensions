namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ConvertToAutoPropertyWhenPossible" )]
public static class AppPreference
{
    public const            string           FALSE = "false";
    public const            string           TRUE  = "true";
    private static readonly Lock             _lock = new();
    private static          IAppPreferences? _source;


    public static IAppPreferences Source
    {
        get
        {
            lock (_lock) { return _source ??= AppPreferenceFile.Create(); }
        }
        set
        {
            lock (_lock) { _source = value; }
        }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool? GetBool( this string value ) => value switch
                                                        {
                                                            null  => null,
                                                            NULL  => null,
                                                            TRUE  => true,
                                                            FALSE => false,
                                                            _     => string.Equals( TRUE, value, StringComparison.OrdinalIgnoreCase )
                                                        };

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetString( this bool value ) => value
                                                             ? TRUE
                                                             : FALSE;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetString( this bool? value ) => value.HasValue
                                                              ? value.Value.GetString()
                                                              : NULL;


    public static string GetPreference( this string sharedName, string key, string? oldKey = null, string defaultValue = EMPTY ) => Source.Get( key, sharedName, oldKey, defaultValue );
    public static TValue GetPreference<TValue>( this string sharedName, string key, TValue defaultValue, in string? oldKey = null )
        where TValue : IParsable<TValue>, IFormattable
    {
        string value = Source.Get( key, sharedName, oldKey, defaultValue.ToString( null, CultureInfo.CurrentCulture ) );

        return TValue.TryParse( value, CultureInfo.CurrentCulture, out TValue? result )
                   ? result
                   : defaultValue;
    }


    public static bool GetPreference( this string sharedName, string key, bool defaultValue, string? oldKey = null )
    {
        string result = sharedName.GetPreference( key, oldKey, defaultValue.GetString() );
        return result.GetBool() is true;
    }
    public static bool? GetPreference( this string sharedName, string key, bool? defaultValue, string? oldKey = null )
    {
        string result = sharedName.GetPreference( key, oldKey, defaultValue.GetString() );
        return result.GetBool();
    }
    public static Uri GetPreference( this string sharedName, string key, in Uri defaultValue, string? oldKey = null )
    {
        string value = sharedName.GetPreference( key, oldKey, defaultValue.OriginalString );

        try
        {
            return Uri.TryCreate( value, UriKind.RelativeOrAbsolute, out Uri? host )
                       ? host
                       : defaultValue;
        }
        catch ( Exception e )
        {
            Debug.WriteLine( e );
            sharedName.RemovePreference( key, oldKey );
            return defaultValue;
        }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void SetPreference( this string sharedName, string key, bool?   value ) => sharedName.SetPreference( key, value.GetString() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void SetPreference( this string sharedName, string key, bool    value ) => sharedName.SetPreference( key, value.GetString() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void SetPreference( this string sharedName, string key, Uri     value ) => sharedName.SetPreference( key, value.ToString() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void SetPreference( this string sharedName, string key, string? value ) => Source.Set( key, value ?? string.Empty, sharedName );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void SetPreference<TValue>( this string sharedName, string key, TValue value )
        where TValue : IParsable<TValue>, IFormattable => Source.Set( key, value, sharedName );


    public static void RemovePreference( this string sharedName, string key, string? oldKey = null ) => Source.Remove( key, sharedName, oldKey );
}
