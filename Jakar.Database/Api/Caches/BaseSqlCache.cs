// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:08 PM


namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "StaticMemberInGenericType" ) ]
public abstract class BaseSqlCache<TRecord> : ISqlCache<TRecord> where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public static readonly ImmutableDictionary<DbInstance, ImmutableDictionary<string, Descriptor>> SqlProperties     = SQL.CreateDescriptorMapping<TRecord>();
    protected readonly     ConcurrentDictionary<Key, string>                                        _deleteParameters = new();
    protected readonly     ConcurrentDictionary<Key, string>                                        _existParameters  = new();
    protected readonly     ConcurrentDictionary<Key, string>                                        _getParameters    = new();
    protected readonly     ConcurrentDictionary<Key, string>                                        _insertOrUpdate   = new();
    protected readonly     ConcurrentDictionary<Key, string>                                        _tryInsert        = new();
    protected readonly     ConcurrentDictionary<Key, string>                                        _whereParameters  = new();
    protected readonly     ConcurrentDictionary<SqlCacheType, string>                               _sql              = new();
    protected readonly     ConcurrentDictionary<string, string>                                     _whereColumn      = new();
    protected readonly     ConcurrentDictionary<string, string>                                     _whereIDColumn    = new();


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


    public virtual SqlCommand All()       => $"SELECT * FROM {TableName}";
    public         SqlCommand First()     => default;
    public         SqlCommand Last()      => default;
    public         SqlCommand SortedIDs() => default;


    public SqlCommand Delete( in RecordID<TRecord> id )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(id), id );

        string sql = $"DELETE FROM {TableName} WHERE {IdColumnName} in @{nameof(id)}";
        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Delete( in IEnumerable<RecordID<TRecord>> ids )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(ids), ids.Select( x => x.Value ) );

        string sql = $"DELETE FROM {TableName} WHERE {IdColumnName} in @{nameof(ids)}";
        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Delete( in bool matchAll, in DynamicParameters parameters )
    {
        using var buffer = new ValueStringBuilder( 1000 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        string sql = $"DELETE FROM {TableName} WHERE {buffer.Span};";
        return new SqlCommand( sql, parameters );
    }


    public SqlCommand Next( in   RecordPair<TRecord> pair )                              => default;
    public SqlCommand Next( in   RecordID<TRecord>   id, in DateTimeOffset dateCreated ) => default;
    public SqlCommand NextID( in RecordPair<TRecord> pair )                              => default;
    public SqlCommand NextID( in RecordID<TRecord>   id, in DateTimeOffset dateCreated ) => default;


    public          SqlCommand Count()                                                => default;
    public          SqlCommand Random()                                               => default;
    public          SqlCommand Random( in int                  count )                => default;
    public          SqlCommand Random( in Guid?                userID, in int count ) => default;
    public          SqlCommand Random( in RecordID<UserRecord> id,     in int count ) => default;
    public          SqlCommand Single() => default;
    public abstract SqlCommand Insert( in         TRecord record );
    public abstract SqlCommand TryInsert( in      TRecord record, in bool matchAll, in DynamicParameters parameters );
    public abstract SqlCommand InsertOrUpdate( in TRecord record, in bool matchAll, in DynamicParameters parameters );
    public          SqlCommand Update( in         TRecord record ) => default;


    public virtual SqlCommand Where<TValue>( in string columnName, in TValue? value )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        if ( _whereColumn.TryGetValue( columnName, out string? sql ) is false ) { _whereColumn[columnName] = sql = $"SELECT * FROM {TableName} WHERE {columnName} = @{nameof(value)}"; }

        return new SqlCommand( sql, parameters );
    }
    public virtual SqlCommand Where( in bool matchAll, in DynamicParameters parameters )
    {
        var key = Key.Create( matchAll, parameters );
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
        var key = Key.Create( matchAll, parameters );
        if ( _existParameters.TryGetValue( key, out string? sql ) ) { return new SqlCommand( sql, parameters ); }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        _existParameters[key] = sql = $"EXISTS( SELECT {IdColumnName} FROM {TableName} WHERE {buffer.Span} )";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Get( in bool matchAll, in DynamicParameters parameters )
    {
        var key = Key.Create( matchAll, parameters );
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



    protected readonly struct Key( bool matchAll, ImmutableArray<string> parameters ) : IEquatable<Key>
    {
        private readonly int                    _hash = HashCode.Combine( matchAll, parameters );
        public           bool                   MatchAll   { get; } = matchAll;
        public           ImmutableArray<string> Parameters { get; } = parameters;


        public override bool Equals( object? other ) => other is Key key && Equals( key );
        public bool Equals( Key other )
        {
            if ( other._hash != _hash ) { return false; }

            return MatchAll == other.MatchAll && Parameters.All( other.Parameters.Contains );
        }
        public override int GetHashCode() => _hash;


        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static Key Create( in bool matchAll, in DynamicParameters parameters ) => new(matchAll, ImmutableArray.CreateRange( parameters.ParameterNames ));


        public static bool operator ==( Key left, Key right ) => left.Equals( right );
        public static bool operator !=( Key left, Key right ) => !(left == right);
    }
}
