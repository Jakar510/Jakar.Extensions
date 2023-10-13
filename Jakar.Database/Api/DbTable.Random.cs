// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:14 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?>       Random( CancellationToken token                                                     = default ) => this.Call( Random, token );
    public IAsyncEnumerable<TRecord> Random( int               count, [ EnumeratorCancellation ] CancellationToken token = default ) => this.Call( Random, count, token );

    public IAsyncEnumerable<TRecord> Random( UserRecord user, int count, [ EnumeratorCancellation ] CancellationToken token = default ) => this.Call( Random, user, count, token );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = _cache[Instance, SqlStatement.Single];

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, sql, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql.SQL, e ); }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async IAsyncEnumerable<TRecord> Random( DbConnection connection, DbTransaction? transaction, UserRecord user, int count, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( OwnerUserID, user.OwnerUserID );

        string sql = Instance switch
                     {
                         DbInstance.MsSql    => _randomUserCountMsSql ??= $"SELECT TOP {count} * FROM {SchemaTableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod}",
                         DbInstance.Postgres => _randomUserCountPostgres ??= $"SELECT * FROM {SchemaTableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod} LIMIT {count}",
                         _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                     };

        await foreach ( TRecord record in Where( connection, transaction, new SqlCommand( sql, parameters ), token ) ) { yield return record; }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual IAsyncEnumerable<TRecord> Random( DbConnection connection, DbTransaction? transaction, int count, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        SqlCommand sql = Instance switch
                         {
                             DbInstance.MsSql    => _randomCountMsSql ??= $"SELECT TOP {count} * FROM {SchemaTableName} ORDER BY {RandomMethod}",
                             DbInstance.Postgres => _randomCountPostgres ??= $"SELECT * FROM {SchemaTableName} ORDER BY {RandomMethod} LIMIT {count}",
                             _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                         };

        return Where( connection, transaction, sql, token );
    }
}
