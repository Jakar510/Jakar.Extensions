#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public interface IUserDevice<TID> : IEquatable<IUserDevice<TID>>, IUniqueID<TID> where TID : struct, IComparable<TID>, IEquatable<TID>
{
    public DateTime TimeStamp { get; }
    public Guid     DeviceID  { get; }


    /// <summary>
    ///     <see cref = "DeviceType" />
    /// </summary>
    public int DeviceTypeID { get; }

    public string DeviceName { get; }


    /// <summary>
    ///     <see cref = "DeviceIdiom" />
    /// </summary>
    public string Idiom { get; }

    public string Manufacturer { get; }
    public string Model        { get; }
    string        OsVersion    { get; }


    /// <summary>
    ///     <see cref = "DevicePlatform" />
    /// </summary>
    public string Platform { get; }


    /// <summary>
    ///     Last known
    ///     <see cref = "IPAddress" />
    /// </summary>
    public string? Ip { get; }
}



/// <summary>
///     Debug and/or identify info for IT
/// </summary>
[Serializable]
public class UserDevice<TID> : ObservableClass, IUserDevice<TID> where TID : struct, IComparable<TID>, IEquatable<TID>
{
    private string?  _ip;
    public  DateTime TimeStamp    { get; init; }
    public  Guid     DeviceID     { get; init; }
    public  int      DeviceTypeID { get; init; }
    public  string   DeviceName   { get; init; } = string.Empty;
    public  string   Idiom        { get; init; } = string.Empty;
    public  string   Manufacturer { get; init; } = string.Empty;
    public  string   Model        { get; init; } = string.Empty;
    public  string   OsVersion    { get; init; } = string.Empty;
    public  string   Platform     { get; init; } = string.Empty;

    public string? Ip
    {
        get => _ip;
        set => SetProperty( ref _ip, value );
    }


    [Key] public TID ID { get; init; }


    public UserDevice() { }
    public UserDevice( string model, string manufacturer, string deviceName, DeviceType deviceType, DeviceIdiom idiom, DevicePlatform platform, AppVersion osVersion, Guid? deviceID, TID id = default! )
    {
        Model        = model;
        Manufacturer = manufacturer;
        DeviceName   = deviceName;
        DeviceTypeID = deviceType.AsInt();
        Idiom        = idiom.ToString();
        Platform     = platform.ToString();
        OsVersion    = osVersion.ToString();
        DeviceID     = deviceID ?? Guid.NewGuid();
        TimeStamp    = DateTime.UtcNow;
        ID           = id;
    }
    public UserDevice( IUserDevice<TID> device, TID id = default! )
    {
        ID           = id;
        DeviceID     = device.DeviceID;
        TimeStamp    = device.TimeStamp;
        Model        = device.Model;
        Manufacturer = device.Manufacturer;
        DeviceName   = device.DeviceName;
        DeviceTypeID = device.DeviceTypeID;
        Idiom        = device.Idiom;
        OsVersion    = device.OsVersion;
        Platform     = device.Platform;
    }


    // public static UserDevice<TID> Create( Guid? deviceID ) => new(DeviceInfo.Model, DeviceInfo.Manufacturer, DeviceInfo.Name, (DeviceType)DeviceInfo.DeviceType, DeviceInfo.Idiom, DeviceInfo.Platform, DeviceInfo.Version, deviceID);


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
        hashCode.Add( Ip );
        hashCode.Add( Model );
        hashCode.Add( Manufacturer );
        hashCode.Add( DeviceName );
        hashCode.Add( DeviceTypeID );
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

        return TimeStamp.Equals( other.TimeStamp ) && Ip == other.Ip && Model == other.Model && Manufacturer == other.Manufacturer && DeviceName == other.DeviceName && DeviceTypeID == other.DeviceTypeID && Idiom == other.Idiom &&
               Platform == other.Platform && OsVersion == other.OsVersion;
    }
}
