// Jakar.Extensions :: Jakar.Database
// 10/10/2023  10:37 AM


namespace Jakar.Database;


public interface ISqlCacheFactory
{
    public ISqlCache<TRecord> GetSqlCache<TRecord>( IConnectableDbRoot dbRoot ) where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>;
}



public class SqlCacheFactory : ISqlCacheFactory
{
    public ISqlCache<TRecord> GetSqlCache<TRecord>( IConnectableDbRoot dbRoot ) where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        return dbRoot.Instance switch
               {
                   DbInstance.MsSql    => new MsSqlServer<TRecord>(),
                   DbInstance.Postgres => new PostgresSql<TRecord>(),
                   _                   => throw new OutOfRangeException( nameof(dbRoot.Instance), dbRoot.Instance )
               };
    }
}
