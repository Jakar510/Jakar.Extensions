// Jakar.Extensions :: Jakar.Database
// 01/24/2025  07:01

using ZiggyCreatures.Caching.Fusion;



namespace Jakar.Database;


public static class FusionCacheExtensions
{
    public static async Task<ErrorOrResult<TResult>> GetOrCreateAsync<TArg, TResult>( this FusionCache cache, string idKey, TArg definition, Func<TArg, CancellationToken, ValueTask<ErrorOrResult<TResult>>> factory, FusionCacheEntryOptions? options, CancellationToken token )
    {
        MaybeValue<TResult> maybeValue = await cache.TryGetAsync<TResult>(idKey, options, token);
        if ( maybeValue.HasValue ) { return maybeValue.Value; }

        ErrorOrResult<TResult> result = await factory(definition, token);
        if ( !result.TryGetValue(out TResult? record, out Errors? errors) ) { return errors; }

        await cache.SetAsync(idKey, record, options, token);
        return record;
    }
}
