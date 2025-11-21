// Jakar.Extensions :: Jakar.Extensions
// 04/13/2024  00:04

namespace Jakar.Extensions;


public static partial class Tasks
{
    private static readonly Action<ILogger, string, Exception?>         __logCallerCallback         = LoggerMessage.Define<string>(LogLevel.Error, new EventId(0,         nameof(Log)), "{Caller}", new LogDefineOptions { SkipEnabledCheck            = true });
    private static readonly Action<ILogger, string, string, Exception?> __logCallerVariableCallback = LoggerMessage.Define<string, string>(LogLevel.Error, new EventId(0, nameof(Log)), "{Caller}.{Variable}", new LogDefineOptions { SkipEnabledCheck = true });


    public static void Log( ILogger logger, Exception e, string caller )
    {
        if ( logger.IsEnabled(LogLevel.Error) ) { __logCallerCallback(logger, caller, e); }
    }
    public static void Log( ILogger logger, Exception e, string caller, string variable )
    {
        if ( logger.IsEnabled(LogLevel.Error) ) { __logCallerVariableCallback(logger, caller, variable, e); }
    }



    extension( Task task )
    {
        public async void SafeFireAndForget( Action<Exception> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e ) { onError(e); }
        }
        public async void SafeFireAndForget( ILogger logger, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e ) { Log(logger, e, caller, variable); }
        }
        public async void SafeFireAndForget( Func<Exception, Task> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, ValueTask> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
    }



    extension<TValue>( Task<TValue> task )
    {
        public async void SafeFireAndForget( ILogger logger, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e ) { Log(logger, e, caller, variable); }
        }
        public async void SafeFireAndForget( ILogger logger, Action<TValue> next, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);
                next(result);
            }
            catch ( Exception e ) { Log(logger, e, caller, variable); }
        }
        public async void SafeFireAndForget( ILogger logger, Func<TValue, Task> next, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e ) { Log(logger, e, caller, variable); }
        }
        public async void SafeFireAndForget( ILogger logger, Func<TValue, ValueTask> next, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e ) { Log(logger, e, caller, variable); }
        }


        public async void SafeFireAndForget( Action<Exception> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e ) { onError(e); }
        }
        public async void SafeFireAndForget( Action<Exception> onError, Action<TValue> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);
                next(result);
            }
            catch ( Exception e ) { onError(e); }
        }
        public async void SafeFireAndForget( Action<Exception> onError, Func<TValue, Task> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e ) { onError(e); }
        }
        public async void SafeFireAndForget( Action<Exception> onError, Func<TValue, ValueTask> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e ) { onError(e); }
        }


        public async void SafeFireAndForget( Func<Exception, Task> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, Task> onError, Action<TValue> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);
                next(result);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, Task> onError, Func<TValue, Task> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, Task> onError, Func<TValue, ValueTask> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }


        public async void SafeFireAndForget( Func<Exception, ValueTask> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, ValueTask> onError, Action<TValue> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);
                next(result);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, ValueTask> onError, Func<TValue, Task> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, ValueTask> onError, Func<TValue, ValueTask> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
    }



    extension( ValueTask task )
    {
        public async void SafeFireAndForget( Func<Exception, Task> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, ValueTask> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Action<Exception> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e ) { onError(e); }
        }
    }



    extension<TValue>( ValueTask<TValue> task )
    {
        public async void SafeFireAndForget( Action<Exception> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e ) { onError(e); }
        }
        public async void SafeFireAndForget( Action<Exception> onError, Action<TValue> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);
                next(result);
            }
            catch ( Exception e ) { onError(e); }
        }
        public async void SafeFireAndForget( Action<Exception> onError, Func<TValue, Task> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e ) { onError(e); }
        }
        public async void SafeFireAndForget( Action<Exception> onError, Func<TValue, ValueTask> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e ) { onError(e); }
        }


        public async void SafeFireAndForget( Func<Exception, Task> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, Task> onError, Action<TValue> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);
                next(result);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, Task> onError, Func<TValue, Task> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, Task> onError, Func<TValue, ValueTask> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }


        public async void SafeFireAndForget( Func<Exception, ValueTask> onError )
        {
            try { await task.ConfigureAwait(false); }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, ValueTask> onError, Action<TValue> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);
                next(result);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, ValueTask> onError, Func<TValue, Task> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
        public async void SafeFireAndForget( Func<Exception, ValueTask> onError, Func<TValue, ValueTask> next )
        {
            try
            {
                TValue result = await task.ConfigureAwait(false);

                await next(result)
                   .ConfigureAwait(false);
            }
            catch ( Exception e )
            {
                await onError(e)
                   .ConfigureAwait(false);
            }
        }
    }
}
