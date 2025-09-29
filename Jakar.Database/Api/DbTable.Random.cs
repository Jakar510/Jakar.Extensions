// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:14 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TClass>
{
    public ValueTask<ErrorOrResult<TClass>> Random( CancellationToken token                                                                                                     = default ) => this.Call(Random, token);
    public IAsyncEnumerable<TClass>         Random( int               count, [EnumeratorCancellation] CancellationToken token                                                   = default ) => this.Call(Random, count, token);
    public IAsyncEnumerable<TClass>         Random( UserRecord        user,  int                                        count, [EnumeratorCancellation] CancellationToken token = default ) => this.Call(Random, user,  count, token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TClass>> Random( NpgsqlConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetRandom();

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            return await connection.QueryFirstAsync<TClass>(command);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual IAsyncEnumerable<TClass> Random( NpgsqlConnection connection, DbTransaction? transaction, UserRecord user, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetRandom(user, count);
        return Where(connection, transaction, sql, token);
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual IAsyncEnumerable<TClass> Random( NpgsqlConnection connection, DbTransaction? transaction, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetRandom(count);
        return Where(connection, transaction, sql, token);
    }
}
