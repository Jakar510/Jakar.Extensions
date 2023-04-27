// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:14 PM

namespace Jakar.Database;


public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?> Random( CancellationToken token                                                   = default ) => this.Call( Random, token );
    public ValueTask<IEnumerable<TRecord>> Random( int              count, CancellationToken token                          = default ) => this.Call( Random, count, token );
    public ValueTask<TRecord?> Random( UserRecord        user,  int               count, CancellationToken token = default ) => this.Call( Random, user,  count, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return default; }

        string sql = Instance switch
                     {
                         DbInstance.MsSql    => $"SELECT TOP 1 * FROM {SchemaTableName} ORDER BY {RandomMethod}",
                         DbInstance.Postgres => $"SELECT * FROM {SchemaTableName} ORDER BY {RandomMethod} LIMIT 1",
                         _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                     };

        try { return await connection.QueryFirstAsync<TRecord>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, UserRecord user, int count, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return default; }

        var param = new DynamicParameters();
        param.Add( OwnerUserID, user.OwnerUserID );

        string sql = Instance switch
                     {
                         DbInstance.MsSql    => $"SELECT TOP {count} * FROM {SchemaTableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod}",
                         DbInstance.Postgres => $"SELECT * FROM {SchemaTableName} WHERE {OwnerUserID} = @{OwnerUserID} ORDER BY {RandomMethod} LIMIT {count}",
                         _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                     };

        try { return await connection.QueryFirstAsync<TRecord>( sql, param, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual ValueTask<IEnumerable<TRecord>> Random( DbConnection connection, DbTransaction? transaction, int count, CancellationToken token = default )
    {
        string sql = Instance switch
                     {
                         DbInstance.MsSql    => $"SELECT TOP {count} * FROM {SchemaTableName} ORDER BY {RandomMethod}",
                         DbInstance.Postgres => $"SELECT * FROM {SchemaTableName} ORDER BY {RandomMethod} LIMIT {count}",
                         _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                     };

        return Where( connection, transaction, sql, default, token );
    }
}
