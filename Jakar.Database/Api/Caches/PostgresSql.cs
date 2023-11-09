// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:20 PM

namespace Jakar.Database;


public sealed class PostgresSql<TRecord> : BaseSqlCache<TRecord>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public override DbInstance Instance
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => DbInstance.Postgres;
    }


    public override SqlCommand First()
    {
        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.First] = sql = $"SELECT TOP 1 * FROM {TableName} ORDER BY {DateCreated} ASC";
        return sql;
    }
    public override SqlCommand Last()
    {
        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.First] = sql = $"SELECT TOP 1 * FROM {TableName} ORDER BY {DateCreated} DESC";
        return sql;
    }
    public override SqlCommand Random()
    {
        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.Random] = sql = $"SELECT TOP 1 * FROM {TableName} ORDER BY {RandomMethod}";
        return sql;
    }
    public override SqlCommand Random( in int count )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(count), count );

        if ( _sql.TryGetValue( SqlCacheType.RandomCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomCount] = sql = $"SELECT TOP @{nameof(count)} * FROM {TableName} ORDER BY {RandomMethod}"; }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Random( in Guid? userID, in int count )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(count),  count );
        parameters.Add( nameof(userID), userID );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserIDCount, out string? sql ) is false )
        {
            _sql[SqlCacheType.RandomUserIDCount] = sql = @$"SELECT TOP @{nameof(count)} * FROM {TableName} WHERE {nameof(IOwnedTableRecord.OwnerUserID)} = @{nameof(userID)}";
        }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Random( in RecordID<UserRecord> id, in int count )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(count), count );
        parameters.Add( nameof(id),    id );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomUserCount] = sql = @$"SELECT TOP @{nameof(count)} * FROM {TableName} WHERE {nameof(IOwnedTableRecord.CreatedBy)} = @{nameof(id)}"; }

        return new SqlCommand( sql, parameters );
    }
}
