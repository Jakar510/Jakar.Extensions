namespace Jakar.Extensions;


/// <summary> An <see cref="ICommand"/> backed by an asynchronous operation (mirrors <c>CommunityToolkit.Mvvm.Input.IAsyncRelayCommand</c>). </summary>
public interface IAsyncCommand : ICommand, INotifyPropertyChanged
{
    /// <summary> The task of the most recent execution, or <see langword="null"/> if the command has never run. </summary>
    Task? ExecutionTask { get; }

    /// <summary> <see langword="true"/> while running a handler that accepts a <see cref="CancellationToken"/> whose cancellation has not been requested. </summary>
    bool CanBeCanceled { get; }

    /// <summary> <see langword="true"/> once cancellation of the current execution has been requested. </summary>
    bool IsCancellationRequested { get; }

    /// <summary> <see langword="true"/> while <see cref="ExecutionTask"/> has not completed. </summary>
    bool IsRunning { get; }


    /// <summary> Raises <see cref="ICommand.CanExecuteChanged"/>. </summary>
    void NotifyCanExecuteChanged();

    /// <summary> Starts an execution and returns its task. </summary>
    Task ExecuteAsync( object? parameter );

    /// <summary> Requests cancellation of the current execution, if its handler accepts a <see cref="CancellationToken"/>. </summary>
    void Cancel();
}



/// <summary> A strongly typed <see cref="IAsyncCommand"/> (mirrors <c>CommunityToolkit.Mvvm.Input.IAsyncRelayCommand&lt;T&gt;</c>). </summary>
public interface IAsyncCommand<in TValue> : IAsyncCommand
{
    /// <inheritdoc cref="ICommand.CanExecute(object?)"/>
    bool CanExecute( TValue? parameter );

    /// <inheritdoc cref="ICommand.Execute(object?)"/>
    void Execute( TValue? parameter );

    /// <inheritdoc cref="IAsyncCommand.ExecuteAsync(object?)"/>
    Task ExecuteAsync( TValue? parameter );
}
