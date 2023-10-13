// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?> Single( string          id,  CancellationToken  token                               = default ) => this.Call( Single, id,  token );
    public ValueTask<TRecord?> Single( string          sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Single, sql, parameters, token );
    public ValueTask<TRecord?> SingleOrDefault( string id,  CancellationToken  token                               = default ) => this.Call( SingleOrDefault, id,  token );
    public ValueTask<TRecord?> SingleOrDefault( string sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( SingleOrDefault, sql, parameters, token );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, string id, CancellationToken token = default )
    {
        DynamicParameters parameters = Database.GetParameters( id, variableName: nameof(TableRecord<TRecord>.ID) );
        string            sql        = _cache[Instance, SqlStatement.Single];

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            return await connection.QuerySingleAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        try { return await connection.QuerySingleAsync<TRecord>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, string id, CancellationToken token = default )
    {
        DynamicParameters parameters = Database.GetParameters( id, variableName: ID );
        string            sql        = _cache[Instance, SqlStatement.Single];

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            return await connection.QuerySingleOrDefaultAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            return await connection.QuerySingleOrDefaultAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
}
