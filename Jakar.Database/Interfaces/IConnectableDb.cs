// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:38 PM

namespace Jakar.Database;


public interface IConnectableDb
{
    public DbInstance Instance      { get; }
    public string     CurrentSchema { get; }


    public DbConnection Connect();
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token );
}



public interface IConnectableDb<TRecord> : IConnectableDb where TRecord : TableRecord<TRecord> { }
