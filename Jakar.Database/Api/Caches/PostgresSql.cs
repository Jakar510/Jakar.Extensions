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

    /*
    protected virtual ImmutableDictionary<SqlStatement, string> Get_PostgresSql()
    {
        string TableName = TableName;
        string DateCreated     = DbInstance.Postgres.GetDateCreated();
        string IdColumnName    = DbInstance.Postgres.GetID_ColumnName();
        string RandomMethod    = DbInstance.Postgres.GetRandomMethod();
        string OwnerUserID     = DbInstance.Postgres.GetOwnerUserID();
        string KeyValuePairs   = string.Join( ',', KeyValuePairs );
        string ColumnNames     = string.Join( ',', ColumnNames );

        var dict = new Dictionary<SqlStatement, string>
                   {
                       [SqlStatement.All]             = $"SELECT * FROM {TableName}",
                       [SqlStatement.Update]          = $"UPDATE {TableName} SET {KeyValuePairs} WHERE {IdColumnName} = @{SQL.ID};",
                       [SqlStatement.Single]          = $"SELECT * FROM {TableName} WHERE {IdColumnName} = @{IdColumnName}",
                       [SqlStatement.Random]          = $"SELECT TOP 1 * FROM {TableName} ORDER BY {RandomMethod}",
                       [SqlStatement.RandomCount]     = $"SELECT TOP @{SQL.COUNT} * FROM {TableName} ORDER BY {RandomMethod}",
                       [SqlStatement.RandomUserCount] = $"SELECT TOP @{SQL.COUNT} * FROM {TableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod}",
                       [SqlStatement.Next]            = @$"SELECT * FROM {TableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {TableName} WHERE {IdColumnName} > @{SQL.ID}), 0) )",
                       [SqlStatement.SortedIDs]       = @$"SELECT {IdColumnName}, {DateCreated} FROM {TableName} ORDER BY {DateCreated} DESC",
                       [SqlStatement.NextID]          = @$"SELECT {IdColumnName} FROM {TableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {TableName} WHERE {IdColumnName} > @{SQL.ID}), 0) )",
                       [SqlStatement.Last]            = $"SELECT * FROM {TableName} ORDER BY {IdColumnName} DESC LIMIT 1",
                       [SqlStatement.SingleInsert]    = $"INSERT INTO {TableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )});",
                       [SqlStatement.TryInsert] = $"""
                                                   IF NOT EXISTS(SELECT * FROM {TableName} WHERE @where)
                                                   BEGIN
                                                       INSERT INTO {TableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                   END

                                                   ELSE
                                                   BEGIN
                                                       SELECT {IdColumnName} = NULL
                                                   END
                                                   """,
                       [SqlStatement.InsertOrUpdate] = $"""
                                                        IF NOT EXISTS(SELECT * FROM {TableName} WHERE @where)
                                                        BEGIN
                                                            INSERT INTO {TableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                        END

                                                        ELSE
                                                        BEGIN
                                                            UPDATE {TableName} SET {KeyValuePairs} WHERE {IdColumnName} = @{SQL.ID};

                                                            SELECT TOP 1 {IdColumnName} FROM {TableName} WHERE @where
                                                        END
                                                        """,
                       [SqlStatement.First]  = $"SELECT TOP 1 * FROM {TableName} ORDER BY {DateCreated} ASC LIMIT 1",
                       [SqlStatement.Delete] = $"DELETE FROM {TableName} WHERE {IdColumnName} = @{SQL.ID};",
                       [SqlStatement.Count]  = $"SELECT COUNT({IdColumnName}) FROM {TableName}"
                   };

        return dict.ToImmutableDictionary();
    }
    */


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
