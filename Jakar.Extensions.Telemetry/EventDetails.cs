// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  15:01

namespace Jakar.Extensions.Telemetry;


public sealed class EventDetails : Dictionary<string, string?>
{
    public EventDetails() : base( 20 ) { }
    public EventDetails( IDictionary<string, string?> dictionary ) : base( dictionary ) => EnsureCapacity( 20 );


    // public override string ToString() => this.ToJson();
    public static void AddAppState<TValue>( ref readonly TValue dictionary, string appName )
        where TValue : class, IDictionary<string, object?> => dictionary[nameof(AppState)] = AppState( appName );
    public static EventDetails AppState( string appName ) =>
        new()
        {
            [nameof(IAppSettings.AppName)]         = appName,
            [nameof(DateTime)]                     = DateTimeOffset.UtcNow.ToString( CultureInfo.InvariantCulture ),
            [nameof(LanguageApi.SelectedLanguage)] = CultureInfo.CurrentUICulture.DisplayName
        };
}
