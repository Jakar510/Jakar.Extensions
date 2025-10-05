// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:14 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask<ErrorOrResult<TSelf>> Random( CancellationToken token                                                                                                     = default ) => this.Call(Random, token);
    public IAsyncEnumerable<TSelf>         Random( int               count, [EnumeratorCancellation] CancellationToken token                                                   = default ) => this.Call(Random, count, token);
    public IAsyncEnumerable<TSelf>         Random( UserRecord        user,  int                                        count, [EnumeratorCancellation] CancellationToken token = default ) => this.Call(Random, user,  count, token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TSelf>> Random( NpgsqlConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetRandom();

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            return await connection.QueryFirstAsync<TSelf>(command);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual IAsyncEnumerable<TSelf> Random( NpgsqlConnection connection, DbTransaction? transaction, UserRecord user, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetRandom(user, count);
        return Where(connection, transaction, sql, token);
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual IAsyncEnumerable<TSelf> Random( NpgsqlConnection connection, DbTransaction? transaction, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetRandom(count);
        return Where(connection, transaction, sql, token);
    }
}
