﻿// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:06 PM

namespace Jakar.Database;


public partial class DbTable<TRecord>
{
    public ValueTask Delete( TRecord                   record,   CancellationToken token                               = default ) => this.TryCall( Delete, record,   token );
    public ValueTask Delete( IEnumerable<TRecord>      records,  CancellationToken token                               = default ) => this.TryCall( Delete, records,  token );
    public ValueTask Delete( IAsyncEnumerable<TRecord> records,  CancellationToken token                               = default ) => this.TryCall( Delete, records,  token );
    public ValueTask Delete( Guid                      id,       CancellationToken token                               = default ) => this.TryCall( Delete, id,       token );
    public ValueTask Delete( IEnumerable<Guid>         ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( IAsyncEnumerable<Guid>    ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( bool                      matchAll, DynamicParameters parameters, CancellationToken token = default ) => this.TryCall( Delete, matchAll, parameters, token );


    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, TRecord              record,  CancellationToken token = default ) => Delete( connection, transaction, record.ID,                   token );
    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, CancellationToken token = default ) => Delete( connection, transaction, records.Select( x => x.ID ), token );
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        HashSet<TRecord> ids = await records.ToHashSet( token );
        await Delete( connection, transaction, ids, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<Guid> ids, CancellationToken token = default )
    {
        HashSet<Guid> records = await ids.ToHashSet( token );
        await Delete( connection, transaction, records, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, Guid id, CancellationToken token = default )
    {
        string cmd = $"DELETE FROM {SchemaTableName} WHERE {ID} = {id};";

        if ( token.IsCancellationRequested ) { return; }

        await connection.ExecuteScalarAsync( cmd, default, transaction );
    }
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<Guid> ids, CancellationToken token = default )
    {
        string sql = $"DELETE FROM {SchemaTableName} WHERE {ID} in ( {string.Join( ',', ids.Select( x => $"'{x}'" ) )} );";

        if ( token.IsCancellationRequested ) { return; }

        try { await connection.ExecuteScalarAsync( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        string cmd = $"DELETE FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                             ? "AND"
                                                                             : "OR",
                                                                         parameters.ParameterNames.Select( KeyValuePair ) )};";

        if ( token.IsCancellationRequested ) { return; }

        await connection.ExecuteScalarAsync( cmd, parameters, transaction );
    }
}