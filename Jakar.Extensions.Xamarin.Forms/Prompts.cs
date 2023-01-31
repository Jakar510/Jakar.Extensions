#nullable enable
using Acr.UserDialogs;



namespace Jakar.Extensions.Xamarin.Forms;


public abstract class Prompts : IUserDialogs
{
    protected readonly          Debug        _debug;
    protected readonly          IAppSettings _services;
    protected readonly          IUserDialogs _dialogs = UserDialogs.Instance;
    protected internal abstract string       Cancel { get; }
    protected internal abstract string       No     { get; }
    protected internal abstract string       Ok     { get; }
    protected internal abstract string       Yes    { get; }


    protected Prompts( IAppSettings services, Debug debug )
    {
        _services = services;
        _debug    = debug;
    }


    public bool HandleException( Exception e )
    {
        Task.Run( async () => await InternalHandleExceptionAsync( e ) );
        return InternalHandleException( e );
    }


    /// <summary> switch the type of exception to show what ever prompt you want </summary>
    /// <param name="e"> </param>
    /// <returns> </returns>
    protected abstract bool InternalHandleException( Exception e );


    public abstract ValueTask HandleExceptionAsync( Exception                e, Page page, CancellationToken token );
    public abstract ValueTask HandleExceptionAsync<TFeedBackPage>( Exception e, Page page, CancellationToken token ) where TFeedBackPage : Page, new();


    protected virtual async ValueTask InternalHandleExceptionAsync( Exception e )
    {
        switch ( e )
        {
            case OperationCanceledException:
            case NameResolutionException:
            case RequestAbortedException:
            case TimeoutException:
                return;


            default:
                await _debug.HandleExceptionAsync( e );
                return;
        }
    }


    public virtual async ValueTask SendFeedBack<TFeedBackPage>( string? title, string? message, Page page, Exception e, FileSystemApi api, CancellationToken token = default ) where TFeedBackPage : Page, new()
    {
        await _debug.HandleExceptionAsync( e );

        if ( await ConfirmAsync( title, message, Yes, No, token ) )
        {
            _services.ScreenShotAddress = await api.GetScreenShot();
            await page.Navigation.PushAsync( new TFeedBackPage() );
        }
        else { _services.ScreenShotAddress = null; }
    }
    public async ValueTask SendFeedBack<TFeedBackPage>( string? title, string? message, Page page, Exception e, FileSystemApi api, Func<Exception, TFeedBackPage> func, CancellationToken token = default ) where TFeedBackPage : Page
    {
        await _debug.HandleExceptionAsync( e );

        if ( await ConfirmAsync( title, message, Yes, No, token ) )
        {
            _services.ScreenShotAddress = await api.GetScreenShot();
            await page.Navigation.PushAsync( func( e ) );
        }
        else { _services.ScreenShotAddress = null; }
    }


    public async ValueTask<bool> HandleExceptionAsync( Exception e, CancellationToken token ) => !token.IsCancellationRequested && await HandleExceptionAsync( e );
    public async ValueTask<bool> HandleExceptionAsync( Exception e )
    {
        await InternalHandleExceptionAsync( e );
        return InternalHandleException( e );
    }


    public void DebugMessage( Exception e, [CallerMemberName] string? caller = default )
    {
        if ( !_debug.CanDebug ) { return; }

        if ( !string.IsNullOrWhiteSpace( caller ) ) { caller = $"DEBUG: {caller}"; }

        //if ( !Debug.CanDebug )
        //{
        //	Alert(e.Message, caller);
        //	return;
        //}

        Alert( caller, e.ToString() );
    }



    #region IUserDialogs

    #region Toasts

    public IDisposable Toast( string?      title, TimeSpan? dismissTimer = null ) => _dialogs.Toast( title, dismissTimer );
    public IDisposable Toast( ToastConfig? cfg ) => _dialogs.Toast( cfg );

    #endregion



    #region Alerts

    protected void Alert( string?          title, string? message ) => _dialogs.Alert( message,             title, Ok );
    public IDisposable Alert( string?      title, string? message, string? ok ) => _dialogs.Alert( message, title, ok );
    public IDisposable Alert( AlertConfig? config ) => _dialogs.Alert( config );


    public Task AlertAsync( string? title, string? message, CancellationToken? cancelToken                            = default ) => AlertAsync( message, title, Ok, cancelToken );
    public Task AlertAsync( string? title, string? message, string?            okText, CancellationToken? cancelToken = default ) => _dialogs.AlertAsync( message, title, okText, cancelToken );

    public Task AlertAsync( AlertConfig? config, CancellationToken? cancelToken = default ) => _dialogs.AlertAsync( config, cancelToken );

    #endregion



    #region ActionSheets

    public IDisposable ActionSheet( ActionSheetConfig? config ) => _dialogs.ActionSheet( config );

    public Task<string> ActionSheetAsync( string? title, string? cancel, string? destructive, CancellationToken? cancelToken = null, params string[] buttons ) => _dialogs.ActionSheetAsync( title, cancel, destructive, cancelToken, buttons );

    #endregion



    #region Confirm

    public IDisposable Confirm( ConfirmConfig?     config ) => _dialogs.Confirm( config );
    public Task<bool> ConfirmAsync( ConfirmConfig? config, CancellationToken? cancelToken = null ) => _dialogs.ConfirmAsync( config, cancelToken );

    public async ValueTask<bool> ConfirmAsync( string? title, string? message, CancellationToken? cancelToken ) => await ConfirmAsync( message, title, Yes, No, cancelToken );

    public Task<bool> ConfirmAsync( string? title, string? message, string? yes, string? no, CancellationToken? cancelToken ) => _dialogs.ConfirmAsync( message, title, yes, no, cancelToken );

    #endregion



    #region Dates

    public IDisposable DatePrompt( DatePromptConfig?                 config ) => _dialogs.DatePrompt( config );
    public Task<DatePromptResult> DatePromptAsync( DatePromptConfig? config, CancellationToken? cancelToken = null ) => _dialogs.DatePromptAsync( config, cancelToken );

    public Task<DatePromptResult> DatePromptAsync( string? title = null, DateTime? selectedDate = null, CancellationToken? cancelToken = null ) =>
        _dialogs.DatePromptAsync( title, selectedDate, cancelToken );

    #endregion



    #region Times

    public IDisposable TimePrompt( TimePromptConfig?                 config ) => _dialogs.TimePrompt( config );
    public Task<TimePromptResult> TimePromptAsync( TimePromptConfig? config, CancellationToken? cancelToken = null ) => _dialogs.TimePromptAsync( config, cancelToken );

    public Task<TimePromptResult> TimePromptAsync( string? title = null, TimeSpan? selectedTime = null, CancellationToken? cancelToken = null ) =>
        _dialogs.TimePromptAsync( title, selectedTime, cancelToken );

    #endregion



    #region GenericPrompts

    public IDisposable Prompt( PromptConfig? config ) => _dialogs.Prompt( config );

    public Task<PromptResult> PromptAsync( string? message, string? title = null, string? okText = null, string? cancelText = null, string? placeholder = "", InputType inputType = InputType.Default, CancellationToken? cancelToken = null ) =>
        _dialogs.PromptAsync( message, title, okText, cancelText, placeholder, inputType, cancelToken );

    public Task<PromptResult> PromptAsync( PromptConfig? config, CancellationToken? cancelToken = null ) => _dialogs.PromptAsync( config, cancelToken );

    #endregion



    #region Login

    public IDisposable Login( LoginConfig?            config ) => _dialogs.Login( config );
    public Task<LoginResult> LoginAsync( string?      title = null, string?            message     = null, CancellationToken? cancelToken = null ) => _dialogs.LoginAsync( title, message, cancelToken );
    public Task<LoginResult> LoginAsync( LoginConfig? config,       CancellationToken? cancelToken = null ) => _dialogs.LoginAsync( config, cancelToken );

    #endregion



    #region Loading

    public void ShowLoading( string? title = null, MaskType? maskType = null ) => _dialogs.ShowLoading( title, maskType );
    public void HideLoading() => _dialogs.HideLoading();


    public IProgressDialog Progress( ProgressDialogConfig? config ) => _dialogs.Progress( config );

    public IProgressDialog Loading( string? title = null, Action? onCancel = null, string? cancelText = null, bool show = true, MaskType? maskType = null ) => _dialogs.Loading( title, onCancel, cancelText, show, maskType );

    public IProgressDialog Progress( string? title = null, Action? onCancel = null, string? cancelText = null, bool show = true, MaskType? maskType = null ) => _dialogs.Progress( title, onCancel, cancelText, show, maskType );

    #endregion

    #endregion
}
