// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:16 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?>                         Next( Activity?      activity, RecordPair<TRecord> pair, CancellationToken token = default ) => this.Call( Next,   activity, pair, token );
    public ValueTask<Guid?>                            NextID( Activity?    activity, RecordPair<TRecord> pair, CancellationToken token = default ) => this.Call( NextID, activity, pair, token );
    public ValueTask<IEnumerable<RecordPair<TRecord>>> SortedIDs( Activity? activity, CancellationToken   token = default ) => this.Call( SortedIDs, activity, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Next( DbConnection connection, DbTransaction? transaction, Activity? activity, RecordPair<TRecord> pair, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Next( pair );

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            return await connection.ExecuteScalarAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<IEnumerable<RecordPair<TRecord>>> SortedIDs( DbConnection connection, DbTransaction? transaction, Activity? activity, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.SortedIDs();

        try
        {
            CommandDefinition                command = _database.GetCommand( activity, sql, transaction, token );
            IEnumerable<RecordPair<TRecord>> pairs   = await connection.QueryAsync<RecordPair<TRecord>>( command );
            return pairs;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<Guid?> NextID( DbConnection connection, DbTransaction? transaction, Activity? activity, RecordPair<TRecord> pair, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.NextID( pair );

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            return await connection.ExecuteScalarAsync<Guid>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
