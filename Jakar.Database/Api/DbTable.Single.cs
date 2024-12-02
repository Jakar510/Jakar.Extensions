// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<ErrorOrResult<TRecord>> Single(  RecordID<TRecord> id,  CancellationToken  token                               = default ) => this.Call( Single,  id,  token );
    public ValueTask<ErrorOrResult<TRecord>> Single(  string            sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Single,  sql, parameters, token );
    public ValueTask<ErrorOrResult<TRecord>> SingleOrDefault(  RecordID<TRecord> id,  CancellationToken  token                               = default ) => this.Call( SingleOrDefault,  id,  token );
    public ValueTask<ErrorOrResult<TRecord>> SingleOrDefault(  string            sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( SingleOrDefault,  sql, parameters, token );


    public ValueTask<ErrorOrResult<TRecord>> Single( DbConnection connection, DbTransaction? transaction,  RecordID<TRecord> id,  CancellationToken  token                               = default ) => Single( connection, transaction,  _sqlCache.Single( id ),            token );
    public ValueTask<ErrorOrResult<TRecord>> Single( DbConnection connection, DbTransaction? transaction,  string            sql, DynamicParameters? parameters, CancellationToken token = default ) => Single( connection, transaction,  new SqlCommand( sql, parameters ), token );
    public virtual async ValueTask<ErrorOrResult<TRecord>> Single( DbConnection connection, DbTransaction? transaction,  SqlCommand sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommand(  sql, transaction, token );
            return await connection.QuerySingleAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    public ValueTask<ErrorOrResult<TRecord>> SingleOrDefault( DbConnection connection, DbTransaction? transaction,  RecordID<TRecord> id,  CancellationToken  token                               = default ) => SingleOrDefault( connection, transaction,  _sqlCache.Single( id ),            token );
    public ValueTask<ErrorOrResult<TRecord>> SingleOrDefault( DbConnection connection, DbTransaction? transaction,  string            sql, DynamicParameters? parameters, CancellationToken token = default ) => SingleOrDefault( connection, transaction,  new SqlCommand( sql, parameters ), token );
    public virtual async ValueTask<ErrorOrResult<TRecord>> SingleOrDefault( DbConnection connection, DbTransaction? transaction,  SqlCommand sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommand(  sql, transaction, token );
            return await connection.QuerySingleOrDefaultAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
