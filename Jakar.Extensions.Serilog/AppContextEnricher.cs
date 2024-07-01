// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 07/01/2024  12:07

namespace Jakar.Extensions.Serilog;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class AppContextEnricher<TApp>() : ILogEventEnricher
    where TApp : IAppID
{
    public const string TOTAL_MEMORY                      = "TotalMemory";
    public const string TOTAL_PAUSE_DURATION              = "TotalPauseDuration";
    public const string TOTAL_ALLOCATED_BYTES             = "TotalAllocatedBytes";
    public const string THREAD_ID                         = "ThreadID";
    public const string THREAD_NAME                       = "ThreadName";
    public const string TWO_LETTER_ISO_LANGUAGE_NAME      = nameof(CultureInfo.TwoLetterISOLanguageName);
    public const string THREE_LETTER_ISO_LANGUAGE_NAME    = nameof(CultureInfo.ThreeLetterISOLanguageName);
    public const string UI_TWO_LETTER_ISO_LANGUAGE_NAME   = $"UI_{nameof(CultureInfo.TwoLetterISOLanguageName)}";
    public const string UI_THREE_LETTER_ISO_LANGUAGE_NAME = $"UI_{nameof(CultureInfo.ThreeLetterISOLanguageName)}";


    public virtual void Enrich( LogEvent log, ILogEventPropertyFactory factory )
    {
        log.AddPropertyIfAbsent( factory.CreateProperty( nameof(TApp.AppName),              TApp.AppName ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( nameof(TApp.AppID),                TApp.AppID ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( nameof(TApp.AppVersion),           TApp.AppVersion.ToString() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( TOTAL_MEMORY,                      GC.GetTotalMemory( false ) ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( TOTAL_PAUSE_DURATION,              GC.GetTotalPauseDuration() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( TOTAL_ALLOCATED_BYTES,             GC.GetTotalAllocatedBytes() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( THREAD_ID,                         Environment.CurrentManagedThreadId ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( THREAD_NAME,                       Thread.CurrentThread.Name ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( THREE_LETTER_ISO_LANGUAGE_NAME,    Thread.CurrentThread.CurrentCulture.ThreeLetterISOLanguageName ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( TWO_LETTER_ISO_LANGUAGE_NAME,      Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( UI_THREE_LETTER_ISO_LANGUAGE_NAME, Thread.CurrentThread.CurrentUICulture.ThreeLetterISOLanguageName ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( UI_TWO_LETTER_ISO_LANGUAGE_NAME,   Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName ) );
    }
}
