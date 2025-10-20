// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask<ErrorOrResult<TSelf>> Single( RecordID<TSelf>          id,  CancellationToken  token                               = default ) => this.Call(Single, id,  token);
    public ValueTask<ErrorOrResult<TSelf>> Single( string                    sql, PostgresParameters? parameters, CancellationToken token = default ) => this.Call(Single, sql, parameters, token);
    public ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( RecordID<TSelf> id,  CancellationToken  token                               = default ) => this.Call(SingleOrDefault, id,  token);
    public ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( string           sql, PostgresParameters? parameters, CancellationToken token = default ) => this.Call(SingleOrDefault, sql, parameters, token);


    public ValueTask<ErrorOrResult<TSelf>> Single( NpgsqlConnection connection, NpgsqlTransaction? transaction, RecordID<TSelf> id,  CancellationToken  token                               = default ) => Single(connection, transaction, SqlCommand<TSelf>.Get(in id),           token);
    public ValueTask<ErrorOrResult<TSelf>> Single( NpgsqlConnection connection, NpgsqlTransaction? transaction, string           sql, PostgresParameters? parameters, CancellationToken token = default ) => Single(connection, transaction, new SqlCommand(sql, parameters), token);
    public virtual async ValueTask<ErrorOrResult<TSelf>> Single( NpgsqlConnection connection, NpgsqlTransaction? transaction, SqlCommand<TSelf> sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            return await connection.QuerySingleAsync<TSelf>(command);
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(sql, e); }
    }


    public ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( NpgsqlConnection connection, NpgsqlTransaction? transaction, RecordID<TSelf> id,  CancellationToken  token                               = default ) => SingleOrDefault(connection, transaction, SqlCommand<TSelf>.Get(in id),           token);
    public ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( NpgsqlConnection connection, NpgsqlTransaction? transaction, string           sql, PostgresParameters? parameters, CancellationToken token = default ) => SingleOrDefault(connection, transaction, new SqlCommand(sql, parameters), token);
    public virtual async ValueTask<ErrorOrResult<TSelf>> SingleOrDefault( NpgsqlConnection connection, NpgsqlTransaction? transaction, SqlCommand<TSelf> sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            TSelf?           record  = await connection.QuerySingleOrDefaultAsync<TSelf>(command);

            return record is null
                       ? Error.NotFound()
                       : record;
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(sql, e); }
    }
}
