// Jakar.Extensions :: Jakar.Extensions
// 04/13/2024  00:04

namespace Jakar.Extensions;


public static partial class Tasks
{
    private const           string                                      EMPTY                      = BaseRecord.EMPTY;
    private static readonly Action<ILogger, string, string, Exception?> _logCallerVariableCallback = LoggerMessage.Define<string, string>( LogLevel.Error, new EventId( 0, nameof(Log) ), "{Caller}.{Variable}", new LogDefineOptions { SkipEnabledCheck = true } );
    private static readonly Action<ILogger, string, Exception?>         _logCallerCallback         = LoggerMessage.Define<string>( LogLevel.Error, new EventId( 0,         nameof(Log) ), "{Caller}", new LogDefineOptions { SkipEnabledCheck            = true } );


    public static void Log( ILogger logger, Exception e, string caller )
    {
        if ( logger.IsEnabled( LogLevel.Error ) ) { _logCallerCallback( logger, caller, e ); }
    }
    public static void Log( ILogger logger, Exception e, string caller, string variable )
    {
        if ( logger.IsEnabled( LogLevel.Error ) ) { _logCallerVariableCallback( logger, caller, variable, e ); }
    }


    public static async void SafeFireAndForget( this Task task, ILogger logger, [CallerArgumentExpression( nameof(task) )] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
    {
        try { await task; }
        catch ( Exception e ) { Log( logger, e, caller, variable ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, ILogger logger, [CallerArgumentExpression( nameof(task) )] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
    {
        try { await task; }
        catch ( Exception e ) { Log( logger, e, caller, variable ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, ILogger logger, Action<T> next, [CallerArgumentExpression( nameof(task) )] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
    {
        try
        {
            T result = await task;
            next( result );
        }
        catch ( Exception e ) { Log( logger, e, caller, variable ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, ILogger logger, Func<T, Task> next, [CallerArgumentExpression( nameof(task) )] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { Log( logger, e, caller, variable ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, ILogger logger, Func<T, ValueTask> next, [CallerArgumentExpression( nameof(task) )] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { Log( logger, e, caller, variable ); }
    }


    public static async void SafeFireAndForget( this Task task, Action<Exception> onError )
    {
        try { await task; }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Action<Exception> onError )
    {
        try { await task; }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Action<Exception> onError, Action<T> next )
    {
        try
        {
            T result = await task;
            next( result );
        }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Action<Exception> onError, Func<T, Task> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Action<Exception> onError, Func<T, ValueTask> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { onError( e ); }
    }


    public static async void SafeFireAndForget( this ValueTask task, Action<Exception> onError )
    {
        try { await task; }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Action<Exception> onError )
    {
        try { await task; }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Action<Exception> onError, Action<T> next )
    {
        try
        {
            T result = await task;
            next( result );
        }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Action<Exception> onError, Func<T, Task> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Action<Exception> onError, Func<T, ValueTask> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { onError( e ); }
    }


    public static async void SafeFireAndForget( this Task task, Func<Exception, Task> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception, Task> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception, Task> onError, Action<T> next )
    {
        try
        {
            T result = await task;
            next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception, Task> onError, Func<T, Task> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception, Task> onError, Func<T, ValueTask> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }


    public static async void SafeFireAndForget( this ValueTask task, Func<Exception, Task> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception, Task> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception, Task> onError, Action<T> next )
    {
        try
        {
            T result = await task;
            next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception, Task> onError, Func<T, Task> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception, Task> onError, Func<T, ValueTask> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }


    public static async void SafeFireAndForget( this Task task, Func<Exception, ValueTask> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception, ValueTask> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception, ValueTask> onError, Action<T> next )
    {
        try
        {
            T result = await task;
            next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception, ValueTask> onError, Func<T, Task> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception, ValueTask> onError, Func<T, ValueTask> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }


    public static async void SafeFireAndForget( this ValueTask task, Func<Exception, ValueTask> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception, ValueTask> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception, ValueTask> onError, Action<T> next )
    {
        try
        {
            T result = await task;
            next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception, ValueTask> onError, Func<T, Task> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception, ValueTask> onError, Func<T, ValueTask> next )
    {
        try
        {
            T result = await task;
            await next( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
}
