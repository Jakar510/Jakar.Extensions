namespace Jakar.Extensions;
#nullable enable



public interface IUserDevice<TID> : IEquatable<IUserDevice<TID>>, IUniqueID<TID> where TID : struct, IComparable<TID>, IEquatable<TID>
{
    public DateTime TimeStamp { get; }


    /// <summary> DeviceIdiom </summary>
    public DeviceIdiom Idiom { get; }

    /// <summary> DevicePlatform </summary>
    public DevicePlatform Platform { get; }


    /// <summary> DeviceType </summary>
    public DeviceType DeviceType { get; }

    public Guid   DeviceID     { get; }
    public string DeviceName   { get; }
    public string Manufacturer { get; }
    public string Model        { get; }
    string        OsVersion    { get; }


    /// <summary>
    ///     Last known
    ///     <see cref = "IPAddress" />
    /// </summary>
    public string? IP { get; }
}



/// <summary>
///     Debug and/or identify info for IT
/// </summary>
[Serializable]
public class UserDevice<TID> : ObservableClass, IUserDevice<TID> where TID : struct, IComparable<TID>, IEquatable<TID>
{
    private string?        _ip;
    public  DateTime       TimeStamp    { get; init; }
    public  DeviceIdiom    Idiom        { get; init; }
    public  DevicePlatform Platform     { get; init; }
    public  DeviceType     DeviceType   { get; init; }
    public  Guid           DeviceID     { get; init; }
    public  string         DeviceName   { get; init; } = string.Empty;
    public  string         Manufacturer { get; init; } = string.Empty;
    public  string         Model        { get; init; } = string.Empty;
    public  string         OsVersion    { get; init; } = string.Empty;


    public string? IP
    {
        get => _ip;
        set => SetProperty( ref _ip, value );
    }


    public TID ID { get; init; }


    public UserDevice() { }
    public UserDevice( string model, string manufacturer, string deviceName, DeviceType deviceType, DeviceIdiom idiom, DevicePlatform platform, AppVersion osVersion, Guid? deviceID, TID id = default! )
    {
        Model        = model;
        Manufacturer = manufacturer;
        DeviceName   = deviceName;
        DeviceType   = deviceType;
        Idiom        = idiom;
        Platform     = platform;
        OsVersion    = osVersion.ToString();
        DeviceID     = deviceID ?? Guid.NewGuid();
        TimeStamp    = DateTime.UtcNow;
        ID           = id;
    }
    public UserDevice( IUserDevice<TID> device )
    {
        ID           = device.ID;
        DeviceID     = device.DeviceID;
        TimeStamp    = device.TimeStamp;
        Model        = device.Model;
        Manufacturer = device.Manufacturer;
        DeviceName   = device.DeviceName;
        DeviceType   = device.DeviceType;
        Idiom        = device.Idiom;
        OsVersion    = device.OsVersion;
        Platform     = device.Platform;
    }


    // public static UserDevice<TID> Create( Guid? deviceID ) => new(DeviceInfo.Model, DeviceInfo.Manufacturer, DeviceInfo.Name, DeviceInfo.DeviceType, DeviceInfo.Idiom, DeviceInfo.Platform, DeviceInfo.Version, deviceID);


    public override bool Equals( object? obj )
    {
        if (obj is null) { return false; }

        if (ReferenceEquals( this, obj )) { return true; }

        return obj is UserDevice<TID> device && Equals( device );
    }
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add( TimeStamp );
        hashCode.Add( IP );
        hashCode.Add( Model );
        hashCode.Add( Manufacturer );
        hashCode.Add( DeviceName );
        hashCode.Add( DeviceType );
        hashCode.Add( Idiom );
        hashCode.Add( Platform );
        hashCode.Add( OsVersion );
        return hashCode.ToHashCode();
    }


    public static bool operator ==( UserDevice<TID>? left, UserDevice<TID>? right ) => Equals( left, right );
    public static bool operator !=( UserDevice<TID>? left, UserDevice<TID>? right ) => !Equals( left, right );
    public bool Equals( IUserDevice<TID>? other )
    {
        if (other is null) { return false; }

        if (ReferenceEquals( this, other )) { return true; }

        return TimeStamp.Equals( other.TimeStamp ) && IP == other.IP && Model == other.Model && Manufacturer == other.Manufacturer && DeviceName == other.DeviceName && DeviceType == other.DeviceType && Idiom == other.Idiom &&
               Platform == other.Platform && OsVersion == other.OsVersion;
    }
}
