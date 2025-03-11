// Jakar.Extensions :: Jakar.Extensions.Serilog
// 03/10/2025  16:03

using Google.Protobuf.WellKnownTypes;



namespace Jakar.Extensions.Serilog;


public static class Enrichers
{
    public static void TryEnrich( this LogEvent log, ILogEventPropertyFactory factory, string sourceContext ) => log.AddPropertyIfAbsent( factory.CreateProperty( Constants.SourceContextPropertyName, sourceContext ) );
    public static void TryEnrich( this LogEvent log, ILogEventPropertyFactory factory, Thread thread )
    {
        if ( log.Level < LogEventLevel.Warning ) { return; }

        log.AddPropertyIfAbsent( factory.CreateProperty( "TOTAL_MEMORY",          GC.GetTotalMemory( false ) ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( "TOTAL_PAUSE_DURATION",  GC.GetTotalPauseDuration() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( "TOTAL_ALLOCATED_BYTES", GC.GetTotalAllocatedBytes() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( "CURRENT_THREAD_ID",     Environment.CurrentManagedThreadId ) ); // nameof(Environment.CurrentManagedThreadId)
        log.AddPropertyIfAbsent( factory.CreateProperty( "THREAD_ID",             thread.ManagedThreadId ) );             // nameof(Environment.CurrentManagedThreadId)
        log.AddPropertyIfAbsent( factory.CreateProperty( "THREAD_NAME",           thread.Name ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( "LANGUAGE",              thread.CurrentCulture.ToString() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( "UI_LANGUAGE",           thread.CurrentUICulture.ToString() ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( "ISO_LANGUAGE",          thread.CurrentCulture.ThreeLetterISOLanguageName ) );
        log.AddPropertyIfAbsent( factory.CreateProperty( "UI_ISO_LANGUAGE",       thread.CurrentUICulture.ThreeLetterISOLanguageName ) );
    }
}
