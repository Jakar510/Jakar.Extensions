// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:20 PM

namespace Jakar.Database;


public sealed class PostgresSql<TRecord> : BaseSqlCache<TRecord>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public override DbTypeInstance Instance { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => DbTypeInstance.Postgres; }

    public override string TableName { get; } = $"\"{TRecord.TableName}\"";


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
        DynamicParameters parameters = new();
        parameters.Add( nameof(count), count );

        if ( _sql.TryGetValue( SqlCacheType.RandomCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomCount] = sql = $"SELECT TOP @{nameof(count)} * FROM {TableName} ORDER BY {RandomMethod}"; }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Random(  scoped in Guid? userID, in int count )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(count),  count );
        parameters.Add( nameof(userID), userID );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserIDCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomUserIDCount] = sql = @$"SELECT TOP @{nameof(count)} * FROM {TableName} WHERE {nameof(IOwnedTableRecord.OwnerUserID)} = @{nameof(userID)}"; }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Random(  scoped in RecordID<UserRecord> id, in int count )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(count), count );
        parameters.Add( nameof(id),    id );

        if ( _sql.TryGetValue( SqlCacheType.RandomUserCount, out string? sql ) is false ) { _sql[SqlCacheType.RandomUserCount] = sql = @$"SELECT TOP @{nameof(count)} * FROM {TableName} WHERE {nameof(IOwnedTableRecord.CreatedBy)} = @{nameof(id)}"; }

        return new SqlCommand( sql, parameters );
    }
    public override SqlCommand Insert( in TRecord record )
    {
        DynamicParameters param = new DynamicParameters( record );

        if ( _sql.TryGetValue( SqlCacheType.Insert, out string? sql ) ) { return new SqlCommand( sql, param ); }

        using ValueStringBuilder keys = new ValueStringBuilder( 1000 );
        keys.AppendJoin( ',', _Properties.Values.Select( x => x.ColumnName ) );

        using ValueStringBuilder values = new ValueStringBuilder( 1000 );
        values.AppendJoin( ',', _Properties.Values.Select( x => x.VariableName ) );

        _sql[SqlCacheType.Insert] = sql = $"""
                                             INSERT INTO {TableName} ({keys.Span}) values ({values.Span}) RETURNING {IdColumnName};
                                           """;

        return new SqlCommand( sql, param );
    }
    public override SqlCommand TryInsert( in TRecord record, in bool matchAll, in DynamicParameters parameters )
    {
        Key key   = Key.Create( matchAll, parameters );
        DynamicParameters param = new DynamicParameters( record );
        param.AddDynamicParams( parameters );

        if ( _tryInsert.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, param ); }

        using ValueStringBuilder buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        using ValueStringBuilder keys = new ValueStringBuilder( 1000 );
        keys.AppendJoin( ',', _Properties.Values.Select( x => x.ColumnName ) );

        using ValueStringBuilder values = new ValueStringBuilder( 1000 );
        values.AppendJoin( ',', _Properties.Values.Select( x => x.VariableName ) );

        _tryInsert[key] = sql = $"""
                                 IF NOT EXISTS(SELECT * FROM {TableName} WHERE {buffer.Span})
                                 BEGIN
                                     INSERT INTO {TableName} ({keys.Span}) values ({values.Span}) RETURNING {IdColumnName};
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
        DynamicParameters               param = new DynamicParameters( record );
        RecordID<TRecord> id    = record.ID;
        param.Add( nameof(id), id );
        param.AddDynamicParams( parameters );

        if ( _insertOrUpdate.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, param ); }

        using ValueStringBuilder buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        using ValueStringBuilder keys = new ValueStringBuilder( 1000 );
        keys.AppendJoin( ',', _Properties.Values.Select( x => x.ColumnName ) );

        using ValueStringBuilder values = new ValueStringBuilder( 1000 );
        values.AppendJoin( ',', _Properties.Values.Select( x => x.VariableName ) );

        using ValueStringBuilder keyValuePairs = new ValueStringBuilder( 1000 );
        keyValuePairs.AppendJoin( ',', _Properties.Values.Select( x => x.KeyValuePair ) );

        _insertOrUpdate[key] = sql = $"""
                                      IF NOT EXISTS(SELECT * FROM {TableName} WHERE {buffer.Span})
                                      BEGIN
                                          INSERT INTO {TableName} ({keys.Span}) values ({values.Span}) RETURNING {IdColumnName};
                                      END

                                      ELSE
                                      BEGIN
                                          UPDATE {TableName} SET {keyValuePairs.Span} WHERE {IdColumnName} = @{nameof(id)};
                                          SELECT @{nameof(id)};
                                      END
                                      """;

        return new SqlCommand( sql, param );
    }
}
