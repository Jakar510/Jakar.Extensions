// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?> First( CancellationToken          token = default ) => this.Call( First,          token );
    public ValueTask<TRecord?> FirstOrDefault( CancellationToken token = default ) => this.Call( FirstOrDefault, token );


    public virtual async ValueTask<TRecord?> First( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} ORDER BY {ID} ASC LIMIT 1";

        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QueryFirstAsync<TRecord>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }
    public virtual async ValueTask<TRecord?> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} ORDER BY {ID} ASC LIMIT 1";

        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QueryFirstOrDefaultAsync<TRecord>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }
}
