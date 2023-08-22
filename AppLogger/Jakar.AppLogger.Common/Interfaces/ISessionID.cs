// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/09/2022  4:05 PM

namespace Jakar.AppLogger.Common;


public interface ISessionID
{
    public Guid? SessionID { get; }
}



public interface IStartSession : ISessionID
{
    public Guid AppID    { get; }
    public Guid DeviceID { get; }
}



public interface ILogInfo : IStartSession
{
    public Guid LogID { get; }
}



public interface ILogScopes
{
    public HashSet<Guid> ScopeIDs { get; }
}
