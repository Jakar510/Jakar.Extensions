// Jakar.Extensions :: Jakar.Database
// 12/22/2023  12:5

namespace Jakar.Database;


public static partial class Log
{
    private const string EMPTY = "";


    [ LoggerMessage( EventId = 300, Level = LogLevel.Error, Message = "Cache Failure: [ {Table} ]" ) ] public static partial void CacheFailure( ILogger logger, Exception e, string table );


    [ LoggerMessage( EventId = 200, Level = LogLevel.Information, Message = "Remote Host: {RemoteIP} {ClassName}.{Caller}" ) ]
    public static partial void Sender( ILogger logger, string remoteIP, string className, [ CallerMemberName ] string caller = EMPTY );
    public static void Sender( ILogger logger, HttpContext context, string className, [ CallerMemberName ] string caller = EMPTY ) => Sender( logger, context.Connection.RemoteIpAddress?.ToString() ?? string.Empty, className, caller );
    public static void Sender<T>( ILogger logger, HttpContext context, [ CallerMemberName ] string caller = EMPTY )
        where T : notnull => Sender( logger, context.Connection.RemoteIpAddress?.ToString() ?? string.Empty, typeof(T).Name, caller );
    public static void Sender<T>( ILogger logger, HttpContext context, T cls, [ CallerMemberName ] string caller = EMPTY )
        where T : notnull => Sender( logger, context.Connection.RemoteIpAddress?.ToString() ?? string.Empty, cls.GetType().Name, caller );


    [ LoggerMessage( EventId = 101, Level = LogLevel.Critical, Message = "'{ServiceName}'-> {IsCancelled} in {ClassName}.{Caller}" ) ]
    public static partial void ServiceStopped( ILogger logger, string serviceName, bool isCancelled, string className, Exception? e, [ CallerMemberName ] string caller = EMPTY );
    public static void ServiceStopped<T, TService>( ILogger logger, CancellationToken token, Exception? e = default, [ CallerMemberName ] string caller = EMPTY )
        where T : notnull => ServiceStopped( logger, typeof(T).Name, token.IsCancellationRequested, typeof(TService).FullName ?? typeof(TService).Name, e, caller );
    public static void ServiceStopped<T, TService>( ILogger logger, T t, TService cls, CancellationToken token, Exception? e = default, [ CallerMemberName ] string caller = EMPTY )
        where T : notnull
        where TService : notnull => ServiceStopped( logger, t.GetType().Name, token.IsCancellationRequested, cls.GetType().FullName ?? cls.GetType().Name, e, caller );


    [ LoggerMessage( EventId = 100, Level = LogLevel.Critical, Message = "'{ServiceName}' Failed in {ClassName}.{Caller}" ) ]
    public static partial void ServiceError( ILogger logger, Exception e, string serviceName, string className, [ CallerMemberName ] string caller = EMPTY );
    public static void ServiceError<T, TService>( ILogger logger, Exception e, [ CallerMemberName ] string caller = EMPTY )
        where T : notnull => ServiceError( logger, e, typeof(T).Name, typeof(TService).FullName ?? typeof(TService).Name, caller );
    public static void ServiceError<T, TService>( ILogger logger, Exception e, T t, TService cls, [ CallerMemberName ] string caller = EMPTY )
        where T : notnull
        where TService : notnull => ServiceError( logger, e, t.GetType().Name, cls.GetType().FullName ?? cls.GetType().Name, caller );


    [ LoggerMessage( EventId = 1, Level = LogLevel.Error, Message = "{ClassName}.{Caller}" ) ] public static partial void Error( ILogger logger, Exception e, string className, [ CallerMemberName ] string caller = EMPTY );
    public static void Error<T>( ILogger logger, Exception e, [ CallerMemberName ] string caller = EMPTY )
        where T : notnull => Error( logger, e, typeof(T).Name, caller );
    public static void Error<T>( ILogger logger, Exception e, T cls, [ CallerMemberName ] string caller = EMPTY )
        where T : notnull => Error( logger, e, cls.GetType().Name, caller );


    [ LoggerMessage( EventId = 2, Level = LogLevel.Critical, Message = "{ClassName}.{Caller}" ) ] public static partial void Critical( ILogger logger, Exception e, string className, [ CallerMemberName ] string caller = EMPTY );
}
