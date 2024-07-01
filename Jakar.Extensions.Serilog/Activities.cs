// Jakar.Extensions :: Jakar.Extensions.Serilog
// 07/01/2024  15:07

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;



namespace Jakar.Extensions.Serilog;


public static class Activities
{
    private static readonly Action<ILogger, string, Exception?> _logEventCallback = LoggerMessage.Define<string>( LogLevel.Information, new EventId( -1329573148, nameof(LogEvent) ), "{EventName}", new LogDefineOptions { SkipEnabledCheck = true } );


    public static void LogEvent( this ILogger logger, [CallerMemberName] string eventName = BaseRecord.EMPTY )
    {
        if ( logger.IsEnabled( LogLevel.Information ) ) { _logEventCallback( logger, eventName, null ); }
    }


    public static Activity AddEvent( this Activity activity, [CallerMemberName] string eventID                                 = BaseRecord.EMPTY ) => activity.AddEvent( null, eventID );
    public static Activity AddEvent( this Activity activity, ActivityTagsCollection?   tags, [CallerMemberName] string eventID = BaseRecord.EMPTY ) => activity.AddEvent( new ActivityEvent( eventID, DateTimeOffset.UtcNow, tags ) );
}