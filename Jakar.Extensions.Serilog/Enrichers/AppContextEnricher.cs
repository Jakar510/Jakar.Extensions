// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  14:01

namespace Jakar.Extensions.Serilog;


public sealed class AppContextEnricher( SeriloggerOptions options ) : ILogEventEnricher

{
    private readonly SeriloggerOptions _options = options;
    private          LogEventProperty? _appName;
    private          LogEventProperty? _appID;
    private          LogEventProperty? _appVersion;
    private          LogEventProperty? _timestamp;
    private          LogEventProperty? _selectedLanguage;

    public static AppContextEnricher Create( SeriloggerOptions options ) => new(options);
    public void Enrich( LogEvent log, ILogEventPropertyFactory factory )
    {
        if ( log.Level is LogEventLevel.Verbose ) { return; }

        log.AddPropertyIfAbsent( _appName          ??= factory.CreateProperty( nameof(SeriloggerOptions.AppName),    _options.AppName ) );
        log.AddPropertyIfAbsent( _appID            ??= factory.CreateProperty( nameof(SeriloggerOptions.AppID),      _options.AppID.ToString() ) );
        log.AddPropertyIfAbsent( _appVersion       ??= factory.CreateProperty( nameof(SeriloggerOptions.AppVersion), _options.AppVersion.ToString() ) );
        log.AddPropertyIfAbsent( _timestamp        ??= factory.CreateProperty( EventDetails.TIMESTAMP,               DateTimeOffset.UtcNow.ToString() ) );
        log.AddPropertyIfAbsent( _selectedLanguage ??= factory.CreateProperty( nameof(LanguageApi.SelectedLanguage), CultureInfo.CurrentUICulture.DisplayName ) );

        log.TryEnrich( factory, Thread.CurrentThread );
    }
}
