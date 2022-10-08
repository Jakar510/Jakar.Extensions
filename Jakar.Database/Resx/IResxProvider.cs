// Jakar.Extensions :: Jakar.Database.Resx
// 10/07/2022  10:37 PM

namespace Jakar.Database;


public interface IResxProvider : IConnectableDb
{
    public DbTable<ResxRowTable> Resx { get; }
}
