// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:13 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask<ErrorOrResult<TSelf>> Last( CancellationToken token = default ) => this.Call(Last, token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TSelf>> Last( NpgsqlConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetLast();

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            return await connection.QueryFirstAsync<TSelf>(command);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }


    public ValueTask<ErrorOrResult<TSelf>> LastOrDefault( CancellationToken token = default ) => this.Call(LastOrDefault, token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TSelf>> LastOrDefault( NpgsqlConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetLast();

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            TSelf?           record  = await connection.QueryFirstOrDefaultAsync<TSelf>(command);

            return record is null
                       ? Error.NotFound()
                       : record;
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }
}
