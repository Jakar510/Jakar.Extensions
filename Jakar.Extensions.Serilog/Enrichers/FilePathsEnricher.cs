// Jakar.Extensions :: Jakar.Extensions
// 01/06/2025  09:01

using System.Runtime.InteropServices;
using Serilog.Context;



namespace Jakar.Extensions.Serilog;


public sealed class FilePathsEnricher() : ILogEventEnricher
{
    private static ISerilogger _Logger => ISerilogger.Instance ?? throw new InvalidOperationException( $"{nameof(ISerilogger)} has not been created" );
    void ILogEventEnricher.Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory )
    {
        try
        {
            if ( logEvent.Level < LogEventLevel.Error ) { return; }

            using Disposables disposables = new();
            List<Task>        tasks       = new(20);

            if ( _Logger.Settings.IncludeAppStateOnError )
            {
                tasks.Add( Handle( disposables, _Logger.Settings.Paths.ScreenShotAddress ) );

                ReadOnlyMemory<byte> data = _Logger.ScreenShotData;
                if ( data.IsEmpty is false ) { disposables.Add( data.GetAttachment( IFilePaths.SCREEN_SHOT_FILE, MimeTypeNames.Image.PNG ).AddFileToLogContext() ); }
            }

            if ( _Logger.Settings.IncludeAppStateOnError )
            {
                tasks.Add( Handle( disposables, _Logger.Settings.Paths.FeedbackFile ) );
                tasks.Add( Handle( disposables, _Logger.Settings.Paths.AppStateFile ) );
                tasks.Add( Handle( disposables, _Logger.Settings.Paths.CrashFile ) );
                tasks.Add( Handle( disposables, _Logger.Settings.Paths.IncomingFile ) );
                tasks.Add( Handle( disposables, _Logger.Settings.Paths.OutgoingFile ) );
                tasks.Add( Handle( disposables, _Logger.Settings.Paths.AppDataZipFile ) );
                tasks.Add( Handle( disposables, _Logger.Settings.Paths.AppCacheZipFile ) );
                tasks.Add( Handle( disposables, _Logger.Settings.Paths.ZipLogsFile ) );
            }

            Task.WhenAll( CollectionsMarshal.AsSpan( tasks ) ).CallSynchronously();
        }
        catch ( Exception e ) { Debug.WriteLine( e ); }
    }
    private static async Task Handle( Disposables disposables, LocalFile? file, CancellationToken token = default )
    {
        if ( file?.Exists is not true ) { return; }

        byte[] content = await file.ReadAsync().AsBytes( token );
        file.Delete();

        disposables.Add( LogContext.PushProperty( file.Name, Convert.ToBase64String( content ) ) );
    }
}
