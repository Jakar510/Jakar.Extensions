#nullable enable
using Acr.UserDialogs;
using Xamarin.CommunityToolkit.ObjectModel;



namespace Jakar.Extensions.Xamarin.Forms;


public class Commands
{
    protected readonly Prompts _prompts;
    public Commands( Prompts prompts ) => _prompts = prompts;


    public ICommand LoadingCommand( Func<CancellationToken, Task>                       func, Page     page ) => LoadingCommand( func,                                MaskType.Black, _prompts.Cancel, page );
    public ICommand LoadingCommand( Func<CancellationToken, Task>                       func, Page     page,  string cancel ) => LoadingCommand( func,                MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<CancellationToken, Task>                       func, MaskType mask,  string cancel, Page     page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<CancellationToken, Task>                       func, string?  title, string cancel, MaskType mask, Page page ) => new AsyncCommand( () => LoadingAsyncTask( func, title, cancel, mask, page ) );
    public ICommand LoadingCommand( Func<CancellationToken, ValueTask>                  func, Page     page ) => LoadingCommand( func,                                MaskType.Black, _prompts.Cancel, page );
    public ICommand LoadingCommand( Func<CancellationToken, ValueTask>                  func, Page     page,  string cancel ) => LoadingCommand( func,                MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<CancellationToken, ValueTask>                  func, MaskType mask,  string cancel, Page     page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<CancellationToken, ValueTask>                  func, string?  title, string cancel, MaskType mask, Page page ) => new AsyncCommand( () => LoadingAsyncTask( func, title, cancel, mask, page ) );
    public ICommand LoadingCommand( Func<Task>                                          func, Page     page ) => LoadingCommand( func,                                MaskType.Black, _prompts.Cancel, page );
    public ICommand LoadingCommand( Func<Task>                                          func, Page     page,  string cancel ) => LoadingCommand( func,                MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<Task>                                          func, MaskType mask,  string cancel, Page     page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<Task>                                          func, string?  title, string cancel, MaskType mask, Page page ) => new AsyncCommand( () => LoadingAsyncTask( func, title, cancel, mask, page ) );
    public ICommand LoadingCommand( Func<ValueTask>                                     func, Page     page ) => LoadingCommand( func,                                MaskType.Black, _prompts.Cancel, page );
    public ICommand LoadingCommand( Func<ValueTask>                                     func, Page     page,  string cancel ) => LoadingCommand( func,                MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<ValueTask>                                     func, MaskType mask,  string cancel, Page     page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<ValueTask>                                     func, string?  title, string cancel, MaskType mask, Page page ) => new AsyncCommand( () => LoadingAsyncTask( func, title, cancel, mask, page ) );
    public ICommand LoadingCommand( Func<IProgressDialog, CancellationToken, Task>      func, Page     page ) => LoadingCommand( func,                                MaskType.Black, _prompts.Cancel, page );
    public ICommand LoadingCommand( Func<IProgressDialog, CancellationToken, Task>      func, Page     page,  string cancel ) => LoadingCommand( func,                MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<IProgressDialog, CancellationToken, Task>      func, MaskType mask,  string cancel, Page     page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<IProgressDialog, CancellationToken, Task>      func, string?  title, string cancel, MaskType mask, Page page ) => new AsyncCommand( () => LoadingAsyncTask( func, title, cancel, mask, page ) );
    public ICommand LoadingCommand( Func<IProgressDialog, CancellationToken, ValueTask> func, Page     page ) => LoadingCommand( func,                                MaskType.Black, _prompts.Cancel, page );
    public ICommand LoadingCommand( Func<IProgressDialog, CancellationToken, ValueTask> func, Page     page,  string cancel ) => LoadingCommand( func,                MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<IProgressDialog, CancellationToken, ValueTask> func, MaskType mask,  string cancel, Page     page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<IProgressDialog, CancellationToken, ValueTask> func, string?  title, string cancel, MaskType mask, Page page ) => new AsyncCommand( () => LoadingAsyncTask( func, title, cancel, mask, page ) );
    public ICommand LoadingCommand( Func<IProgressDialog, Task>                         func, Page     page ) => LoadingCommand( func,                                MaskType.Black, _prompts.Cancel, page );
    public ICommand LoadingCommand( Func<IProgressDialog, Task>                         func, Page     page,  string cancel ) => LoadingCommand( func,                MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<IProgressDialog, Task>                         func, MaskType mask,  string cancel, Page     page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<IProgressDialog, Task>                         func, string?  title, string cancel, MaskType mask, Page page ) => new AsyncCommand( () => LoadingAsyncTask( func, title, cancel, mask, page ) );
    public ICommand LoadingCommand( Func<IProgressDialog, ValueTask>                    func, Page     page ) => LoadingCommand( func,                                MaskType.Black, _prompts.Cancel, page );
    public ICommand LoadingCommand( Func<IProgressDialog, ValueTask>                    func, Page     page,  string cancel ) => LoadingCommand( func,                MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<IProgressDialog, ValueTask>                    func, MaskType mask,  string cancel, Page     page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<IProgressDialog, ValueTask>                    func, string?  title, string cancel, MaskType mask, Page page ) => new AsyncCommand( () => LoadingAsyncTask( func, title, cancel, mask, page ) );


    public ICommand LoadingCommand( Action                  func, Page page ) => LoadingCommand( func,                                page, _prompts.Cancel, MaskType.Black );
    public ICommand LoadingCommand( Action                  func, Page page, string  cancel ) => LoadingCommand( func,                page, cancel,          MaskType.Black );
    public ICommand LoadingCommand( Action                  func, Page page, string  cancel, MaskType mask ) => LoadingCommand( func, page, null,            cancel, mask );
    public ICommand LoadingCommand( Action                  func, Page page, string? title,  string   cancel, MaskType mask ) => new AsyncCommand( () => LoadingAction( func, title, cancel, mask, page ) );
    public ICommand LoadingCommand( Action<IProgressDialog> func, Page page ) => LoadingCommand( func,                                page, _prompts.Cancel, MaskType.Black );
    public ICommand LoadingCommand( Action<IProgressDialog> func, Page page, string  cancel ) => LoadingCommand( func,                page, cancel,          MaskType.Black );
    public ICommand LoadingCommand( Action<IProgressDialog> func, Page page, string  cancel, MaskType mask ) => LoadingCommand( func, page, null,            cancel, mask );
    public ICommand LoadingCommand( Action<IProgressDialog> func, Page page, string? title,  string   cancel, MaskType mask ) => new AsyncCommand( () => LoadingAction( func, title, cancel, mask, page ) );
    public async Task LoadingAction( Action func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var source = new CancellationTokenSource();

        ProgressDialogConfig config = new ProgressDialogConfig().SetTitle( title )
                                                                .SetIsDeterministic( false )
                                                                .SetMaskType( mask )
                                                                .SetCancel( cancel, source.Cancel );

        using ( _prompts.Progress( config ) )
        {
            try { func(); }
            catch ( OperationCanceledException ) { }
            catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
        }
    }
    public async Task LoadingAction( Action<IProgressDialog> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var             source = new CancellationTokenSource();
        using IProgressDialog dialog = _prompts.Loading( title, source.Cancel, cancel, true, mask );

        try { func( dialog ); }
        catch ( OperationCanceledException ) { }
        catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
    }


    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page ) => await LoadingAsyncTask( func,                               page, _prompts.Cancel, MaskType.Black );
    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page, string cancel ) => await LoadingAsyncTask( func,                page, cancel,          MaskType.Black );
    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel,          mask, page );
    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var source = new CancellationTokenSource();

        ProgressDialogConfig config = new ProgressDialogConfig().SetTitle( title )
                                                                .SetIsDeterministic( false )
                                                                .SetMaskType( mask )
                                                                .SetCancel( cancel, source.Cancel );

        using ( _prompts.Progress( config ) )
        {
            try { await func( source.Token ); }
            catch ( OperationCanceledException ) { }
            catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
        }
    }


    public async Task LoadingAsyncTask( Func<CancellationToken, ValueTask> func, Page page ) => await LoadingAsyncTask( func,                               page, _prompts.Cancel, MaskType.Black );
    public async Task LoadingAsyncTask( Func<CancellationToken, ValueTask> func, Page page, string cancel ) => await LoadingAsyncTask( func,                page, cancel,          MaskType.Black );
    public async Task LoadingAsyncTask( Func<CancellationToken, ValueTask> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel,          mask, page );
    public async Task LoadingAsyncTask( Func<CancellationToken, ValueTask> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var source = new CancellationTokenSource();

        ProgressDialogConfig config = new ProgressDialogConfig().SetTitle( title )
                                                                .SetIsDeterministic( false )
                                                                .SetMaskType( mask )
                                                                .SetCancel( cancel, source.Cancel );

        using ( _prompts.Progress( config ) )
        {
            try { await func( source.Token ); }
            catch ( OperationCanceledException ) { }
            catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
        }
    }


    public async Task LoadingAsyncTask( Func<Task> func, Page page ) => await LoadingAsyncTask( func,                               page, _prompts.Cancel, MaskType.Black );
    public async Task LoadingAsyncTask( Func<Task> func, Page page, string cancel ) => await LoadingAsyncTask( func,                page, cancel,          MaskType.Black );
    public async Task LoadingAsyncTask( Func<Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel,          mask, page );
    public async Task LoadingAsyncTask( Func<Task> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var source = new CancellationTokenSource();

        ProgressDialogConfig config = new ProgressDialogConfig().SetTitle( title )
                                                                .SetIsDeterministic( false )
                                                                .SetMaskType( mask )
                                                                .SetCancel( cancel, source.Cancel );

        using ( _prompts.Progress( config ) )
        {
            try { await func(); }
            catch ( OperationCanceledException ) { }
            catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
        }
    }

    public async Task LoadingAsyncTask( Func<ValueTask> func, Page page ) => await LoadingAsyncTask( func,                               page, _prompts.Cancel, MaskType.Black );
    public async Task LoadingAsyncTask( Func<ValueTask> func, Page page, string cancel ) => await LoadingAsyncTask( func,                page, cancel,          MaskType.Black );
    public async Task LoadingAsyncTask( Func<ValueTask> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel,          mask, page );
    public async Task LoadingAsyncTask( Func<ValueTask> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var source = new CancellationTokenSource();

        ProgressDialogConfig config = new ProgressDialogConfig().SetTitle( title )
                                                                .SetIsDeterministic( false )
                                                                .SetMaskType( mask )
                                                                .SetCancel( cancel, source.Cancel );

        using ( _prompts.Progress( config ) )
        {
            try { await func(); }
            catch ( OperationCanceledException ) { }
            catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
        }
    }


    public async Task LoadingAsyncTask( Func<IProgressDialog, ValueTask> func, Page page ) => await LoadingAsyncTask( func,                               page, _prompts.Cancel, MaskType.Black );
    public async Task LoadingAsyncTask( Func<IProgressDialog, ValueTask> func, Page page, string cancel ) => await LoadingAsyncTask( func,                page, cancel,          MaskType.Black );
    public async Task LoadingAsyncTask( Func<IProgressDialog, ValueTask> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel,          mask, page );
    public async Task LoadingAsyncTask( Func<IProgressDialog, ValueTask> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var             source = new CancellationTokenSource();
        using IProgressDialog dialog = _prompts.Loading( title, source.Cancel, cancel, true, mask );

        try { await func( dialog ); }
        catch ( OperationCanceledException ) { }
        catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
    }


    public async Task LoadingAsyncTask( Func<IProgressDialog, Task> func, Page page ) => await LoadingAsyncTask( func,                               page, _prompts.Cancel, MaskType.Black );
    public async Task LoadingAsyncTask( Func<IProgressDialog, Task> func, Page page, string cancel ) => await LoadingAsyncTask( func,                page, cancel,          MaskType.Black );
    public async Task LoadingAsyncTask( Func<IProgressDialog, Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel,          mask, page );
    public async Task LoadingAsyncTask( Func<IProgressDialog, Task> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var             source = new CancellationTokenSource();
        using IProgressDialog dialog = _prompts.Loading( title, source.Cancel, cancel, true, mask );

        try { await func( dialog ); }
        catch ( OperationCanceledException ) { }
        catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
    }


    public async Task LoadingAsyncTask( Func<IProgressDialog, CancellationToken, ValueTask> func, Page page ) => await LoadingAsyncTask( func,                               page, _prompts.Cancel, MaskType.Black );
    public async Task LoadingAsyncTask( Func<IProgressDialog, CancellationToken, ValueTask> func, Page page, string cancel ) => await LoadingAsyncTask( func,                page, cancel,          MaskType.Black );
    public async Task LoadingAsyncTask( Func<IProgressDialog, CancellationToken, ValueTask> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel,          mask, page );
    public async Task LoadingAsyncTask( Func<IProgressDialog, CancellationToken, ValueTask> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var             source = new CancellationTokenSource();
        using IProgressDialog dialog = _prompts.Loading( title, source.Cancel, cancel, true, mask );

        try { await func( dialog, source.Token ); }
        catch ( OperationCanceledException ) { }
        catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
    }


    public async Task LoadingAsyncTask( Func<IProgressDialog, CancellationToken, Task> func, Page page ) => await LoadingAsyncTask( func,                               page, _prompts.Cancel, MaskType.Black );
    public async Task LoadingAsyncTask( Func<IProgressDialog, CancellationToken, Task> func, Page page, string cancel ) => await LoadingAsyncTask( func,                page, cancel,          MaskType.Black );
    public async Task LoadingAsyncTask( Func<IProgressDialog, CancellationToken, Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel,          mask, page );
    public async Task LoadingAsyncTask( Func<IProgressDialog, CancellationToken, Task> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) { throw new ArgumentNullException( nameof(func) ); }

        using var             source = new CancellationTokenSource();
        using IProgressDialog dialog = _prompts.Loading( title, source.Cancel, cancel, true, mask );

        try { await func( dialog, source.Token ); }
        catch ( OperationCanceledException ) { }
        catch ( Exception e ) { await _prompts.HandleExceptionAsync( e, page, source.Token ); }
    }
}
