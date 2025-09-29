namespace Jakar.Extensions;


public interface IAppInfo : IDeviceName
{
    public     Guid       AppID       { get; }
    public     string     AppName     { get; }
    public     AppVersion AppVersion  { get; }
    public     int?       BuildNumber { get; }
    public     Guid       DebugID     { get; set; }
    public     Guid       DeviceID    { get; set; }
    public new string     DeviceName  { get; set; }
    public     FilePaths  Paths       { get; }
}
