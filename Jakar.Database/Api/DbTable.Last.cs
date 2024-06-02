// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:13 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?> Last( Activity? activity, CancellationToken token = default ) => this.Call( Last, activity, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Last( DbConnection connection, DbTransaction? transaction, Activity? activity, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Last();

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    public ValueTask<TRecord?> LastOrDefault( Activity? activity, CancellationToken token = default ) => this.Call( LastOrDefault, activity, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> LastOrDefault( DbConnection connection, DbTransaction? transaction, Activity? activity, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Last();

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            return await connection.QueryFirstOrDefaultAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
