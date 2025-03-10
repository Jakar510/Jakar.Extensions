// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  14:01

namespace Jakar.Extensions.Serilog;


public sealed class AppContextEnricher<TApp>() : ILogEventEnricher
    where TApp : IApp
{
    private LogEventProperty? _appName;
    private LogEventProperty? _appID;
    private LogEventProperty? _appVersion;
    private LogEventProperty? _timestamp;
    private LogEventProperty? _selectedLanguage;


    public void Enrich( LogEvent log, ILogEventPropertyFactory factory )
    {
        if ( log.Level is LogEventLevel.Verbose ) { return; }

        log.AddPropertyIfAbsent( _appName          ??= factory.CreateProperty( nameof(TApp.AppName),                 TApp.AppName ) );
        log.AddPropertyIfAbsent( _appID            ??= factory.CreateProperty( nameof(TApp.AppID),                   TApp.AppID.ToString() ) );
        log.AddPropertyIfAbsent( _appVersion       ??= factory.CreateProperty( nameof(TApp.AppVersion),              TApp.AppVersion.ToString() ) );
        log.AddPropertyIfAbsent( _timestamp        ??= factory.CreateProperty( EventDetails.TIMESTAMP,               DateTimeOffset.UtcNow ) );
        log.AddPropertyIfAbsent( _selectedLanguage ??= factory.CreateProperty( nameof(LanguageApi.SelectedLanguage), CultureInfo.CurrentUICulture.DisplayName ) );
        log.TryEnrich( factory, Thread.CurrentThread );
    }
}
