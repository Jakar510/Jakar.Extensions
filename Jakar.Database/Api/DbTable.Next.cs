// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:16 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TClass>
{
    public ValueTask<ErrorOrResult<TClass>>           Next( RecordPair<TClass>    pair, CancellationToken token = default ) => this.Call( Next,   pair, token );
    public ValueTask<Guid?>                            NextID( RecordPair<TClass>  pair, CancellationToken token = default ) => this.Call( NextID, pair, token );
    public ValueTask<IEnumerable<RecordPair<TClass>>> SortedIDs( CancellationToken token = default ) => this.Call( SortedIDs, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<ErrorOrResult<TClass>> Next( DbConnection connection, DbTransaction? transaction, RecordPair<TClass> pair, CancellationToken token = default )
    {
        SqlCommand sql = TClass.SQL.Next( in pair );

        try
        {
            CommandDefinition command = _database.GetCommand( in sql, transaction, token );
            var               record  = await connection.ExecuteScalarAsync<TClass>( command );

            return record is null
                       ? Error.NotFound()
                       : record;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<IEnumerable<RecordPair<TClass>>> SortedIDs( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = TClass.SQL.SortedID();

        try
        {
            CommandDefinition                command = _database.GetCommand( in sql, transaction, token );
            IEnumerable<RecordPair<TClass>> pairs   = await connection.QueryAsync<RecordPair<TClass>>( command );
            return pairs;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<Guid?> NextID( DbConnection connection, DbTransaction? transaction, RecordPair<TClass> pair, CancellationToken token = default )
    {
        SqlCommand sql = TClass.SQL.NextID( in pair );

        try
        {
            CommandDefinition command = _database.GetCommand( in sql, transaction, token );
            return await connection.ExecuteScalarAsync<Guid>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
