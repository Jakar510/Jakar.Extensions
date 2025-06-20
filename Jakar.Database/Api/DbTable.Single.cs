// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TClass>
{
    public ValueTask<ErrorOrResult<TClass>> Single( RecordID<TClass>          id,  CancellationToken  token                               = default ) => this.Call( Single, id,  token );
    public ValueTask<ErrorOrResult<TClass>> Single( string                     sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Single, sql, parameters, token );
    public ValueTask<ErrorOrResult<TClass>> SingleOrDefault( RecordID<TClass> id,  CancellationToken  token                               = default ) => this.Call( SingleOrDefault, id,  token );
    public ValueTask<ErrorOrResult<TClass>> SingleOrDefault( string            sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( SingleOrDefault, sql, parameters, token );


    public ValueTask<ErrorOrResult<TClass>> Single( DbConnection connection, DbTransaction? transaction, RecordID<TClass> id,  CancellationToken  token                               = default ) => Single( connection, transaction, TClass.SQL.Get( in id ),          token );
    public ValueTask<ErrorOrResult<TClass>> Single( DbConnection connection, DbTransaction? transaction, string            sql, DynamicParameters? parameters, CancellationToken token = default ) => Single( connection, transaction, new SqlCommand( sql, parameters ), token );
    public virtual async ValueTask<ErrorOrResult<TClass>> Single( DbConnection connection, DbTransaction? transaction, SqlCommand sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommand( in sql, transaction, token );
            return await connection.QuerySingleAsync<TClass>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    public ValueTask<ErrorOrResult<TClass>> SingleOrDefault( DbConnection connection, DbTransaction? transaction, RecordID<TClass> id,  CancellationToken  token                               = default ) => SingleOrDefault( connection, transaction, TClass.SQL.Get( in id ),          token );
    public ValueTask<ErrorOrResult<TClass>> SingleOrDefault( DbConnection connection, DbTransaction? transaction, string            sql, DynamicParameters? parameters, CancellationToken token = default ) => SingleOrDefault( connection, transaction, new SqlCommand( sql, parameters ), token );
    public virtual async ValueTask<ErrorOrResult<TClass>> SingleOrDefault( DbConnection connection, DbTransaction? transaction, SqlCommand sql, CancellationToken token = default )
    {
        try
        {
            CommandDefinition command = _database.GetCommand( in sql, transaction, token );
            TClass?          record  = await connection.QuerySingleOrDefaultAsync<TClass>( command );

            return record is null
                       ? Error.NotFound()
                       : record;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
