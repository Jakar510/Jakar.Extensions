#nullable enable
using Acr.UserDialogs;



namespace Jakar.Extensions.Xamarin.Forms;


public abstract class Prompts : IUserDialogs
{
    protected readonly          AppDebug     _debug;
    protected readonly          IAppSettings _settings;
    protected readonly          IUserDialogs _dialogs = UserDialogs.Instance;
    protected internal abstract string       Cancel { get; }
    protected internal abstract string       No     { get; }
    protected internal abstract string       Ok     { get; }
    protected internal abstract string       Yes    { get; }


    protected Prompts( IAppSettings services, AppDebug debug )
    {
        _settings = services;
        _debug    = debug;
    }


    public bool HandleException( Exception e )
    {
        Task.Run( async () => await InternalHandleExceptionAsync( e ) );
        return InternalHandleException( e );
    }


    public async ValueTask<bool> HandleExceptionAsync( Exception e )
    {
        await InternalHandleExceptionAsync( e );
        return InternalHandleException( e );
    }
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


    /// <summary> switch the type of exception to show what ever prompt you want </summary>
    /// <param name="e"> </param>
    /// <returns> </returns>
    protected abstract bool InternalHandleException( Exception e );


    public abstract ValueTask HandleExceptionAsync( Exception                e, Page page, CancellationToken token );
    public abstract ValueTask HandleExceptionAsync<TFeedBackPage>( Exception e, Page page, CancellationToken token ) where TFeedBackPage : Page, new();
    public virtual async ValueTask HandleExceptionAsync<TFeedBackPage>( Exception e, Page page, Func<Exception, TFeedBackPage> func ) where TFeedBackPage : Page
    {
        if ( await HandleExceptionAsync( e ) ) { await page.Navigation.PushAsync( func( e ), true ); }
    }


    public virtual async ValueTask SendFeedBack<TFeedBackPage>( string? title, string? message, Page page, Exception e, CancellationToken token = default ) where TFeedBackPage : Page, new()
    {
        await _debug.HandleExceptionAsync( e );

        if ( await ConfirmAsync( title, message, Yes, No, token ) )
        {
            _settings.ScreenShotAddress = await _debug.GetScreenShot();
            await page.Navigation.PushAsync( new TFeedBackPage() );
        }
        else { _settings.ScreenShotAddress = null; }
    }
    public async ValueTask SendFeedBack<TFeedBackPage>( string? title, string? message, Page page, Exception e, Func<Exception, TFeedBackPage> func, CancellationToken token = default ) where TFeedBackPage : Page
    {
        await _debug.HandleExceptionAsync( e );

        if ( await ConfirmAsync( title, message, Yes, No, token ) )
        {
            _settings.ScreenShotAddress = await _debug.GetScreenShot();
            await page.Navigation.PushAsync( func( e ) );
        }
        else { _settings.ScreenShotAddress = null; }
    }


    public void DebugMessage( Exception e, [CallerMemberName] string? caller = default )
    {
        if ( !_debug.CanDebug ) { return; }

        Alert( $"DEBUG: {caller}", e.ToString() );
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


    public Task AlertAsync( string? title, string? message, CancellationToken? token                            = default ) => AlertAsync( message, title, Ok, token );
    public Task AlertAsync( string? title, string? message, string?            okText, CancellationToken? token = default ) => _dialogs.AlertAsync( message, title, okText, token );

    public Task AlertAsync( AlertConfig? config, CancellationToken? token = default ) => _dialogs.AlertAsync( config, token );

    #endregion



    #region ActionSheets

    public IDisposable ActionSheet( ActionSheetConfig? config ) => _dialogs.ActionSheet( config );

    public Task<string> ActionSheetAsync( string? title, string? cancel, string? destructive, CancellationToken? token = null, params string[] buttons ) => _dialogs.ActionSheetAsync( title, cancel, destructive, token, buttons );

    #endregion



    #region Confirm

    public IDisposable Confirm( ConfirmConfig?         config ) => _dialogs.Confirm( config );
    public Task<bool> ConfirmAsync( ConfirmConfig?     config, CancellationToken? token = null ) => _dialogs.ConfirmAsync( config, token );
    public async ValueTask<bool> ConfirmAsync( string? title,  string?            message, CancellationToken? token ) => await ConfirmAsync( message, title, Yes, No, token );
    public Task<bool> ConfirmAsync( string?            title,  string?            message, string?            yes, string? no, CancellationToken? token ) => _dialogs.ConfirmAsync( message, title, yes, no, token );

    #endregion



    #region Dates

    public IDisposable DatePrompt( DatePromptConfig?                 config ) => _dialogs.DatePrompt( config );
    public Task<DatePromptResult> DatePromptAsync( DatePromptConfig? config, CancellationToken? token = null ) => _dialogs.DatePromptAsync( config, token );

    public Task<DatePromptResult> DatePromptAsync( string? title = null, DateTime? selectedDate = null, CancellationToken? token = null ) =>
        _dialogs.DatePromptAsync( title, selectedDate, token );

    #endregion



    #region Times

    public IDisposable TimePrompt( TimePromptConfig?                 config ) => _dialogs.TimePrompt( config );
    public Task<TimePromptResult> TimePromptAsync( TimePromptConfig? config, CancellationToken? token = null ) => _dialogs.TimePromptAsync( config, token );

    public Task<TimePromptResult> TimePromptAsync( string? title = null, TimeSpan? selectedTime = null, CancellationToken? token = null ) =>
        _dialogs.TimePromptAsync( title, selectedTime, token );

    #endregion



    #region GenericPrompts

    public IDisposable Prompt( PromptConfig? config ) => _dialogs.Prompt( config );

    public Task<PromptResult> PromptAsync( string? message, string? title = null, string? okText = null, string? cancelText = null, string? placeholder = "", InputType inputType = InputType.Default, CancellationToken? token = null ) =>
        _dialogs.PromptAsync( message, title, okText, cancelText, placeholder, inputType, token );

    public Task<PromptResult> PromptAsync( PromptConfig? config, CancellationToken? token = null ) => _dialogs.PromptAsync( config, token );

    #endregion



    #region Login

    public IDisposable Login( LoginConfig?            config ) => _dialogs.Login( config );
    public Task<LoginResult> LoginAsync( string?      title = null, string?            message = null, CancellationToken? token = null ) => _dialogs.LoginAsync( title, message, token );
    public Task<LoginResult> LoginAsync( LoginConfig? config,       CancellationToken? token   = null ) => _dialogs.LoginAsync( config, token );

    #endregion



    #region Loading

    public void ShowLoading( string? title = null, MaskType? maskType = null ) => _dialogs.ShowLoading( title, maskType );
    public void HideLoading() => _dialogs.HideLoading();


    public IProgressDialog Progress( ProgressDialogConfig? config ) => _dialogs.Progress( config );
    public IProgressDialog Loading( string?                title = null, Action? onCancel = null, string? cancelText = null, bool show = true, MaskType? maskType = null ) => _dialogs.Loading( title, onCancel, cancelText, show, maskType );
    public IProgressDialog Progress( string?               title = null, Action? onCancel = null, string? cancelText = null, bool show = true, MaskType? maskType = null ) => _dialogs.Progress( title, onCancel, cancelText, show, maskType );

    #endregion

    #endregion
}
