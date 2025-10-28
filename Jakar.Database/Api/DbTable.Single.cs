// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask<ErrorOrResult<TSelf>> Single( RecordID<TSelf>          id,  CancellationToken  token                               = default ) => this.Call(Single, id,  token);
    public ValueTask<ErrorOrResult<TSelf>> Single( string                   sql, PostgresParameters parameters, CancellationToken token = default ) => this.Call(Single, sql, parameters, token);
    public ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( RecordID<TSelf> id,  CancellationToken  token                               = default ) => this.Call(SingleOrDefault, id,  token);
    public ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( string          sql, PostgresParameters parameters, CancellationToken token = default ) => this.Call(SingleOrDefault, sql, parameters, token);


    public ValueTask<ErrorOrResult<TSelf>> Single( NpgsqlConnection connection, NpgsqlTransaction? transaction, RecordID<TSelf> id,  CancellationToken  token                               = default ) => Single(connection, transaction, SqlCommand<TSelf>.Get(in id),           token);
    public ValueTask<ErrorOrResult<TSelf>> Single( NpgsqlConnection connection, NpgsqlTransaction? transaction, string          sql, PostgresParameters parameters, CancellationToken token = default ) => Single(connection, transaction, new SqlCommand<TSelf>(sql, parameters), token);
    public virtual async ValueTask<ErrorOrResult<TSelf>> Single( NpgsqlConnection connection, NpgsqlTransaction? transaction, SqlCommand<TSelf> command, CancellationToken token = default )
    {
        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            ErrorOrResult<TSelf>         record = await reader.SingleAsync<TSelf>(token);
            return record;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }


    public ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( NpgsqlConnection connection, NpgsqlTransaction? transaction, RecordID<TSelf> id,  CancellationToken  token                               = default ) => SingleOrDefault(connection, transaction, SqlCommand<TSelf>.Get(in id),           token);
    public ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( NpgsqlConnection connection, NpgsqlTransaction? transaction, string          sql, PostgresParameters parameters, CancellationToken token = default ) => SingleOrDefault(connection, transaction, new SqlCommand<TSelf>(sql, parameters), token);
    public virtual async ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( NpgsqlConnection connection, NpgsqlTransaction? transaction, SqlCommand<TSelf> command, CancellationToken token = default )
    {
        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            ErrorOrResult<TSelf>         record = await reader.SingleOrDefaultAsync<TSelf>(token);
            return record;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }
}
