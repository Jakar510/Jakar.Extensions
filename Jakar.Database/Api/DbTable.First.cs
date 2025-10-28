// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask<ErrorOrResult<TSelf>> First( CancellationToken          token = default ) => this.Call(First,          token);
    public ValueTask<ErrorOrResult<TSelf>> FirstOrDefault( CancellationToken token = default ) => this.Call(FirstOrDefault, token);


    public virtual async ValueTask<ErrorOrResult<TSelf>> First( NpgsqlConnection connection, NpgsqlTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.GetFirst();

        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            ErrorOrResult<TSelf>         record = await reader.FirstAsync<TSelf>(token);
            return record;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }
    public virtual async ValueTask<ErrorOrResult<TSelf>> FirstOrDefault( NpgsqlConnection connection, NpgsqlTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.GetFirst();

        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            ErrorOrResult<TSelf>         record = await reader.FirstOrDefaultAsync<TSelf>(token);
            return record;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }
}
