// Jakar.Extensions :: Jakar.Extensions
// 01/02/2025  12:01

namespace Jakar.Extensions.Serilog;


public sealed class EventDetails : Dictionary<string, string?>
{
    public EventDetails() : base( 20 ) { }
    public EventDetails( IDictionary<string, string?> dictionary ) : base( dictionary ) => EnsureCapacity( 20 );


    public override string ToString() => this.ToJson();
    public static void AddAppState<T>( ref readonly T dictionary, string appName )
        where T : class, IDictionary<string, object?> => dictionary[nameof(AppState)] = AppState( appName );
    public static EventDetails AppState( string appName ) =>
        new()
        {
            [nameof(IAppSettings.AppName)]         = appName,
            [nameof(DateTime)]                     = DateTimeOffset.UtcNow.ToString( CultureInfo.InvariantCulture ),
            [nameof(LanguageApi.SelectedLanguage)] = CultureInfo.CurrentUICulture.DisplayName
        };
}