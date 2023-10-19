// Jakar.Extensions :: Jakar.Database
// 10/10/2023  10:37 AM


namespace Jakar.Database;


public interface ISqlCacheFactory
{
    public ISqlCache<TRecord> GetSqlCache<TRecord>( IConnectableDbRoot dbRoot ) where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>;
}



public class SqlCacheFactory : ISqlCacheFactory
{
    private DbOptions _dbOptions;
    public SqlCacheFactory( IOptions<DbOptions> dbOptions ) { _dbOptions = dbOptions.Value; }


    public ISqlCache<TRecord> GetSqlCache<TRecord>( IConnectableDbRoot dbRoot ) where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        return dbRoot.Instance switch
               {
                   DbInstance.MsSql    => new MsSqlServer<TRecord>( _dbOptions ),
                   DbInstance.Postgres => new PostgresSql<TRecord>( _dbOptions ),
                   _                   => throw new OutOfRangeException( nameof(dbRoot.Instance), dbRoot.Instance )
               };
    }
}
