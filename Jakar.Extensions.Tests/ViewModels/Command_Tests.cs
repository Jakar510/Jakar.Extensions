// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Command<>.Executable))]
public class Command_Tests : Assert
{
    private static readonly object __sender = new();


    [Test] public async Task Action_Invoked()
    {
        int                     calls      = 0;
        Command<int>.Executable executable = new Action(() => calls++);
        await executable.Execute(__sender, 5);
        this.AreEqual(1, calls);
    }

    [Test] public async Task ValueAction_ReceivesParameter()
    {
        int                     received   = 0;
        Command<int>.Executable executable = new Action<int>(x => received = x);
        await executable.Execute(__sender, 5);
        this.AreEqual(5, received);
    }

    [Test] public async Task EventHandler_ReceivesSenderAndEmptyArgs()
    {
        object?                 sender     = null;
        EventArgs?              args       = null;
        Command<int>.Executable executable = new EventHandler(( s, e ) => ( sender, args ) = ( s, e ));
        await executable.Execute(__sender, 5);
        Assert.AreSame(__sender,        sender);
        Assert.AreSame(EventArgs.Empty, args);
    }

    [Test] public async Task ValueEventHandler_ReceivesSenderAndParameter()
    {
        object?                 sender     = null;
        int                     received   = 0;
        Command<int>.Executable executable = new EventHandler<int>(( s, e ) => ( sender, received ) = ( s, e ));
        await executable.Execute(__sender, 5);
        Assert.AreSame(__sender, sender);
        this.AreEqual(5, received);
    }

    [Test] public async Task TaskHandler_IsAwaited()
    {
        TaskCompletionSource tcs      = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int                  received = 0;

        Command<int>.Executable executable = new Func<int, Task>(x =>
                                                                 {
                                                                     received = x;
                                                                     return tcs.Task;
                                                                 });

        ValueTask pending = executable.Execute(__sender, 5);
        this.AreEqual(5, received);
        this.IsFalse(pending.IsCompleted);

        tcs.SetResult();
        await pending;
        this.IsTrue(pending.IsCompletedSuccessfully);
    }

    [Test] public async Task TaskHandler_PropagatesException()
    {
        Command<int>.Executable executable = new Func<int, Task>(static _ => Task.FromException(new InvalidOperationException("boom")));
        bool                    thrown     = false;

        try { await executable.Execute(__sender, 5); }
        catch ( InvalidOperationException ) { thrown = true; }

        this.IsTrue(thrown);
    }

    [Test] public async Task ValueTaskHandler_ReceivesParameter()
    {
        int received = 0;

        Command<int>.Executable executable = new Func<int, ValueTask>(x =>
                                                                      {
                                                                          received = x;
                                                                          return ValueTask.CompletedTask;
                                                                      });

        await executable.Execute(__sender, 5);
        this.AreEqual(5, received);
    }

    [Test] public async Task SenderTaskHandler_ReceivesSenderAndParameter()
    {
        object? sender   = null;
        int     received = 0;

        Command<int>.Executable executable = new Func<object?, int, Task>(( s, x ) =>
                                                                          {
                                                                              ( sender, received ) = ( s, x );
                                                                              return Task.CompletedTask;
                                                                          });

        await executable.Execute(__sender, 5);
        Assert.AreSame(__sender, sender);
        this.AreEqual(5, received);
    }

    [Test] public async Task SenderValueTaskHandler_ReceivesSenderAndParameter()
    {
        object? sender   = null;
        int     received = 0;

        Command<int>.Executable executable = new Func<object?, int, ValueTask>(( s, x ) =>
                                                                               {
                                                                                   ( sender, received ) = ( s, x );
                                                                                   return ValueTask.CompletedTask;
                                                                               });

        await executable.Execute(__sender, 5);
        Assert.AreSame(__sender, sender);
        this.AreEqual(5, received);
    }

    [Test] public void Default_IsNoOp()
    {
        Command<int>.Executable executable = default;
        this.IsTrue(executable.Execute(__sender, 5).IsCompletedSuccessfully);
    }

    [Test] public void NullHandler_IsNoOp()
    {
        Command<int>.Executable executable = (Func<int, ValueTask>)null!;
        this.IsTrue(executable.Execute(__sender, 5).IsCompletedSuccessfully);
    }

    [Test] public void SynchronousHandler_CompletesSynchronously()
    {
        Command<int>.Executable executable = new Action(static () => { });
        this.IsTrue(executable.Execute(__sender, 5).IsCompletedSuccessfully);
    }

    [Test] public void Equality_ComparesHandlerAndKind()
    {
        Action                  action = static () => { };
        Command<int>.Executable a      = action;
        Command<int>.Executable b      = action;
        this.AreEqual(a, b);
    }

    // ─── Cancellable handler shapes ──────────────────────────────────────────

    [Test] public async Task CancellableHandlers_ReceiveToken()
    {
        using CancellationTokenSource cts      = new();
        CancellationToken             expected = cts.Token;
        int                           calls    = 0;

        Command<int>.Executable[] executables =
        [
            new Func<int, CancellationToken, Task>(( x, ct ) =>
                                                   {
                                                       check(x, ct);
                                                       return Task.CompletedTask;
                                                   }),
            new Func<int, CancellationToken, ValueTask>(( x, ct ) =>
                                                        {
                                                            check(x, ct);
                                                            return ValueTask.CompletedTask;
                                                        }),
            new Func<object?, int, CancellationToken, Task>(( s, x, ct ) =>
                                                            {
                                                                Assert.AreSame(__sender, s);
                                                                check(x, ct);
                                                                return Task.CompletedTask;
                                                            }),
            new Func<object?, int, CancellationToken, ValueTask>(( s, x, ct ) =>
                                                                 {
                                                                     Assert.AreSame(__sender, s);
                                                                     check(x, ct);
                                                                     return ValueTask.CompletedTask;
                                                                 })
        ];

        foreach ( Command<int>.Executable executable in executables ) { await executable.Execute(__sender, 5, expected); }

        this.AreEqual(executables.Length, calls);
        return;

        void check( int x, CancellationToken ct )
        {
            this.AreEqual(5,        x);
            this.AreEqual(expected, ct);
            calls++;
        }
    }

    [Test] public void Executable_PreCancelledToken_DoesNotInvoke()
    {
        bool                    invoked    = false;
        Command<int>.Executable executable = new Action(() => invoked = true);

        ValueTask result = executable.Execute(__sender, 5, new CancellationToken(true));

        this.IsTrue(result.IsCanceled);
        this.IsFalse(invoked);
    }


    // ─── Command: one execution at a time, newest cancels the rest ───────────

    private static readonly TimeSpan __timeout = TimeSpan.FromSeconds(5);

    private static Command<int> WaitsForCancellation( Action<int>? onCancelled = null ) =>
        new(new Func<int, CancellationToken, Task>(async ( x, ct ) =>
                                                   {
                                                       try { await Task.Delay(Timeout.Infinite, ct); }
                                                       catch ( OperationCanceledException )
                                                       {
                                                           onCancelled?.Invoke(x);
                                                           throw;
                                                       }
                                                   }));

    [Test] public async Task Command_Execute_RunsHandler()
    {
        int          received = 0;
        Command<int> command  = new(new Action<int>(x => received = x));

        await command.Execute(7);

        this.AreEqual(7, received);
        this.IsFalse(command.IsExecuting);
    }

    [Test] public async Task Command_NewExecution_CancelsPrevious()
    {
        List<int> completed = [];
        bool      cancelled = false;

        Command<int> command = new(new Func<int, CancellationToken, Task>(async ( x, ct ) =>
                                                                          {
                                                                              if ( x == 1 )
                                                                              {
                                                                                  try { await Task.Delay(Timeout.Infinite, ct); }
                                                                                  catch ( OperationCanceledException )
                                                                                  {
                                                                                      cancelled = true;
                                                                                      throw;
                                                                                  }
                                                                              }

                                                                              lock ( completed ) { completed.Add(x); }
                                                                          }));

        Task first  = command.Execute(1);
        Task second = command.Execute(2);
        await Task.WhenAll(first, second).WaitAsync(__timeout);

        this.IsTrue(cancelled);
        this.AreEqual(new[] { 2 }, completed.ToArray());
        this.IsFalse(command.IsExecuting);
    }

    [Test] public async Task Command_NewExecution_WaitsForPreviousThatIgnoresCancellation()
    {
        TaskCompletionSource gate    = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int                  running = 0, maxRunning = 0;
        List<int>            started = [];

        Command<int> command = new(new Func<int, Task>(async x =>
                                                       {
                                                           lock ( started )
                                                           {
                                                               started.Add(x);
                                                               maxRunning = Math.Max(maxRunning, ++running);
                                                           }

                                                           if ( x == 1 ) { await gate.Task; } // ignores cancellation

                                                           lock ( started ) { running--; }
                                                       }));

        Task first  = command.Execute(1);
        Task second = command.Execute(2);

        this.AreEqual(new[] { 1 }, started.ToArray());
        this.IsFalse(second.IsCompleted);
        this.IsTrue(command.IsExecuting);

        gate.SetResult();
        await Task.WhenAll(first, second).WaitAsync(__timeout);

        this.AreEqual(new[] { 1, 2 }, started.ToArray());
        this.AreEqual(1,              maxRunning);
        this.IsFalse(command.IsExecuting);
    }

    [Test] public async Task Command_SupersededWhileWaiting_NeverRuns_AndLatestStillWaitsForOldest()
    {
        TaskCompletionSource gate    = new(TaskCreationOptions.RunContinuationsAsynchronously);
        List<int>            started = [];

        Command<int> command = new(new Func<int, Task>(async x =>
                                                       {
                                                           lock ( started ) { started.Add(x); }

                                                           if ( x == 1 ) { await gate.Task; } // ignores cancellation
                                                       }));

        Task first  = command.Execute(1);
        Task second = command.Execute(2); // waits for 1
        Task third  = command.Execute(3); // cancels 2 while it waits; must still wait for 1

        await second.WaitAsync(__timeout);
        this.AreEqual(new[] { 1 }, started.ToArray());
        this.IsFalse(third.IsCompleted);

        gate.SetResult();
        await Task.WhenAll(first, third).WaitAsync(__timeout);

        this.AreEqual(new[] { 1, 3 }, started.ToArray());
    }

    [Test] public async Task Command_Cancel_CancelsCurrent()
    {
        bool         cancelled = false;
        Command<int> command   = WaitsForCancellation(_ => cancelled = true);

        Task run = command.Execute(1);
        command.Cancel();
        await run.WaitAsync(__timeout);

        this.IsTrue(cancelled);
        this.IsFalse(command.IsExecuting);
    }

    [Test] public async Task Command_Cancel_AfterCompletion_IsNoOp()
    {
        Command<int> command = new(new Action<int>(static _ => { }));
        await command.Execute(1);

        command.Cancel();
        command.Cancel();

        this.IsFalse(command.IsExecuting);
    }

    [Test] public async Task Command_Dispose_CancelsCurrent()
    {
        bool         cancelled = false;
        Command<int> command   = WaitsForCancellation(_ => cancelled = true);

        Task run = command.Execute(1);
        command.Dispose();
        await run.WaitAsync(__timeout);

        this.IsTrue(cancelled);
    }

    [Test] public async Task Command_CallerToken_CancelsExecution()
    {
        using CancellationTokenSource cts       = new();
        bool                          cancelled = false;
        Command<int>                  command   = WaitsForCancellation(_ => cancelled = true);

        Task run = command.Execute(1, cts.Token);
        await cts.CancelAsync();
        await run.WaitAsync(__timeout);

        this.IsTrue(cancelled);
    }

    [Test] public async Task Command_HandlerException_IsNotThrown_AndNextExecutionRuns()
    {
        int received = 0;

        Command<int> command = new(new Func<int, Task>(x => x == 1
                                                                ? Task.FromException(new InvalidOperationException("boom"))
                                                                : Task.FromResult(received = x)));

        await command.Execute(1);
        await command.Execute(2);

        this.AreEqual(2, received);
        this.IsFalse(command.IsExecuting);
    }

    [Test] [Repeat(20)] public async Task Command_ConcurrentCallers_NeverOverlap_AndLastOneRuns()
    {
        const int CALLERS = 64;
        int       running = 0, maxRunning = 0, finished = 0;

        Command<int> command = new(new Func<int, CancellationToken, Task>(async ( _, ct ) =>
                                                                          {
                                                                              int now = Interlocked.Increment(ref running);
                                                                              interlockedMax(ref maxRunning, now);

                                                                              try
                                                                              {
                                                                                  await Task.Delay(1, ct);
                                                                                  Interlocked.Increment(ref finished);
                                                                              }
                                                                              finally { Interlocked.Decrement(ref running); }
                                                                          }));

        // Dedicated threads (not the thread pool) so a start barrier cannot starve the pool.
        using Barrier barrier = new(CALLERS);
        Task[]        runs    = new Task[CALLERS];
        Thread[]      threads = new Thread[CALLERS];

        for ( int i = 0; i < CALLERS; i++ )
        {
            int value = i;

            threads[i] = new Thread(() =>
                                    {
                                        barrier.SignalAndWait();
                                        runs[value] = command.Execute(value);
                                    }) { IsBackground = true };

            threads[i].Start();
        }

        foreach ( Thread thread in threads ) { thread.Join(); }

        await Task.WhenAll(runs).WaitAsync(__timeout);

        this.AreEqual(1, maxRunning);
        this.IsTrue(finished >= 1); // the newest execution is never superseded, so it runs to completion
        this.IsFalse(command.IsExecuting);
        return;

        static void interlockedMax( ref int target, int value )
        {
            int current;

            do { current = Volatile.Read(ref target); }
            while ( value > current && Interlocked.CompareExchange(ref target, value, current) != current );
        }
    }
}
