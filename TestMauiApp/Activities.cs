// Jakar.Extensions :: TestMauiApp
// 06/27/2024  16:06

using System.Runtime.CompilerServices;



namespace TestMauiApp;


public static class Activities
{
    public const            string                              LOGS_DIRECTORY    = "Logs";
    public const            string                              LOGS_FILE         = "App.Logs";
    private static readonly Action<ILogger, string, Exception?> _logEventCallback = LoggerMessage.Define<string>( LogLevel.Information, new EventId( -1329573148, nameof(LogEvent) ), "{EventName}", new LogDefineOptions { SkipEnabledCheck = true } );
    public static           LocalDirectory                      LogsDirectory { get; set; } = LOGS_DIRECTORY;
    public static           LocalFile                           LogsFile      { get; set; } = Path.Combine( LOGS_DIRECTORY, LOGS_FILE );


    public static LocalDirectory SetLogsDirectory( LocalDirectory directory ) => LogsDirectory = directory.Combine( LOGS_DIRECTORY );
    public static LocalFile      SetLogsFile( LocalDirectory      directory ) => SetLogsDirectory( directory ).Join( LOGS_FILE );


    public static void LogEvent( this ILogger logger, [CallerMemberName] string eventName = BaseRecord.EMPTY )
    {
        if ( logger.IsEnabled( LogLevel.Information ) ) { _logEventCallback( logger, eventName, null ); }
    }


    public static Activity AddEvent( this Activity activity, [CallerMemberName] string eventID                                 = BaseRecord.EMPTY ) => activity.AddEvent( null, eventID );
    public static Activity AddEvent( this Activity activity, ActivityTagsCollection?   tags, [CallerMemberName] string eventID = BaseRecord.EMPTY ) => activity.AddEvent( new ActivityEvent( eventID, DateTimeOffset.UtcNow, tags ) );
}
