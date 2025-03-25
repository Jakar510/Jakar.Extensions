// Jakar.Extensions :: Jakar.Extensions
// 01/15/2025  07:01

namespace Jakar.Extensions;

public static class AppPreferenceExtensions
{
    private static readonly ShareKeyCollection _sharedKeys = new();
    public static           string             GetSharedKey( this string sharedName, string key ) => _sharedKeys.GetOrAdd( sharedName, key );



    /// <summary> A collection of sharedName -> key -> $"{sharedName}.{key}". </summary>
    private sealed class ShareKeyCollection : ConcurrentDictionary<(string SharedName, string Key), string>
    {
        private static string CreateKey( (string SharedName, string Key) pair )                   => $"{pair.SharedName}.{pair.Key}";
        public         string GetOrAdd( string                           sharedName, string key ) => GetOrAdd( (sharedName, key), CreateKey );
    }
}


public interface IAppPreferences
{
    bool ContainsKey( string key, string sharedName );
    void Remove( string      key, string sharedName, string? oldKey = null );
    void Clear( string       sharedName );


    void Set( string key, string value, string sharedName );
    void Set( string key, Uri    value, string sharedName );
    void Set( string key, bool   value, string sharedName );
    void Set( string key, bool?  value, string sharedName );
    void Set<TValue>( string key, TValue value, string sharedName )
        where TValue : IParsable<TValue>, IFormattable;


    TValue Get<TValue>( string key, TValue defaultValue, string sharedName, string? oldKey = null )
        where TValue : IParsable<TValue>, IFormattable;
    string Get( string key, string sharedName,   string? oldKey = null, string  defaultValue = EMPTY );
    Uri    Get( string key, Uri    defaultValue, string  sharedName,    string? oldKey       = null );
    bool   Get( string key, bool   defaultValue, string  sharedName,    string? oldKey       = null );
    bool?  Get( string key, bool?  defaultValue, string  sharedName,    string? oldKey       = null );
}
