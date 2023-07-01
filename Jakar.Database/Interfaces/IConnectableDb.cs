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
    public DbConnection Connect();
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token );
}
