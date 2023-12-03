using Microsoft.Extensions.Caching.Distributed;



namespace Jakar.Database;


public partial class Database
{
    private readonly IDistributedCache _distributedCache;

    // private readonly IDatabase         _redisDatabase;


    public async ValueTask<TRecord?> TryGetRecord<TRecord>( string key, CancellationToken token )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>, IMsJsonContext<TRecord>
    {
        byte[]? data = await _distributedCache.GetAsync( key, token ).ConfigureAwait( false );
        if ( data == null ) { return default; }

        string json = Encoding.Default.GetString( data );
        System.Text.Json.JsonSerializer.Deserialize( json, TRecord.JsonTypeInfo() );
        return json.FromJson<TRecord>();
    }
    public async ValueTask SetRecord<TRecord>( TRecord record, CancellationToken token )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>, IMsJsonContext<TRecord>
    {
        System.Text.Json.JsonSerializer.Serialize( record, TRecord.JsonTypeInfo() );
        byte[] data = Encoding.Default.GetBytes( record.ToJson() );
        await _distributedCache.SetAsync( GetRedisKey( record ), data, token: token ).ConfigureAwait( false );
    }
    public static string GetRedisKey<TRecord>( TRecord key )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord> => $"{typeof(TRecord).Name}:{key.ID.Value}";
}
