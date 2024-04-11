// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?> First( CancellationToken          token = default ) => this.Call( First,          token );
    public ValueTask<TRecord?> FirstOrDefault( CancellationToken token = default ) => this.Call( FirstOrDefault, token );


    public virtual async ValueTask<TRecord?> First( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.First();

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
    public virtual async ValueTask<TRecord?> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.First();

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            return await connection.QueryFirstOrDefaultAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
