// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask<ErrorOrResult<TSelf>> First( CancellationToken          token = default ) => this.Call(First,          token);
    public ValueTask<ErrorOrResult<TSelf>> FirstOrDefault( CancellationToken token = default ) => this.Call(FirstOrDefault, token);


    public virtual async ValueTask<ErrorOrResult<TSelf>> First( NpgsqlConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetFirst();

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            return await connection.QueryFirstAsync<TSelf>(command);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }
    public virtual async ValueTask<ErrorOrResult<TSelf>> FirstOrDefault( NpgsqlConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetFirst();

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
