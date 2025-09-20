// Jakar.Extensions :: Jakar.Extensions
// 01/06/2025  09:01

using System.Runtime.InteropServices;
using Serilog.Context;
using Serilog.Debugging;



namespace Jakar.Extensions.Serilog;


public sealed class FilePathsEnricher( ISerilogger serilogger ) : ILogEventEnricher
{
    private readonly ISerilogger __logger = serilogger;
    void ILogEventEnricher.Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory )
    {
        try
        {
            if ( logEvent.Level < LogEventLevel.Error ) { return; }

            using Disposables disposables = new();
            List<Task>        tasks       = new(20);

            if ( __logger.Settings.IncludeAppStateOnError )
            {
                tasks.Add( Handle( disposables, __logger.Settings.Paths.Screenshot ) );

                ReadOnlyMemory<byte> data = __logger.ScreenShotData;
                if ( !data.IsEmpty ) { disposables.Add( data.GetAttachment( FilePaths.SCREEN_SHOT_FILE, MimeTypeNames.Image.PNG ).AddFileToLogContext() ); }
            }

            if ( __logger.Settings.IncludeAppStateOnError )
            {
                tasks.Add( Handle( disposables, __logger.Settings.Paths.FeedbackFile ) );
                tasks.Add( Handle( disposables, __logger.Settings.Paths.AppStateFile ) );
                tasks.Add( Handle( disposables, __logger.Settings.Paths.CrashFile ) );
                tasks.Add( Handle( disposables, __logger.Settings.Paths.IncomingFile ) );
                tasks.Add( Handle( disposables, __logger.Settings.Paths.OutgoingFile ) );
                tasks.Add( Handle( disposables, __logger.Settings.Paths.AppDataZipFile ) );
                tasks.Add( Handle( disposables, __logger.Settings.Paths.AppCacheZipFile ) );
                tasks.Add( Handle( disposables, __logger.Settings.Paths.LogsZipFile ) );
            }

            Task.WhenAll( CollectionsMarshal.AsSpan( tasks ) ).CallSynchronously();
        }
        catch ( Exception e ) { SelfLog.WriteLine( "{Exception}", e ); }
    }
    private static async Task Handle( Disposables disposables, LocalFile? file, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        if ( file?.Exists is not true ) { return; }
        
        byte[]              content       = await file.ReadAsync().AsBytes( token);
        file.Delete();

        disposables.Add( LogContext.PushProperty( file.Name, Convert.ToBase64String( content ) ) );
    }
}
