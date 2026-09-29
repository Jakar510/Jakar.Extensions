// Jakar.Extensions :: Jakar.Extensions
// 01/15/2025  07:01

namespace Jakar.Extensions;


public interface IAppPreferences : IAsyncDisposable
{
    bool ContainsKey( string key, string sharedName );
    void Remove( string      key, string sharedName, params ReadOnlySpan<string> alternateKeys );
    void Clear( string       sharedName );
    void Clear();


    void Set( string key, string value, string sharedName );
    void Set( string key, Uri    value, string sharedName );
    void Set( string key, bool   value, string sharedName );
    void Set( string key, bool?  value, string sharedName );
    void Set<TValue>( string key, TValue value, string sharedName )
        where TValue : IParsable<TValue>, IFormattable;


    TValue Get<TValue>( string key, TValue defaultValue, string sharedName, params ReadOnlySpan<string> alternateKeys )
        where TValue : IParsable<TValue>, IFormattable;
    string Get( string key, string sharedName,   string defaultValue = EMPTY, params ReadOnlySpan<string> alternateKeys );
    Uri    Get( string key, Uri    defaultValue, string sharedName,           params ReadOnlySpan<string> alternateKeys );
    bool   Get( string key, bool   defaultValue, string sharedName,           params ReadOnlySpan<string> alternateKeys );
    bool?  Get( string key, bool?  defaultValue, string sharedName,           params ReadOnlySpan<string> alternateKeys );
}
