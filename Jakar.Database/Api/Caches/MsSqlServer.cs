// Jakar.Extensions :: Jakar.Database
// 10/20/2023  7:15 PM

namespace Jakar.Database;


public sealed class MsSqlServer<TRecord> : BaseSqlCache<TRecord> where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public override DbInstance Instance => DbInstance.MsSql;

    /*
    protected virtual ImmutableDictionary<SqlStatement, string> Get_MsSql()
    {
        string IdColumnName    = DbInstance.MsSql.GetID_ColumnName();
        string RandomMethod    = DbInstance.MsSql.GetRandomMethod();
        string OwnerUserID     = DbInstance.MsSql.GetOwnerUserID();
        string KeyValuePairs   = string.Join( ',', KeyValuePairs );
        string ColumnNames     = string.Join( ',', ColumnNames );

        var dict = new Dictionary<SqlStatement, string>
        {
                       [SqlStatement.All]             = $"SELECT * FROM {TableName}",
                       [SqlStatement.Update]          = $"UPDATE {TableName} SET {KeyValuePairs} WHERE {IdColumnName} = @{IdColumnName};",
                       [SqlStatement.Single]          = $"SELECT * FROM {TableName} WHERE {IdColumnName} = @{IdColumnName}",
                       [SqlStatement.Random]          = $"SELECT * FROM {TableName} ORDER BY {RandomMethod} LIMIT 1",
                       [SqlStatement.RandomCount]     = $"SELECT * FROM {TableName} ORDER BY {RandomMethod} LIMIT @{SQL.COUNT}",
                       [SqlStatement.RandomUserCount] = $"SELECT * FROM {TableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod} LIMIT @{SQL.COUNT}",
                       [SqlStatement.Next]            = @$"SELECT * FROM {TableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {TableName} WHERE {IdColumnName} > @{SQL.ID}), 0) )",
                       [SqlStatement.SortedIDs]       = @$"SELECT {IdColumnName}, {DateCreated} FROM {TableName} ORDER BY {DateCreated} DESC",
                       [SqlStatement.NextID]          = @$"SELECT {IdColumnName} FROM {TableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {TableName} WHERE {IdColumnName} > @{SQL.ID}), 0) )",
                       [SqlStatement.Last]            = $"SELECT * FROM {TableName} ORDER BY {IdColumnName} DESC LIMIT 1",
                       [SqlStatement.SingleInsert]    = $"SET NOCOUNT ON INSERT INTO {TableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )});",
                       [SqlStatement.TryInsert] = $"""
                                                   IF NOT EXISTS(SELECT * FROM {TableName} WHERE @where)
                                                   BEGIN
                                                       SET NOCOUNT ON INSERT INTO {TableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                   END

                                                   ELSE
                                                   BEGIN
                                                       SELECT {IdColumnName} = NULL
                                                   END
                                                   """,
                       [SqlStatement.InsertOrUpdate] = $"""
                                                        IF NOT EXISTS(SELECT * FROM {TableName} WHERE @where)
                                                        BEGIN
                                                            SET NOCOUNT ON INSERT INTO {TableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                        END

                                                        ELSE
                                                        BEGIN
                                                            UPDATE {TableName} SET {KeyValuePairs} WHERE {IdColumnName} = @{SQL.ID};

                                                            SELECT {IdColumnName} FROM {TableName} WHERE @where LIMIT 1
                                                        END

                                                        """
            ,
            [SqlStatement.First]  = $"SELECT * FROM {TableName} ORDER BY {DateCreated} ASC LIMIT 1",
                       [SqlStatement.Delete] = $"DELETE FROM {TableName} WHERE {IdColumnName} = @{SQL.ID};",
                       [SqlStatement.Count]  = $"SELECT COUNT({IdColumnName}) FROM {TableName}"
                   };

        return dict.ToImmutableDictionary();
    }
    */


    public override SqlCommand First() => default;
    public override SqlCommand Last() => default;
    public override SqlCommand Random()
    {
        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.Random] = sql = $"SELECT * FROM {TableName} ORDER BY {RandomMethod} LIMIT 1";
        return sql;
    }
    public override SqlCommand Random( in int count )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(count), count );

        if ( _sql.TryGetValue( SqlCacheType.RandomCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomCount] = sql = $"SELECT * FROM {TableName} ORDER BY {RandomMethod} LIMIT @{nameof(count)}"; }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Random( in Guid? userID, in int count )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(count),  count );
        parameters.Add( nameof(userID), userID );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserIDCount, out string? sql ) is false )
        {
            _sql[SqlCacheType.RandomUserIDCount] = sql = @$"SELECT * FROM {TableName} WHERE {nameof(IOwnedTableRecord.OwnerUserID)} = @{nameof(userID)} LIMIT @{nameof(count)}";
        }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Random( in RecordID<UserRecord> id, in int count )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(count), count );
        parameters.Add( nameof(id),    id );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomUserCount] = sql = @$"SELECT * FROM {TableName} WHERE {nameof(IOwnedTableRecord.CreatedBy)} = @{nameof(id)} LIMIT @{nameof(count)}"; }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Single() => default;
    public override SqlCommand Insert( in TRecord record ) => default;
    public override SqlCommand TryInsert( in TRecord record, in bool matchAll, in DynamicParameters parameters ) => default;
    public override SqlCommand InsertOrUpdate( in TRecord record, in bool matchAll, in DynamicParameters parameters ) => default;
}
