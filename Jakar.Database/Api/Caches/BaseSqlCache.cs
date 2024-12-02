// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:08 PM


using NoAlloq;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "StaticMemberInGenericType" ), SuppressMessage( "ReSharper", "RedundantVerbatimStringPrefix" )]
public abstract class BaseSqlCache<TRecord>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public static readonly FrozenDictionary<DbTypeInstance, FrozenDictionary<string, Descriptor>> SqlProperties     = SQL.CreateDescriptorMapping<TRecord>();
    protected readonly     ConcurrentDictionary<SqlCacheType, string>                             _sql              = new(EqualityComparer<SqlCacheType>.Default);
    protected readonly     ConcurrentDictionary<SqlKey, string>                                   _deleteParameters = new(SqlKey.Equalizer);
    protected readonly     ConcurrentDictionary<SqlKey, string>                                   _existParameters  = new(SqlKey.Equalizer);
    protected readonly     ConcurrentDictionary<SqlKey, string>                                   _getParameters    = new(SqlKey.Equalizer);
    protected readonly     ConcurrentDictionary<SqlKey, string>                                   _insertOrUpdate   = new(SqlKey.Equalizer);
    protected readonly     ConcurrentDictionary<SqlKey, string>                                   _tryInsert        = new(SqlKey.Equalizer);
    protected readonly     ConcurrentDictionary<SqlKey, string>                                   _whereParameters  = new(SqlKey.Equalizer);
    protected readonly     ConcurrentDictionary<string, string>                                   _whereColumn      = new(StringComparer.Ordinal);
    protected readonly     ConcurrentDictionary<string, string>                                   _whereIDColumn    = new(StringComparer.Ordinal);


    protected       IEnumerable<string>                  _KeyValuePairs { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Properties.Values.Select( static x => x.KeyValuePair ); }
    protected       FrozenDictionary<string, Descriptor> _Properties    { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => SqlProperties[Instance]; }
    public          string                               CreatedBy      { get; init; }
    public          string                               DateCreated    { get; init; }
    public          string                               IdColumnName   { get; init; }
    public abstract DbTypeInstance                       Instance       { get; }
    public          string                               LastModified   { get; init; }
    public          string                               RandomMethod   { get; init; }
    public virtual  string                               TableName      => TRecord.TableName;


    protected BaseSqlCache()
    {
        RandomMethod = Instance.GetRandomMethod();
        CreatedBy    = Instance.GetCreatedBy();
        DateCreated  = Instance.GetDateCreated();
        IdColumnName = Instance.GetID_ColumnName();
        LastModified = Instance.GetLastModified();
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] protected IEnumerable<string> GetKeyValuePairs( DynamicParameters parameters ) => parameters.ParameterNames.Select( GetKeyValuePair );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] protected string              GetKeyValuePair( string             columnName ) => _Properties[columnName].KeyValuePair;


    public virtual void Reset()
    {
        _deleteParameters.Clear();
        _existParameters.Clear();
        _getParameters.Clear();
        _insertOrUpdate.Clear();
        _tryInsert.Clear();
        _whereParameters.Clear();
        _sql.Clear();
        _whereColumn.Clear();
        _whereIDColumn.Clear();
    }


    public abstract SqlCommand First();
    public abstract SqlCommand Last();
    public abstract SqlCommand Random();
    public abstract SqlCommand Random( int                  count );
    public abstract SqlCommand Random( Guid?                userID, int count );
    public abstract SqlCommand Random( RecordID<UserRecord> id,     int count );
    public abstract SqlCommand Insert( TRecord              record );
    public abstract SqlCommand TryInsert( TRecord           record, bool matchAll, DynamicParameters parameters );
    public abstract SqlCommand InsertOrUpdate( TRecord      record, bool matchAll, DynamicParameters parameters );


    public virtual SqlCommand Single( RecordID<TRecord> id )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(id), id );

        if ( _sql.TryGetValue( SqlCacheType.Single, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _sql[SqlCacheType.Single] = sql = $"SELECT * FROM {TableName} WHERE {IdColumnName} = @{nameof(id)}";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand All()
    {
        if ( _sql.TryGetValue( SqlCacheType.All, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.All] = sql = $"SELECT * FROM {TableName}";
        return sql;
    }


    public virtual SqlCommand SortedIDs()
    {
        if ( _sql.TryGetValue( SqlCacheType.SortedIDs, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.SortedIDs] = sql = @$"SELECT {IdColumnName}, {DateCreated} FROM {TableName} ORDER BY {DateCreated} DESC";
        return sql;
    }


    public virtual SqlCommand Delete( RecordID<TRecord> id )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(id), id );

        if ( _sql.TryGetValue( SqlCacheType.DeleteRecord, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _sql[SqlCacheType.DeleteRecord] = sql = $"DELETE FROM {TableName} WHERE {IdColumnName} in @{nameof(id)}";
        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Delete( IEnumerable<RecordID<TRecord>> ids )
    {
        Guid[]            idValues   = [..ids.Select( static x => x.value )];
        DynamicParameters parameters = new();
        parameters.Add( nameof(ids), idValues );

        if ( _sql.TryGetValue( SqlCacheType.DeleteRecords, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _sql[SqlCacheType.DeleteRecords] = sql = $"DELETE FROM {TableName} WHERE {IdColumnName} in @{nameof(ids)}";
        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Delete( bool matchAll, DynamicParameters parameters )
    {
        var key = SqlKey.Create( matchAll, parameters );
        if ( _deleteParameters.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var buffer = new ValueStringBuilder( 1000 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        _deleteParameters[key] = sql = $"DELETE FROM {TableName} WHERE {buffer.Span};";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Next( RecordPair<TRecord> pair ) => Next( pair.id, pair.dateCreated );
    public virtual SqlCommand Next( RecordID<TRecord> id, DateTimeOffset dateCreated )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(id),          id );
        parameters.Add( nameof(dateCreated), dateCreated );

        if ( _sql.TryGetValue( SqlCacheType.NextID, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _sql[SqlCacheType.NextID] = sql = @$"SELECT * FROM {TableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {TableName} WHERE {DateCreated} > @{nameof(dateCreated)}), 0) )";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand NextID( RecordPair<TRecord> pair ) => NextID( pair.id, pair.dateCreated );
    public virtual SqlCommand NextID( RecordID<TRecord> id, DateTimeOffset dateCreated )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(id),          id );
        parameters.Add( nameof(dateCreated), dateCreated );

        if ( _sql.TryGetValue( SqlCacheType.NextID, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _sql[SqlCacheType.NextID] = sql = @$"SELECT {IdColumnName} FROM {TableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {TableName} WHERE {DateCreated} > @{nameof(dateCreated)}), 0) )";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Count()
    {
        if ( _sql.TryGetValue( SqlCacheType.Count, out string? sql ) ) { return sql; }

        _sql[SqlCacheType.Count] = sql = @$"SELECT COUNT(*) FROM {TableName};";
        return sql;
    }


    public virtual SqlCommand Update( TRecord record )
    {
        var parameters = record.ToDynamicParameters();
        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _sql[SqlCacheType.Random] = sql = $"UPDATE {TableName} SET {_KeyValuePairs} WHERE {IdColumnName} = @{SQL.ID};";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Where( bool matchAll, DynamicParameters parameters )
    {
        var key = SqlKey.Create( matchAll, parameters );
        if ( _whereParameters.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( static x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        _whereParameters[key] = sql = $"SELECT * FROM {TableName} WHERE {buffer.Span}";
        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Where<TValue>( string columnName, TValue? value )
    {
        if ( _Properties.ContainsKey( columnName ) ) { throw new ArgumentException( $"'{columnName}' is not a valid column: {_Properties.Keys.ToJson()}" ); }

        DynamicParameters parameters = new();
        parameters.Add( nameof(value), value );

        if ( _whereColumn.TryGetValue( columnName, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _whereColumn[columnName] = sql = $"SELECT * FROM {TableName} WHERE {columnName} = @{nameof(value)}";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand WhereID<TValue>( string columnName, TValue? value )
    {
        if ( _Properties.ContainsKey( columnName ) ) { throw new ArgumentException( $"'{columnName}' is not a valid column: {_Properties.Keys.ToJson()}" ); }

        DynamicParameters parameters = new();
        parameters.Add( nameof(value), value );

        if ( _whereIDColumn.TryGetValue( columnName, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _whereIDColumn[columnName] = sql = $"SELECT {IdColumnName} FROM {TableName} WHERE {columnName} = @{nameof(value)}";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Exists( bool matchAll, DynamicParameters parameters )
    {
        var key = SqlKey.Create( matchAll, parameters );
        if ( _existParameters.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( static x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        _existParameters[key] = sql = $"EXISTS( SELECT {IdColumnName} FROM {TableName} WHERE {buffer.Span} )";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Get( bool matchAll, DynamicParameters parameters )
    {
        var key = SqlKey.Create( matchAll, parameters );
        if ( _getParameters.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( static x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        _getParameters[key] = sql = $"SELECT * FROM {TableName} WHERE {buffer.Span}";
        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Get( RecordID<TRecord> id )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(id), id.value );

        if ( _sql.TryGetValue( SqlCacheType.GetIDs, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _sql[SqlCacheType.GetIDs] = sql = $"SELECT * FROM {TableName} WHERE {IdColumnName} = @{nameof(id)}";
        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Get( IEnumerable<RecordID<TRecord>> ids )
    {
        Guid[]            idValues   = [..ids.Select( static x => x.value )];
        DynamicParameters parameters = new();
        parameters.Add( nameof(ids), idValues );

        if ( _sql.TryGetValue( SqlCacheType.GetIDs, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _sql[SqlCacheType.GetIDs] = sql = $"SELECT * FROM {TableName} WHERE {IdColumnName} in @{nameof(ids)}";
        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Get( ReadOnlySpan<RecordID<TRecord>> ids )
    {
        Guid[]            idValues   = [..ids.Select( static x => x.value )];
        DynamicParameters parameters = new();
        parameters.Add( nameof(ids), idValues );

        if ( _sql.TryGetValue( SqlCacheType.GetIDs, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        _sql[SqlCacheType.GetIDs] = sql = $"SELECT * FROM {TableName} WHERE {IdColumnName} in @{nameof(ids)}";
        return new SqlCommand( sql, parameters );
    }
}
