// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<ErrorOrResult<TRecord>> First( CancellationToken          token = default ) => this.Call( First,          token );
    public ValueTask<ErrorOrResult<TRecord>> FirstOrDefault( CancellationToken token = default ) => this.Call( FirstOrDefault, token );


    public virtual async ValueTask<ErrorOrResult<TRecord>> First( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.GetFirst();

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
    public virtual async ValueTask<ErrorOrResult<TRecord>> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.GetFirst();

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            TRecord?          record  = await connection.QueryFirstOrDefaultAsync<TRecord>( command );

            return record is null
                       ? Error.NotFound()
                       : record;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
