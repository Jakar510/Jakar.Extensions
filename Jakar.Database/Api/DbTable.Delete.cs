// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:06 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
    public ValueTask Delete( TRecord                             record,   CancellationToken token                               = default ) => this.TryCall( Delete, record,   token );
    public ValueTask Delete( IEnumerable<TRecord>                records,  CancellationToken token                               = default ) => this.TryCall( Delete, records,  token );
    public ValueTask Delete( IAsyncEnumerable<TRecord>           records,  CancellationToken token                               = default ) => this.TryCall( Delete, records,  token );
    public ValueTask Delete( Guid                                id,       CancellationToken token                               = default ) => this.TryCall( Delete, id,       token );
    public ValueTask Delete( IEnumerable<Guid>                   ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( IAsyncEnumerable<Guid>              ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( RecordID<TRecord>                   id,       CancellationToken token                               = default ) => this.TryCall( Delete, id,       token );
    public ValueTask Delete( IEnumerable<RecordID<TRecord>>      ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( IAsyncEnumerable<RecordID<TRecord>> ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( bool                                matchAll, DynamicParameters parameters, CancellationToken token = default ) => this.TryCall( Delete, matchAll, parameters, token );

    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, TRecord              record,  CancellationToken token = default ) => Delete( connection, transaction, record.ID,                   token );
    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, CancellationToken token = default ) => Delete( connection, transaction, records.Select( x => x.ID ), token );
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        HashSet<TRecord> ids = await records.ToHashSet( token );
        await Delete( connection, transaction, ids, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<RecordID<TRecord>> ids, CancellationToken token = default )
    {
        HashSet<RecordID<TRecord>> records = await ids.ToHashSet( token );
        await Delete( connection, transaction, records, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, RecordID<TRecord> id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(id), id );

        _delete ??= $"DELETE FROM {SchemaTableName} WHERE {ID_ColumnName} = @{nameof(id)};";

        CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( _delete, parameters ), token );
        await connection.ExecuteScalarAsync( command );
    }
    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<RecordID<TRecord>> ids, CancellationToken token = default ) => await Delete( connection, transaction, ids.Select( x => x.Value ), token );
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<Guid> ids, CancellationToken token = default )
    {
        HashSet<Guid> records = await ids.ToHashSet( token );
        await Delete( connection, transaction, records, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, Guid id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(id), id );

        _delete ??= $"DELETE FROM {SchemaTableName} WHERE {ID_ColumnName} = @{nameof(id)};";

        CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( _delete, parameters ), token );
        await connection.ExecuteScalarAsync( command );
    }

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<Guid> ids, CancellationToken token = default )
    {
        SqlCommand sql = $"DELETE FROM {SchemaTableName} WHERE {ID_ColumnName} in ( {string.Join( ',', ids.Select( x => $"'{x}'" ) )} );";

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, sql, token );
            await connection.ExecuteScalarAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql.SQL, e ); }
    }
    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        int     hash = GetHash( parameters );
        string? sql  = default;
        if ( hash > 0 && !_deleteGuids.TryGetValue( hash, out sql ) ) { _deleteGuids[hash] = sql = GetDeleteSql( matchAll, parameters ); }

        sql ??= GetDeleteSql( matchAll, parameters );

        CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( sql ), token );
        await connection.ExecuteScalarAsync( command );
    }


    private string GetDeleteSql( bool matchAll, DynamicParameters parameters ) =>
        $"DELETE FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                ? "AND"
                                                                : "OR",
                                                            parameters.ParameterNames.Select( KeyValuePair ) )};";
}
