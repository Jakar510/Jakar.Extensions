// Jakar.Extensions :: Jakar.Database
// 10/20/2023  7:15 PM

namespace Jakar.Database;


public sealed class MsSqlServer<TRecord> : BaseSqlCache<TRecord>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public override DbInstance Instance
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => DbInstance.MsSql;
    }

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
                       [SqlStatement.First]  = $"SELECT * FROM {TableName} ORDER BY {DateCreated} ASC LIMIT 1",
                       [SqlStatement.Delete] = $"DELETE FROM {TableName} WHERE {IdColumnName} = @{SQL.ID};",
                       [SqlStatement.Count]  = $"SELECT COUNT({IdColumnName}) FROM {TableName}"
                   };

        return dict.ToImmutableDictionary();
    }
    */


    public override SqlCommand First()
    {
        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.First] = sql = $"SELECT * FROM {TableName} ORDER BY {DateCreated} ASC LIMIT 1";
        return sql;
    }
    public override SqlCommand Last()
    {
        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.First] = sql = $"SELECT * FROM {TableName} ORDER BY {DateCreated} DESC LIMIT 1";
        return sql;
    }
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
    public override SqlCommand Insert( in TRecord record )
    {
        DynamicParameters parameters = record.ToDynamicParameters();

        if ( _sql.TryGetValue( SqlCacheType.Insert, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var keys = new ValueStringBuilder( 1000 );
        keys.AppendJoin( ',', _Properties.Values.Select( x => x.ColumnName ) );

        using var values = new ValueStringBuilder( 1000 );
        values.AppendJoin( ',', _Properties.Values.Select( x => x.VariableName ) );

        _sql[SqlCacheType.Insert] = sql = $"SET NOCOUNT ON INSERT INTO {TableName} ( {keys.Span} ) OUTPUT INSERTED.ID values ( {values.Span} )";

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand TryInsert( in TRecord record, in bool matchAll, in DynamicParameters parameters )
    {
        Key key   = Key.Create( matchAll, parameters );
        DynamicParameters param = record.ToDynamicParameters();
        param.AddDynamicParams( parameters );

        if ( _tryInsert.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, param ); }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        using var keys = new ValueStringBuilder( 1000 );
        keys.AppendJoin( ',', _Properties.Values.Select( x => x.ColumnName ) );

        using var values = new ValueStringBuilder( 1000 );
        values.AppendJoin( ',', _Properties.Values.Select( x => x.VariableName ) );

        _tryInsert[key] = sql = $"""
                                 IF NOT EXISTS(SELECT * FROM {TableName} WHERE {buffer.Span})
                                 BEGIN
                                     SET NOCOUNT ON INSERT INTO {TableName} ( {keys.Span} ) OUTPUT INSERTED.ID values ( {values.Span} )
                                 END

                                 ELSE
                                 BEGIN
                                     SELECT {IdColumnName} = NULL
                                 END
                                 """;

        return new SqlCommand( sql, param );
    }
    public override SqlCommand InsertOrUpdate( in TRecord record, in bool matchAll, in DynamicParameters parameters )
    {
        Key               key   = Key.Create( matchAll, parameters );
        var               param = new DynamicParameters( record );
        RecordID<TRecord> id    = record.ID;
        param.Add( nameof(id), id );
        param.AddDynamicParams( parameters );

        if ( _insertOrUpdate.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, param ); }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        using var keys = new ValueStringBuilder( 1000 );
        keys.AppendJoin( ',', _Properties.Values.Select( x => x.ColumnName ) );

        using var values = new ValueStringBuilder( 1000 );
        values.AppendJoin( ',', _Properties.Values.Select( x => x.VariableName ) );

        using var keyValuePairs = new ValueStringBuilder( 1000 );
        keyValuePairs.AppendJoin( ',', _Properties.Values.Select( x => x.KeyValuePair ) );

        _insertOrUpdate[key] = sql = $"""
                                      IF NOT EXISTS(SELECT * FROM {TableName} WHERE {buffer.Span})
                                      BEGIN
                                          SET NOCOUNT ON INSERT INTO {TableName} ( {keys.Span} ) OUTPUT INSERTED.ID values ( {values.Span} )
                                      END

                                      ELSE
                                      BEGIN
                                          UPDATE {TableName} SET {keyValuePairs.Span} WHERE {IdColumnName} = @{nameof(id)};
                                      
                                          SELECT {IdColumnName} FROM {TableName} WHERE @where LIMIT 1
                                      END
                                      """;

        return new SqlCommand( sql, param );
    }
}
