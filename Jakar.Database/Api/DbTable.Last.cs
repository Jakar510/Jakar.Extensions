// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:13 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TClass>
{
    public ValueTask<ErrorOrResult<TClass>> Last( CancellationToken token = default ) => this.Call(Last, token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TClass>> Last( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TClass.SQL.GetLast();

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            return await connection.QueryFirstAsync<TClass>(command);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }


    public ValueTask<ErrorOrResult<TClass>> LastOrDefault( CancellationToken token = default ) => this.Call(LastOrDefault, token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TClass>> LastOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TClass.SQL.GetLast();

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
