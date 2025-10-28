// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  15:01

using Serilog.Events;



namespace Jakar.Extensions;


[StructLayout(LayoutKind.Auto)]
[Serializable]
[DefaultValue(nameof(Invalid))]
[method: JsonConstructor]
public readonly struct AppInformation( AppVersion version, Guid appID, string appName, string? packageName ) : IJsonModel<AppInformation>
{
    public static readonly AppInformation Invalid     = new(AppVersion.Default, Guid.Empty, EMPTY, null);
    public readonly        AppVersion     Version     = version;
    public readonly        Guid           AppID       = appID;
    public readonly        string         AppName     = appName;
    public readonly        string?        PackageName = packageName;


    public static JsonSerializerContext          JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<AppInformation>   JsonTypeInfo  => JakarExtensionsContext.Default.AppInformation;
    public static JsonTypeInfo<AppInformation[]> JsonArrayInfo => JakarExtensionsContext.Default.AppInformationArray;


    public StructureValue   GetStructureValue() => new([Enricher.GetProperty(Version, nameof(Version)), Enricher.GetProperty(AppID, nameof(AppID)), Enricher.GetProperty(AppName, nameof(AppName)), Enricher.GetProperty(PackageName, nameof(PackageName))]);
    public LogEventProperty GetProperty()       => new(nameof(AppInformation), GetStructureValue());


    public static bool TryFromJson( string? json, out AppInformation result )
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
    public static AppInformation FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));


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
