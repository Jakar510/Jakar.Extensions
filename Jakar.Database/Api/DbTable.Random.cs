// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:14 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?>       Random( Activity? activity, CancellationToken token                                                                                                     = default ) => this.Call( Random, activity, token );
    public IAsyncEnumerable<TRecord> Random( Activity? activity, int               count, [EnumeratorCancellation] CancellationToken token                                                   = default ) => this.Call( Random, activity, count, token );
    public IAsyncEnumerable<TRecord> Random( Activity? activity, UserRecord        user,  int                                        count, [EnumeratorCancellation] CancellationToken token = default ) => this.Call( Random, activity, user,  count, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, Activity? activity, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Random();

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual IAsyncEnumerable<TRecord> Random( DbConnection connection, DbTransaction? transaction, Activity? activity, UserRecord user, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Random( user.ID.Value, count );
        return Where( connection, transaction, activity, sql, token );
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual IAsyncEnumerable<TRecord> Random( DbConnection connection, DbTransaction? transaction, Activity? activity, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Random( count );
        return Where( connection, transaction, activity, sql, token );
    }
}
