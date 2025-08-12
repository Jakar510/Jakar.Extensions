// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  15:01

namespace Jakar.Extensions;


[StructLayout(LayoutKind.Auto), Serializable, DefaultValue(nameof(Invalid))]
public readonly record struct AppInfo( AppVersion Version, Guid AppID, string AppName, string? PackageName ) : IComparable, IComparable<AppInfo>
{
    public static readonly AppInfo    Invalid     = new(AppVersion.Default, Guid.Empty, string.Empty, null);
    public readonly        AppVersion Version     = Version;
    public readonly        Guid       AppID       = AppID;
    public readonly        string     AppName     = AppName;
    public readonly        string?    PackageName = PackageName;


    public int CompareTo( object? other )
    {
        if ( other is AppInfo source ) { return CompareTo(source); }

        throw new ExpectedValueTypeException(nameof(other), other, typeof(AppInfo));
    }
    public int CompareTo( AppInfo other )
    {
        int appNameComparison = string.Compare(AppName, other.AppName, StringComparison.Ordinal);
        if ( appNameComparison != 0 ) { return appNameComparison; }

        int appIDComparison = AppID.CompareTo(other.AppID);
        if ( appIDComparison != 0 ) { return appIDComparison; }

        int versionComparison = Version.CompareTo(other.Version);
        if ( versionComparison != 0 ) { return versionComparison; }

        return string.Compare(PackageName, other.PackageName, StringComparison.Ordinal);
    }
}
