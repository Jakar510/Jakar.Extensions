/*
// Jakar.Extensions :: Jakar.Database
// 10/20/2023  7:15 PM

namespace Jakar.Database;


public sealed class MsSqlServer<TRecord> : BaseSqlCache<TRecord>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
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
    public override SqlCommand Random( int count )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(count), count );

        if ( _sql.TryGetValue( SqlCacheType.RandomCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomCount] = sql = $"SELECT * FROM {TableName} ORDER BY {RandomMethod} LIMIT @{nameof(count)}"; }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Random( Guid? userID, int count )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(count),  count );
        parameters.Add( nameof(userID), userID );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserIDCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomUserIDCount] = sql = @$"SELECT * FROM {TableName} WHERE {nameof(IOwnedTableRecord.CreatedBy)} = @{nameof(userID)} LIMIT @{nameof(count)}"; }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Random( RecordID<UserRecord> id, int count )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(count), count );
        parameters.Add( nameof(id),    id );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomUserCount] = sql = @$"SELECT * FROM {TableName} WHERE {nameof(IOwnedTableRecord.CreatedBy)} = @{nameof(id)} LIMIT @{nameof(count)}"; }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Insert( TRecord record )
    {
        DynamicParameters parameters = record.ToDynamicParameters();

        if ( _sql.TryGetValue( SqlCacheType.Insert, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        StringBuilder keys = new(1000);
        keys.AppendJoin( ',', _Properties.Values.Select( x => x.ColumnName ) );

        StringBuilder values = new(1000);
        values.AppendJoin( ',', _Properties.Values.Select( x => x.VariableName ) );

        _sql[SqlCacheType.Insert] = sql = $"SET NOCOUNT ON INSERT INTO {TableName} ( {keys} ) OUTPUT INSERTED.ID values ( {values} )";

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand TryInsert( TRecord record, bool matchAll, DynamicParameters parameters )
    {
        SqlKey            key   = SqlKey.Create( matchAll, parameters );
        DynamicParameters param = record.ToDynamicParameters();
        param.AddDynamicParams( parameters );

        if ( _tryInsert.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, param ); }

        StringBuilder buffer = new(parameters.ParameterNames.Sum( x => x.Length ) * 2);
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters, TODO ) );

        StringBuilder keys = new(1000);
        keys.AppendJoin( ',', _Properties.Values.Select( x => x.ColumnName ) );

        StringBuilder values = new(1000);
        values.AppendJoin( ',', _Properties.Values.Select( x => x.VariableName ) );

        _tryInsert[key] = sql = $"""
                                 IF NOT EXISTS(SELECT * FROM {TableName} WHERE {buffer})
                                 BEGIN
                                     SET NOCOUNT ON INSERT INTO {TableName} ( {keys} ) OUTPUT INSERTED.ID values ( {values} )
                                 END

                                 ELSE
                                 BEGIN
                                     SELECT {IdColumnName} = NULL
                                 END
                                 """;

        return new SqlCommand( sql, param );
    }
    public override SqlCommand InsertOrUpdate( TRecord record, bool matchAll, DynamicParameters parameters )
    {
        SqlKey            key   = SqlKey.Create( matchAll, parameters );
        DynamicParameters param = new(record);
        RecordID<TRecord> id    = record.ID;
        param.Add( nameof(id), id );
        param.AddDynamicParams( parameters );

        if ( _insertOrUpdate.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, param ); }

        StringBuilder buffer = new(parameters.ParameterNames.Sum( x => x.Length ) * 2);
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters, TODO ) );

        StringBuilder keys = new(1000);
        keys.AppendJoin( ',', _Properties.Values.Select( x => x.ColumnName ) );

        StringBuilder values = new(1000);
        values.AppendJoin( ',', _Properties.Values.Select( x => x.VariableName ) );

        StringBuilder keyValuePairs = new(1000);
        keyValuePairs.AppendJoin( ',', _Properties.Values.Select( x => x.KeyValuePair ) );

        _insertOrUpdate[key] = sql = $"""
                                      IF NOT EXISTS(SELECT * FROM {TableName} WHERE {buffer})
                                      BEGIN
                                          SET NOCOUNT ON INSERT INTO {TableName} ( {keys} ) OUTPUT INSERTED.ID values ( {values} )
                                      END

                                      ELSE
                                      BEGIN
                                          UPDATE {TableName} SET {keyValuePairs} WHERE {IdColumnName} = @{nameof(id)};
                                      
                                          SELECT {IdColumnName} FROM {TableName} WHERE @where LIMIT 1
                                      END
                                      """;

        return new SqlCommand( sql, param );
    }
}
*/
