// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:16 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask<ErrorOrResult<TSelf>>           Next( RecordPair<TSelf>     pair, CancellationToken token = default ) => this.Call(Next,   pair, token);
    public ValueTask<Guid?>                           NextID( RecordPair<TSelf>   pair, CancellationToken token = default ) => this.Call(NextID, pair, token);
    public ValueTask<IEnumerable<RecordPair<TSelf>>> SortedIDs( CancellationToken token = default ) => this.Call(SortedIDs, token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TSelf>> Next( NpgsqlConnection connection, NpgsqlTransaction? transaction, RecordPair<TSelf> pair, CancellationToken token = default )
    {
        SqlCommand<TSelf> sql = SqlCommand<TSelf>.GetNext(in pair);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            TSelf?           record  = await connection.ExecuteScalarAsync<TSelf>(command);

            return record is null
                       ? Error.NotFound()
                       : record;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(sql, e); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<IEnumerable<RecordPair<TSelf>>> SortedIDs( NpgsqlConnection connection, NpgsqlTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand<TSelf> sql = SqlCommand<TSelf>.GetSortedID();

        try
        {
            CommandDefinition               command = _database.GetCommand(in sql, transaction, token);
            IEnumerable<RecordPair<TSelf>> pairs   = await connection.QueryAsync<RecordPair<TSelf>>(command);
            return pairs;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(sql, e); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<Guid?> NextID( NpgsqlConnection connection, NpgsqlTransaction? transaction, RecordPair<TSelf> pair, CancellationToken token = default )
    {
        SqlCommand<TSelf> sql = SqlCommand<TSelf>.GetNextID(in pair);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            return await connection.ExecuteScalarAsync<Guid>(command);
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(sql, e); }
    }
}
