// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/09/2022  6:12 PM

namespace Jakar.AppLogger.Common;


[Serializable] public sealed record StartSession( string AppLoggerSecret, DateTimeOffset AppStartTime, DeviceDescriptor Device ) : BaseRecord;



[Serializable]
// ReSharper disable once InconsistentNaming
public sealed record StartSessionReply( Guid SessionID, Guid AppID, Guid DeviceID ) : BaseRecord, IStartSession
{
    Guid? ISessionID.SessionID => SessionID;
}
