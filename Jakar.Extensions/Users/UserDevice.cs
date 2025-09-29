namespace Jakar.Extensions;


public interface IDeviceID
{
    public Guid DeviceID { get; }
}



public interface IDeviceName
{
    public string DeviceName { get; }
}



public interface IUserDevice<out TID> : IUniqueID<TID>, IDeviceID
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public string         DeviceName { get; }
    public DeviceTypes    DeviceType { get; }
    public DeviceCategory Idiom      { get; }


    /// <summary> Last known <see cref="IPAddress"/> </summary>
    public string? IP { get; }

    public string         Manufacturer { get; }
    public string         Model        { get; }
    AppVersion            OsVersion    { get; }
    public DevicePlatform Platform     { get; }
    public DateTimeOffset TimeStamp    { get; }
}



[Serializable]
public record UserDevice<TID>( string         Model,
                               string         Manufacturer,
                               string         DeviceName,
                               DeviceTypes    DeviceType,
                               DeviceCategory Idiom,
                               DevicePlatform Platform,
                               AppVersion     OsVersion,
                               Guid           DeviceID,
                               TID            ID = default ) : ObservableRecord, IUserDevice<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    private string?        __ip;
    public  string?        IP        { get => __ip; set => SetProperty(ref __ip, value); }
    public  DateTimeOffset TimeStamp { get;         init; } = DateTimeOffset.UtcNow;

    public UserDevice( IUserDevice<TID> device ) : this(device.Model, device.Manufacturer, device.DeviceName, device.DeviceType, device.Idiom, device.Platform, device.OsVersion, device.DeviceID, device.ID) { }

    // public static UserDevice<TID> Create( Guid? deviceID ) => new(DeviceInfo.Model, DeviceInfo.Manufacturer, DeviceInfo.AppName, DeviceInfo.DeviceType, DeviceInfo.Idiom, DeviceInfo.Platform, DeviceInfo.AppVersion, deviceID);
}
