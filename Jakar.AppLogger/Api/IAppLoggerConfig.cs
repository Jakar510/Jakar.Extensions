namespace Jakar.AppLogger;


public interface IAppLoggerConfig
{
    public Guid InstallID { get; set; }
    public string AppName { get; set; }
    public Guid? SessionID { get; set; }
}
