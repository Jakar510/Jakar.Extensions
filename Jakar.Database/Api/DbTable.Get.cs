// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:07 PM

using Newtonsoft.Json.Linq;



namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
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
        string sql = _cache[Instance][SqlStatement.Count];

        try { return await connection.QueryFirstAsync<long>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<bool> Exists( DbConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        SqlCommand sql = GetExistsSql( matchAll, parameters );

        try
        {
            CommandDefinition   command = _database.GetCommandDefinition( transaction, sql, token );
            IEnumerable<string> results = await connection.QueryAsync<string>( command );
            return results.Any();
        }
        catch ( Exception e ) { throw new SqlException( sql.SQL, parameters, e ); }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<Guid?> GetID<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, CancellationToken token = default )
    {
        SqlCommand sql = GetWhereIDSql( columnName, value );

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, sql, token );
            return await connection.QuerySingleAsync<Guid?>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Guid? id, CancellationToken token = default ) =>
        id.HasValue
            ? await Get( connection, transaction, ID_ColumnName, id.Value, token )
            : default;
    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Guid id, CancellationToken token = default ) =>
        await Get( connection, transaction, ID_ColumnName, id, token );

    public virtual async IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<Guid> ids, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        HashSet<Guid> set = await ids.ToHashSet( token );
        await foreach ( TRecord record in Get( connection, transaction, set, token ) ) { yield return record; }
    }

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IEnumerable<Guid> ids, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        SqlCommand sql = Get_GetSql( ids );
        return Where( connection, transaction, sql, token );
    }


    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, RecordID<TRecord>? id, CancellationToken token = default ) => await Get( connection, transaction, ID_ColumnName, id?.Value, token );
    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, RecordID<TRecord>  id, CancellationToken token = default ) => await Get( connection, transaction, ID_ColumnName, id.Value,  token );

    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, string columnName, object? value, CancellationToken token = default ) =>
        await Get( connection, transaction, true, Database.GetParameters( value, default, columnName ), token );

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = Get_GetSql( matchAll, parameters );

        try
        {
            CommandDefinition     command = _database.GetCommandDefinition( transaction, sql, token );
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
