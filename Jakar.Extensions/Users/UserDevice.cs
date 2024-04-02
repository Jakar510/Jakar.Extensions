namespace Jakar.Extensions;


public interface IUserDevice<out TID> : IUniqueID<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
{
    public Guid        DeviceID   { get; }
    public string      DeviceName { get; }
    public DeviceType  DeviceType { get; }
    public DeviceIdiom Idiom      { get; }


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
                               DeviceType     DeviceType,
                               DeviceIdiom    Idiom,
                               DevicePlatform Platform,
                               AppVersion     OsVersion,
                               Guid           DeviceID,
                               TID            ID = default ) : ObservableRecord, IUserDevice<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
{
    private string?        _ip;
    public  string?        IP        { get => _ip; set => SetProperty( ref _ip, value ); }
    public  DateTimeOffset TimeStamp { get;        init; } = DateTimeOffset.UtcNow;

    public UserDevice( IUserDevice<TID> device ) : this( device.Model, device.Manufacturer, device.DeviceName, device.DeviceType, device.Idiom, device.Platform, device.OsVersion, device.DeviceID, device.ID ) { }

    // public static UserDevice<TID> Create( Guid? deviceID ) => new(DeviceInfo.Model, DeviceInfo.Manufacturer, DeviceInfo.Name, DeviceInfo.DeviceType, DeviceInfo.Idiom, DeviceInfo.Platform, DeviceInfo.Version, deviceID);
}
