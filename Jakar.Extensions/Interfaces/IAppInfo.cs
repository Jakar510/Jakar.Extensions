namespace Jakar.Extensions;


public interface IAppInfo : IDeviceName
{
    public     int?       BuildNumber { get; } 
    public     Guid       AppID       { get; }
    public     string     AppName     { get; }
    public     AppVersion AppVersion  { get; }
    public     FilePaths  Paths       { get; }
    public     Guid       DebugID     { get; set; }
    public     Guid       DeviceID    { get; set; }
    public new string     DeviceName  { get; set; }
}
