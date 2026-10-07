// Jakar.SystemTextJson
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     Source-generated metadata for every <c> [JsonModel] </c> type, registered lazily by a generated module initializer, so runtime lookups
///     (e.g. <c> Json.GetTypeInfo&lt;T&gt;() </c>) find these types without the app calling <c> Json.AddResolver </c> for their contexts.
/// </summary>
/// <remarks> Registration only stores a factory: the context isn't touched until the type is first looked up. Each type resolves to its <b> own </b> context's metadata and options. </remarks>
public static class JsonModelRegistry
{
    private static readonly ConcurrentDictionary<Type, Func<JsonTypeInfo>> __factories = new();
    private static readonly ConcurrentDictionary<Type, JsonTypeInfo>       __resolved  = new();


    /// <summary> Called by generated code. The first registration of a type wins. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Register<T>( Func<JsonTypeInfo<T>> factory )
    {
        ArgumentNullException.ThrowIfNull(factory);
        __factories.TryAdd(typeof(T), factory);
    }


    public static bool IsRegistered<T>() => __factories.ContainsKey(typeof(T));


    public static bool TryGet( Type type, [NotNullWhen(true)] out JsonTypeInfo? info )
    {
        ArgumentNullException.ThrowIfNull(type);

        if ( __resolved.TryGetValue(type, out info) ) { return true; }

        if ( __factories.TryGetValue(type, out Func<JsonTypeInfo>? factory) )
        {
            info = __resolved.GetOrAdd(type, factory());
            return true;
        }

        info = null;
        return false;
    }


    public static bool TryGet<T>( [NotNullWhen(true)] out JsonTypeInfo<T>? info )
    {
        if ( __resolved.TryGetValue(typeof(T), out JsonTypeInfo? cached) )
        {
            info = (JsonTypeInfo<T>)cached;
            return true;
        }

        if ( __factories.TryGetValue(typeof(T), out Func<JsonTypeInfo>? factory) )
        {
            info = (JsonTypeInfo<T>)__resolved.GetOrAdd(typeof(T), factory());
            return true;
        }

        info = null;
        return false;
    }
}
