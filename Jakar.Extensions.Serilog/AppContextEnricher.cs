// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 07/01/2024  12:07

namespace Jakar.Extensions.Serilog;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class AppContextEnricher<TApp>() : ILogEventEnricher
    where TApp : IAppID
{
    public const string TOTAL_MEMORY                      = nameof(TOTAL_MEMORY);
    public const string TOTAL_PAUSE_DURATION              = nameof(TOTAL_PAUSE_DURATION);
    public const string TOTAL_ALLOCATED_BYTES             = nameof(TOTAL_ALLOCATED_BYTES);
    public const string THREAD_ID                         = $"{nameof(Environment)}.{nameof(Environment.CurrentManagedThreadId)}";
    public const string THREAD_NAME                       = $"{nameof(Thread)}.{nameof(Thread.Name)}";
    public const string TWO_LETTER_ISO_LANGUAGE_NAME      = $"{nameof(CultureInfo)}.{nameof(CultureInfo.TwoLetterISOLanguageName)}";
    public const string THREE_LETTER_ISO_LANGUAGE_NAME    = $"{nameof(CultureInfo)}.{nameof(CultureInfo.ThreeLetterISOLanguageName)}";
    public const string UI_TWO_LETTER_ISO_LANGUAGE_NAME   = $"{nameof(CultureInfo)}.{nameof(CultureInfo.TwoLetterISOLanguageName)}.UI";
    public const string UI_THREE_LETTER_ISO_LANGUAGE_NAME = $"{nameof(CultureInfo)}.{nameof(CultureInfo.ThreeLetterISOLanguageName)}.UI";


    public virtual void Enrich( LogEvent log, ILogEventPropertyFactory factory ) => Enrich( log, factory, Thread.CurrentThread );
    public virtual void Enrich( LogEvent log, ILogEventPropertyFactory factory, Thread thread )
    {
        log.AddPropertyIfAbsent( factory.CreateProperty( nameof(TApp.AppName),              TApp.AppName ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( nameof(TApp.AppID),                TApp.AppID.ToString() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( nameof(TApp.AppVersion),           TApp.AppVersion.ToString() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( TOTAL_MEMORY,                      GC.GetTotalMemory( false ) ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( TOTAL_PAUSE_DURATION,              GC.GetTotalPauseDuration() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( TOTAL_ALLOCATED_BYTES,             GC.GetTotalAllocatedBytes() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( THREAD_ID,                         Environment.CurrentManagedThreadId ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( THREAD_NAME,                       thread.Name ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( THREE_LETTER_ISO_LANGUAGE_NAME,    thread.CurrentCulture.ThreeLetterISOLanguageName ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( TWO_LETTER_ISO_LANGUAGE_NAME,      thread.CurrentCulture.TwoLetterISOLanguageName ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( UI_THREE_LETTER_ISO_LANGUAGE_NAME, thread.CurrentUICulture.ThreeLetterISOLanguageName ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( UI_TWO_LETTER_ISO_LANGUAGE_NAME,   thread.CurrentUICulture.TwoLetterISOLanguageName ) );
    }
}
