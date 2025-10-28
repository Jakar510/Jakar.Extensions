using Serilog.Core;
using Serilog.Events;



namespace Jakar.Extensions;


public interface IDeviceID
{
    public string DeviceID { get; }
}



public interface ISessionID<out T> : IDeviceID
{
    public T SessionID { get; }
}



public interface IDeviceName
{
    public string DeviceName { get; }
}



public interface IDeviceInformation : IEquatable<IDeviceInformation>, IDeviceID
{
    public DeviceCategory Category   { get; }
    public string?        DeviceName { get; }

    /// <summary> Last known <see cref="IPAddress"/> </summary>
    public string? Ip { get; }

    public string?        Manufacturer { get; }
    public string?        Model        { get; }
    string?               OsVersion    { get; }
    public string?        PackageName  { get; }
    public DevicePlatform Platform     { get; }
    public DeviceTypes    Type         { get; }
    string?               Version      { get; set; }

    public void SetDeviceID( string id );
}



public interface IUserDevice<out TID> : IDeviceInformation, IUniqueID<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    /// <summary> Last known <see cref="IPAddress"/> </summary>
    public string? IP { get; }

    public DateTimeOffset TimeStamp { get; }
}



[Serializable]
public class DeviceInformation : BaseClass, IDeviceInformation, ILogEventEnricher
{
    protected AppVersion?       _appVersion;
    protected DeviceCategory    _category = DeviceCategory.Unknown;
    protected DevicePlatform    _platform = DevicePlatform.Unknown;
    protected DeviceTypes       _type;
    protected LogEventProperty? _property;
    protected string            _deviceID = EMPTY;
    protected string?           _deviceName;
    protected string?           _ip;
    protected string?           _manufacturer;
    protected string?           _model;
    protected string?           _osVersion;
    protected string?           _packageName;
    protected string?           _version;


    public virtual DeviceCategory Category     { get => _category;     set => SetProperty(ref _category,     value); }
    public virtual string         DeviceID     { get => _deviceID;     set => SetProperty(ref _deviceID,     value); }
    public virtual string?        DeviceName   { get => _deviceName;   set => SetProperty(ref _deviceName,   value); }
    public virtual string?        Ip           { get => _ip;           set => SetProperty(ref _ip,           value); }
    public virtual string?        Manufacturer { get => _manufacturer; set => SetProperty(ref _manufacturer, value); }
    public virtual string?        Model        { get => _model;        set => SetProperty(ref _model,        value); }
    public virtual string?        OsVersion    { get => _osVersion;    set => SetProperty(ref _osVersion,    value); }
    public virtual string?        PackageName  { get => _packageName;  set => SetProperty(ref _packageName,  value); }
    public virtual DevicePlatform Platform     { get => _platform;     set => SetProperty(ref _platform,     value); }
    public virtual DeviceTypes    Type         { get => _type;         set => SetProperty(ref _type,         value); }
    public virtual string?        Version      { get => _version;      set => SetProperty(ref _version,      value); }


    public DeviceInformation() { }
    public DeviceInformation( string? model, string? manufacturer, string? deviceName, DeviceTypes deviceType, DeviceCategory category, DevicePlatform platform, AppVersion? osVersion, string deviceID )
    {
        Model        = model;
        Manufacturer = manufacturer;
        DeviceName   = deviceName;
        Type         = deviceType;
        Category     = category;
        Platform     = platform;
        OsVersion    = osVersion?.ToString();
        DeviceID     = deviceID;
    }
    public DeviceInformation( IDeviceInformation device )
    {
        DeviceID     = device.DeviceID;
        Model        = device.Model;
        Manufacturer = device.Manufacturer;
        DeviceName   = device.DeviceName;
        Type         = device.Type;
        Category     = device.Category;
        OsVersion    = device.OsVersion;
        Platform     = device.Platform;
        Version      = device.Version;
        Ip           = device.Ip;
        PackageName  = device.PackageName;
    }
    public void SetDeviceID( string id ) => DeviceID = id;


    public virtual LogEventProperty ToProperty() =>
        _property ??= new LogEventProperty(nameof(DeviceInformation),
                                           new StructureValue([
                                                                  Enricher.GetProperty(Category,     nameof(Category)),
                                                                  Enricher.GetProperty(DeviceID,     nameof(DeviceID)),
                                                                  Enricher.GetProperty(DeviceName,   nameof(DeviceName)),
                                                                  Enricher.GetProperty(Manufacturer, nameof(Manufacturer)),
                                                                  Enricher.GetProperty(Model,        nameof(Model)),
                                                                  Enricher.GetProperty(OsVersion,    nameof(OsVersion)),
                                                                  Enricher.GetProperty(PackageName,  nameof(PackageName)),
                                                                  Enricher.GetProperty(Platform,     nameof(Platform)),
                                                                  Enricher.GetProperty(Type,         nameof(Type)),
                                                                  Enricher.GetProperty(Version,      nameof(Version))
                                                              ]));
    public void Enrich( LogEvent log, ILogEventPropertyFactory propertyFactory ) => log.AddPropertyIfAbsent(ToProperty());


    public static bool operator ==( DeviceInformation? left, DeviceInformation? right ) => Equals(left, right);
    public static bool operator !=( DeviceInformation? left, DeviceInformation? right ) => !Equals(left, right);


    // public static UserDevice Create( Guid? deviceID ) => new(DeviceInfo.Model, DeviceInfo.Manufacturer, DeviceInfo.Name, (DeviceType)DeviceInfo.DeviceType, DeviceInfo.Idiom, DeviceInfo.Platform, DeviceInfo.Version, deviceID);


    public override bool Equals( object? obj )
    {
        if ( obj is null ) { return false; }

        if ( ReferenceEquals(this, obj) ) { return true; }

        return obj is DeviceInformation device && Equals(device);
    }
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(Category);
        hashCode.Add(DeviceID);
        hashCode.Add(DeviceName);
        hashCode.Add(Manufacturer);
        hashCode.Add(Model);
        hashCode.Add(OsVersion);
        hashCode.Add(PackageName);
        hashCode.Add(Platform);
        hashCode.Add(Type);
        hashCode.Add(Version);
        return hashCode.ToHashCode();
    }
    public virtual bool Equals( IDeviceInformation? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Type     == other.Type                                                                       &&
               Platform == other.Platform                                                                   &&
               Category == other.Category                                                                   &&
               string.Equals(Manufacturer, other.Manufacturer, StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(Model,        other.Model,        StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(DeviceName,   other.DeviceName,   StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(Version,      other.Version,      StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(PackageName,  other.PackageName,  StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(OsVersion,    other.OsVersion,    StringComparison.InvariantCultureIgnoreCase) &&
               DeviceID == other.DeviceID;
    }
}
