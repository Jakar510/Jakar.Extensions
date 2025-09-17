// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  14:01

namespace Jakar.Extensions.Serilog;


public sealed class AppContextEnricher( SeriloggerOptions options ) : ILogEventEnricher

{
    private readonly SeriloggerOptions __options = options;
    private          LogEventProperty? __appName;
    private          LogEventProperty? __appID;
    private          LogEventProperty? __appVersion;
    private          LogEventProperty? __timestamp;
    private          LogEventProperty? __selectedLanguage;

    public static AppContextEnricher Create( SeriloggerOptions options ) => new(options);
    public void Enrich( LogEvent log, ILogEventPropertyFactory factory )
    {
        if ( log.Level is LogEventLevel.Verbose ) { return; }

        log.AddPropertyIfAbsent( __appName          ??= factory.CreateProperty( nameof(SeriloggerOptions.AppName),    __options.AppName ) );
        log.AddPropertyIfAbsent( __appID            ??= factory.CreateProperty( nameof(SeriloggerOptions.AppID),      __options.AppID.ToString() ) );
        log.AddPropertyIfAbsent( __appVersion       ??= factory.CreateProperty( nameof(SeriloggerOptions.AppVersion), __options.AppVersion.ToString() ) );
        log.AddPropertyIfAbsent( __timestamp        ??= factory.CreateProperty( EventDetails.TIMESTAMP,               DateTimeOffset.UtcNow.ToString() ) );
        log.AddPropertyIfAbsent( __selectedLanguage ??= factory.CreateProperty( nameof(LanguageApi.SelectedLanguage), CultureInfo.CurrentUICulture.DisplayName ) );

        log.TryEnrich( factory, Thread.CurrentThread );
    }
}
