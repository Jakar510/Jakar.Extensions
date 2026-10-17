namespace Jakar.Extensions;


public static class AsyncCommandExtensions
{
    /// <summary> Creates an <see cref="ICommand"/> that cancels <paramref name="command"/>; it can execute only while <see cref="IAsyncCommand.CanBeCanceled"/> is <see langword="true"/>. </summary>
    public static ICommand CreateCancelCommand( this IAsyncCommand command ) => new CancelCommand(ThrowIfNull(command));



    private sealed class CancelCommand : ICommand
    {
        private readonly IAsyncCommand __command;


        public event EventHandler? CanExecuteChanged;


        public CancelCommand( IAsyncCommand command )
        {
            __command               =  command;
            command.PropertyChanged += OnPropertyChanged;
        }


        public bool CanExecute( object? parameter ) => __command.CanBeCanceled;
        public void Execute( object?    parameter ) => __command.Cancel();


        private void OnPropertyChanged( object? sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName is null or "" or nameof(IAsyncCommand.CanBeCanceled) ) { CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
        }
    }
}
