// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:16 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?>                         Next( RecordPair<TRecord>    pair, CancellationToken token = default ) => this.Call( Next,   pair, token );
    public ValueTask<Guid?>                            NextID( Guid?                id,   CancellationToken token = default ) => this.Call( NextID, id,   token );
    public ValueTask<IEnumerable<RecordPair<TRecord>>> SortedIDs( CancellationToken token = default ) => this.Call( SortedIDs, token );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> Next( DbConnection connection, DbTransaction? transaction, RecordPair<TRecord> pair, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(RecordPair<TRecord>.ID),          pair.ID );
        parameters.Add( nameof(RecordPair<TRecord>.DateCreated), pair.DateCreated );

        string sql = _cache[Instance][SqlStatement.Next];

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            return await connection.ExecuteScalarAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<IEnumerable<RecordPair<TRecord>>> SortedIDs( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = _cache[Instance][SqlStatement.SortedIDs];

        try
        {
            CommandDefinition                command = _database.GetCommandDefinition( transaction, sql, token );
            IEnumerable<RecordPair<TRecord>> pairs   = await connection.QueryAsync<RecordPair<TRecord>>( command );
            return pairs;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<Guid?> NextID( DbConnection connection, DbTransaction? transaction, Guid? id, CancellationToken token = default )
    {
        if ( id is null ) { return default; }

        var parameters = new DynamicParameters();
        parameters.Add( ID, id.Value );

        string sql = _cache[Instance][SqlStatement.NextID];

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            return await connection.ExecuteScalarAsync<Guid>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
}
