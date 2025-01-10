// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:13 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<ErrorOrResult<TRecord>> Last( CancellationToken token = default ) => this.Call( Last, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<ErrorOrResult<TRecord>> Last( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.GetLast();

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    public ValueTask<ErrorOrResult<TRecord>> LastOrDefault( CancellationToken token = default ) => this.Call( LastOrDefault, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<ErrorOrResult<TRecord>> LastOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.GetLast();

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            var               record  = await connection.QueryFirstOrDefaultAsync<TRecord>( command );

            return record is null
                       ? Error.NotFound()
                       : record;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
