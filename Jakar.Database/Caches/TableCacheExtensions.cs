using Microsoft.Extensions.Caching.Memory;



namespace Jakar.Database.Caches;


public static class TableCacheExtensions
{
    public static DistributedCacheEntryOptions RedisOptions  { get; set; } = new();
    public static MemoryCacheEntryOptions      MemoryOptions { get; set; } = new();


    [Pure] public static bool HasExpired( this DateTimeOffset? date, in TimeSpan lifeSpan ) => date.HasValue && HasExpired( date.Value, lifeSpan );
    [Pure] public static bool HasExpired( this DateTimeOffset  date, in TimeSpan lifeSpan ) => DateTimeOffset.UtcNow - date >= lifeSpan;


    public static async Task RefreshAsync<TRecord>( this ITableCache cache, RecordID<TRecord> id, CancellationToken token )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        if ( id.IsNotValid() ) { return; }

        await cache.RefreshAsync( id.Key, token );
    }


    public static async Task RemoveAsync<TRecord>( this ITableCache cache, RecordID<TRecord> id, CancellationToken token )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        if ( id.IsNotValid() ) { return; }

        string key = id.Key;
        ((IMemoryCache)cache).Remove( key );
        await cache.RemoveAsync( key, token );
    }


    public static async ValueTask<TRecord?> TryGetRecord<TRecord>( this ITableCache cache, RecordID<TRecord> id, CancellationToken token )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        if ( id.IsNotValid() ) { return default; }

        string key = id.Key;
        if ( cache.TryGetValue( key, out object? value ) && value is TRecord record ) { return record; }

        byte[]? data = await cache.GetAsync( key, token ).ConfigureAwait( false );
        if ( data == null ) { return default; }

        string json = Encoding.Default.GetString( data );
        return json.FromJson<TRecord>();
    }


    public static bool TryAddOrUpdate<TRecord>( this ITableCache cache, TRecord record, MemoryCacheEntryOptions? options = null )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        RecordID<TRecord> id = record.ID;
        if ( id.IsNotValid() ) { return false; }

        options ??= MemoryOptions;
        string            key   = id.Key;
        using ICacheEntry entry = cache.CreateEntry( key );
        entry.Value = record;
        entry.SetOptions( options );
        return true;
    }
    public static async ValueTask<bool> TryAddOrUpdate<TRecord>( this ITableCache cache, TRecord record, CancellationToken token, DistributedCacheEntryOptions? options = null )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        RecordID<TRecord> id = record.ID;
        if ( id.IsNotValid() ) { return false; }

        byte[] data = Encoding.Default.GetBytes( record.ToJson() );
        await cache.SetAsync( id.Key, data, options ?? RedisOptions, token ).ConfigureAwait( false );
        return true;
    }
}
