namespace Jakar.Extensions;


public class Command<TValue> : BaseClass, ICommand
{
    protected          bool?                    _canExecuteValue;
    protected readonly Func<TValue?, bool>?     _canExecute;
    protected readonly Executable               _execute;
    protected          CancellationTokenSource? _source;


    public event EventHandler? CanExecuteChanged { add => _eventManager.AddEventHandler(value); remove => _eventManager.RemoveEventHandler(value); }


    public Command( Executable execute, Func<TValue?, bool>? canExecute = null )
    {
        _execute    = execute;
        _canExecute = canExecute;
    }


    bool ICommand.CanExecute( object? parameter ) => CanExecute(parameter);
    void ICommand.Execute( object?    parameter ) => Execute(parameter);


    public virtual bool CanExecute( TValue? value )
    {
        bool result = _canExecute?.Invoke(value) ?? true;
        if ( Nullable.Equals(_canExecuteValue, result) ) { return result; }

        _canExecuteValue = result;
        _eventManager.RaiseEvent(this, nameof(CanExecuteChanged));
        return result;
    }
    protected virtual bool CanExecute( object? parameter ) => CanExecute(parameter is TValue value
                                                                             ? value
                                                                             : default);


    public virtual async ValueTask Execute( TValue? parameter )
    {
        try { await _execute.Execute(this, parameter); }
        catch ( Exception e ) { SelfLogger.WriteLine("{Error} \n {StackTrace}", e.Message, e.ToString()); }
    }
    protected virtual async void Execute( object? parameter )
    {
        try
        {
            await Execute(parameter is TValue value
                              ? value
                              : default);
        }
        catch ( Exception e ) { SelfLogger.WriteLine("'{Error}' \n {StackTrace}", e.Message, e.ToString()); }
    }



    public readonly struct Executable( int                                index,
                                       Action?                            Action,
                                       Action<TValue?>?                   ValueAction,
                                       EventHandler?                      EventHandler,
                                       EventHandler<TValue?>?             ValueEventHandler,
                                       Func<TValue?, Task>?               TaskHandler,
                                       Func<TValue?, ValueTask>?          ValueTaskHandler,
                                       Func<object?, TValue?, Task>?      SenderTaskHandler,
                                       Func<object?, TValue?, ValueTask>? SenderValueTaskHandler )
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
                    Action?.Invoke();
                    return;

                case 1:
                    ValueAction?.Invoke(parameter);
                    return;

                case 2:
                    ValueEventHandler?.Invoke(sender, parameter);
                    return;

                case 3:
                    EventHandler?.Invoke(sender, EventArgs.Empty);
                    return;

                case 4:
                    TaskHandler?.Invoke(parameter);
                    return;

                case 5:
                    ValueTaskHandler?.Invoke(parameter);
                    return;

                case 6:
                    Debug.Assert(SenderTaskHandler is not null, nameof(SenderTaskHandler) + " is not null");
                    await SenderTaskHandler.Invoke(sender, parameter);
                    return;

                case 7:
                    Debug.Assert(SenderValueTaskHandler is not null, nameof(SenderValueTaskHandler) + " is not null");
                    await SenderValueTaskHandler.Invoke(sender, parameter);
                    return;


                default:
                    throw new OutOfRangeException(index);
            }
        }
    }
}
