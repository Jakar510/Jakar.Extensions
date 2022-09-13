#nullable enable
namespace Jakar.Extensions;


public static class DictionaryExtensions
{
    public static void ForEach<TKey, TValue>( this IDictionary<TKey, TValue> dict, Action<TKey, TValue> action )
    {
        foreach ( ( TKey key, TValue value ) in dict ) { action(key, value); }
    }

    public static async Task ForEachAsync<TKey, TValue>( this IDictionary<TKey, TValue> dict, Func<TKey, TValue, Task> action, bool continueOnCapturedContext = true )
    {
        foreach ( ( TKey key, TValue value ) in dict )
        {
            await action(key, value)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }


    public static void AddDefault<TKey, TValue>( this IDictionary<TKey, TValue?> dict, TKey key ) => dict.Add(key, default);

    public static void AddDefault<TKey, TValue>( this IDictionary<TKey, TValue?> dict, IEnumerable<TKey> keys )
    {
        foreach ( TKey value in keys ) { dict.AddDefault(value); }
    }
}
