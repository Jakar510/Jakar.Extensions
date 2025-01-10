// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  16:01

namespace Jakar.Extensions.Telemetry;


public readonly struct AppContext : IEquatable<AppContext>
{
    public required string     Name    { get; init; }
    public required Guid       ID      { get; init; }
    public required AppVersion Version { get; init; }

    public static AppContext Create<TApp>()
        where TApp : IAppID => Create( TApp.AppName, TApp.AppID, TApp.AppVersion );
    public static AppContext Create( string appName, Guid appID, AppVersion appVersion ) => new()
                                                                                            {
                                                                                                Name    = appName,
                                                                                                ID      = appID,
                                                                                                Version = appVersion
                                                                                            };


    public          bool Equals( AppContext other )                       => Name == other.Name      && ID.Equals( other.ID ) && Version.Equals( other.Version );
    public override bool Equals( object?    other )                       => other is AppContext app && Equals( app );
    public override int  GetHashCode()                                    => HashCode.Combine( Name, ID, Version );
    public static   bool operator ==( AppContext left, AppContext right ) => left.Equals( right );
    public static   bool operator !=( AppContext left, AppContext right ) => !left.Equals( right );
}
