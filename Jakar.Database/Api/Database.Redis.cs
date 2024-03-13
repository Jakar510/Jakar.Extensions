namespace Jakar.Database;


public partial class Database
{
    public async ValueTask<TRecord?> TryGetRecord<TRecord>( RecordID<TRecord> id, CancellationToken token )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        byte[]? data = await _distributedCache.GetAsync( GetRedisKey( id ), token ).ConfigureAwait( false );
        if ( data == null ) { return default; }

        string json = Encoding.Default.GetString( data );
        return json.FromJson<TRecord>();
    }
    public async ValueTask AddOrUpdate<TRecord>( TRecord record, CancellationToken token )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        byte[] data = Encoding.Default.GetBytes( record.ToJson() );
        await _distributedCache.SetAsync( GetRedisKey( record.ID ), data, token ).ConfigureAwait( false );
    }
    public static string GetRedisKey<TRecord>( RecordID<TRecord> id )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord> => $"{typeof(TRecord).Name}:{id.Value}";
}
