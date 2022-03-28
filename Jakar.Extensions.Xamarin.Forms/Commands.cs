using System.Windows.Input;
using Acr.UserDialogs;


namespace Jakar.Extensions.Xamarin.Forms;


/// <summary>
/// Will move to Xamarin.Community.Toolkit.AsyncCommand when the following issues are resolved, as Xamarin.Forms 5.x is broken on Android until then:
/// [Bug] Linker issue with Xamarin.AndroidX.Core #13969: https://github.com/xamarin/Xamarin.Forms/issues/13969
/// [Bug] package androidx.activity.contextaware does not exist. JAVAC0000: androidx.activity.contextaware.OnContextAvailableListener #14042: https://github.com/xamarin/Xamarin.Forms/issues/14042
/// [Bug] android.content.res.Resources$NotFoundException: Unable to find resource ID #0xffffffff #13843: https://github.com/xamarin/Xamarin.Forms/issues/13843
/// [Bug] https://developercommunity.visualstudio.com/t/package-androidxactivitycontextaware-does-not-exis/1376876?from=email
/// </summary>
public class Commands<TDeviceID, TViewPage>
{
    protected Prompts<TDeviceID, TViewPage> _Prompts { get; set; }

    public Commands( Prompts<TDeviceID, TViewPage> prompts ) => _Prompts = prompts;


    public ICommand LoadingCommand( Func<CancellationToken, Task> func, Page     page )                           => LoadingCommand(func, MaskType.Black, _Prompts.Cancel, page);
    public ICommand LoadingCommand( Func<CancellationToken, Task> func, Page     page, string cancel )            => LoadingCommand(func, MaskType.Black, cancel, page);
    public ICommand LoadingCommand( Func<CancellationToken, Task> func, MaskType mask, string cancel, Page page ) => LoadingCommand(func, null, cancel, mask, page);
    public ICommand LoadingCommand( Func<CancellationToken, Task> func, string? title, string cancel, MaskType mask, Page page ) => new Command(async () => await LoadingAsyncTask(func,
                                                                                                                                                                                   title,
                                                                                                                                                                                   cancel,
                                                                                                                                                                                   mask,
                                                                                                                                                                                   page).ConfigureAwait(false));


    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page )                               => await LoadingAsyncTask(func, page, _Prompts.Cancel, MaskType.Black).ConfigureAwait(false);
    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page, string cancel )                => await LoadingAsyncTask(func, page, cancel, MaskType.Black).ConfigureAwait(false);
    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask(func, null, cancel, mask, page).ConfigureAwait(false);
    public async Task LoadingAsyncTask( Func<CancellationToken, Task> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) throw new ArgumentNullException(nameof(func));

        using var            cancelSrc = new CancellationTokenSource();
        ProgressDialogConfig config    = new ProgressDialogConfig().SetTitle(title).SetIsDeterministic(false).SetMaskType(mask).SetCancel(cancel, cancelSrc.Cancel);

        using ( _Prompts.Progress(config) )
        {
            try { await func(cancelSrc.Token).ConfigureAwait(false); }
            catch ( OperationCanceledException ) { }
            catch ( Exception e ) { await _Prompts.HandleExceptionAsync(e, page, cancelSrc.Token).ConfigureAwait(false); }
        }
    }


    public ICommand LoadingCommand( Func<Task> func, Page     page )                                           => LoadingCommand(func, MaskType.Black, _Prompts.Cancel, page);
    public ICommand LoadingCommand( Func<Task> func, Page     page,  string cancel )                           => LoadingCommand(func, MaskType.Black, cancel, page);
    public ICommand LoadingCommand( Func<Task> func, MaskType mask,  string cancel, Page     page )            => LoadingCommand(func, null, cancel, mask, page);
    public ICommand LoadingCommand( Func<Task> func, string?  title, string cancel, MaskType mask, Page page ) => new Command(async () => await LoadingAsyncTask(func, title, cancel, mask, page).ConfigureAwait(false));


    public async Task LoadingAsyncTask( Func<Task> func, Page page )                               => await LoadingAsyncTask(func, page, _Prompts.Cancel, MaskType.Black).ConfigureAwait(false);
    public async Task LoadingAsyncTask( Func<Task> func, Page page, string cancel )                => await LoadingAsyncTask(func, page, cancel, MaskType.Black).ConfigureAwait(false);
    public async Task LoadingAsyncTask( Func<Task> func, Page page, string cancel, MaskType mask ) => await LoadingAsyncTask(func, null, cancel, mask, page).ConfigureAwait(false);
    public async Task LoadingAsyncTask( Func<Task> func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) throw new ArgumentNullException(nameof(func));

        using var            cancelSrc = new CancellationTokenSource();
        ProgressDialogConfig config    = new ProgressDialogConfig().SetTitle(title).SetIsDeterministic(false).SetMaskType(mask).SetCancel(cancel, cancelSrc.Cancel);

        using ( _Prompts.Progress(config) )
        {
            try { await func().ConfigureAwait(false); }
            catch ( OperationCanceledException ) { }
            catch ( Exception e ) { await _Prompts.HandleExceptionAsync(e, page, cancelSrc.Token).ConfigureAwait(false); }
        }
    }


    public ICommand LoadingCommand( Action func, Page page )                => LoadingCommand(func, page, _Prompts.Cancel, MaskType.Black);
    public ICommand LoadingCommand( Action func, Page page, string cancel ) => LoadingCommand(func, page, cancel, MaskType.Black);
    public ICommand LoadingCommand( Action func, Page page, string cancel, MaskType mask ) => LoadingCommand(func,
                                                                                                             page,
                                                                                                             null,
                                                                                                             cancel,
                                                                                                             mask);
    public ICommand LoadingCommand( Action func, Page page, string? title, string cancel, MaskType mask ) => new Command(async () => await LoadingAction(func, title, cancel, mask, page).ConfigureAwait(false));
    public async Task LoadingAction( Action func, string? title, string cancel, MaskType mask, Page page )
    {
        if ( func is null ) throw new ArgumentNullException(nameof(func));

        using var            cancelSrc = new CancellationTokenSource();
        ProgressDialogConfig config    = new ProgressDialogConfig().SetTitle(title).SetIsDeterministic(false).SetMaskType(mask).SetCancel(cancel, cancelSrc.Cancel);

        using ( _Prompts.Progress(config) )
        {
            try { func(); }
            catch ( OperationCanceledException ) { }
            catch ( Exception e ) { await _Prompts.HandleExceptionAsync(e, page, cancelSrc.Token).ConfigureAwait(false); }
        }
    }
}
