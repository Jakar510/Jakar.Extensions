// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:08 PM


namespace Jakar.Database;


public abstract class BaseSqlCache<TRecord> : ISqlCache<TRecord> where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public static readonly ImmutableDictionary<DbInstance, ImmutableDictionary<string, Descriptor>> SqlProperties    = SQL.CreateDescriptorMapping<TRecord>();
    protected readonly     ConcurrentDictionary<int, string>                                        _delete          = new();
    protected readonly     ConcurrentDictionary<int, string>                                        _deleteIDs       = new();
    protected readonly     ConcurrentDictionary<int, string>                                        _exists          = new();
    protected readonly     ConcurrentDictionary<int, string>                                        _get             = new();
    protected readonly     ConcurrentDictionary<int, string>                                        _whereParameters = new();
    protected readonly     ConcurrentDictionary<string, string>                                     _commons         = new();
    protected readonly     ConcurrentDictionary<string, string>                                     _getID           = new();
    protected readonly     ConcurrentDictionary<string, string>                                     _whereColumn     = new();
    protected readonly     DbOptions                                                                _dbOptions;


    public          string     CreatedBy    { get; init; }
    public          string     DateCreated  { get; init; }
    public          string     IdColumnName { get; init; }
    public abstract DbInstance Instance     { get; }
    public          string     LastModified { get; init; }
    public          string     OwnerUserID  { get; init; }
    public          string     RandomMethod { get; init; }
    public virtual  string     TableName    => TRecord.TableName;


    protected BaseSqlCache( IOptions<DbOptions> dbOptions )
    {
        _dbOptions   = dbOptions.Value;
        RandomMethod = Instance.GetRandomMethod();
        CreatedBy    = Instance.GetCreatedBy();
        DateCreated  = Instance.GetDateCreated();
        IdColumnName = Instance.GetID_ColumnName();
        LastModified = Instance.GetLastModified();
        OwnerUserID  = Instance.GetOwnerUserID();
    }


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] protected IEnumerable<string> GetDescriptors( DynamicParameters   parameters ) => parameters.ParameterNames.Select( name => SqlProperties[Instance][name].KeyValuePair );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] protected IEnumerable<string> GetKeyValuePairs( DynamicParameters parameters ) => parameters.ParameterNames.Select( KeyValuePair );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] protected string              KeyValuePair( string                columnName ) => SqlProperties[Instance][columnName].KeyValuePair;


    public virtual SqlCommand All() => $"SELECT * FROM {TableName}";


    public virtual SqlCommand Delete( in IEnumerable<RecordID<TRecord>> ids )
    {
        ConcurrentDictionary<int, string> dictionary = _deleteIDs;
        int                               hash       = ids.GetHash();

        if ( hash > 0 && dictionary.TryGetValue( hash, out string? sql ) )
        {
            Debug.Assert( sql is not null );
            return new SqlCommand( sql );
        }

        using var buffer = new ValueStringBuilder( 1000 );
        buffer.AppendJoin( SQL.LIST_SEPARATOR, ids, SQL.GUID_FORMAT );

        dictionary[hash] = sql = $"DELETE FROM {TableName} WHERE {IdColumnName} in ( {buffer.Span} );";
        return new SqlCommand( sql );
    }


    public virtual SqlCommand Delete( in IEnumerable<Guid> ids )
    {
        ConcurrentDictionary<int, string> dictionary = _delete;

        int hash = ids.GetHash();

        if ( hash > 0 && dictionary.TryGetValue( hash, out string? sql ) )
        {
            Debug.Assert( sql is not null );
            return sql;
        }

        using var buffer = new ValueStringBuilder( 1000 );
        buffer.AppendJoin( SQL.LIST_SEPARATOR, ids, SQL.GUID_FORMAT );

        return dictionary[hash] = $"DELETE FROM {TableName} WHERE {IdColumnName} in ( {buffer.Span} );";
    }


    public virtual SqlCommand Delete( in bool matchAll, in DynamicParameters parameters )
    {
        ConcurrentDictionary<int, string> dictionary = _delete;

        int hash = parameters.GetHash();

        if ( hash > 0 && dictionary.TryGetValue( hash, out string? sql ) )
        {
            Debug.Assert( sql is not null );
            return new SqlCommand( sql, parameters );
        }

        using var buffer = new ValueStringBuilder( 1000 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        dictionary[hash] = sql = $"DELETE FROM {TableName} WHERE {buffer.Span};";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Where<TValue>( string columnName, TValue? value )
    {
        ConcurrentDictionary<string, string> dictionary = _whereColumn;

        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        if ( dictionary.TryGetValue( columnName, out string? sql ) )
        {
            Debug.Assert( sql is not null );
            return new SqlCommand( sql, parameters );
        }

        dictionary[columnName] = sql = $"SELECT * FROM {TableName} WHERE {columnName} = @{nameof(value)}";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Where( in bool matchAll, in DynamicParameters parameters )
    {
        ConcurrentDictionary<int, string> dictionary = _whereParameters;

        int hash = parameters.GetHash();

        if ( hash > 0 && dictionary.TryGetValue( hash, out string? sql ) )
        {
            Debug.Assert( sql is not null );
            return new SqlCommand( sql, parameters );
        }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );

        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        dictionary[hash] = sql = $"SELECT * FROM {TableName} WHERE {buffer.Span}";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand WhereID<TValue>( string columnName, TValue? value )
    {
        ConcurrentDictionary<string, string> dictionary = _whereColumn;

        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        if ( dictionary.TryGetValue( columnName, out string? sql ) )
        {
            Debug.Assert( sql is not null );
            return new SqlCommand( sql, parameters );
        }

        dictionary[columnName] = sql = $"SELECT {IdColumnName} FROM {TableName} WHERE {columnName} = @{nameof(value)}";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand GetExists( in bool matchAll, in DynamicParameters parameters )
    {
        ConcurrentDictionary<int, string> dictionary = _exists;

        int hash = parameters.GetHash();

        if ( hash > 0 && dictionary.TryGetValue( hash, out string? sql ) )
        {
            Debug.Assert( sql is not null );
            return new SqlCommand( sql, parameters );
        }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );
        buffer.AppendJoin( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) );

        dictionary[hash] = sql = Instance switch
                                 {
                                     DbInstance.MsSql    => $"SELECT TOP 1 {IdColumnName} FROM {TableName} WHERE {buffer.Span}",
                                     DbInstance.Postgres => $"SELECT {IdColumnName} FROM {TableName} WHERE {buffer.Span} LIMIT 1",
                                     _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                                 };

        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Get( in bool matchAll, in DynamicParameters parameters )
    {
        ConcurrentDictionary<int, string> dictionary = _get;

        int hash = parameters.GetHash();

        if ( hash > 0 && dictionary.TryGetValue( hash, out string? sql ) )
        {
            Debug.Assert( sql is not null );
            return new SqlCommand( sql, parameters );
        }

        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );

        buffer.AppendJoin( matchAll.GetAndOr(), GetDescriptors( parameters ) );

        dictionary[hash] = sql = $"SELECT * FROM {TableName} WHERE {buffer.Span}";
        return new SqlCommand( sql, parameters );
    }


    public virtual SqlCommand Get( in IEnumerable<Guid> ids )
    {
        ConcurrentDictionary<int, string> dictionary = _get;

        int hash = ids.GetHash();

        if ( hash > 0 && dictionary.TryGetValue( hash, out string? sql ) )
        {
            Debug.Assert( sql is not null );
            return new SqlCommand( sql );
        }

        using var buffer = new ValueStringBuilder( 1000 );
        buffer.AppendJoin( SQL.LIST_SEPARATOR, ids, SQL.GUID_FORMAT );

        dictionary[hash] = sql = $"SELECT * FROM {TableName} WHERE {IdColumnName} in ( {buffer.Span} )";
        return new SqlCommand( sql );
    }
}
