// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:11 PM

namespace Jakar.Database;


public partial class DbTable<TRecord>
{
    private string? _first;
    private string? _firstOrDefault;


    public ValueTask<TRecord?> First( CancellationToken          token = default ) => this.Call( First,          token );
    public ValueTask<TRecord?> FirstOrDefault( CancellationToken token = default ) => this.Call( FirstOrDefault, token );


    public virtual async ValueTask<TRecord?> First( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        _first ??= $"SELECT * FROM {SchemaTableName} ORDER BY {ID_ColumnName} ASC LIMIT 1";

        try
        {
            CommandDefinition command = GetCommandDefinition( _first, default, transaction, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( _first, e ); }
    }
    public virtual async ValueTask<TRecord?> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        _firstOrDefault ??= $"SELECT * FROM {SchemaTableName} ORDER BY {ID_ColumnName} ASC LIMIT 1";

        try
        {
            CommandDefinition command = GetCommandDefinition( _firstOrDefault, default, transaction, token );
            return await connection.QueryFirstOrDefaultAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( _firstOrDefault, e ); }
    }
}
