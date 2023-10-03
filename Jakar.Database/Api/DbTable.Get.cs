// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:07 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
    private readonly ConcurrentDictionary<int, string>    _existsMsSql    = new();
    private readonly ConcurrentDictionary<int, string>    _existsPostgres = new();
    private readonly ConcurrentDictionary<int, string>    _get            = new();
    private readonly ConcurrentDictionary<int, string>    _getGuid        = new();
    private readonly ConcurrentDictionary<string, string> _getID          = new();
    private          string?                              _count;


    public ValueTask<long>           Count( CancellationToken    token = default )                                                              => this.Call( Count, token );
    public ValueTask<bool>           Exists( bool                matchAll,   DynamicParameters  parameters, CancellationToken token )           => this.TryCall( Exists, matchAll, parameters, token );
    public ValueTask<Guid?>          GetID( string               sql,        DynamicParameters? parameters, CancellationToken token = default ) => this.Call( GetID, sql,        parameters, token );
    public ValueTask<Guid?>          GetID( string               columnName, object             value,      CancellationToken token = default ) => this.Call( GetID, columnName, value,      token );
    public IAsyncEnumerable<TRecord> Get( IEnumerable<Guid>      ids,        CancellationToken  token                               = default ) => this.Call( Get, ids,        token );
    public IAsyncEnumerable<TRecord> Get( IAsyncEnumerable<Guid> ids,        CancellationToken  token                               = default ) => this.Call( Get, ids,        token );
    public ValueTask<TRecord?>       Get( bool                   matchAll,   DynamicParameters  parameters, CancellationToken token = default ) => this.Call( Get, matchAll,   parameters, token );
    public ValueTask<TRecord?>       Get( string                 columnName, object?            value,      CancellationToken token = default ) => this.Call( Get, columnName, value,      token );
    public ValueTask<TRecord?>       Get( Guid                   id,         CancellationToken  token = default ) => this.Call( Get, id, token );
    public ValueTask<TRecord?>       Get( Guid?                  id,         CancellationToken  token = default ) => this.Call( Get, id, token );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<long> Count( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        _count ??= $"SELECT COUNT({ID_ColumnName}) FROM {SchemaTableName}";

        try { return await connection.QueryFirstAsync<long>( _count, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( _count, e ); }
    }

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<bool> Exists( DbConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        int     hash = GetHash( parameters );
        string? sql  = default;

        if ( hash > 0 && !_existsMsSql.TryGetValue( hash, out sql ) ) { _existsMsSql[hash] = sql = GetExistsSql( matchAll, parameters ); }

        if ( hash > 0 && !_existsPostgres.TryGetValue( hash, out sql ) ) { _existsPostgres[hash] = sql = GetExistsSql( matchAll, parameters ); }

        sql ??= GetExistsSql( matchAll, parameters );

        try
        {
            CommandDefinition   command = _database.GetCommandDefinition( sql, parameters, transaction, token );
            IEnumerable<string> results = await connection.QueryAsync<string>( command );
            return results.Any();
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
    private string GetExistsSql( bool matchAll, DynamicParameters parameters ) =>
        Instance switch
        {
            DbInstance.MsSql => $"SELECT TOP 1 {ID_ColumnName} FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                                              ? "AND"
                                                                                                              : "OR",
                                                                                                          parameters.ParameterNames.Select( KeyValuePair ) )}",
            DbInstance.Postgres => $"SELECT {ID_ColumnName} FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                                           ? "AND"
                                                                                                           : "OR",
                                                                                                       parameters.ParameterNames.Select( KeyValuePair ) )} LIMIT 1",
            _ => throw new OutOfRangeException( nameof(Instance), Instance )
        };


    public async ValueTask<Guid?> GetID( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommandDefinition( sql, parameters, transaction, token );
            return await connection.QuerySingleAsync<Guid?>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<Guid?> GetID( DbConnection connection, DbTransaction? transaction, string columnName, object value, CancellationToken token = default )
    {
        if ( !_getID.TryGetValue( columnName, out string? sql ) ) { _getID[columnName] = sql = $"SELECT {ID_ColumnName} FROM {SchemaTableName} WHERE {columnName} = @{nameof(value)}"; }

        return await GetID( connection, transaction, sql, Database.GetParameters( value ), token );
    }


    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Guid? id, CancellationToken token = default ) =>
        id.HasValue
            ? await Get( connection, transaction, ID_ColumnName, id.Value, token )
            : default;
    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Guid id, CancellationToken token = default ) =>
        await Get( connection, transaction, ID_ColumnName, id, token );


    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, RecordID<TRecord>? id, CancellationToken token = default ) => await Get( connection, transaction, ID_ColumnName, id?.Value, token );
    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, RecordID<TRecord>  id, CancellationToken token = default ) => await Get( connection, transaction, ID_ColumnName, id.Value,  token );


    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, string columnName, object? value, CancellationToken token = default ) =>
        await Get( connection, transaction, true, Database.GetParameters( value, default, columnName ), token );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        int hash = GetHash( parameters );

        if ( !_get.TryGetValue( hash, out string? sql ) )
        {
            _get[hash] = sql = $"SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                         ? "AND"
                                                                                         : "OR",
                                                                                     parameters.ParameterNames.Select( x => GetDescriptor( x ).KeyValuePair ) )}";
        }

        try
        {
            CommandDefinition     command = _database.GetCommandDefinition( sql, parameters, transaction, token );
            IEnumerable<TRecord?> results = await connection.QueryAsync<TRecord>( command );
            IEnumerable<TRecord>  records = results.WhereNotNull();
            TRecord?              result  = default;

            // ReSharper disable once PossibleMultipleEnumeration
            foreach ( TRecord record in records )
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if ( result is not null ) { throw new DuplicateRecordException( $"Record IDs: {string.Join( ',', records.Select( x => x.ID ) )}" ); }

                result = record;
            }

            return result;
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public virtual async IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<Guid> ids, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        HashSet<Guid> set = await ids.ToHashSet( token );
        await foreach ( TRecord record in Get( connection, transaction, set, token ) ) { yield return record; }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IEnumerable<Guid> ids, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {ID_ColumnName} in ( {string.Join( ',', ids.Select( x => $"'{x}'" ) )} )";

        return Where( connection, transaction, sql, default, token );
    }
}
