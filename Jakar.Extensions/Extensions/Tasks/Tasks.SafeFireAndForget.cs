// Jakar.Extensions :: Jakar.Extensions
// 04/13/2024  00:04

namespace Jakar.Extensions;


/// <remarks>
///     <para> <c>SafeFireAndForget</c> guarantees, for every overload: </para>
///     <list type="bullet">
///         <item> It never throws after returning and never crashes the process: failures of the task go to the error handler, failures of <c>next</c> go to the error handler, and failures of the error handler itself go to <see cref="SelfLogger"/>. </item>
///         <item> Each call handles its task exactly once: <c>next</c> or the error handler runs at most once, and a <see cref="ValueTask"/> / <see cref="ValueTask{TResult}"/> is consumed exactly once. </item>
///         <item> An already-completed task is handled synchronously on the calling thread (as <c>await</c> would), without allocating. Otherwise handlers run on the thread pool (as with <c>ConfigureAwait(false)</c>). </item>
///         <item> The error handler receives the same exception <c>await</c> would throw (cancellation included). </item>
///     </list>
///     Arguments are validated eagerly, so a null task or handler throws <see cref="ArgumentNullException"/> at the call site instead of failing later on the thread pool.
/// </remarks>
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
        public void SafeFireAndForget()                                     => Observe(ThrowIfNull(task), new SelfLogError());
        public void SafeFireAndForget( Action<Exception>          onError ) => Observe(ThrowIfNull(task), new ActionError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, Task>      onError ) => Observe(ThrowIfNull(task), new TaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, ValueTask> onError ) => Observe(ThrowIfNull(task), new ValueTaskError(ThrowIfNull(onError)));

        public void SafeFireAndForget( ILogger logger, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY ) => Observe(ThrowIfNull(task), new LoggerError(ThrowIfNull(logger), caller, variable));
    }



    extension<TValue>( Task<TValue> task )
    {
        public void SafeFireAndForget( ILogger logger, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY ) => Observe(ThrowIfNull(task), new LoggerError(ThrowIfNull(logger), caller, variable));
        public void SafeFireAndForget( ILogger logger, Action<TValue> next, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY ) =>
            Observe(ThrowIfNull(task), new ActionNext<TValue>(ThrowIfNull(next)), new LoggerError(ThrowIfNull(logger), caller, variable));
        public void SafeFireAndForget( ILogger logger, Func<TValue, Task> next, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY ) =>
            Observe(ThrowIfNull(task), new TaskNext<TValue>(ThrowIfNull(next)), new LoggerError(ThrowIfNull(logger), caller, variable));
        public void SafeFireAndForget( ILogger logger, Func<TValue, ValueTask> next, [CallerArgumentExpression(nameof(task))] string variable = EMPTY, [CallerMemberName] string caller = EMPTY ) =>
            Observe(ThrowIfNull(task), new ValueTaskNext<TValue>(ThrowIfNull(next)), new LoggerError(ThrowIfNull(logger), caller, variable));


        public void SafeFireAndForget( Action<Exception> onError )                               => Observe(ThrowIfNull(task), new ActionError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Action<Exception> onError, Action<TValue>          next ) => Observe(ThrowIfNull(task), new ActionNext<TValue>(ThrowIfNull(next)),    new ActionError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Action<Exception> onError, Func<TValue, Task>      next ) => Observe(ThrowIfNull(task), new TaskNext<TValue>(ThrowIfNull(next)),      new ActionError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Action<Exception> onError, Func<TValue, ValueTask> next ) => Observe(ThrowIfNull(task), new ValueTaskNext<TValue>(ThrowIfNull(next)), new ActionError(ThrowIfNull(onError)));


        public void SafeFireAndForget( Func<Exception, Task> onError )                               => Observe(ThrowIfNull(task), new TaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, Task> onError, Action<TValue>          next ) => Observe(ThrowIfNull(task), new ActionNext<TValue>(ThrowIfNull(next)),    new TaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, Task> onError, Func<TValue, Task>      next ) => Observe(ThrowIfNull(task), new TaskNext<TValue>(ThrowIfNull(next)),      new TaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, Task> onError, Func<TValue, ValueTask> next ) => Observe(ThrowIfNull(task), new ValueTaskNext<TValue>(ThrowIfNull(next)), new TaskError(ThrowIfNull(onError)));


        public void SafeFireAndForget( Func<Exception, ValueTask> onError )                               => Observe(ThrowIfNull(task), new ValueTaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, ValueTask> onError, Action<TValue>          next ) => Observe(ThrowIfNull(task), new ActionNext<TValue>(ThrowIfNull(next)),    new ValueTaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, ValueTask> onError, Func<TValue, Task>      next ) => Observe(ThrowIfNull(task), new TaskNext<TValue>(ThrowIfNull(next)),      new ValueTaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, ValueTask> onError, Func<TValue, ValueTask> next ) => Observe(ThrowIfNull(task), new ValueTaskNext<TValue>(ThrowIfNull(next)), new ValueTaskError(ThrowIfNull(onError)));
    }



    extension( ValueTask task )
    {
        public void SafeFireAndForget()                                     => Observe(task, new SelfLogError());
        public void SafeFireAndForget( Func<Exception, Task>      onError ) => Observe(task, new TaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, ValueTask> onError ) => Observe(task, new ValueTaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Action<Exception>          onError ) => Observe(task, new ActionError(ThrowIfNull(onError)));
    }



    extension<TValue>( ValueTask<TValue> task )
    {
        public void SafeFireAndForget( Action<Exception> onError )                               => Observe(task, new NoNext<TValue>(),                         new ActionError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Action<Exception> onError, Action<TValue>          next ) => Observe(task, new ActionNext<TValue>(ThrowIfNull(next)),    new ActionError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Action<Exception> onError, Func<TValue, Task>      next ) => Observe(task, new TaskNext<TValue>(ThrowIfNull(next)),      new ActionError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Action<Exception> onError, Func<TValue, ValueTask> next ) => Observe(task, new ValueTaskNext<TValue>(ThrowIfNull(next)), new ActionError(ThrowIfNull(onError)));


        public void SafeFireAndForget( Func<Exception, Task> onError )                               => Observe(task, new NoNext<TValue>(),                         new TaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, Task> onError, Action<TValue>          next ) => Observe(task, new ActionNext<TValue>(ThrowIfNull(next)),    new TaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, Task> onError, Func<TValue, Task>      next ) => Observe(task, new TaskNext<TValue>(ThrowIfNull(next)),      new TaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, Task> onError, Func<TValue, ValueTask> next ) => Observe(task, new ValueTaskNext<TValue>(ThrowIfNull(next)), new TaskError(ThrowIfNull(onError)));


        public void SafeFireAndForget( Func<Exception, ValueTask> onError )                               => Observe(task, new NoNext<TValue>(),                         new ValueTaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, ValueTask> onError, Action<TValue>          next ) => Observe(task, new ActionNext<TValue>(ThrowIfNull(next)),    new ValueTaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, ValueTask> onError, Func<TValue, Task>      next ) => Observe(task, new TaskNext<TValue>(ThrowIfNull(next)),      new ValueTaskError(ThrowIfNull(onError)));
        public void SafeFireAndForget( Func<Exception, ValueTask> onError, Func<TValue, ValueTask> next ) => Observe(task, new ValueTaskNext<TValue>(ThrowIfNull(next)), new ValueTaskError(ThrowIfNull(onError)));
    }



    // ─── Core ────────────────────────────────────────────────────────────────
    // The handlers are passed as generic structs, so each overload compiles to a specialized, allocation-free fast path with no delegate wrapping.
    // Only a task that is still pending allocates (one state machine), and only an incomplete handler result allocates beyond that.

    private static void Observe<TError>( Task task, TError onError )
        where TError : struct, IErrorHandler
    {
        if ( task.IsCompleted ) { OnCompleted(task, onError); }
        else { _ = ObserveAsync(task, onError); }
    }
    private static void OnCompleted<TError>( Task task, TError onError )
        where TError : struct, IErrorHandler
    {
        if ( !task.IsCompletedSuccessfully ) { OnError(onError, GetException(task)); }
    }
    private static async Task ObserveAsync<TError>( Task task, TError onError )
        where TError : struct, IErrorHandler
    {
        await task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        OnCompleted(task, onError);
    }


    private static void Observe<TValue, TNext, TError>( Task<TValue> task, TNext next, TError onError )
        where TNext : struct, INextHandler<TValue>
        where TError : struct, IErrorHandler
    {
        if ( task.IsCompleted ) { OnCompleted(task, next, onError); }
        else { _ = ObserveAsync(task, next, onError); }
    }
    private static void OnCompleted<TValue, TNext, TError>( Task<TValue> task, TNext next, TError onError )
        where TNext : struct, INextHandler<TValue>
        where TError : struct, IErrorHandler
    {
        if ( task.IsCompletedSuccessfully ) { OnSuccess(task.Result, next, onError); }
        else { OnError(onError, GetException(task)); }
    }
    private static async Task ObserveAsync<TValue, TNext, TError>( Task<TValue> task, TNext next, TError onError )
        where TNext : struct, INextHandler<TValue>
        where TError : struct, IErrorHandler
    {
        // SuppressThrowing is only allowed on the non-generic awaitable; the outcome is read from the task afterwards.
        await ( (Task)task ).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        OnCompleted(task, next, onError);
    }


    private static void Observe<TError>( ValueTask task, TError onError )
        where TError : struct, IErrorHandler
    {
        if ( !task.IsCompleted )
        {
            _ = ObserveAsync(task, onError);
            return;
        }

        // GetResult consumes the ValueTask (returning a pooled IValueTaskSource to its pool) and throws exactly what await would.
        try { task.GetAwaiter().GetResult(); }
        catch ( Exception e ) { OnError(onError, e); }
    }
    private static async Task ObserveAsync<TError>( ValueTask task, TError onError )
        where TError : struct, IErrorHandler
    {
        try { await task.ConfigureAwait(false); }
        catch ( Exception e ) { OnError(onError, e); }
    }


    private static void Observe<TValue, TNext, TError>( ValueTask<TValue> task, TNext next, TError onError )
        where TNext : struct, INextHandler<TValue>
        where TError : struct, IErrorHandler
    {
        if ( !task.IsCompleted )
        {
            _ = ObserveAsync(task, next, onError);
            return;
        }

        TValue value;

        try { value = task.GetAwaiter().GetResult(); }
        catch ( Exception e )
        {
            OnError(onError, e);
            return;
        }

        OnSuccess(value, next, onError);
    }
    private static async Task ObserveAsync<TValue, TNext, TError>( ValueTask<TValue> task, TNext next, TError onError )
        where TNext : struct, INextHandler<TValue>
        where TError : struct, IErrorHandler
    {
        TValue value;

        try { value = await task.ConfigureAwait(false); }
        catch ( Exception e )
        {
            OnError(onError, e);
            return;
        }

        OnSuccess(value, next, onError);
    }


    /// <summary> Runs <paramref name="next"/>; if it fails (synchronously or asynchronously) the error handler runs once. Never throws. </summary>
    private static void OnSuccess<TValue, TNext, TError>( TValue value, TNext next, TError onError )
        where TNext : struct, INextHandler<TValue>
        where TError : struct, IErrorHandler
    {
        if ( TNext.IsNoOp ) { return; }

        ValueTask pending;

        try
        {
            pending = next.Invoke(value);

            if ( pending.IsCompleted )
            {
                pending.GetAwaiter().GetResult();
                return;
            }
        }
        catch ( Exception e )
        {
            OnError(onError, e);
            return;
        }

        _ = AwaitNext(pending, onError);
    }
    private static async Task AwaitNext<TError>( ValueTask pending, TError onError )
        where TError : struct, IErrorHandler
    {
        try { await pending.ConfigureAwait(false); }
        catch ( Exception e ) { OnError(onError, e); }
    }


    /// <summary> Runs the error handler once; if the handler itself fails, the failure goes to <see cref="SelfLogger"/>. Never throws. </summary>
    private static void OnError<TError>( TError onError, Exception exception )
        where TError : struct, IErrorHandler
    {
        ValueTask pending;

        try
        {
            pending = onError.Handle(exception);

            if ( pending.IsCompleted )
            {
                pending.GetAwaiter().GetResult();
                return;
            }
        }
        catch ( Exception e )
        {
            ReportUnhandled(e);
            return;
        }

        _ = AwaitErrorHandler(pending);
    }
    private static async Task AwaitErrorHandler( ValueTask pending )
    {
        try { await pending.ConfigureAwait(false); }
        catch ( Exception e ) { ReportUnhandled(e); }
    }


    /// <summary> Last resort: nothing is left to report a failure to if <see cref="SelfLogger"/>'s own output throws, and it must not escape. </summary>
    private static void ReportUnhandled( Exception e )
    {
        try { SelfLogger.WriteLine(e); }
        catch
        {
            // ignored
        }
    }


    /// <summary> The exception <c>await</c> would throw for a completed, unsuccessful task. Reading <see cref="Task.Exception"/> also marks it observed. </summary>
    private static Exception GetException( Task task )
    {
        if ( task.Exception is { } aggregate ) { return aggregate.InnerExceptions[0]; }

        // Cancelled: let the awaiter produce the exact exception await would (the original OperationCanceledException, or a TaskCanceledException).
        try { task.GetAwaiter().GetResult(); }
        catch ( Exception e ) { return e; }

        return new TaskCanceledException(task);
    }


    // ─── Handler strategies ──────────────────────────────────────────────────



    private interface IErrorHandler
    {
        ValueTask Handle( Exception exception );
    }



    private readonly struct SelfLogError : IErrorHandler
    {
        public ValueTask Handle( Exception exception )
        {
            SelfLogger.WriteLine(exception);
            return ValueTask.CompletedTask;
        }
    }



    private readonly struct ActionError( Action<Exception> handler ) : IErrorHandler
    {
        public ValueTask Handle( Exception exception )
        {
            handler(exception);
            return ValueTask.CompletedTask;
        }
    }



    private readonly struct TaskError( Func<Exception, Task> handler ) : IErrorHandler
    {
        public ValueTask Handle( Exception exception ) => new(handler(exception));
    }



    private readonly struct ValueTaskError( Func<Exception, ValueTask> handler ) : IErrorHandler
    {
        public ValueTask Handle( Exception exception ) => handler(exception);
    }



    private readonly struct LoggerError( ILogger logger, string caller, string variable ) : IErrorHandler
    {
        public ValueTask Handle( Exception exception )
        {
            Log(logger, exception, caller, variable);
            return ValueTask.CompletedTask;
        }
    }



    private interface INextHandler<in TValue>
    {
        abstract static bool IsNoOp { get; }
        ValueTask            Invoke( TValue value );
    }



    private readonly struct NoNext<TValue> : INextHandler<TValue>
    {
        public static bool      IsNoOp                 => true;
        public        ValueTask Invoke( TValue value ) => ValueTask.CompletedTask;
    }



    private readonly struct ActionNext<TValue>( Action<TValue> next ) : INextHandler<TValue>
    {
        public static bool IsNoOp => false;
        public ValueTask Invoke( TValue value )
        {
            next(value);
            return ValueTask.CompletedTask;
        }
    }



    private readonly struct TaskNext<TValue>( Func<TValue, Task> next ) : INextHandler<TValue>
    {
        public static bool      IsNoOp                 => false;
        public        ValueTask Invoke( TValue value ) => new(next(value));
    }



    private readonly struct ValueTaskNext<TValue>( Func<TValue, ValueTask> next ) : INextHandler<TValue>
    {
        public static bool      IsNoOp                 => false;
        public        ValueTask Invoke( TValue value ) => next(value);
    }
}
