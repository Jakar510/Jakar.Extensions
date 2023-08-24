// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:14 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    private string? _randomMsSql;
    private string? _randomPostgres;
    private string? _randomCountMsSql;
    private string? _randomCountPostgres;
    private string? _randomUserCountMsSql;
    private string? _randomUserCountPostgres;


    public ValueTask<TRecord?> Random( CancellationToken token                                                   = default ) => this.Call( Random, token );
    public ValueTask<IEnumerable<TRecord>> Random( int   count, CancellationToken token                          = default ) => this.Call( Random, count, token );
    public ValueTask<TRecord?> Random( UserRecord        user,  int               count, CancellationToken token = default ) => this.Call( Random, user,  count, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = Instance switch
                     {
                         DbInstance.MsSql    => _randomMsSql ??= $"SELECT TOP 1 * FROM {SchemaTableName} ORDER BY {RandomMethod}",
                         DbInstance.Postgres => _randomPostgres ??= $"SELECT * FROM {SchemaTableName} ORDER BY {RandomMethod} LIMIT 1",
                         _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                     };

        try
        {
            CommandDefinition command = GetCommandDefinition( sql, default, transaction, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, UserRecord user, int count, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( OwnerUserID, user.OwnerUserID );

        string sql = Instance switch
                     {
                         DbInstance.MsSql    => _randomUserCountMsSql ??= $"SELECT TOP {count} * FROM {SchemaTableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod}",
                         DbInstance.Postgres => _randomUserCountPostgres ??= $"SELECT * FROM {SchemaTableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod} LIMIT {count}",
                         _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                     };

        try
        {
            CommandDefinition command = GetCommandDefinition( sql, parameters, transaction, token );
            return await connection.QueryFirstAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual ValueTask<IEnumerable<TRecord>> Random( DbConnection connection, DbTransaction? transaction, int count, CancellationToken token = default )
    {
        string sql = Instance switch
                     {
                         DbInstance.MsSql    => _randomCountMsSql ??= $"SELECT TOP {count} * FROM {SchemaTableName} ORDER BY {RandomMethod}",
                         DbInstance.Postgres => _randomCountPostgres ??= $"SELECT * FROM {SchemaTableName} ORDER BY {RandomMethod} LIMIT {count}",
                         _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                     };

        return Where( connection, transaction, sql, default, token );
    }
}
