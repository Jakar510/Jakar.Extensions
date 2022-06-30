


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public abstract class BaseResourceDictionaryManager
{
    protected static ResourceDictionary _ResourcesCurrent => Application.Current.Resources;

    protected BaseResourceDictionaryManager() => Init();

    protected abstract void Init();
}



/// <summary>
/// For this implementation, a single Normal Enum and Themed Enum is needed. 
/// </summary>
/// <typeparam name="TKey">Normal Enum</typeparam>
/// <typeparam name="TThemedKey">Themed Enum</typeparam>
public abstract class ResourceDictionaryManager<TKey, TThemedKey> : BaseResourceDictionaryManager where TThemedKey : Enum
                                                                                                  where TKey : Enum
{
    public static string GetFullKey( in  OSAppTheme theme, in TThemedKey key ) => $"{theme}.{typeof(TThemedKey).FullName}.{key}";
    public static string GetShortKey( in OSAppTheme theme, in TThemedKey key ) => $"{theme}{key}";

    /// <summary>
    /// Default implementation uses <see cref="GetFullKey"/>.
    /// You may override this to use <see cref="GetShortKey"/> or which ever pattern you wish.
    /// </summary>
    /// <param name="theme"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual string GetKey( in OSAppTheme theme, in TThemedKey key ) => GetFullKey(theme, key);


    public void Add( in TThemedKey key, in Style light, in Style dark )
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        Add(OSAppTheme.Light, key, light);
        Add(OSAppTheme.Dark, key, dark);
    }

    public void Add( in TThemedKey key, in Style style )
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        Add(OSAppTheme.Light, key, style);
        Add(OSAppTheme.Dark, key, style);
    }

    public void Add<TValue>( in OSAppTheme theme, in TThemedKey key, TValue value ) => _ResourcesCurrent.Add(GetKey(theme, key), value);

    public void Add<TValue>( in TKey key, TValue value )
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        _ResourcesCurrent.Add(key.ToString(), value);
    }

    public TValue Get<TValue>( in OSAppTheme theme, in TThemedKey key )
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        string k = GetKey(theme, key);
        if ( !_ResourcesCurrent.TryGetValue(k, out object value) ) { throw new KeyNotFoundException(k); }

        if ( value is TValue item ) { return item; }

        throw new ExpectedValueTypeException(nameof(item), value, typeof(TValue));
    }

    public TValue Get<TValue>( in TKey key )
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        string k = key.ToString();

        if ( !_ResourcesCurrent.TryGetValue(k, out object value) ) { throw new KeyNotFoundException(k); }

        if ( value is TValue item ) { return item; }

        throw new ExpectedValueTypeException(nameof(item), value, typeof(TValue));
    }


    /// <summary>
    /// <see cref="Color"/> is used in <see cref="BindAppTheme"/>
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="property"></param>
    /// <param name="key"></param>
    public void BindAppTheme( in BindableObject bindable, in BindableProperty property, in TThemedKey key ) =>
        BindAppTheme<Color>(bindable, property, key);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="bindable"></param>
    /// <param name="property"></param>
    /// <param name="key"></param>
    public void BindAppTheme<TValue>( in BindableObject bindable, in BindableProperty property, in TThemedKey key ) => bindable.SetOnAppTheme(property, Get<TValue>(OSAppTheme.Light, key), Get<TValue>(OSAppTheme.Dark, key));
}



/// <summary>
/// Recommended class to use for this implementation: <see cref="KeyNames"/>
/// </summary>
public abstract class ResourceDictionaryManager : BaseResourceDictionaryManager
{
    public static string GetFullKey<TThemedKey>( in  OSAppTheme theme, in TThemedKey key ) where TThemedKey : Enum => $"{theme}.{typeof(TThemedKey).FullName}.{key}";
    public static string GetShortKey<TThemedKey>( in OSAppTheme theme, in TThemedKey key ) where TThemedKey : Enum => $"{theme}{key}";

    /// <summary>
    /// Default implementation uses <see cref="GetFullKey{TThemedKey}"/>.
    /// You may override this to use <see cref="GetShortKey{TThemedKey}"/> or which ever pattern you wish.
    /// </summary>
    /// <typeparam name="TThemedKey"></typeparam>
    /// <param name="theme"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual string GetKey<TThemedKey>( in OSAppTheme theme, in TThemedKey key ) where TThemedKey : Enum => GetFullKey(theme, key);


    public void Add<TThemedKey>( in TThemedKey key, in Style light, in Style dark ) where TThemedKey : Enum
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        Add(OSAppTheme.Light, key, light);
        Add(OSAppTheme.Dark, key, dark);
    }

    public void Add<TThemedKey>( in TThemedKey key, in Style style ) where TThemedKey : Enum
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        Add(OSAppTheme.Light, key, style);
        Add(OSAppTheme.Dark, key, style);
    }

    public void Add<TThemedKey, TValue>( in OSAppTheme theme, in TThemedKey key, TValue value ) where TThemedKey : Enum => _ResourcesCurrent.Add(GetKey(theme, key), value);

    public void Add<TKey, TValue>( in TKey key, TValue value ) where TKey : Enum
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        _ResourcesCurrent.Add(key.ToString(), value);
    }

    public TValue Get<TThemedKey, TValue>( in OSAppTheme theme, in TThemedKey key ) where TThemedKey : Enum
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        string k = GetKey(theme, key);
        if ( !_ResourcesCurrent.TryGetValue(k, out object value) ) { throw new KeyNotFoundException(k); }

        if ( value is TValue item ) { return item; }

        throw new ExpectedValueTypeException(nameof(item), value, typeof(TValue));
    }

    public TValue Get<TKey, TValue>( in TKey key ) where TKey : Enum
    {
        if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

        string k = key.ToString();

        if ( !_ResourcesCurrent.TryGetValue(k, out object value) ) { throw new KeyNotFoundException(k); }

        if ( value is TValue item ) { return item; }

        throw new ExpectedValueTypeException(nameof(item), value, typeof(TValue));
    }


    /// <summary>
    /// <see cref="Color"/> is used in <see cref="BindAppTheme{TValue, TThemedKey}"/>
    /// </summary>
    /// <typeparam name="TThemedKey"></typeparam>
    /// <param name="bindable"></param>
    /// <param name="property"></param>
    /// <param name="key"></param>
    public void BindAppTheme<TThemedKey>( in BindableObject bindable, in BindableProperty property, in TThemedKey key ) where TThemedKey : Enum =>
        BindAppTheme<Color, TThemedKey>(bindable, property, key);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TThemedKey"></typeparam>
    /// <param name="bindable"></param>
    /// <param name="property"></param>
    /// <param name="key"></param>
    public void BindAppTheme<TValue, TThemedKey>( in BindableObject bindable, in BindableProperty property, in TThemedKey key ) where TThemedKey : Enum => bindable.SetOnAppTheme(property, Get<TThemedKey, TValue>(OSAppTheme.Light, key), Get<TThemedKey, TValue>(OSAppTheme.Dark, key));
}
