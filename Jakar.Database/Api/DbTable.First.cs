// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TClass>
{
    public ValueTask<ErrorOrResult<TClass>> First( CancellationToken          token = default ) => this.Call(First,          token);
    public ValueTask<ErrorOrResult<TClass>> FirstOrDefault( CancellationToken token = default ) => this.Call(FirstOrDefault, token);


    public virtual async ValueTask<ErrorOrResult<TClass>> First( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TClass.SQL.GetFirst();

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            return await connection.QueryFirstAsync<TClass>(command);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }
    public virtual async ValueTask<ErrorOrResult<TClass>> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TClass.SQL.GetFirst();

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            TClass?           record  = await connection.QueryFirstOrDefaultAsync<TClass>(command);

            return record is null
                       ? Error.NotFound()
                       : record;
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }
}
