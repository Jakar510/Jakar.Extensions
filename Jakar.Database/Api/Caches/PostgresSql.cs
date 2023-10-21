﻿// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:20 PM

namespace Jakar.Database;


public sealed class PostgresSql<TRecord> : BaseSqlCache<TRecord> where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public override DbInstance Instance => DbInstance.Postgres;

    /*
    protected virtual ImmutableDictionary<SqlStatement, string> Get_PostgresSql()
    {
        string SchemaTableName = SchemaTableName;
        string DateCreated     = DbInstance.Postgres.GetDateCreated();
        string IdColumnName    = DbInstance.Postgres.GetID_ColumnName();
        string RandomMethod    = DbInstance.Postgres.GetRandomMethod();
        string OwnerUserID     = DbInstance.Postgres.GetOwnerUserID();
        string KeyValuePairs   = string.Join( ',', KeyValuePairs );
        string ColumnNames     = string.Join( ',', ColumnNames );

        var dict = new Dictionary<SqlStatement, string>
                   {
                       [SqlStatement.All]             = $"SELECT * FROM {SchemaTableName}",
                       [SqlStatement.Update]          = $"UPDATE {SchemaTableName} SET {KeyValuePairs} WHERE {IdColumnName} = @{SQL.ID};",
                       [SqlStatement.Single]          = $"SELECT * FROM {SchemaTableName} WHERE {IdColumnName} = @{IdColumnName}",
                       [SqlStatement.Random]          = $"SELECT TOP 1 * FROM {SchemaTableName} ORDER BY {RandomMethod}",
                       [SqlStatement.RandomCount]     = $"SELECT TOP @{SQL.COUNT} * FROM {SchemaTableName} ORDER BY {RandomMethod}",
                       [SqlStatement.RandomUserCount] = $"SELECT TOP @{SQL.COUNT} * FROM {SchemaTableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod}",
                       [SqlStatement.Next]            = @$"SELECT * FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {SchemaTableName} WHERE {IdColumnName} > @{SQL.ID}), 0) )",
                       [SqlStatement.SortedIDs]       = @$"SELECT {IdColumnName}, {DateCreated} FROM {SchemaTableName} ORDER BY {DateCreated} DESC",
                       [SqlStatement.NextID]          = @$"SELECT {IdColumnName} FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {SchemaTableName} WHERE {IdColumnName} > @{SQL.ID}), 0) )",
                       [SqlStatement.Last]            = $"SELECT * FROM {SchemaTableName} ORDER BY {IdColumnName} DESC LIMIT 1",
                       [SqlStatement.SingleInsert]    = $"INSERT INTO {SchemaTableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )});",
                       [SqlStatement.TryInsert] = $"""
                                                   IF NOT EXISTS(SELECT * FROM {SchemaTableName} WHERE @where)
                                                   BEGIN
                                                       INSERT INTO {SchemaTableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                   END

                                                   ELSE
                                                   BEGIN
                                                       SELECT {IdColumnName} = NULL
                                                   END
                                                   """,
                       [SqlStatement.InsertOrUpdate] = $"""
                                                        IF NOT EXISTS(SELECT * FROM {SchemaTableName} WHERE @where)
                                                        BEGIN
                                                            INSERT INTO {SchemaTableName} ({ColumnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                        END

                                                        ELSE
                                                        BEGIN
                                                            UPDATE {SchemaTableName} SET {KeyValuePairs} WHERE {IdColumnName} = @{SQL.ID};

                                                            SELECT TOP 1 {IdColumnName} FROM {SchemaTableName} WHERE @where
                                                        END
                                                        """,
                       [SqlStatement.First]  = $"SELECT TOP 1 * FROM {SchemaTableName} ORDER BY {DateCreated} ASC LIMIT 1",
                       [SqlStatement.Delete] = $"DELETE FROM {SchemaTableName} WHERE {IdColumnName} = @{SQL.ID};",
                       [SqlStatement.Count]  = $"SELECT COUNT({IdColumnName}) FROM {SchemaTableName}"
                   };

        return dict.ToImmutableDictionary();
    }
    */
}
