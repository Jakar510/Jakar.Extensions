// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:13 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?> Last( CancellationToken token = default ) => this.Call( Last, token );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> Last( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        _last ??= $"SELECT * FROM {SchemaTableName} ORDER BY {ID_ColumnName} DESC LIMIT 1";

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, _last.Value, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( _last.Value.SQL, e ); }
    }


    public ValueTask<TRecord?> LastOrDefault( CancellationToken token = default ) => this.Call( LastOrDefault, token );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> LastOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        _lastOrDefault ??= $"SELECT * FROM {SchemaTableName} ORDER BY {ID_ColumnName} DESC LIMIT 1";

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, _lastOrDefault.Value, token );
            return await connection.QueryFirstOrDefaultAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( _lastOrDefault.Value.SQL, e ); }
    }
}
