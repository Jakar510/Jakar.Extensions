using Microsoft.Extensions.Caching.Memory;



namespace Jakar.Database.Caches;


public static class TableCaches
{
    private const string RECORD_ID = "Record.ID cannot be empty";
    private const string KEY       = "Key cannot be null";

    public static DistributedCacheEntryOptions RedisOptions  { get; set; } = new();
    public static MemoryCacheEntryOptions      MemoryOptions { get; set; } = new();


    [Pure] public static bool HasExpired( this DateTimeOffset? date, scoped in TimeSpan lifeSpan ) => date.HasValue && HasExpired( date.Value, lifeSpan );
    [Pure] public static bool HasExpired( this DateTimeOffset  date, scoped in TimeSpan lifeSpan ) => DateTimeOffset.UtcNow - date >= lifeSpan;


    public static Task RefreshAsync<T>( this ITableCache cache, scoped in RecordID<T> id, CancellationToken token )
        where T : ITableRecord<T>, IDbReaderMapping<T>
    {
        if ( id.IsNotValid() ) { throw new ArgumentException( RECORD_ID, nameof(id) ); }

        return RefreshAsync( cache, id.Key, token );
    }
    public static Task RefreshAsync( this ITableCache cache, string key, CancellationToken token )
    {
        if ( string.IsNullOrWhiteSpace( key ) ) { throw new ArgumentException( KEY, nameof(key) ); }

        return cache.RefreshAsync( key, token );
    }


    public static Task RemoveAsync<T>( this ITableCache cache, scoped in RecordID<T> id, CancellationToken token )
        where T : ITableRecord<T>, IDbReaderMapping<T>
    {
        if ( id.IsNotValid() ) { throw new ArgumentException( RECORD_ID, nameof(id) ); }

        return cache.RemoveAsync( id.Key, token );
    }
    public static Task RemoveAsync( this ITableCache cache, string key, CancellationToken token )
    {
        if ( string.IsNullOrWhiteSpace( key ) ) { throw new ArgumentException( KEY, nameof(key) ); }

        ((IMemoryCache)cache).Remove( key );
        return cache.RemoveAsync( key, token );
    }


    public static ValueTask<T?> TryGet<T>( this ITableCache cache, scoped in RecordID<T> id, CancellationToken token )
        where T : ITableRecord<T>, IDbReaderMapping<T>
    {
        if ( id.IsNotValid() ) { throw new ArgumentException( RECORD_ID, nameof(id) ); }

        return cache.TryGet<T>( id.Key, token );
    }
    public static async ValueTask<T?> TryGet<T>( this ITableCache cache, string key, CancellationToken token )
        where T : notnull
    {
        if ( string.IsNullOrWhiteSpace( key ) ) { throw new ArgumentException( KEY, nameof(key) ); }

        if ( cache.TryGetValue( key, out object? value ) && value is T record ) { return record; }

        byte[]? data = await cache.GetAsync( key, token ).ConfigureAwait( false );
        if ( data is null ) { return default; }

        string json = Encoding.Default.GetString( data );
        return json.FromJson<T>();
    }


    public static void AddOrUpdate<T>( this ITableCache cache, T record, MemoryCacheEntryOptions? options = null )
        where T : ITableRecord<T>, IDbReaderMapping<T>
    {
        RecordID<T> id = record.ID;
        if ( id.IsNotValid() ) { throw new ArgumentException( RECORD_ID, nameof(record.ID) ); }

        cache.AddOrUpdate( id.Key, record, options );
    }
    public static void AddOrUpdate<T>( this ITableCache cache, string key, T record, MemoryCacheEntryOptions? options = null )
    {
        if ( string.IsNullOrWhiteSpace( key ) ) { throw new ArgumentException( KEY, nameof(key) ); }

        options ??= MemoryOptions;
        using ICacheEntry entry = cache.CreateEntry( key );
        entry.Value = record;
        entry.SetOptions( options );
    }


    public static Task AddOrUpdate<T>( this ITableCache cache, T record, CancellationToken token, DistributedCacheEntryOptions? options = null )
        where T : ITableRecord<T>, IDbReaderMapping<T>
    {
        RecordID<T> id = record.ID;
        if ( id.IsNotValid() ) { throw new ArgumentException( RECORD_ID, nameof(record.ID) ); }

        return cache.AddOrUpdate( id.Key, record, token, options );
    }
    public static Task AddOrUpdate<T>( this ITableCache cache, string key, T record, CancellationToken token, DistributedCacheEntryOptions? options = null )
        where T : notnull
    {
        if ( string.IsNullOrWhiteSpace( key ) ) { throw new ArgumentException( KEY, nameof(key) ); }

        byte[] data = Encoding.Default.GetBytes( record.ToJson() );
        return cache.SetAsync( key, data, options ?? RedisOptions, token );
    }
}
