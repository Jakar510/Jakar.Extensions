// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:38 PM

namespace Jakar.Database;


public interface IConnectableDb
{
    public DbConnection Connect();
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token );
}
