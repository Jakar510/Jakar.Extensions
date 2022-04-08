﻿namespace Jakar.Extensions.Models.Authorization;


public interface IUserDevice : IEquatable<IUserDevice>, IDataBaseID
{
    public DateTime TimeStamp    { get; }
    public Guid     DeviceID     { get; }
    public string   Model        { get; }
    public string   Manufacturer { get; }
    public string   DeviceName   { get; }
    string          OsVersion    { get; }


    /// <summary>
    /// Last known <see cref="IPAddress"/>
    /// </summary>
    public string? Ip { get; }


    /// <summary>
    /// <see cref="DevicePlatform"/>
    /// </summary>
    public string Platform { get; }


    /// <summary>
    /// <see cref="DeviceIdiom"/>
    /// </summary>
    public string Idiom { get; }


    /// <summary>
    /// <see cref="DeviceType"/>
    /// </summary>
    public int DeviceTypeID { get; }
}



/// <summary>
/// Debug and/or identify info for IT
/// </summary>
[Serializable]
[Table("UserDevices")]
public class UserDevice : BaseClass, IUserDevice
{
    public DateTime TimeStamp    { get; init; }
    public Guid     DeviceID     { get; init; }
    public string   Model        { get; init; } = string.Empty;
    public string   Manufacturer { get; init; } = string.Empty;
    public string   DeviceName   { get; init; } = string.Empty;
    public int      DeviceTypeID { get; init; }
    public string   Idiom        { get; init; } = string.Empty;
    public string   Platform     { get; init; } = string.Empty;
    public string   OsVersion    { get; init; } = string.Empty;
    public string?  Ip           { get; init; }


    public UserDevice() { }

    public UserDevice( string model, string manufacturer, string deviceName, DeviceType deviceType, DeviceIdiom idiom, DevicePlatform platform, AppVersion osVersion, Guid? deviceID, long id = default )
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

    public UserDevice( IUserDevice device, long id = default )
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


    public static UserDevice Create( Guid? deviceID ) => new(DeviceInfo.Model, DeviceInfo.Manufacturer, DeviceInfo.Name, DeviceInfo.DeviceType, DeviceInfo.Idiom, DeviceInfo.Platform, DeviceInfo.Version, deviceID);


    public override bool Equals( object? obj )
    {
        if ( obj is null ) { return false; }

        if ( ReferenceEquals(this, obj) ) { return true; }

        return obj is IUserDevice device && Equals(device);
    }

    public bool Equals( IUserDevice? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return TimeStamp.Equals(other.TimeStamp) &&
               Ip == other.Ip &&
               Model == other.Model &&
               Manufacturer == other.Manufacturer &&
               DeviceName == other.DeviceName &&
               DeviceTypeID == other.DeviceTypeID &&
               Idiom == other.Idiom &&
               Platform == other.Platform &&
               OsVersion == other.OsVersion;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(TimeStamp);
        hashCode.Add(Ip);
        hashCode.Add(Model);
        hashCode.Add(Manufacturer);
        hashCode.Add(DeviceName);
        hashCode.Add(DeviceTypeID);
        hashCode.Add(Idiom);
        hashCode.Add(Platform);
        hashCode.Add(OsVersion);
        return hashCode.ToHashCode();
    }

    public static bool operator ==( UserDevice? left, UserDevice? right ) => Equals(left, right);

    public static bool operator !=( UserDevice? left, UserDevice? right ) => !Equals(left, right);
}
