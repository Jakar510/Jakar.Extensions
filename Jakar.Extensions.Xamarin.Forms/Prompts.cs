using Acr.UserDialogs;
using Jakar.Extensions.Xamarin.Forms.Statics;



namespace Jakar.Extensions.Xamarin.Forms;


public abstract class Prompts<TViewPage> : IUserDialogs where TViewPage : struct, Enum
{
    private IAppSettings? _services;

    protected IAppSettings _Services
    {
        get => _services ?? throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services)));
        private set => _services = value;
    }

    private Debug<TViewPage>? _debug;

    protected Debug<TViewPage> _Debug
    {
        get => _debug ?? throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services)));
        private set => _debug = value;
    }

    protected internal abstract string Cancel { get; }
    protected internal abstract string Ok     { get; }
    protected internal abstract string Yes    { get; }
    protected internal abstract string No     { get; }


    private IUserDialogs _Dialogs { get; } = UserDialogs.Instance;

    public void Init( IAppSettings     services ) => _Services = services;
    public void Init( Debug<TViewPage> services ) => _Debug = services;


    public abstract Task HandleExceptionAsync( Exception                e, Page page, CancellationToken token );
    public abstract Task HandleExceptionAsync<TFeedBackPage>( Exception e, Page page, CancellationToken token ) where TFeedBackPage : Page, new();


    public async Task SendFeedBack<TFeedBackPage>( string? title, string? message, Page page, Exception e, FileSystemApi api, CancellationToken token = default ) where TFeedBackPage : Page, new() =>
        await SendFeedBack<TFeedBackPage>(title, message, Yes, No, page, e, api, token).ConfigureAwait(false);

    public async Task SendFeedBack<TFeedBackPage>( string? title, string? message, string? yes, string? no, Page page, Exception e, FileSystemApi api, CancellationToken token = default ) where TFeedBackPage : Page, new()
    {
        if ( page is null ) throw new ArgumentNullException(nameof(page));
        if ( e is null ) throw new ArgumentNullException(nameof(e));

        await _Debug.HandleExceptionAsync(e).ConfigureAwait(false);

        if ( await ConfirmAsync(title, message, yes, no, token).ConfigureAwait(false) )
        {
            _Services.ScreenShotAddress = await api.GetScreenShot().ConfigureAwait(false);

            await page.Navigation.PushAsync(new TFeedBackPage()).ConfigureAwait(false);
        }
        else { _Services.ScreenShotAddress = null; }
    }


    public async Task<bool> HandleExceptionAsync( Exception e, CancellationToken token )
    {
        if ( token.IsCancellationRequested ) { return false; }

        return await HandleExceptionAsync(e).ConfigureAwait(false);
    }

    public async Task<bool> HandleExceptionAsync( Exception e )
    {
        await InternalHandleExceptionAsync(e).ConfigureAwait(false);

        return InternalHandleException(e);
    }

    public bool HandleException( Exception e )
    {
        Task.Run(async () => await InternalHandleExceptionAsync(e).ConfigureAwait(false));

        return InternalHandleException(e);
    }


    protected virtual async Task InternalHandleExceptionAsync( Exception e )
    {
        switch ( e )
        {
            case null: throw new ArgumentNullException(nameof(e));

            case OperationCanceledException:
            case NameResolutionException:
            case RequestAbortedException:
            case TimeoutException:
                return;


            default:
                await _Debug.HandleExceptionAsync(e).ConfigureAwait(false);
                return;
        }
    }


    /// <summary>
    /// switch the type of exception to show what ever prompt you want
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected abstract bool InternalHandleException( Exception e );


    public void DebugMessage( Exception e, [CallerMemberName] string caller = "" )
    {
        if ( !_Debug.CanDebug ) { return; }

        if ( !string.IsNullOrWhiteSpace(caller) ) { caller = $"DEBUG: {caller}"; }

        //if ( !Debug.CanDebug )
        //{
        //	Alert(e.Message, caller);
        //	return;
        //}

        Alert(caller, e.ToString());
    }



    #region IUserDialogs

    #region Toasts

    public IDisposable Toast( string?      title, TimeSpan? dismissTimer = null ) => _Dialogs.Toast(title, dismissTimer);
    public IDisposable Toast( ToastConfig? cfg ) => _Dialogs.Toast(cfg);

    #endregion



    #region Alerts

    protected void Alert( string?          title, string? message ) => _Dialogs.Alert(message,             title, Ok);
    public IDisposable Alert( string?      title, string? message, string? ok ) => _Dialogs.Alert(message, title, ok);
    public IDisposable Alert( AlertConfig? config ) => _Dialogs.Alert(config);


    public Task AlertAsync( string? title, string? message, CancellationToken? cancelToken                            = default ) => AlertAsync(message, title, Ok, cancelToken);
    public Task AlertAsync( string? title, string? message, string?            okText, CancellationToken? cancelToken = default ) => _Dialogs.AlertAsync(message, title, okText, cancelToken);

    public Task AlertAsync( AlertConfig? config, CancellationToken? cancelToken = default ) => _Dialogs.AlertAsync(config, cancelToken);

    #endregion



    #region ActionSheets

    public IDisposable ActionSheet( ActionSheetConfig? config ) => _Dialogs.ActionSheet(config);

    public Task<string> ActionSheetAsync( string? title, string? cancel, string? destructive, CancellationToken? cancelToken = null, params string[] buttons ) => _Dialogs.ActionSheetAsync(title, cancel, destructive, cancelToken, buttons);

    #endregion



    #region Confirm

    public IDisposable Confirm( ConfirmConfig?     config ) => _Dialogs.Confirm(config);
    public Task<bool> ConfirmAsync( ConfirmConfig? config, CancellationToken? cancelToken = null ) => _Dialogs.ConfirmAsync(config, cancelToken);

    public async Task<bool> ConfirmAsync( string? title, string? message, CancellationToken? cancelToken ) => await ConfirmAsync(message, title, Yes, No, cancelToken).ConfigureAwait(false);

    public Task<bool> ConfirmAsync( string? title, string? message, string? yes, string? no, CancellationToken? cancelToken ) => _Dialogs.ConfirmAsync(message, title, yes, no, cancelToken);

    #endregion



    #region Dates

    public IDisposable DatePrompt( DatePromptConfig?                 config ) => _Dialogs.DatePrompt(config);
    public Task<DatePromptResult> DatePromptAsync( DatePromptConfig? config, CancellationToken? cancelToken = null ) => _Dialogs.DatePromptAsync(config, cancelToken);

    public Task<DatePromptResult> DatePromptAsync( string? title = null, DateTime? selectedDate = null, CancellationToken? cancelToken = null ) =>
        _Dialogs.DatePromptAsync(title, selectedDate, cancelToken);

    #endregion



    #region Times

    public IDisposable TimePrompt( TimePromptConfig?                 config ) => _Dialogs.TimePrompt(config);
    public Task<TimePromptResult> TimePromptAsync( TimePromptConfig? config, CancellationToken? cancelToken = null ) => _Dialogs.TimePromptAsync(config, cancelToken);

    public Task<TimePromptResult> TimePromptAsync( string? title = null, TimeSpan? selectedTime = null, CancellationToken? cancelToken = null ) =>
        _Dialogs.TimePromptAsync(title, selectedTime, cancelToken);

    #endregion



    #region GenericPrompts

    public IDisposable Prompt( PromptConfig? config ) => _Dialogs.Prompt(config);

    public Task<PromptResult> PromptAsync( string? message, string? title = null, string? okText = null, string? cancelText = null, string? placeholder = "", InputType inputType = InputType.Default, CancellationToken? cancelToken = null ) =>
        _Dialogs.PromptAsync(message, title, okText, cancelText, placeholder, inputType, cancelToken);

    public Task<PromptResult> PromptAsync( PromptConfig? config, CancellationToken? cancelToken = null ) => _Dialogs.PromptAsync(config, cancelToken);

    #endregion



    #region Login

    public IDisposable Login( LoginConfig?            config ) => _Dialogs.Login(config);
    public Task<LoginResult> LoginAsync( string?      title = null, string?            message     = null, CancellationToken? cancelToken = null ) => _Dialogs.LoginAsync(title, message, cancelToken);
    public Task<LoginResult> LoginAsync( LoginConfig? config,       CancellationToken? cancelToken = null ) => _Dialogs.LoginAsync(config, cancelToken);

    #endregion



    #region Loading

    public void ShowLoading( string? title = null, MaskType? maskType = null ) => _Dialogs.ShowLoading(title, maskType);
    public void HideLoading() => _Dialogs.HideLoading();


    public IProgressDialog Progress( ProgressDialogConfig? config ) => _Dialogs.Progress(config);

    public IProgressDialog Loading( string? title = null, Action? onCancel = null, string? cancelText = null, bool show = true, MaskType? maskType = null ) => _Dialogs.Loading(title, onCancel, cancelText, show, maskType);

    public IProgressDialog Progress( string? title = null, Action? onCancel = null, string? cancelText = null, bool show = true, MaskType? maskType = null ) => _Dialogs.Progress(title, onCancel, cancelText, show, maskType);

    #endregion

    #endregion
}
