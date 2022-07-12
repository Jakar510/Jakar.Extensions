namespace Jakar.AppLogger.Client;


public interface IAppLoggerConfig
{
    public Guid     InstallID          { get; set; }
    public Guid?    SessionID          { get; set; }
    public string   AppName            { get; }
    public LogLevel LogLevel           { get; }
    public Device   Device             { get; }
    public DateTime AppLaunchTimestamp { get; }
    public string?  UserID             { get; }
}
