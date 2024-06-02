// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?> Single( Activity?          activity, RecordID<TRecord> id,  CancellationToken  token                               = default ) => this.Call( Single, activity, id,  token );
    public ValueTask<TRecord?> Single( Activity?          activity, string            sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Single, activity, sql, parameters, token );
    public ValueTask<TRecord?> SingleOrDefault( Activity? activity, RecordID<TRecord> id,  CancellationToken  token                               = default ) => this.Call( SingleOrDefault, activity, id,  token );
    public ValueTask<TRecord?> SingleOrDefault( Activity? activity, string            sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( SingleOrDefault, activity, sql, parameters, token );


    public ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, Activity? activity, RecordID<TRecord> id,  CancellationToken  token                               = default ) => Single( connection, transaction, activity, _sqlCache.Single( id ),            token );
    public ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, Activity? activity, string            sql, DynamicParameters? parameters, CancellationToken token = default ) => Single( connection, transaction, activity, new SqlCommand( sql, parameters ), token );
    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, Activity? activity, SqlCommand sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            return await connection.QuerySingleAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    public ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, Activity? activity, RecordID<TRecord> id,  CancellationToken  token                               = default ) => SingleOrDefault( connection, transaction, activity, _sqlCache.Single( id ),            token );
    public ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, Activity? activity, string            sql, DynamicParameters? parameters, CancellationToken token = default ) => SingleOrDefault( connection, transaction, activity, new SqlCommand( sql, parameters ), token );
    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, Activity? activity, SqlCommand sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            return await connection.QuerySingleOrDefaultAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
