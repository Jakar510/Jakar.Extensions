// Jakar.Extensions :: Jakar.Extensions
// 01/15/2025  07:01

namespace Jakar.Extensions;


public interface IAppPreferences
{
    bool ContainsKey( string key, string sharedName );
    void Remove( string      key, string sharedName, string? oldKey = null );
    void Clear( string       sharedName );


    void Set( string key, string value, string sharedName );
    void Set( string key, Uri    value, string sharedName );
    void Set( string key, bool   value, string sharedName );
    void Set( string key, bool?  value, string sharedName );
    void Set<T>( string key, T value, string sharedName )
        where T : IParsable<T>, IFormattable;


    T Get<T>( string key, T defaultValue, string sharedName, string? oldKey = null )
        where T : IParsable<T>, IFormattable;
    string Get( string key, string sharedName,   string? oldKey = null, string  defaultValue = BaseRecord.EMPTY );
    Uri    Get( string key, Uri    defaultValue, string  sharedName,    string? oldKey       = null );
    bool   Get( string key, bool   defaultValue, string  sharedName,    string? oldKey       = null );
    bool?  Get( string key, bool?  defaultValue, string  sharedName,    string? oldKey       = null );
}
