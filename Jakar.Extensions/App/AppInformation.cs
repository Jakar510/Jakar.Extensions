// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  15:01

namespace Jakar.Extensions;


[StructLayout(LayoutKind.Auto)]
[Serializable]
[DefaultValue(nameof(Invalid))]
[method: JsonCreator]
[JsonModel]
public readonly partial struct AppInformation( AppVersion version, Guid appID, string appName, string? packageName )
{
    public static readonly AppInformation Invalid     = new(AppVersion.Default, Guid.Empty, EMPTY, null);
    public readonly        AppVersion     Version     = version;
    public readonly        Guid           AppID       = appID;
    public readonly        string         AppName     = appName;
    public readonly        string?        PackageName = packageName;


    public int CompareTo( object? other ) => other is AppInformation app
                                                 ? CompareTo(app)
                                                 : throw new ExpectedValueTypeException(other, typeof(AppInformation));
    public int CompareTo( AppInformation other )
    {
        int appNameComparison = string.Compare(AppName, other.AppName, StringComparison.Ordinal);
        if ( appNameComparison != 0 ) { return appNameComparison; }

        int appIDComparison = AppID.CompareTo(other.AppID);
        if ( appIDComparison != 0 ) { return appIDComparison; }

        int versionComparison = Version.CompareTo(other.Version);
        if ( versionComparison != 0 ) { return versionComparison; }

        return string.Compare(PackageName, other.PackageName, StringComparison.Ordinal);
    }
    public          bool Equals( AppInformation other ) => Version.Equals(other.Version) && AppID.Equals(other.AppID) && string.Equals(AppName, other.AppName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(PackageName, other.PackageName, StringComparison.InvariantCultureIgnoreCase);
    public override bool Equals( object?        obj )   => obj is AppInformation other   && Equals(other);
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(Version);
        hashCode.Add(AppID);
        hashCode.Add(AppName,     StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(PackageName, StringComparer.InvariantCultureIgnoreCase);
        return hashCode.ToHashCode();
    }


    public static bool operator ==( AppInformation? left, AppInformation? right ) => Nullable.Equals(left, right);
    public static bool operator !=( AppInformation? left, AppInformation? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( AppInformation  left, AppInformation  right ) => EqualityComparer<AppInformation>.Default.Equals(left, right);
    public static bool operator !=( AppInformation  left, AppInformation  right ) => !EqualityComparer<AppInformation>.Default.Equals(left, right);
    public static bool operator >( AppInformation   left, AppInformation  right ) => Comparer<AppInformation>.Default.Compare(left, right) > 0;
    public static bool operator >=( AppInformation  left, AppInformation  right ) => Comparer<AppInformation>.Default.Compare(left, right) >= 0;
    public static bool operator <( AppInformation   left, AppInformation  right ) => Comparer<AppInformation>.Default.Compare(left, right) < 0;
    public static bool operator <=( AppInformation  left, AppInformation  right ) => Comparer<AppInformation>.Default.Compare(left, right) <= 0;
}
