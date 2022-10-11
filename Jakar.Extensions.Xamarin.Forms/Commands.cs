#nullable enable
using Acr.UserDialogs;
using Xamarin.CommunityToolkit.ObjectModel;



namespace Jakar.Extensions.Xamarin.Forms;


public class Commands
{
    protected Prompts _Prompts { get; set; }

    public Commands( Prompts prompts ) => _Prompts = prompts;


    public ICommand LoadingCommand( Func<CancellationToken, Task> func, Page     page ) => LoadingCommand( func,                           MaskType.Black, _Prompts.Cancel, page );
    public ICommand LoadingCommand( Func<CancellationToken, Task> func, Page     page, string cancel ) => LoadingCommand( func,            MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<CancellationToken, Task> func, MaskType mask, string cancel, Page page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<CancellationToken, Task> func, string? title, string cancel, MaskType mask, Page page ) => new AsyncCommand( async () => await LoadingAsyncTask( func, title, cancel, mask, page )
                                                                                                                                                                     .ConfigureAwait( false ) );


    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page ) => await LoadingAsyncTask( func, page, _Prompts.Cancel, MaskType.Black )
                                                                                              .ConfigureAwait( false );
    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page, string cancel ) => await LoadingAsyncTask( func, page, cancel, MaskType.Black )
                                                                                                             .ConfigureAwait( false );
    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel, mask, page )
                                                                                                                            .ConfigureAwait( false );
    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, string? title, string cancel, MaskType mask, Page page )
    {
        if (func is null) { throw new ArgumentNullException( nameof(func) ); }

        using var cancelSrc = new CancellationTokenSource();

        ProgressDialogConfig config = new ProgressDialogConfig().SetTitle( title )
                                                                .SetIsDeterministic( false )
                                                                .SetMaskType( mask )
                                                                .SetCancel( cancel, cancelSrc.Cancel );

        using (_Prompts.Progress( config ))
        {
            try
            {
                await func( cancelSrc.Token )
                   .ConfigureAwait( false );
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                await _Prompts.HandleExceptionAsync( e, page, cancelSrc.Token )
                              .ConfigureAwait( false );
            }
        }
    }


    public ICommand LoadingCommand( Func<Task> func, Page     page ) => LoadingCommand( func,                           MaskType.Black, _Prompts.Cancel, page );
    public ICommand LoadingCommand( Func<Task> func, Page     page, string cancel ) => LoadingCommand( func,            MaskType.Black, cancel,          page );
    public ICommand LoadingCommand( Func<Task> func, MaskType mask, string cancel, Page page ) => LoadingCommand( func, null,           cancel,          mask, page );
    public ICommand LoadingCommand( Func<Task> func, string? title, string cancel, MaskType mask, Page page ) => new AsyncCommand( async () => await LoadingAsyncTask( func, title, cancel, mask, page )
                                                                                                                                                  .ConfigureAwait( false ) );


    public async Task LoadingAsyncTask( Func<Task> func, Page page ) => await LoadingAsyncTask( func, page, _Prompts.Cancel, MaskType.Black )
                                                                           .ConfigureAwait( false );
    public async Task LoadingAsyncTask( Func<Task> func, Page page, string cancel ) => await LoadingAsyncTask( func, page, cancel, MaskType.Black )
                                                                                          .ConfigureAwait( false );
    public async Task LoadingAsyncTask( Func<Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask( func, null, cancel, mask, page )
                                                                                                         .ConfigureAwait( false );
    public async Task LoadingAsyncTask( Func<Task> func, string? title, string cancel, MaskType mask, Page page )
    {
        if (func is null) { throw new ArgumentNullException( nameof(func) ); }

        using var cancelSrc = new CancellationTokenSource();

        ProgressDialogConfig config = new ProgressDialogConfig().SetTitle( title )
                                                                .SetIsDeterministic( false )
                                                                .SetMaskType( mask )
                                                                .SetCancel( cancel, cancelSrc.Cancel );

        using (_Prompts.Progress( config ))
        {
            try
            {
                await func()
                   .ConfigureAwait( false );
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                await _Prompts.HandleExceptionAsync( e, page, cancelSrc.Token )
                              .ConfigureAwait( false );
            }
        }
    }


    public ICommand LoadingCommand( Action func, Page page ) => LoadingCommand( func,                               page, _Prompts.Cancel, MaskType.Black );
    public ICommand LoadingCommand( Action func, Page page, string cancel ) => LoadingCommand( func,                page, cancel,          MaskType.Black );
    public ICommand LoadingCommand( Action func, Page page, string cancel, MaskType mask ) => LoadingCommand( func, page, null,            cancel, mask );
    public ICommand LoadingCommand( Action func, Page page, string? title, string cancel, MaskType mask ) => new AsyncCommand( async () => await LoadingAction( func, title, cancel, mask, page )
                                                                                                                                              .ConfigureAwait( false ) );
    public async Task LoadingAction( Action func, string? title, string cancel, MaskType mask, Page page )
    {
        if (func is null) { throw new ArgumentNullException( nameof(func) ); }

        using var cancelSrc = new CancellationTokenSource();

        ProgressDialogConfig config = new ProgressDialogConfig().SetTitle( title )
                                                                .SetIsDeterministic( false )
                                                                .SetMaskType( mask )
                                                                .SetCancel( cancel, cancelSrc.Cancel );

        using (_Prompts.Progress( config ))
        {
            try { func(); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                await _Prompts.HandleExceptionAsync( e, page, cancelSrc.Token )
                              .ConfigureAwait( false );
            }
        }
    }
}
