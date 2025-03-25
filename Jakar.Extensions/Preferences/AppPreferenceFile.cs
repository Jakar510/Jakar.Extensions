// Jakar.Extensions :: Jakar.Extensions
// 01/15/2025  07:01

namespace Jakar.Extensions;


public sealed class AppPreferenceFile( LocalFile file ) : IAppPreferences, IAsyncDisposable // TODO: Add watcher to update if file changes
{
    public const     string    DEFAULT_FILE_NAME = "Settings.ini";
    private readonly IniConfig _config           = IniConfig.ReadFromFile( file );
    private readonly LocalFile _file             = file;


    public static AppPreferenceFile Create()                           => Create( LocalDirectory.CurrentDirectory );
    public static AppPreferenceFile Create( LocalDirectory directory ) => Create( directory.Join( DEFAULT_FILE_NAME ) );
    public static AppPreferenceFile Create( LocalFile      file )      => new(file);
    public async  ValueTask         DisposeAsync()                     => await SaveAsync();
    private async Task              SaveAsync()                        => await _config.WriteToFile( _file );


    public bool ContainsKey( string key, string sharedName ) => _config[sharedName].ContainsKey( key );
    public void Remove( string key, string sharedName, string? oldKey = null )
    {
        _config[sharedName].TryRemove( key, out _ );
        if ( oldKey is not null ) { _config[sharedName].TryRemove( oldKey, out _ ); }

        _ = SaveAsync();
    }
    public void Clear( string sharedName )
    {
        _config[sharedName].Clear();
        _ = SaveAsync();
    }


    public void Set( string key, string value, string sharedName )
    {
        _config[sharedName][key] = value;
        _                        = SaveAsync();
    }
    public void Set( string key, Uri   value, string sharedName ) => Set( key, value.ToString(),  sharedName );
    public void Set( string key, bool  value, string sharedName ) => Set( key, value.GetString(), sharedName );
    public void Set( string key, bool? value, string sharedName ) => Set( key, value.GetString(), sharedName );
    public void Set<TValue>( string key, TValue value, string sharedName )
        where TValue : IParsable<TValue>, IFormattable
    {
        _config[sharedName][key] = value.ToString( null, CultureInfo.CurrentCulture );
        _                        = SaveAsync();
    }


    public TValue Get<TValue>( string key, TValue defaultValue, string sharedName, string? oldKey = null )
        where TValue : IParsable<TValue>, IFormattable
    {
        string? value = Get( key, sharedName, oldKey, defaultValue.ToString( null, CultureInfo.CurrentCulture ) );

        return TValue.TryParse( value, CultureInfo.CurrentCulture, out TValue? result )
                   ? result
                   : defaultValue;
    }
    public Uri Get( string key, Uri defaultValue, string sharedName, string? oldKey = null ) => Uri.TryCreate( Get( key, sharedName ), UriKind.RelativeOrAbsolute, out Uri? result )
                                                                                                    ? result
                                                                                                    : defaultValue;
    public bool  Get( string key, bool  defaultValue, string sharedName, string? oldKey = null ) => Get( key, sharedName ).GetBool() is true;
    public bool? Get( string key, bool? defaultValue, string sharedName, string? oldKey = null ) => Get( key, sharedName ).GetBool();
    public string Get( string key, string sharedName, string? oldKey = null, string defaultValue = EMPTY )
    {
        IniConfig.Section section = _config[sharedName];

        string? result = oldKey is not null && section.TryGetValue( oldKey, out string? value )
                             ? value
                             : section[key];

        return result ?? defaultValue;
    }
}
