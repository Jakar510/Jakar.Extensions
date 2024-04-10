// Jakar.Extensions :: Jakar.Database
// 10/10/2023  10:37 AM


namespace Jakar.Database;


public interface ISqlCacheFactory
{
    public ISqlCache<TRecord> GetSqlCache<TRecord>( IConnectableDbRoot dbRoot )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>;
}



public sealed class SqlCacheFactory : ISqlCacheFactory
{
    public ISqlCache<TRecord> GetSqlCache<TRecord>( IConnectableDbRoot dbRoot )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord> =>
        dbRoot.DbTypeInstance switch
        {
            DbTypeInstance.MsSql    => new MsSqlServer<TRecord>(),
            DbTypeInstance.Postgres => new PostgresSql<TRecord>(),
            _                   => throw new OutOfRangeException( nameof(dbRoot.DbTypeInstance), dbRoot.DbTypeInstance )
        };
}
