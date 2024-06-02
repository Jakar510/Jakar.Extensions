// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:07 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<long>           Count( Activity?  activity, CancellationToken                   token = default )                                                   => this.Call( Count, activity, token );
    public ValueTask<bool>           Exists( Activity? activity, bool                                matchAll,   DynamicParameters parameters, CancellationToken token ) => this.TryCall( Exists, activity, matchAll, parameters, token );
    public IAsyncEnumerable<TRecord> Get( Activity?    activity, IEnumerable<RecordID<TRecord>>      ids,        CancellationToken token                               = default ) => this.Call( Get, activity, ids,        token );
    public IAsyncEnumerable<TRecord> Get( Activity?    activity, IAsyncEnumerable<RecordID<TRecord>> ids,        CancellationToken token                               = default ) => this.Call( Get, activity, ids,        token );
    public ValueTask<TRecord?>       Get( Activity?    activity, bool                                matchAll,   DynamicParameters parameters, CancellationToken token = default ) => this.Call( Get, activity, matchAll,   parameters, token );
    public ValueTask<TRecord?>       Get( Activity?    activity, string                              columnName, object?           value,      CancellationToken token = default ) => this.Call( Get, activity, columnName, value,      token );
    public ValueTask<TRecord?>       Get( Activity?    activity, RecordID<TRecord>                   id,         CancellationToken token = default ) => this.Call( Get, activity, id, token );
    public ValueTask<TRecord?>       Get( Activity?    activity, RecordID<TRecord>?                  id,         CancellationToken token = default ) => this.Call( Get, activity, id, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<long> Count( DbConnection connection, DbTransaction? transaction, Activity? activity, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Count();

        try { return await connection.QueryFirstAsync<long>( sql.SQL, sql.Parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<bool> Exists( DbConnection connection, DbTransaction transaction, Activity? activity, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        SqlCommand sql = _sqlCache.Exists( matchAll, parameters );

        try
        {
            CommandDefinition   command = _database.GetCommand( activity, sql, transaction, token );
            IEnumerable<string> results = await connection.QueryAsync<string>( command );
            return results.Any();
        }
        catch ( Exception e ) { throw new SqlException( sql.SQL, parameters, e ); }
    }


    public virtual async IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, Activity? activity, IAsyncEnumerable<RecordID<TRecord>> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        HashSet<RecordID<TRecord>> set = await ids.ToHashSet( token );
        await foreach ( TRecord record in Get( connection, transaction, activity, set, token ) ) { yield return record; }
    }

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, Activity? activity, IEnumerable<RecordID<TRecord>> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Get( ids );
        return Where( connection, transaction, activity, sql, token );
    }


    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Activity? activity, RecordID<TRecord>? id, CancellationToken token = default ) => id.HasValue
                                                                                                                                                                                     ? await Get( connection, transaction, activity, id.Value, token )
                                                                                                                                                                                     : null;
    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Activity? activity, RecordID<TRecord> id, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Get( id );

        try
        {
            CommandDefinition     command = _database.GetCommand( activity, sql, transaction, token );
            IEnumerable<TRecord?> results = await connection.QueryAsync<TRecord>( command );
            IEnumerable<TRecord>  records = results.WhereNotNull();
            TRecord?              result  = null;

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


    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Activity? activity, string columnName, object? value, CancellationToken token = default ) => await Get( connection, transaction, activity, true, Database.GetParameters( value, default, columnName ), token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Activity? activity, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Get( matchAll, parameters );

        try
        {
            CommandDefinition     command = _database.GetCommand( activity, sql, transaction, token );
            IEnumerable<TRecord?> results = await connection.QueryAsync<TRecord>( command );
            IEnumerable<TRecord>  records = results.WhereNotNull();
            TRecord?              result  = null;

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
