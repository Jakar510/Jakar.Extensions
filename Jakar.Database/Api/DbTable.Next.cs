// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:16 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<ErrorOrResult<TRecord>>           Next( RecordPair<TRecord>    pair, CancellationToken token = default ) => this.Call( Next,   pair, token );
    public ValueTask<Guid?>                            NextID( RecordPair<TRecord>  pair, CancellationToken token = default ) => this.Call( NextID, pair, token );
    public ValueTask<IEnumerable<RecordPair<TRecord>>> SortedIDs( CancellationToken token = default ) => this.Call( SortedIDs, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<ErrorOrResult<TRecord>> Next( DbConnection connection, DbTransaction? transaction, RecordPair<TRecord> pair, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.Next( pair );

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            var               record  = await connection.ExecuteScalarAsync<TRecord>( command );

            return record is null
                       ? Error.NotFound()
                       : record;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<IEnumerable<RecordPair<TRecord>>> SortedIDs( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.SortedID();

        try
        {
            CommandDefinition                command = _database.GetCommand( sql, transaction, token );
            IEnumerable<RecordPair<TRecord>> pairs   = await connection.QueryAsync<RecordPair<TRecord>>( command );
            return pairs;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<Guid?> NextID( DbConnection connection, DbTransaction? transaction, RecordPair<TRecord> pair, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.NextID( pair );

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            return await connection.ExecuteScalarAsync<Guid>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
