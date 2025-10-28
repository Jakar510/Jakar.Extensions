// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:16 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask<ErrorOrResult<TSelf>>           Next( RecordPair<TSelf>      pair, CancellationToken token = default ) => this.Call(Next,   pair, token);
    public ValueTask<Guid?>                          NextID( RecordPair<TSelf>    pair, CancellationToken token = default ) => this.Call(NextID, pair, token);
    public ValueTask<IEnumerable<RecordPair<TSelf>>> SortedIDs( CancellationToken token = default ) => this.Call(SortedIDs, token);


    public virtual async ValueTask<ErrorOrResult<TSelf>> Next( NpgsqlConnection connection, NpgsqlTransaction? transaction, RecordPair<TSelf> pair, CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.GetNext(in pair);

        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            ErrorOrResult<TSelf>         record = await reader.SingleAsync<TSelf>(token);
            return record;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }


    public virtual async ValueTask<IEnumerable<RecordPair<TSelf>>> SortedIDs( NpgsqlConnection connection, NpgsqlTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.GetSortedID();

        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            List<RecordPair<TSelf>>      pairs  = [];
            while ( await reader.ReadAsync(token) ) { pairs.Add(RecordPair<TSelf>.Create(reader)); }

            return pairs;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }


    public virtual async ValueTask<Guid?> NextID( NpgsqlConnection connection, NpgsqlTransaction? transaction, RecordPair<TSelf> pair, CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.GetNextID(in pair);

        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            Guid?                        id     = null;
            if ( await reader.ReadAsync(token) ) { id = reader.GetGuid(0); }

            return id;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }
}
