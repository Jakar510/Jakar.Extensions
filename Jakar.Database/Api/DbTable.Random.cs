// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:14 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?>       Random( CancellationToken token                                                   = default ) => this.Call( Random, token );
    public IAsyncEnumerable<TRecord> Random( int               count, [EnumeratorCancellation] CancellationToken token = default ) => this.Call( Random, count, token );

    public IAsyncEnumerable<TRecord> Random( UserRecord user, int count, [EnumeratorCancellation] CancellationToken token = default ) => this.Call( Random, user, count, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Random();

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, sql, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual IAsyncEnumerable<TRecord> Random( DbConnection connection, DbTransaction? transaction, UserRecord user, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Random( user.OwnerUserID, count );
        return Where( connection, transaction, sql, token );
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual IAsyncEnumerable<TRecord> Random( DbConnection connection, DbTransaction? transaction, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Random( count );
        return Where( connection, transaction, sql, token );
    }
}
