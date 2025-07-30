// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  16:01

namespace Jakar.Extensions.Telemetry;


public sealed class AppContext : IEquatable<AppContext>, IComparable<AppContext>, IComparable
{
    internal readonly string     name;
    internal readonly Guid       id;
    internal readonly AppVersion version;


    public static   EqualComparer<AppContext> Sorter  => EqualComparer<AppContext>.Default;
    public required string             Name    { get => name;    [MemberNotNull( nameof(name) )] init => name = value; }
    public required Guid               ID      { get => id;      init => id = value; }
    public required AppVersion         Version { get => version; [MemberNotNull( nameof(version) )] init => version = value; }


    public static AppContext Create<TApp>()
        where TApp : IAppID => Create( TApp.AppName, TApp.AppID, TApp.AppVersion );
    public static AppContext Create( string appName, Guid appID, AppVersion appVersion ) => new()
                                                                                            {
                                                                                                Name    = appName,
                                                                                                ID      = appID,
                                                                                                Version = appVersion
                                                                                            };


    public int CompareTo( AppContext? other )
    {
        if ( other is null ) { return -1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int nameComparison = string.Compare( name, other.name, StringComparison.Ordinal );
        if ( nameComparison != 0 ) { return nameComparison; }

        int idComparison = id.CompareTo( other.id );
        if ( idComparison != 0 ) { return idComparison; }

        return version.CompareTo( other.version );
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is AppContext other
                   ? CompareTo( other )
                   : throw new ArgumentException( $"Object must be of type {nameof(AppContext)}" );
    }
    public bool Equals( AppContext? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals( this, other ) || Name == other.Name && ID.Equals( other.ID ) && Version.Equals( other.Version );
    }
    public override bool Equals( object? other ) => other is AppContext app && Equals( app );
    public override int  GetHashCode()           => HashCode.Combine( Name, ID, Version );


    public static bool operator ==( AppContext left, AppContext right ) => Sorter.Equals( left, right );
    public static bool operator !=( AppContext left, AppContext right ) => Sorter.DoesNotEqual( left, right );
    public static bool operator <( AppContext  left, AppContext right ) => Sorter.Compare( left, right ) < 0;
    public static bool operator >( AppContext  left, AppContext right ) => Sorter.Compare( left, right ) > 0;
    public static bool operator <=( AppContext left, AppContext right ) => Sorter.Compare( left, right ) <= 0;
    public static bool operator >=( AppContext left, AppContext right ) => Sorter.Compare( left, right ) >= 0;
}
