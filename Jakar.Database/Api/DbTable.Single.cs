// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?> Single( RecordID<TRecord>          id,  CancellationToken  token                               = default ) => this.Call( Single, id,  token );
    public ValueTask<TRecord?> Single( string                     sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Single, sql, parameters, token );
    public ValueTask<TRecord?> SingleOrDefault( RecordID<TRecord> id,  CancellationToken  token                               = default ) => this.Call( SingleOrDefault, id,  token );
    public ValueTask<TRecord?> SingleOrDefault( string            sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( SingleOrDefault, sql, parameters, token );


    public ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, RecordID<TRecord> id, CancellationToken token = default ) => Single( connection, transaction, SqlCache.Single( id ), token );
    public ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default ) => Single( connection, transaction, new SqlCommand( sql, parameters ), token );
    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, SqlCommand sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, sql, token );
            return await connection.QuerySingleAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    public ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, RecordID<TRecord> id, CancellationToken token = default ) => SingleOrDefault( connection, transaction, SqlCache.Single( id ), token );
    public ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default ) =>
        SingleOrDefault( connection, transaction, new SqlCommand( sql, parameters ), token );
    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, SqlCommand sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, sql, token );
            return await connection.QuerySingleOrDefaultAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
