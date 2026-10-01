// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Microsoft.Extensions.Logging;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Tasks))]
[NonParallelizable] // some tests capture SelfLogger, which is process-wide
public class SafeFireAndForget_Tests : Assert
{
    private static readonly TimeSpan __timeout = TimeSpan.FromSeconds(5);


    // ─── Task ────────────────────────────────────────────────────────────────

    [Test]
    public void Task_CompletedSuccessfully_CallsNothing()
    {
        int errors = 0;
        Task.CompletedTask.SafeFireAndForget(_ => errors++);
        this.AreEqual(0, errors);
    }

    [Test]
    public void Task_AlreadyFaulted_HandlesSynchronously_WithOriginalException()
    {
        InvalidOperationException error    = new("boom");
        Exception?                received = null;

        Task.FromException(error).SafeFireAndForget(e => received = e);

        Assert.AreSame(error, received);
    }

    [Test]
    public void Task_AlreadyCancelled_PassesCancellation()
    {
        Exception? received = null;
        Task.FromCanceled(new CancellationToken(true)).SafeFireAndForget(e => received = e);
        Assert.IsInstanceOf<TaskCanceledException>(received);
    }

    [Test]
    public async Task Task_CancelledByOperationCanceledException_PassesTheSameExceptionAwaitWould()
    {
        using CancellationTokenSource cts    = new();
        OperationCanceledException    thrown = new(cts.Token);
        TaskCompletionSource<Exception> received = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task task = @throw();
        task.SafeFireAndForget(e => received.TrySetResult(e));

        Assert.AreSame(thrown, await received.Task.WaitAsync(__timeout));
        return;

        async Task @throw()
        {
            await Task.Yield();
            throw thrown;
        }
    }

    [Test]
    public async Task Task_Pending_HandlesFailureOnce_OnCompletion()
    {
        TaskCompletionSource            source   = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Exception> received = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int                             calls    = 0;

        source.Task.SafeFireAndForget(e =>
                                      {
                                          Interlocked.Increment(ref calls);
                                          received.TrySetResult(e);
                                      });

        this.AreEqual(0, calls);
        InvalidOperationException error = new("late");
        source.SetException(error);

        Assert.AreSame(error, await received.Task.WaitAsync(__timeout));
        await Task.Delay(50);
        this.AreEqual(1, calls);
    }

    [Test]
    public async Task Task_AsyncErrorHandler_IsAwaited()
    {
        TaskCompletionSource handled = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task.FromException(new InvalidOperationException()).SafeFireAndForget((Func<Exception, Task>)(async _ =>
                                                                               {
                                                                                   await Task.Yield();
                                                                                   handled.TrySetResult();
                                                                               }));

        await handled.Task.WaitAsync(__timeout);
    }

    [Test]
    public void Task_EachCall_HandlesItsTaskOnce()
    {
        int  calls = 0;
        Task task  = Task.FromException(new InvalidOperationException());

        task.SafeFireAndForget(_ => calls++);
        task.SafeFireAndForget(_ => calls++);

        this.AreEqual(2, calls);
    }

    [Test]
    public void Task_ThrowingErrorHandler_GoesToSelfLogger_AndDoesNotThrow()
    {
        string? output = Capture(static () => Task.FromException(new InvalidOperationException("original"))
                                                 .SafeFireAndForget((Action<Exception>)( static _ => throw new ArgumentException("handler failed") )));

        this.IsTrue(output!.Contains("handler failed", StringComparison.Ordinal));
    }

    [Test]
    public async Task Task_FaultedAsyncErrorHandler_GoesToSelfLogger()
    {
        TaskCompletionSource<string> output = new(TaskCreationOptions.RunContinuationsAsynchronously);
        SelfLogger.Enable(m => output.TrySetResult(m));

        try
        {
            Task.FromException(new InvalidOperationException("original"))
                .SafeFireAndForget((Func<Exception, Task>)( static async _ =>
                                   {
                                       await Task.Yield();
                                       throw new ArgumentException("async handler failed");
                                   } ));

            this.IsTrue(( await output.Task.WaitAsync(__timeout) ).Contains("async handler failed", StringComparison.Ordinal));
        }
        finally { SelfLogger.Disable(); }
    }

    [Test]
    public void Task_NoHandler_LogsToSelfLogger()
    {
        string? output = Capture(static () => Task.FromException(new InvalidOperationException("unhandled")).SafeFireAndForget());
        this.IsTrue(output!.Contains("unhandled", StringComparison.Ordinal));
    }

    [Test]
    public void Task_Logger_LogsOnce_WithCallerAndVariable()
    {
        CountingLogger logger = new();
        Task           work   = Task.FromException(new InvalidOperationException("logged"));

        work.SafeFireAndForget(logger);

        this.AreEqual(1, logger.Count);
        this.AreEqual("logged", logger.Exception?.Message);
        this.AreEqual($"{nameof(Task_Logger_LogsOnce_WithCallerAndVariable)}.{nameof(work)}", logger.Message);
    }

    [Test]
    public void NullArguments_ThrowAtTheCallSite()
    {
        Assert.Throws<ArgumentNullException>(static () => ( (Task)null! ).SafeFireAndForget());
        Assert.Throws<ArgumentNullException>(static () => Task.CompletedTask.SafeFireAndForget((Action<Exception>)null!));
        Assert.Throws<ArgumentNullException>(static () => Task.FromResult(1).SafeFireAndForget(static _ => { }, (Action<int>)null!));
        Assert.Throws<ArgumentNullException>(static () => ValueTask.CompletedTask.SafeFireAndForget((Func<Exception, Task>)null!));
    }


    // ─── Task<T> ─────────────────────────────────────────────────────────────

    [Test]
    public void TaskOfT_Completed_RunsNextSynchronously()
    {
        int received = 0;
        Task.FromResult(5).SafeFireAndForget(static _ => Fail("no error expected"), x => received = x);
        this.AreEqual(5, received);
    }

    [Test]
    public void TaskOfT_Faulted_SkipsNext_AndHandlesOnce()
    {
        int  nextCalls = 0, errorCalls = 0;
        Task.FromException<int>(new InvalidOperationException()).SafeFireAndForget(_ => errorCalls++, _ => nextCalls++);

        this.AreEqual(0, nextCalls);
        this.AreEqual(1, errorCalls);
    }

    [Test]
    public void TaskOfT_ThrowingNext_GoesToErrorHandlerOnce()
    {
        ArgumentException error    = new("next failed");
        Exception?        received = null;
        int               calls    = 0;

        Task.FromResult(5).SafeFireAndForget(e =>
                                             {
                                                 calls++;
                                                 received = e;
                                             },
                                             (Action<int>)( _ => throw error ));

        Assert.AreSame(error, received);
        this.AreEqual(1, calls);
    }

    [Test]
    public async Task TaskOfT_FaultedAsyncNext_GoesToErrorHandler()
    {
        TaskCompletionSource<Exception> received = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task.FromResult(5).SafeFireAndForget(e => received.TrySetResult(e),
                                             (Func<int, Task>)( static async _ =>
                                             {
                                                 await Task.Yield();
                                                 throw new ArgumentException("async next failed");
                                             } ));

        this.AreEqual("async next failed", ( await received.Task.WaitAsync(__timeout) ).Message);
    }

    [Test]
    public async Task TaskOfT_Pending_RunsNextOnce()
    {
        TaskCompletionSource<int> source   = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<int> received = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int                       calls    = 0;

        source.Task.SafeFireAndForget(static _ => Fail("no error expected"),
                                      x =>
                                      {
                                          Interlocked.Increment(ref calls);
                                          received.TrySetResult(x);
                                      });

        source.SetResult(9);

        this.AreEqual(9, await received.Task.WaitAsync(__timeout));
        await Task.Delay(50);
        this.AreEqual(1, calls);
    }


    // ─── ValueTask / ValueTask<T> ────────────────────────────────────────────

    [Test]
    public void ValueTask_Default_CallsNothing()
    {
        int errors = 0;
        default(ValueTask).SafeFireAndForget(_ => errors++);
        this.AreEqual(0, errors);
    }

    [Test]
    public async Task ValueTask_PooledSource_IsConsumedExactlyOnce( [Values] bool completeFirst, [Values] bool fail )
    {
        CountingSource<int>  source = new();
        TaskCompletionSource done   = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int                  errors = 0;

        if ( completeFirst ) { source.Complete(fail); }

        new ValueTask(source, source.Version).SafeFireAndForget(_ =>
                                                                {
                                                                    Interlocked.Increment(ref errors);
                                                                    done.TrySetResult();
                                                                });

        if ( !completeFirst ) { source.Complete(fail); }

        if ( fail ) { await done.Task.WaitAsync(__timeout); }
        else { await source.Consumed.WaitAsync(__timeout); }

        await Task.Delay(50);
        this.AreEqual(1, source.GetResultCalls);
        this.AreEqual(fail ? 1 : 0, errors);
    }

    [Test]
    public async Task ValueTaskOfT_PooledSource_IsConsumedExactlyOnce_AndRunsNextOnce( [Values] bool completeFirst )
    {
        CountingSource<int>       source   = new();
        TaskCompletionSource<int> received = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int                       calls    = 0;

        if ( completeFirst ) { source.Complete(false, 7); }

        new ValueTask<int>(source, source.Version).SafeFireAndForget(static _ => Fail("no error expected"),
                                                                     x =>
                                                                     {
                                                                         Interlocked.Increment(ref calls);
                                                                         received.TrySetResult(x);
                                                                     });

        if ( !completeFirst ) { source.Complete(false, 7); }

        this.AreEqual(7, await received.Task.WaitAsync(__timeout));
        await Task.Delay(50);
        this.AreEqual(1, source.GetResultCalls);
        this.AreEqual(1, calls);
    }

    [Test]
    public void ValueTaskOfT_Faulted_HandlesOnce()
    {
        int errors = 0;
        ValueTask.FromException<int>(new InvalidOperationException()).SafeFireAndForget(_ => errors++, static _ => Fail("next must not run"));
        this.AreEqual(1, errors);
    }



    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static string? Capture( Action action )
    {
        string? output = null;
        SelfLogger.Enable(m => output = m);

        try { action(); }
        finally { SelfLogger.Disable(); }

        return output;
    }



    /// <summary> A pooled-style source that fails loudly if a ValueTask over it is consumed more than once. </summary>
    private sealed class CountingSource<T> : IValueTaskSource<T>, IValueTaskSource
    {
        private          ManualResetValueTaskSourceCore<T> __core = new() { RunContinuationsAsynchronously = true };
        private readonly TaskCompletionSource              __consumed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private          int                               __getResultCalls;

        public short Version        => __core.Version;
        public int   GetResultCalls => Volatile.Read(ref __getResultCalls);
        public Task  Consumed       => __consumed.Task;

        public void Complete( bool fail, T value = default! )
        {
            if ( fail ) { __core.SetException(new InvalidOperationException("source failed")); }
            else { __core.SetResult(value); }
        }

        public T GetResult( short token )
        {
            if ( Interlocked.Increment(ref __getResultCalls) > 1 ) { throw new InvalidOperationException("ValueTask consumed more than once"); }

            __consumed.TrySetResult();
            return __core.GetResult(token);
        }
        void IValueTaskSource.GetResult( short token ) => GetResult(token);
        public ValueTaskSourceStatus GetStatus( short token ) => __core.GetStatus(token);
        public void OnCompleted( Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags ) => __core.OnCompleted(continuation, state, token, flags);
    }



    private sealed class CountingLogger : ILogger
    {
        public int        Count     { get; private set; }
        public Exception? Exception { get; private set; }
        public string?    Message   { get; private set; }

        public IDisposable? BeginScope<TState>( TState state )
            where TState : notnull => null;
        public bool IsEnabled( LogLevel logLevel ) => true;
        public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
        {
            Count++;
            Exception = exception;
            Message   = formatter(state, exception);
        }
    }
}
