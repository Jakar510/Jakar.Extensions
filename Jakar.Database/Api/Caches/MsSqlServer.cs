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
                       [SqlStatement.All]             = $"SELECT * FROM {SchemaTableName}",
                       [SqlStatement.Update]          = $"UPDATE {SchemaTableName} SET {KeyValuePairs} WHERE {IdColumnName} = @{IdColumnName};",
                       [SqlStatement.Single]          = $"SELECT * FROM {SchemaTableName} WHERE {IdColumnName} = @{IdColumnName}",
                       [SqlStatement.Random]          = $"SELECT * FROM {SchemaTableName} ORDER BY {RandomMethod} LIMIT 1",
                       [SqlStatement.RandomCount]     = $"SELECT * FROM {SchemaTableName} ORDER BY {RandomMethod} LIMIT @{SQL.COUNT}",
                       [SqlStatement.RandomUserCount] = $"SELECT * FROM {SchemaTableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod} LIMIT @{SQL.COUNT}",
                       [SqlStatement.Next]            = @$"SELECT * FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {SchemaTableName} WHERE {IdColumnName} > @{SQL.ID}), 0) )",
                       [SqlStatement.SortedIDs]       = @$"SELECT {IdColumnName}, {DateCreated} FROM {SchemaTableName} ORDER BY {DateCreated} DESC",
                       [SqlStatement.NextID]          = @$"SELECT {IdColumnName} FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {SchemaTableName} WHERE {IdColumnName} > @{SQL.ID}), 0) )",
                       [SqlStatement.Last]            = $"SELECT * FROM {SchemaTableName} ORDER BY {IdColumnName} DESC LIMIT 1",
                       [SqlStatement.SingleInsert]    = $"SET NOCOUNT ON INSERT INTO {SchemaTableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )});",
                       [SqlStatement.TryInsert] = $"""
                                                   IF NOT EXISTS(SELECT * FROM {SchemaTableName} WHERE @where)
                                                   BEGIN
                                                       SET NOCOUNT ON INSERT INTO {SchemaTableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                   END

                                                   ELSE
                                                   BEGIN
                                                       SELECT {IdColumnName} = NULL
                                                   END
                                                   """,
                       [SqlStatement.InsertOrUpdate] = $"""
                                                        IF NOT EXISTS(SELECT * FROM {SchemaTableName} WHERE @where)
                                                        BEGIN
                                                            SET NOCOUNT ON INSERT INTO {SchemaTableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                        END

                                                        ELSE
                                                        BEGIN
                                                            UPDATE {SchemaTableName} SET {KeyValuePairs} WHERE {IdColumnName} = @{SQL.ID};

                                                            SELECT {IdColumnName} FROM {SchemaTableName} WHERE @where LIMIT 1
                                                        END

                                                        """
            ,
            [SqlStatement.First]  = $"SELECT * FROM {SchemaTableName} ORDER BY {DateCreated} ASC LIMIT 1",
                       [SqlStatement.Delete] = $"DELETE FROM {SchemaTableName} WHERE {IdColumnName} = @{SQL.ID};",
                       [SqlStatement.Count]  = $"SELECT COUNT({IdColumnName}) FROM {SchemaTableName}"
                   };

        return dict.ToImmutableDictionary();
    }
    */
}
