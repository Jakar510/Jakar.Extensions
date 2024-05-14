// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:07 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<long>           Count( CancellationToken                 token = default )                                                   => this.Call( Count, token );
    public ValueTask<bool>           Exists( bool                             matchAll,   DynamicParameters parameters, CancellationToken token ) => this.TryCall( Exists, matchAll, parameters, token );
    public IAsyncEnumerable<TRecord> Get( IEnumerable<RecordID<TRecord>>      ids,        CancellationToken token                               = default ) => this.Call( Get, ids,        token );
    public IAsyncEnumerable<TRecord> Get( IAsyncEnumerable<RecordID<TRecord>> ids,        CancellationToken token                               = default ) => this.Call( Get, ids,        token );
    public ValueTask<TRecord?>       Get( bool                                matchAll,   DynamicParameters parameters, CancellationToken token = default ) => this.Call( Get, matchAll,   parameters, token );
    public ValueTask<TRecord?>       Get( string                              columnName, object?           value,      CancellationToken token = default ) => this.Call( Get, columnName, value,      token );
    public ValueTask<TRecord?>       Get( RecordID<TRecord>                   id,         CancellationToken token = default ) => this.Call( Get, id, token );
    public ValueTask<TRecord?>       Get( RecordID<TRecord>?                  id,         CancellationToken token = default ) => this.Call( Get, id, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<long> Count( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Count();

        try { return await connection.QueryFirstAsync<long>( sql.SQL, sql.Parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<bool> Exists( DbConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        SqlCommand sql = _sqlCache.Exists( matchAll, parameters );

        try
        {
            CommandDefinition   command = _database.GetCommand( sql, transaction, token );
            IEnumerable<string> results = await connection.QueryAsync<string>( command );
            return results.Any();
        }
        catch ( Exception e ) { throw new SqlException( sql.SQL, parameters, e ); }
    }


    public virtual async IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<RecordID<TRecord>> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        HashSet<RecordID<TRecord>> set = await ids.ToHashSet( token );
        await foreach ( TRecord record in Get( connection, transaction, set, token ) ) { yield return record; }
    }

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IEnumerable<RecordID<TRecord>> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Get( ids );
        return Where( connection, transaction, sql, token );
    }


    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, RecordID<TRecord>? id, CancellationToken token = default ) =>
        id.HasValue
            ? await Get( connection, transaction, id.Value, token )
            : default;
    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, RecordID<TRecord> id, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Get( id );

        try
        {
            CommandDefinition     command = _database.GetCommand( sql, transaction, token );
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
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, string columnName, object? value, CancellationToken token = default ) =>
        await Get( connection, transaction, true, Database.GetParameters( value, default, columnName ), token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Get( matchAll, parameters );

        try
        {
            CommandDefinition     command = _database.GetCommand( sql, transaction, token );
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
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
