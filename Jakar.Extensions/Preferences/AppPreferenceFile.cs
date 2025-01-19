// Jakar.Extensions :: Jakar.Extensions
// 01/15/2025  07:01

namespace Jakar.Extensions;


public sealed class AppPreferenceFile( LocalFile file, LanguageApi language ) : IAppPreferences, IAsyncDisposable // TODO: Add watcher to update if file changes
{
    private readonly IniConfig   _config   = IniConfig.ReadFromFile( file );
    private readonly LocalFile   _file     = file;
    private readonly LanguageApi _language = language;


    public static AppPreferenceFile Create( LanguageApi    language )                        => Create( LocalDirectory.CurrentDirectory, language );
    public static AppPreferenceFile Create( LocalDirectory directory, LanguageApi language ) => new(directory.Join( "Settings.ini" ), language);
    public async  ValueTask         DisposeAsync() => await SaveAsync();
    private async Task              SaveAsync()    => await _config.WriteToFile( _file );


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
    public void Set<T>( string key, T value, string sharedName )
        where T : IParsable<T>, IFormattable
    {
        _config[sharedName][key] = value.ToString( null, _language.CurrentCulture );
        _                        = SaveAsync();
    }


    public T Get<T>( string key, T defaultValue, string sharedName, string? oldKey = null )
        where T : IParsable<T>, IFormattable
    {
        string? value = Get( key, sharedName, oldKey, defaultValue.ToString( null, _language.CurrentCulture ) );

        return T.TryParse( value, _language.CurrentCulture, out T? result )
                   ? result
                   : defaultValue;
    }
    public Uri Get( string key, Uri defaultValue, string sharedName, string? oldKey = null ) => Uri.TryCreate( Get( key, sharedName ), UriKind.RelativeOrAbsolute, out Uri? result )
                                                                                                    ? result
                                                                                                    : defaultValue;
    public bool  Get( string key, bool  defaultValue, string sharedName, string? oldKey = null ) => Get( key, sharedName ).GetBool() is true;
    public bool? Get( string key, bool? defaultValue, string sharedName, string? oldKey = null ) => Get( key, sharedName ).GetBool();
    public string Get( string key, string sharedName, string? oldKey = null, string defaultValue = BaseRecord.EMPTY )
    {
        IniConfig.Section section = _config[sharedName];

        string? result = oldKey is not null && section.TryGetValue( oldKey, out string? value )
                             ? value
                             : section[key];

        return result ?? defaultValue;
    }
}
