// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:08 PM


namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "StaticMemberInGenericType" ), SuppressMessage( "ReSharper", "RedundantVerbatimStringPrefix" ) ]
public abstract class BaseSqlCache<TRecord> : ISqlCache<TRecord> where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public static readonly ImmutableDictionary<DbInstance, ImmutableDictionary<string, Descriptor>> SqlProperties     = SQL.CreateDescriptorMapping<TRecord>();
    protected readonly     ConcurrentDictionary<Key, string>                                        _deleteParameters = new(Key.Equalizer);
    protected readonly     ConcurrentDictionary<Key, string>                                        _existParameters  = new(Key.Equalizer);
    protected readonly     ConcurrentDictionary<Key, string>                                        _getParameters    = new(Key.Equalizer);
    protected readonly     ConcurrentDictionary<Key, string>                                        _insertOrUpdate   = new(Key.Equalizer);
    protected readonly     ConcurrentDictionary<Key, string>                                        _tryInsert        = new(Key.Equalizer);
    protected readonly     ConcurrentDictionary<Key, string>                                        _whereParameters  = new(Key.Equalizer);
    protected readonly     ConcurrentDictionary<SqlCacheType, string>                               _sql              = new();
    protected readonly     ConcurrentDictionary<string, string>                                     _whereColumn      = new();
    protected readonly     ConcurrentDictionary<string, string>                                     _whereIDColumn    = new();


    protected IEnumerable<Descriptor> _Descriptors
    {
        [ Pure, MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => SqlProperties[Instance].Values;
    }
    protected IEnumerable<string> _KeyValuePairs
    {
        [ Pure, MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _Descriptors.Select( x => x.KeyValuePair );
    }


    public          string     CreatedBy    { get; init; }
    public          string     DateCreated  { get; init; }
    public          string     IdColumnName { get; init; }
    public abstract DbInstance Instance     { get; }
    public          string     LastModified { get; init; }
    public          string     OwnerUserID  { get; init; }
    public          string     RandomMethod { get; init; }
    public virtual  string     TableName    => TRecord.TableName;

    protected BaseSqlCache()
    {
        RandomMethod = Instance.GetRandomMethod();
        CreatedBy    = Instance.GetCreatedBy();
        DateCreated  = Instance.GetDateCreated();
        IdColumnName = Instance.GetID_ColumnName();
        LastModified = Instance.GetLastModified();
        OwnerUserID  = Instance.GetOwnerUserID();
    }


    [ Pure, MethodImpl( MethodImplOptions.AggressiveInlining ) ] protected IEnumerable<string> GetDescriptors( DynamicParameters   parameters ) => parameters.ParameterNames.Select( name => SqlProperties[Instance][name].KeyValuePair );
    [ Pure, MethodImpl( MethodImplOptions.AggressiveInlining ) ] protected IEnumerable<string> GetKeyValuePairs( DynamicParameters parameters ) => parameters.ParameterNames.Select( KeyValuePair );
    [ Pure, MethodImpl( MethodImplOptions.AggressiveInlining ) ] protected string              KeyValuePair( string                columnName ) => SqlProperties[Instance][columnName].KeyValuePair;


    public virtual SqlCommand All()
    {
        if ( _sql.TryGetValue( SqlCacheType.All, out string? sql ) is false ) { _sql[SqlCacheType.All] = sql = $"SELECT * FROM {TableName}"; }

        return sql;
    }
    public abstract SqlCommand First();
    public abstract SqlCommand Last();
    public SqlCommand SortedIDs()
    {
        if ( _sql.TryGetValue( SqlCacheType.SortedIDs, out string? sql ) is false ) { _sql[SqlCacheType.SortedIDs] = sql = @$"SELECT {IdColumnName}, {DateCreated} FROM {TableName} ORDER BY {DateCreated} DESC"; }

        return sql;
    }


    public SqlCommand Delete( in RecordID<TRecord> id )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(id), id );

        if ( _sql.TryGetValue( SqlCacheType.DeleteRecord, out string? sql ) is false ) { _sql[SqlCacheType.DeleteRecord] = sql = $"DELETE FROM {TableName} WHERE {IdColumnName} in @{nameof(id)}"; }

        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Delete( in IEnumerable<RecordID<TRecord>> ids )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(ids), ids.Select( x => x.Value ) );

        if ( _sql.TryGetValue( SqlCacheType.DeleteRecords, out string? sql ) is false ) { _sql[SqlCacheType.DeleteRecords] = sql = $"DELETE FROM {TableName} WHERE {IdColumnName} in @{nameof(ids)}"; }

        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Delete( in bool matchAll, in DynamicParameters parameters )
    {
        Key key = Key.Create( matchAll, parameters );
        if ( _deleteParameters.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var buffer = new ValueStringBuilder( 1000 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        _deleteParameters[key] = sql = $"DELETE FROM {TableName} WHERE {buffer.Span};";
        return new SqlCommand( sql, parameters );
    }


    public SqlCommand Next( in RecordPair<TRecord> pair ) => Next( pair.ID, pair.DateCreated );
    public SqlCommand Next( in RecordID<TRecord> id, in DateTimeOffset dateCreated )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(id),          id );
        parameters.Add( nameof(dateCreated), dateCreated );

        if ( _sql.TryGetValue( SqlCacheType.NextID, out string? sql ) is false )
        {
            _sql[SqlCacheType.NextID] = sql = @$"SELECT * FROM {TableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {TableName} WHERE {DateCreated} > @{nameof(dateCreated)}), 0) )";
        }

        return new SqlCommand( sql, parameters );
    }
    public SqlCommand NextID( in RecordPair<TRecord> pair ) => NextID( pair.ID, pair.DateCreated );
    public SqlCommand NextID( in RecordID<TRecord> id, in DateTimeOffset dateCreated )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(id),          id );
        parameters.Add( nameof(dateCreated), dateCreated );

        if ( _sql.TryGetValue( SqlCacheType.NextID, out string? sql ) is false )
        {
            _sql[SqlCacheType.NextID] = sql = @$"SELECT {IdColumnName} FROM {TableName} WHERE ( id = IFNULL((SELECT MIN({IdColumnName}) FROM {TableName} WHERE {DateCreated} > @{nameof(dateCreated)}), 0) )";
        }

        return new SqlCommand( sql, parameters );
    }


    public SqlCommand Count()
    {
        if ( _sql.TryGetValue( SqlCacheType.Count, out string? sql ) is false ) { _sql[SqlCacheType.Count] = sql = @$"SELECT COUNT(*) FROM {TableName};"; }

        return sql;
    }
    public SqlCommand Random()
    {
        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) is false ) { _sql[SqlCacheType.Random] = sql = $"SELECT * FROM {TableName} ORDER BY {RandomMethod} LIMIT 1"; }

        return sql;
    }
    public abstract SqlCommand Random( in int                  count );
    public abstract SqlCommand Random( in Guid?                userID, in int count );
    public abstract SqlCommand Random( in RecordID<UserRecord> id,     in int count );
    public abstract SqlCommand Single();
    public abstract SqlCommand Insert( in         TRecord record );
    public abstract SqlCommand TryInsert( in      TRecord record, in bool matchAll, in DynamicParameters parameters );
    public abstract SqlCommand InsertOrUpdate( in TRecord record, in bool matchAll, in DynamicParameters parameters );
    public SqlCommand Update( in TRecord record )
    {
        var parameters = record.ToDynamicParameters();

        if ( _sql.TryGetValue( SqlCacheType.Random, out string? sql ) is false ) { _sql[SqlCacheType.Random] = sql = $"UPDATE {TableName} SET {_KeyValuePairs} WHERE {IdColumnName} = @{SQL.ID};"; }

        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Where<TValue>( in string columnName, in TValue? value )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        if ( _whereColumn.TryGetValue( columnName, out string? sql ) is false ) { _whereColumn[columnName] = sql = $"SELECT * FROM {TableName} WHERE {columnName} = @{nameof(value)}"; }

        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Where( in bool matchAll, in DynamicParameters parameters )
    {
        Key key = Key.Create( matchAll, parameters );
        if ( _whereParameters.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        _whereParameters[key] = sql = $"SELECT * FROM {TableName} WHERE {buffer.Span}";
        return new SqlCommand( sql, parameters );
    }


    public SqlCommand WhereID<TValue>( in string columnName, in TValue? value )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        if ( _whereIDColumn.TryGetValue( columnName, out string? sql ) is false ) { _whereIDColumn[columnName] = sql = $"SELECT {IdColumnName} FROM {TableName} WHERE {columnName} = @{nameof(value)}"; }

        return new SqlCommand( sql, parameters );
    }
    public SqlCommand Exists( in bool matchAll, in DynamicParameters parameters )
    {
        Key key = Key.Create( matchAll, parameters );
        if ( _existParameters.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        _existParameters[key] = sql = $"EXISTS( SELECT {IdColumnName} FROM {TableName} WHERE {buffer.Span} )";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Get( in bool matchAll, in DynamicParameters parameters )
    {
        Key key = Key.Create( matchAll, parameters );
        if ( _getParameters.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetDescriptors( parameters ) );

        _getParameters[key] = sql = $"SELECT * FROM {TableName} WHERE {buffer.Span}";
        return new SqlCommand( sql, parameters );
    }
    public SqlCommand Get( in RecordID<TRecord> id )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(id), id.Value );

        if ( _sql.TryGetValue( SqlCacheType.GetIDs, out string? sql ) is false ) { _sql[SqlCacheType.GetIDs] = sql = $"SELECT * FROM {TableName} WHERE {IdColumnName} = @{nameof(id)}"; }

        return new SqlCommand( sql, parameters );
    }
    public SqlCommand Get( in IEnumerable<RecordID<TRecord>> ids )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(ids), ids.Select( x => x.Value ) );

        if ( _sql.TryGetValue( SqlCacheType.GetIDs, out string? sql ) is false ) { _sql[SqlCacheType.GetIDs] = sql = $"SELECT * FROM {TableName} WHERE {IdColumnName} in @{nameof(ids)}"; }

        return new SqlCommand( sql, parameters );
    }



    protected readonly struct Key( in bool matchAll, in ImmutableArray<string> parameters ) : IEquatable<Key>
    {
        private readonly int                    _hash       = HashCode.Combine( matchAll, parameters );
        private readonly bool                   _matchAll   = matchAll;
        private readonly ImmutableArray<string> _parameters = parameters;

        public static ValueEqualizer<Key> Equalizer => ValueEqualizer<Key>.Default;

        public override bool Equals( object? other ) => other is Key key && Equals( key );
        public bool Equals( Key other )
        {
            if ( _hash != other._hash ) { return false; }

            if ( _matchAll != other._matchAll ) { return false; }

            if ( _parameters.Length != other._parameters.Length ) { return false; }

            foreach ( ReadOnlySpan<char> parameter in _parameters.AsSpan() )
            {
                foreach ( ReadOnlySpan<char> otherParameter in other._parameters.AsSpan() )
                {
                    if ( parameter.SequenceEqual( otherParameter ) is false ) { return false; }
                }
            }

            return true;
        }
        public override int GetHashCode() => _hash;


        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static Key Create( in bool matchAll, in DynamicParameters parameters ) => new(matchAll, parameters.ParameterNames.ToImmutableArray());


        public static bool operator ==( Key left, Key right ) => left.Equals( right );
        public static bool operator !=( Key left, Key right ) => !(left == right);
    }
}
