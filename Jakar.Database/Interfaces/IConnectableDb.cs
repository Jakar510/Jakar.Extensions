// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:38 PM

namespace Jakar.Database;


public interface IDbOptions
{
    public string     CurrentSchema  { get; }
    public DbInstance Instance       { get; }
    public int        CommandTimeout { get; }
}



public interface IConnectableDb : IDbOptions
{
    public DbConnection            Connect();
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token );
}



public interface IConnectableDbRoot : IConnectableDb
{
    public IAsyncEnumerable<T> Where<T>( DbConnection       connection, DbTransaction?     transaction, string         sql, DynamicParameters? parameters, [ EnumeratorCancellation ] CancellationToken token = default ) where T : IDbReaderMapping<T>;
    public IAsyncEnumerable<T> WhereValue<T>( DbConnection  connection, DbTransaction?     transaction, string         sql, DynamicParameters? parameters, [ EnumeratorCancellation ] CancellationToken token = default ) where T : struct;
    public CommandDefinition   GetCommandDefinition( string sql,        DynamicParameters? parameters,  DbTransaction? transaction, CancellationToken token, CommandType? commandType = default, CommandFlags flags = CommandFlags.None );
}
