// Jakar.Extensions :: Jakar.Database
// 12/22/2023  12:5

namespace Jakar.Database;


public static class DbLog
{
    private const           string                                                    EMPTY                    = "";
    private static readonly Action<ILogger, string, string, Exception?>               __errorCallback          = LoggerMessage.Define<string, string>(LogLevel.Error,    new EventId(1,                 nameof(Error)),    "{ClassName}.{Caller}", new LogDefineOptions { SkipEnabledCheck                                          = true });
    private static readonly Action<ILogger, string, string, Exception?>               __criticalCallback       = LoggerMessage.Define<string, string>(LogLevel.Critical, new EventId(2,                 nameof(Critical)), "{ClassName}.{Caller}", new LogDefineOptions { SkipEnabledCheck                                          = true });
    private static readonly Action<ILogger, string, string, string, Exception?>       __senderCallback         = LoggerMessage.Define<string, string, string>(LogLevel.Information, new EventId(200,    nameof(Sender)),       "Remote Host: {RemoteIP} {ClassName}.{Caller}",   new LogDefineOptions { SkipEnabledCheck            = true });
    private static readonly Action<ILogger, string, string, string, Exception?>       __serviceErrorCallback   = LoggerMessage.Define<string, string, string>(LogLevel.Critical,    new EventId(100,    nameof(ServiceError)), "'{ServiceName}' Failed in {ClassName}.{Caller}", new LogDefineOptions { SkipEnabledCheck            = true });
    private static readonly Action<ILogger, string, bool, string, string, Exception?> __serviceStoppedCallback = LoggerMessage.Define<string, bool, string, string>(LogLevel.Critical, new EventId(101, nameof(ServiceStopped)), "'{ServiceName}'-> {IsCancelled} in {ClassName}.{Caller}", new LogDefineOptions { SkipEnabledCheck = true });
    private static readonly Action<ILogger, string, Exception?>                       __cacheFailureCallback   = LoggerMessage.Define<string>(LogLevel.Error, new EventId(300,                          nameof(CacheFailure)), "Cache Failure: [ {Table} ]", new LogDefineOptions { SkipEnabledCheck                                = true });


    // [ LoggerMessage( EventId = 300, Level = LogLevel.Error, Message = "Cache Failure: [ {Table} ]" ) ] public static  void CacheFailure( ILogger logger, Exception e, string table );
    // [ LoggerMessage( EventId = 200, Level = LogLevel.Information, Message = "Remote Host: {RemoteIP} {ClassName}.{Caller}" ) ] public static  void Sender( ILogger logger, string remoteIP, string className, [ CallerMemberName ] string caller = EMPTY );
    // [ LoggerMessage( EventId = 101, Level = LogLevel.Critical, Message = "'{ServiceName}'-> {IsCancelled} in {ClassName}.{Caller}" ) ] public static  void ServiceStopped( ILogger logger, string serviceName, bool isCancelled, string className, Exception? e, [ CallerMemberName ] string caller = EMPTY );
    // [ LoggerMessage( EventId = 100, Level = LogLevel.Critical, Message = "'{ServiceName}' Failed in {ClassName}.{Caller}" ) ] public static  void ServiceError( ILogger logger, Exception e, string serviceName, string className, [ CallerMemberName ] string caller = EMPTY );
    // [ LoggerMessage( EventId = 2, Level = LogLevel.Critical, Message = "{ClassName}.{Caller}" ) ] public static  void Critical( ILogger logger, Exception e, string className, [ CallerMemberName ] string caller = EMPTY );
    // [ LoggerMessage( EventId = 1, Level = LogLevel.Error, Message = "{ClassName}.{Caller}" ) ] public static  void Error( ILogger logger, Exception e, string className, [ CallerMemberName ] string caller = EMPTY );


    public static void Sender( ILogger logger, string remoteIP, string className, string caller )
    {
        if ( logger.IsEnabled(LogLevel.Information) ) { __senderCallback(logger, remoteIP, className, caller, null); }
    }
    public static void Sender( ILogger logger, HttpContext context, string className, [CallerMemberName] string caller = EMPTY ) { Sender(logger, context.Connection.RemoteIpAddress?.ToString() ?? string.Empty, className, caller); }
    public static void Sender<TValue>( ILogger logger, HttpContext context, [CallerMemberName] string caller = EMPTY )
        where TValue : notnull
    {
        Sender(logger, context.Connection.RemoteIpAddress?.ToString() ?? string.Empty, typeof(TValue).Name, caller);
    }
    public static void Sender<TValue>( ILogger logger, HttpContext context, TValue cls, [CallerMemberName] string caller = EMPTY )
        where TValue : notnull
    {
        Sender(logger, context.Connection.RemoteIpAddress?.ToString() ?? string.Empty, cls.GetType().Name, caller);
    }


    public static void ServiceStopped( ILogger logger, string serviceName, bool isCancelled, string className, Exception? e, string caller )
    {
        if ( logger.IsEnabled(LogLevel.Critical) ) { __serviceStoppedCallback(logger, serviceName, isCancelled, className, caller, e); }
    }
    public static void ServiceStopped<TValue, TService>( ILogger logger, CancellationToken token, Exception? e = null, [CallerMemberName] string caller = EMPTY )
        where TValue : notnull
    {
        ServiceStopped(logger, typeof(TValue).Name, token.IsCancellationRequested, typeof(TService).FullName ?? typeof(TService).Name, e, caller);
    }
    public static void ServiceStopped<TValue, TService>( ILogger logger, TValue t, TService cls, CancellationToken token, Exception? e = null, [CallerMemberName] string caller = EMPTY )
        where TValue : notnull
        where TService : notnull
    {
        ServiceStopped(logger, t.GetType().Name, token.IsCancellationRequested, cls.GetType().FullName ?? cls.GetType().Name, e, caller);
    }


    public static void ServiceError( ILogger logger, Exception e, string serviceName, string className, string caller )
    {
        if ( logger.IsEnabled(LogLevel.Critical) ) { __serviceErrorCallback(logger, serviceName, className, caller, e); }
    }
    public static void ServiceError<TValue, TService>( ILogger logger, Exception e, [CallerMemberName] string caller = EMPTY )
        where TValue : notnull
    {
        ServiceError(logger, e, typeof(TValue).Name, typeof(TService).FullName ?? typeof(TService).Name, caller);
    }
    public static void ServiceError<TValue, TService>( ILogger logger, Exception e, TValue t, TService cls, [CallerMemberName] string caller = EMPTY )
        where TValue : notnull
        where TService : notnull
    {
        ServiceError(logger, e, t.GetType().Name, cls.GetType().FullName ?? cls.GetType().Name, caller);
    }


    public static void Error( ILogger logger, Exception e, string className, string caller )
    {
        if ( logger.IsEnabled(LogLevel.Error) ) { __errorCallback(logger, className, caller, e); }
    }
    public static void Error<TValue>( ILogger logger, Exception e, [CallerMemberName] string caller = EMPTY )
        where TValue : notnull
    {
        Error(logger, e, typeof(TValue).Name, caller);
    }
    public static void Error<TValue>( ILogger logger, Exception e, TValue cls, [CallerMemberName] string caller = EMPTY )
        where TValue : notnull
    {
        Error(logger, e, cls.GetType().Name, caller);
    }


    public static void CacheFailure( ILogger logger, Exception e, string table )
    {
        if ( logger.IsEnabled(LogLevel.Error) ) { __cacheFailureCallback(logger, table, e); }
    }


    public static void Critical( ILogger logger, Exception e, string className, string caller )
    {
        if ( logger.IsEnabled(LogLevel.Critical) ) { __criticalCallback(logger, className, caller, e); }
    }
}
