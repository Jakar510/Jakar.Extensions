// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:20 PM

using NoAlloq;



namespace Jakar.Database;


public sealed class PostgresSql<TRecord>
    where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public SqlCommand NextID( RecordPair<TRecord> pair ) => NextID( pair.id, pair.dateCreated );
    public SqlCommand NextID( RecordID<TRecord> id, DateTimeOffset dateCreated )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(id),          id );
        parameters.Add( nameof(dateCreated), dateCreated );

        if ( _sql.TryGetValue( SqlCacheType.NextID, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        sql = @$"SELECT {ID} FROM {TRecord.TableName} WHERE ( id = IFNULL((SELECT MIN({ID}) FROM {TRecord.TableName} WHERE {DATE_CREATED} > @{nameof(dateCreated)}), 0) )";
        return new SqlCommand( sql, parameters );
    }


    public SqlCommand Random()
    {
        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.Random] = sql = $"SELECT TOP 1 * FROM {TRecord.TableName} ORDER BY {RandomMethod}";
        return sql;
    }
    public SqlCommand Random( int count )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(count), count );

        if ( _sql.TryGetValue( SqlCacheType.RandomCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomCount] = sql = $"SELECT TOP @{nameof(count)} * FROM {TRecord.TableName} ORDER BY {RandomMethod}"; }

        return new SqlCommand( sql, parameters );
    }
    public SqlCommand Random( Guid? userID, int count )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(count),  count );
        parameters.Add( nameof(userID), userID );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserIDCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomUserIDCount] = sql = @$"SELECT TOP @{nameof(count)} * FROM {TRecord.TableName} WHERE {nameof(IOwnedTableRecord.CreatedBy)} = @{nameof(userID)}"; }

        return new SqlCommand( sql, parameters );
    }
    public SqlCommand Random( RecordID<UserRecord> id, int count )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(count), count );
        parameters.Add( nameof(id),    id );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomUserCount] = sql = @$"SELECT TOP @{nameof(count)} * FROM {TRecord.TableName} WHERE {nameof(IOwnedTableRecord.CreatedBy)} = @{nameof(id)}"; }

        return new SqlCommand( sql, parameters );
    }
}
