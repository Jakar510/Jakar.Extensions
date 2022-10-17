// Jakar.Extensions :: Jakar.Database.Resx
// 10/07/2022  10:37 PM

using Jakar.Database.Implementations;



namespace Jakar.Database;


public interface IResxProvider : IConnectableDb
{
    public MsSqlDbTable<ResxRowTable> Resx { get; }
}
