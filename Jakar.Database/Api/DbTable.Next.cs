// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:16 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    protected string? _next;
    protected string? _sortedIDs;
    protected string? _nextID;


    public ValueTask<TRecord?> Next( RecordPair<TRecord>                            pair, CancellationToken token = default ) => this.Call( Next,   pair, token );
    public ValueTask<Guid?> NextID( Guid?                                           id,   CancellationToken token = default ) => this.Call( NextID, id,   token );
    public ValueTask<IEnumerable<RecordPair<TRecord>>> SortedIDs( CancellationToken token = default ) => this.Call( SortedIDs, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Next( DbConnection connection, DbTransaction? transaction, RecordPair<TRecord> pair, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(RecordPair<TRecord>.ID),          pair.ID );
        parameters.Add( nameof(RecordPair<TRecord>.DateCreated), pair.DateCreated );

        _next ??= @$"SELECT * FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({ID_ColumnName}) FROM {SchemaTableName} WHERE {ID_ColumnName} > @{nameof(RecordPair<TRecord>.ID)}), 0) )";

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( _next, parameters, transaction, token );
            return await connection.ExecuteScalarAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( _next, parameters, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<IEnumerable<RecordPair<TRecord>>> SortedIDs( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        _sortedIDs ??= @$"SELECT {ID_ColumnName}, {DateCreated} FROM {SchemaTableName} ORDER BY {DateCreated} DESC";

        try
        {
            CommandDefinition                command = _database.GetCommandDefinition( _sortedIDs, default, transaction, token );
            IEnumerable<RecordPair<TRecord>> pairs   = await connection.QueryAsync<RecordPair<TRecord>>( command );
            return pairs;
        }
        catch ( Exception e ) { throw new SqlException( _sortedIDs, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<Guid?> NextID( DbConnection connection, DbTransaction? transaction, Guid? id, CancellationToken token = default )
    {
        if ( id is null ) { return default; }

        var parameters = new DynamicParameters();
        parameters.Add( nameof(id), id.Value );

        _nextID ??= @$"SELECT {ID_ColumnName} FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({ID_ColumnName}) FROM {SchemaTableName} WHERE {ID_ColumnName} > @{nameof(id)}), 0) )";

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( _nextID, parameters, transaction, token );
            return await connection.ExecuteScalarAsync<Guid>( command );
        }
        catch ( Exception e ) { throw new SqlException( _nextID, parameters, e ); }
    }
}
