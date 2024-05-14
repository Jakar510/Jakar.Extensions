// Jakar.Extensions :: Jakar.Extensions
// 04/13/2024  00:04

namespace Jakar.Extensions;


public static partial class Tasks
{
    [LoggerMessage( EventId = 0, Level = LogLevel.Error, Message = "{Caller}" )] public static partial void Log( ILogger logger, Exception e, string caller );


    public static async void SafeFireAndForget( this Task task, ILogger logger, [CallerArgumentExpression( nameof(task) )] string caller = BaseRecord.EMPTY )
    {
        try { await task; }
        catch ( Exception e ) { Log( logger, e, caller ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, ILogger logger, Action<T>? next = null, [CallerArgumentExpression( nameof(task) )] string caller = BaseRecord.EMPTY )
    {
        try
        {
            T result = await task;
            next?.Invoke( result );
        }
        catch ( Exception e ) { Log( logger, e, caller ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, ILogger logger, Func<T, Task>? next = null, [CallerArgumentExpression( nameof(task) )] string caller = BaseRecord.EMPTY )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { Log( logger, e, caller ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, ILogger logger, Func<T, ValueTask>? next = null, [CallerArgumentExpression( nameof(task) )] string caller = BaseRecord.EMPTY )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { Log( logger, e, caller ); }
    }


    public static async void SafeFireAndForget( this Task task, Action<Exception?> onError )
    {
        try { await task; }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Action<Exception?> onError, Action<T>? next = null )
    {
        try
        {
            T result = await task;
            next?.Invoke( result );
        }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Action<Exception?> onError, Func<T, Task>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Action<Exception?> onError, Func<T, ValueTask>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { onError( e ); }
    }


    public static async void SafeFireAndForget( this ValueTask task, Action<Exception?> onError )
    {
        try { await task; }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Action<Exception?> onError, Action<T>? next = null )
    {
        try
        {
            T result = await task;
            next?.Invoke( result );
        }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Action<Exception?> onError, Func<T, Task>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Action<Exception?> onError, Func<T, ValueTask>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { onError( e ); }
    }


    public static async void SafeFireAndForget( this Task task, Func<Exception?, Task> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception?, Task> onError, Action<T>? next = null )
    {
        try
        {
            T result = await task;
            next?.Invoke( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception?, Task> onError, Func<T, Task>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception?, Task> onError, Func<T, ValueTask>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { await onError( e ); }
    }


    public static async void SafeFireAndForget( this ValueTask task, Func<Exception?, Task> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception?, Task> onError, Action<T>? next = null )
    {
        try
        {
            T result = await task;
            next?.Invoke( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception?, Task> onError, Func<T, Task>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception?, Task> onError, Func<T, ValueTask>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { await onError( e ); }
    }


    public static async void SafeFireAndForget( this Task task, Func<Exception?, ValueTask> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception?, ValueTask> onError, Action<T>? next = null )
    {
        try
        {
            T result = await task;
            next?.Invoke( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception?, ValueTask> onError, Func<T, Task>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this Task<T> task, Func<Exception?, ValueTask> onError, Func<T, ValueTask>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { await onError( e ); }
    }


    public static async void SafeFireAndForget( this ValueTask task, Func<Exception?, ValueTask> onError )
    {
        try { await task; }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception?, ValueTask> onError, Action<T>? next = null )
    {
        try
        {
            T result = await task;
            next?.Invoke( result );
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception?, ValueTask> onError, Func<T, Task>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { await onError( e ); }
    }
    public static async void SafeFireAndForget<T>( this ValueTask<T> task, Func<Exception?, ValueTask> onError, Func<T, ValueTask>? next = null )
    {
        try
        {
            T result = await task;
            if ( next is not null ) { await next( result ); }
        }
        catch ( Exception e ) { await onError( e ); }
    }
}
