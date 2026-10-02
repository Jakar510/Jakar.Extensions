namespace Jakar.Extensions;


/// <summary>
///     An <see cref="ICommand"/> that runs one <see cref="Executable"/> at a time.
///     Starting a new execution cancels the current one and waits for it (and anything before it) to finish before the handler runs.
/// </summary>
public class Command<TValue>( Command<TValue>.Executable execute, Func<TValue?, bool>? canExecute = null ) : BaseClass, ICommand
{
    protected readonly Executable           _execute    = execute;
    protected readonly Func<TValue?, bool>? _canExecute = canExecute;
    protected          bool?                _canExecuteValue;
    private            Execution?           __current;


    public event EventHandler? CanExecuteChanged;


    /// <summary> <see langword="true"/> while an execution (or one it is waiting on) has not finished. </summary>
    public bool IsExecuting { get => Volatile.Read(ref __current)?.Task.IsCompleted is false; }


    bool ICommand.CanExecute( object? parameter ) => CanExecute(parameter);
    void ICommand.Execute( object?    parameter ) => Execute(parameter);


    public virtual bool CanExecute( TValue? value )
    {
        bool result = _canExecute?.Invoke(value) ?? true;
        if ( Nullable.Equals(_canExecuteValue, result) ) { return result; }

        _canExecuteValue = result;
        SendCanExecuteChanged();
        return result;
    }
    protected void SendCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    protected virtual bool CanExecute( object? parameter ) => CanExecute(parameter is TValue value
                                                                             ? value
                                                                             : default);


    /// <summary>
    ///     Cancels the current execution, waits for it to finish, then runs the handler with a token that is cancelled when <paramref name="token"/> is cancelled,
    ///     <see cref="Cancel"/> is called, the command is disposed, or a newer execution starts.
    ///     Cancellation is not treated as an error; other exceptions are logged, never thrown.
    /// </summary>
    public virtual async Task Execute( TValue? parameter, CancellationToken token = default )
    {
        CancellationTokenSource source = token.CanBeCanceled
                                             ? CancellationTokenSource.CreateLinkedTokenSource(token)
                                             : new CancellationTokenSource();

        // Publishing the source and the completion as one reference keeps them consistent without a lock.
        Execution  run         = new(source);
        Execution? previous    = Interlocked.Exchange(ref __current, run);
        Task       previousRun = previous?.Task ?? Task.CompletedTask;

        try
        {
            TryCancel(previous);

            // A cancelled handler may not stop immediately (or may ignore its token): wait for it, and everything before it, so executions never overlap.
            // If this execution is itself superseded or cancelled while waiting, WaitAsync throws and it ends without running its handler.
            if ( !previousRun.IsCompleted ) { await previousRun.WaitAsync(source.Token).ConfigureAwait(false); }

            if ( source.IsCancellationRequested ) { return; }

            await _execute.Execute(this, parameter, source.Token).ConfigureAwait(false);
        }
        catch ( OperationCanceledException ) when ( source.IsCancellationRequested ) { }
        catch ( Exception e ) { SelfLogger.WriteLine(e); }
        finally
        {
            // The run stays published after it finishes: a later caller must still wait on its Task (which may be chained to earlier runs), and cancelling its disposed source is ignored by TryCancel.
            source.Dispose();

            // Only signal completion once every earlier execution has also finished, so a later caller never overlaps a handler that ignored cancellation.
            if ( previousRun.IsCompleted ) { run.TrySetResult(); }
            else { _ = previousRun.ContinueWith(static ( _, state ) => ( (Execution)state! ).TrySetResult(), run, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default); }
        }
    }


    /// <summary> Cancels the current execution, if any. </summary>
    public void Cancel() => TryCancel(Volatile.Read(ref __current));
    private static void TryCancel( Execution? execution )
    {
        if ( execution is null || execution.Task.IsCompleted ) { return; }

        // The execution may finish and dispose its source at any moment; if so there is nothing left to cancel.
        try { execution.Source.Cancel(); }
        catch ( ObjectDisposedException ) { }
        catch ( AggregateException e ) { SelfLogger.WriteLine("{Error} \n {StackTrace}", e.Message, e.ToString()); }
    }
    protected override void Dispose( bool disposing )
    {
        if ( disposing ) { Cancel(); }

        base.Dispose(disposing);
    }
    protected virtual void Execute( object? parameter )
    {
        Execute(parameter is TValue value
                    ? value
                    : default)
           .SafeFireAndForget();
    }



    /// <summary> One execution: its completion (set once it and every earlier execution have finished) and the source that cancels it. </summary>
    private sealed class Execution( CancellationTokenSource source ) : TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)
    {
        public readonly CancellationTokenSource Source = source;
    }



    /// <summary> A single delegate tagged with its shape, so the struct is one reference plus one byte and dispatch needs no type checks. </summary>
    public readonly record struct Executable
    {
        private readonly Delegate? __handler;
        private readonly Kind      __kind;
        private Executable( Kind kind, Delegate? handler )
        {
            __kind    = kind;
            __handler = handler;
        }
        public static implicit operator Executable( Action                                               action ) => new(Kind.Action, action);
        public static implicit operator Executable( Action<TValue?>                                      action ) => new(Kind.ValueAction, action);
        public static implicit operator Executable( EventHandler                                         action ) => new(Kind.EventHandler, action);
        public static implicit operator Executable( EventHandler<TValue?>                                action ) => new(Kind.ValueEventHandler, action);
        public static implicit operator Executable( Func<TValue?, Task>                                  action ) => new(Kind.TaskHandler, action);
        public static implicit operator Executable( Func<TValue?, ValueTask>                             action ) => new(Kind.ValueTaskHandler, action);
        public static implicit operator Executable( Func<object?, TValue?, Task>                         action ) => new(Kind.SenderTaskHandler, action);
        public static implicit operator Executable( Func<object?, TValue?, ValueTask>                    action ) => new(Kind.SenderValueTaskHandler, action);
        public static implicit operator Executable( Func<TValue?, CancellationToken, Task>               action ) => new(Kind.CancellableTaskHandler, action);
        public static implicit operator Executable( Func<TValue?, CancellationToken, ValueTask>          action ) => new(Kind.CancellableValueTaskHandler, action);
        public static implicit operator Executable( Func<object?, TValue?, CancellationToken, Task>      action ) => new(Kind.CancellableSenderTaskHandler, action);
        public static implicit operator Executable( Func<object?, TValue?, CancellationToken, ValueTask> action ) => new(Kind.CancellableSenderValueTaskHandler, action);


        /// <inheritdoc cref="Execute(object?, TValue?, CancellationToken)"/>
        public ValueTask Execute( object? sender, TValue? parameter ) => Execute(sender, parameter, CancellationToken.None);
        /// <summary>
        ///     Invokes the handler. Synchronous handlers complete without allocating; asynchronous handlers' tasks are returned (not awaited here), so no state machine is created.
        ///     <paramref name="token"/> is passed to the cancellable handler shapes; if it is already cancelled, no handler runs. A default or null handler is a no-op.
        /// </summary>
        public ValueTask Execute( object? sender, TValue? parameter, CancellationToken token )
        {
            if ( token.IsCancellationRequested ) { return ValueTask.FromCanceled(token); }

            Delegate? handler = __handler;
            if ( handler is null ) { return ValueTask.CompletedTask; }

            // __kind is only ever set alongside a delegate of the matching type, so the unchecked casts are safe.
            switch ( __kind )
            {
                case Kind.Action:
                    Unsafe.As<Action>(handler)();
                    return ValueTask.CompletedTask;

                case Kind.ValueAction:
                    Unsafe.As<Action<TValue?>>(handler)(parameter);
                    return ValueTask.CompletedTask;

                case Kind.EventHandler:
                    Unsafe.As<EventHandler>(handler)(sender, EventArgs.Empty);
                    return ValueTask.CompletedTask;

                case Kind.ValueEventHandler:
                    Unsafe.As<EventHandler<TValue?>>(handler)(sender, parameter);
                    return ValueTask.CompletedTask;

                case Kind.TaskHandler:
                    return new ValueTask(Unsafe.As<Func<TValue?, Task>>(handler)(parameter));

                case Kind.ValueTaskHandler:
                    return Unsafe.As<Func<TValue?, ValueTask>>(handler)(parameter);

                case Kind.SenderTaskHandler:
                    return new ValueTask(Unsafe.As<Func<object?, TValue?, Task>>(handler)(sender, parameter));

                case Kind.SenderValueTaskHandler:
                    return Unsafe.As<Func<object?, TValue?, ValueTask>>(handler)(sender, parameter);

                case Kind.CancellableTaskHandler:
                    return new ValueTask(Unsafe.As<Func<TValue?, CancellationToken, Task>>(handler)(parameter, token));

                case Kind.CancellableValueTaskHandler:
                    return Unsafe.As<Func<TValue?, CancellationToken, ValueTask>>(handler)(parameter, token);

                case Kind.CancellableSenderTaskHandler:
                    return new ValueTask(Unsafe.As<Func<object?, TValue?, CancellationToken, Task>>(handler)(sender, parameter, token));

                case Kind.CancellableSenderValueTaskHandler:
                    return Unsafe.As<Func<object?, TValue?, CancellationToken, ValueTask>>(handler)(sender, parameter, token);

                default:
                    throw new OutOfRangeException(__kind);
            }
        }



        private enum Kind : byte
        {
            Action,
            ValueAction,
            EventHandler,
            ValueEventHandler,
            TaskHandler,
            ValueTaskHandler,
            SenderTaskHandler,
            SenderValueTaskHandler,
            CancellableTaskHandler,
            CancellableValueTaskHandler,
            CancellableSenderTaskHandler,
            CancellableSenderValueTaskHandler
        }
    }
}
