// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/04/2022  6:26 PM

namespace Jakar.AppLogger.Portal.Data.Interfaces;


public interface ISession
{
    public Guid SessionID { get; init; }
    public long AppID { get; init; }
    public long DeviceID { get; init; }
}



public interface ILogInfo : ISession
{
    public Guid? ScopeID { get; init; }
    public long LogID { get; }
}
