namespace Jakar.Extensions;


public class Command<TValue>( Command<TValue>.Executable execute, Func<TValue?, bool>? canExecute = null ) : BaseClass, ICommand
{
    protected          bool?                    _canExecuteValue;
    protected readonly Func<TValue?, bool>?     _canExecute = canExecute;
    protected readonly Executable               _execute    = execute;
    protected          CancellationTokenSource? _source;


    public event EventHandler? CanExecuteChanged;


    bool ICommand.CanExecute( object? parameter ) => CanExecute(parameter);
    void ICommand.Execute( object?    parameter ) => Execute(parameter);


    public virtual bool CanExecute( TValue? value )
    {
        bool result = _canExecute?.Invoke(value) ?? true;
        if ( Nullable.Equals(_canExecuteValue, result) ) { return result; }

        _canExecuteValue = result;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        return result;
    }
    protected virtual bool CanExecute( object? parameter ) => CanExecute(parameter is TValue value
                                                                             ? value
                                                                             : default);


    public virtual async ValueTask Execute( TValue? parameter )
    {
        try { await _execute.Execute(this, parameter).ConfigureAwait(false); }
        catch ( Exception e ) { SelfLogger.WriteLine("{Error} \n {StackTrace}", e.Message, e.ToString()); }
    }
    protected virtual async void Execute( object? parameter )
    {
        try
        {
            await Execute(parameter is TValue value
                              ? value
                              : default).ConfigureAwait(false);
        }
        catch ( Exception e ) { SelfLogger.WriteLine("'{Error}' \n {StackTrace}", e.Message, e.ToString()); }
    }



    public readonly struct Executable( int                                index,
                                       Action?                            action,
                                       Action<TValue?>?                   valueAction,
                                       EventHandler?                      eventHandler,
                                       EventHandler<TValue?>?             valueEventHandler,
                                       Func<TValue?, Task>?               taskHandler,
                                       Func<TValue?, ValueTask>?          valueTaskHandler,
                                       Func<object?, TValue?, Task>?      senderTaskHandler,
                                       Func<object?, TValue?, ValueTask>? senderValueTaskHandler )
    {
        public static implicit operator Executable( Action                            action ) => new(0, action, null, null, null, null, null, null, null);
        public static implicit operator Executable( Action<TValue?>                   action ) => new(1, null, action, null, null, null, null, null, null);
        public static implicit operator Executable( EventHandler                      action ) => new(3, null, null, action, null, null, null, null, null);
        public static implicit operator Executable( EventHandler<TValue?>             action ) => new(2, null, null, null, action, null, null, null, null);
        public static implicit operator Executable( Func<TValue?, Task>               action ) => new(4, null, null, null, null, action, null, null, null);
        public static implicit operator Executable( Func<TValue?, ValueTask>          action ) => new(5, null, null, null, null, null, action, null, null);
        public static implicit operator Executable( Func<object?, TValue?, Task>      action ) => new(6, null, null, null, null, null, null, action, null);
        public static implicit operator Executable( Func<object?, TValue?, ValueTask> action ) => new(7, null, null, null, null, null, null, null, action);


        public async ValueTask Execute( object? sender, TValue? parameter )
        {
            switch ( index )
            {
                case 0:
                    action?.Invoke();
                    return;

                case 1:
                    valueAction?.Invoke(parameter);
                    return;

                case 2:
                    valueEventHandler?.Invoke(sender, parameter);
                    return;

                case 3:
                    eventHandler?.Invoke(sender, EventArgs.Empty);
                    return;

                case 4:
                    taskHandler?.Invoke(parameter);
                    return;

                case 5:
                    valueTaskHandler?.Invoke(parameter);
                    return;

                case 6:
                    Debug.Assert(senderTaskHandler is not null, nameof(senderTaskHandler) + " is not null");
                    await senderTaskHandler.Invoke(sender, parameter).ConfigureAwait(false);
                    return;

                case 7:
                    Debug.Assert(senderValueTaskHandler is not null, nameof(senderValueTaskHandler) + " is not null");
                    await senderValueTaskHandler.Invoke(sender, parameter).ConfigureAwait(false);
                    return;


                default:
                    throw new OutOfRangeException(index);
            }
        }
    }
}
