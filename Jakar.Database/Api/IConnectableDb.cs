// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:38 PM

namespace Jakar.Database;


public interface IDbOptions
{
    public int?       CommandTimeout { get; }
    public DbInstance Instance       { get; }
}



public interface IDbTable : IAsyncDisposable
{
    public void ResetCaches();
}



public interface IConnectableDb : IDbOptions, IDbTable
{
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token );
}



public interface IConnectableDbRoot : IConnectableDb, ITableCacheFactory
{
    public IAsyncEnumerable<T> Where<T>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : IDbReaderMapping<T>;
    public IAsyncEnumerable<T> WhereValue<T>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : struct;
    public CommandDefinition       GetCommandDefinition( DbTransaction? transaction, in SqlCommand  sql,         CancellationToken token );
    public ValueTask<DbDataReader> ExecuteReaderAsync( DbConnection     connection,  DbTransaction? transaction, SqlCommand        sql, CancellationToken token );
}



public readonly record struct SqlCommand( string SQL, DynamicParameters? Parameters = default, CommandType? CommandType = default, CommandFlags Flags = CommandFlags.None )
{
    public static implicit operator SqlCommand( string sql ) => new(sql);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public CommandDefinition ToCommandDefinition( DbTransaction? transaction, int? timeout, CancellationToken token ) =>
        new(SQL, Parameters, transaction, timeout, CommandType, Flags, token);
}
