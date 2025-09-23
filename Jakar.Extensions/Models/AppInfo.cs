// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  15:01

namespace Jakar.Extensions;


[StructLayout(LayoutKind.Auto), Serializable, DefaultValue(nameof(Invalid))]
public readonly struct AppInformation( AppVersion Version, Guid AppID, string AppName, string? PackageName ) : IJsonModel<AppInformation>, IEqualComparable<AppInformation>
{
    public static JsonSerializerContext          JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<AppInformation>   JsonTypeInfo  => JakarExtensionsContext.Default.AppInformation;
    public static JsonTypeInfo<AppInformation[]> JsonArrayInfo => JakarExtensionsContext.Default.AppInformationArray;
    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(Version);
        hashCode.Add(AppID);
        hashCode.Add(AppName,     StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(PackageName, StringComparer.InvariantCultureIgnoreCase);
        return hashCode.ToHashCode();
    }
    public static readonly AppInformation Invalid     = new(AppVersion.Default, Guid.Empty, string.Empty, null);
    public readonly        AppVersion     Version     = Version;
    public readonly        Guid           AppID       = AppID;
    public readonly        string         AppName     = AppName;
    public readonly        string?        PackageName = PackageName;


    public           JsonNode       ToJsonNode() => Validate.ThrowIfNull(JsonSerializer.SerializeToNode(this, JsonTypeInfo));
    public           string         ToJson()     => Validate.ThrowIfNull(JsonSerializer.Serialize(this, JsonTypeInfo));
    public static bool TryFromJson( string? json,   out AppInformation result )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace(json) )
            {
                result = default;
                return false;
            }

            result = FromJson(json);
            return true;
        }
        catch ( Exception e ) { SelfLogger.WriteLine("{Exception}", e.ToString()); }

        result = default;
        return false;
    }
    public static AppInformation FromJson( string   json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
    public        int            CompareTo( object? other)  => other is AppInformation app ? CompareTo(app) : throw new ExpectedValueTypeException(other, typeof(AppInformation));

    
    public          bool Equals( AppInformation other ) => Version.Equals(other.Version) && AppID.Equals(other.AppID) && string.Equals(AppName, other.AppName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(PackageName, other.PackageName, StringComparison.InvariantCultureIgnoreCase);
    public override bool Equals( object?        obj )   => obj is AppInformation other   && Equals(other);
    public   int CompareTo( AppInformation other )
    {
        int appNameComparison = string.Compare(AppName, other.AppName, StringComparison.Ordinal);
        if ( appNameComparison != 0 ) { return appNameComparison; }

        int appIDComparison = AppID.CompareTo(other.AppID);
        if ( appIDComparison != 0 ) { return appIDComparison; }

        int versionComparison = Version.CompareTo(other.Version);
        if ( versionComparison != 0 ) { return versionComparison; }

        return string.Compare(PackageName, other.PackageName, StringComparison.Ordinal);
    }


    public static bool operator ==( AppInformation? left, AppInformation? right ) => Nullable.Equals(left, right);
    public static bool operator !=( AppInformation? left, AppInformation? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( AppInformation left, AppInformation right ) => EqualityComparer<AppInformation>.Default.Equals(left, right);
    public static bool operator !=( AppInformation left, AppInformation right ) => !EqualityComparer<AppInformation>.Default.Equals(left, right);
    public static bool operator >( AppInformation   left, AppInformation  right ) => Comparer<AppInformation>.Default.Compare(left, right) > 0;
    public static bool operator >=( AppInformation  left, AppInformation  right ) => Comparer<AppInformation>.Default.Compare(left, right) >= 0;
    public static bool operator <( AppInformation   left, AppInformation  right ) => Comparer<AppInformation>.Default.Compare(left, right) < 0;
    public static bool operator <=( AppInformation  left, AppInformation  right ) => Comparer<AppInformation>.Default.Compare(left, right) <= 0;
}
