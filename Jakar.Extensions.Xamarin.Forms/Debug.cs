using Jakar.Extensions.Xamarin.Forms.Statics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;





namespace Jakar.Extensions.Xamarin.Forms;


public class Debug<TDeviceID, TViewPage>
{
    public virtual bool  CanDebug      => Debugger.IsAttached;
    public virtual bool  UseDebugLogin => CanDebug;
    public         Guid? InstallId     { get; protected set; }

    protected bool _ApiEnabled { get; private set; }

    private BaseFileSystemApi?                  _fileSystemApi;
    private IAppSettings<TDeviceID, TViewPage>? _services;

    protected IAppSettings<TDeviceID, TViewPage> _Services
    {
        get => _services ?? throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services)));
        private set => _services = value;
    }


#region Init

    public Task Init( BaseFileSystemApi api, IAppSettings<TDeviceID, TViewPage> services, string app_center_id, params Type[] appCenterServices ) => Task.Run(async () => await InitAsync(api, services, app_center_id, appCenterServices));

    public async Task InitAsync( BaseFileSystemApi api, IAppSettings<TDeviceID, TViewPage> services, string app_center_id, params Type[] appCenterServices )
    {
        _fileSystemApi = api;
        _Services      = services;
        await StartAppCenterAsync(app_center_id, appCenterServices).ConfigureAwait(false);
    }

    public virtual async Task StartAppCenterAsync( string app_center_id, params Type[] services )
    {
        _ApiEnabled = true;

        VersionTracking.Track();

        AppCenter.Start($"ios={app_center_id};android={app_center_id}", services);

        AppCenter.LogLevel = CanDebug
                                 ? LogLevel.Verbose
                                 : LogLevel.Error; //AppCenter.LogLevel = LogLevel.Debug;

        InstallId =   await AppCenter.GetInstallIdAsync().ConfigureAwait(false);
        InstallId ??= Guid.NewGuid();

        AppCenter.SetUserId(InstallId.ToString());

        _Services.CrashDataPending = await Crashes.HasCrashedInLastSessionAsync().ConfigureAwait(false);
    }


    protected void ThrowIfNotEnabled()
    {
        if ( _ApiEnabled ) { return; }

        if ( _services is null ) { throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services))); }

        if ( _fileSystemApi is null ) { throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_fileSystemApi))); }

        throw new ApiDisabledException($"Must call {nameof(Init)} first.");
    }

#endregion


    /// <summary>
    /// </summary>
    /// <param name="e"></param>
    /// <returns><see cref="Task"/> from a <see cref="Task.Run(Action)"/> call</returns>
    public Task HandleException( Exception e ) => HandleException(e, CancellationToken.None);

    /// <summary>
    /// </summary>
    /// <param name="e"></param>
    /// <param name="token"></param>
    /// <returns><see cref="Task"/> from a <see cref="Task.Run(Action)"/> call</returns>
    public Task HandleException( Exception e, CancellationToken token ) => Task.Run(async () => await HandleExceptionAsync(e).ConfigureAwait(false), token);


    public async Task HandleExceptionAsync( Exception e )
    {
        ThrowIfNotEnabled();

        if ( !_Services.SendCrashes ) { return; }

        ReadOnlyMemory<byte> screenShot = await AppShare.TakeScreenShot().ConfigureAwait(false);

        await TrackError(e, screenShot).ConfigureAwait(false);
    }


#region AppStates

    protected async Task Save( ExceptionDetails payload )
    {
        if ( _fileSystemApi is null ) { throw new NullReferenceException(nameof(_fileSystemApi)); }

        using var file = new LocalFile(_fileSystemApi.AppStateFileName);
        await file.WriteToFileAsync(payload.ToPrettyJson()).ConfigureAwait(false);
    }

    protected async Task Save( Dictionary<string, object?> payload )
    {
        if ( _fileSystemApi is null ) { throw new NullReferenceException(nameof(_fileSystemApi)); }

        using var file = new LocalFile(_fileSystemApi.FeedBackFileName);
        await file.WriteToFileAsync(payload.ToPrettyJson()).ConfigureAwait(false);
    }

    public async Task SaveFeedBackAppState( Dictionary<string, string?> feedback, string key = "feedback" )
    {
        ThrowIfNotEnabled();

        var result = new Dictionary<string, object?>
                     {
                         [nameof(AppState)] = AppState(),
                         [key]              = feedback
                     };

        await Save(result).ConfigureAwait(false);
    }


    protected virtual Dictionary<string, string> AppState() => new()
                                                               {
                                                                   [nameof(IAppSettings<TDeviceID, TViewPage>.CurrentViewPage)] = _Services.CurrentViewPage?.ToString() ?? throw new NullReferenceException(nameof(_Services.CurrentViewPage)),
                                                                   [nameof(IAppSettings<TDeviceID, TViewPage>.AppName)]         = _Services.AppName ?? throw new NullReferenceException(nameof(_Services.AppName)),
                                                                   [nameof(DateTime)]                                           = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                                                                   [nameof(AppDeviceInfo.DeviceId)]                             = AppDeviceInfo.DeviceId,
                                                                   [nameof(AppDeviceInfo.VersionNumber)]                        = AppDeviceInfo.VersionNumber,
                                                                   [nameof(LanguageApi.SelectedLanguage)]                       = CultureInfo.CurrentCulture.DisplayName
                                                               };

#endregion


#region Track Exceptions

    public async Task TrackError( Exception e )
    {
        e.Details(out Dictionary<string, string?> eventDetails);
        await TrackError(e, eventDetails, e.FullDetails()).ConfigureAwait(false);
    }

    public async Task TrackError( Exception e, ReadOnlyMemory<byte> screenShot )
    {
        e.Details(out Dictionary<string, string?> dict);
        await TrackError(e, dict, new ExceptionDetails(e), screenShot).ConfigureAwait(false);
    }

    public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails ) => await TrackError(ex, eventDetails, exceptionDetails: null).ConfigureAwait(false);

    public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails, ExceptionDetails? exceptionDetails ) => await TrackError(ex,
                                                                                                                                                    eventDetails,
                                                                                                                                                    exceptionDetails,
                                                                                                                                                    null,
                                                                                                                                                    null).ConfigureAwait(false);

    public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails, ExceptionDetails? exceptionDetails, ReadOnlyMemory<byte> screenShot ) => await TrackError(ex,
                                                                                                                                                                                     eventDetails,
                                                                                                                                                                                     exceptionDetails,
                                                                                                                                                                                     null,
                                                                                                                                                                                     null,
                                                                                                                                                                                     screenShot).ConfigureAwait(false);

    public async Task TrackError( Exception                    ex,
                                  Dictionary<string, string?>? eventDetails,
                                  ExceptionDetails?            exceptionDetails,
                                  string?                      incomingText,
                                  string?                      outgoingText
    ) => await TrackError(ex,
                          eventDetails,
                          exceptionDetails,
                          incomingText,
                          outgoingText,
                          null).ConfigureAwait(false);

    public async Task TrackError( Exception                    ex,
                                  Dictionary<string, string?>? eventDetails,
                                  ExceptionDetails?            exceptionDetails,
                                  string?                      incomingText,
                                  string?                      outgoingText,
                                  ReadOnlyMemory<byte>?        screenShot
    )
    {
        ThrowIfNotEnabled();

        if ( !_Services.SendCrashes ) { return; }

        if ( exceptionDetails is not null ) await Save(exceptionDetails).ConfigureAwait(false);

        var attachments = new List<ErrorAttachmentLog>(5);

        if ( exceptionDetails is not null )
        {
            ErrorAttachmentLog? state = ErrorAttachmentLog.AttachmentWithText(exceptionDetails.ToPrettyJson(), _fileSystemApi!.AppStateFileName);
            attachments.Add(state);
        }

        if ( eventDetails is not null )
        {
            ErrorAttachmentLog? debug = ErrorAttachmentLog.AttachmentWithText(eventDetails.ToPrettyJson(), _fileSystemApi!.DebugFileName);
            attachments.Add(debug);
        }

        if ( screenShot is not null )
        {
            ErrorAttachmentLog? screenShotAttachment = ErrorAttachmentLog.AttachmentWithBinary(( (ReadOnlyMemory<byte>)screenShot ).ToArray(), "ScreenShot.jpeg", "image/jpeg");
            attachments.Add(screenShotAttachment);
        }

        if ( !string.IsNullOrWhiteSpace(incomingText) )
        {
            ErrorAttachmentLog incoming = ErrorAttachmentLog.AttachmentWithText(incomingText.ToPrettyJson(), _fileSystemApi!.IncomingFileName);
            attachments.Add(incoming);
        }

        if ( !string.IsNullOrWhiteSpace(outgoingText) )
        {
            ErrorAttachmentLog outgoing = ErrorAttachmentLog.AttachmentWithText(outgoingText.ToPrettyJson(), _fileSystemApi!.OutgoingFileName);
            attachments.Add(outgoing);
        }

        TrackError(ex, eventDetails, attachments.ToArray());
    }

    public void TrackError( Exception ex, Dictionary<string, string?>? eventDetails, params ErrorAttachmentLog[] attachments )
    {
        ThrowIfNotEnabled();

        if ( !_Services.SendCrashes ) { return; }

        if ( ex is null ) throw new ArgumentNullException(nameof(ex));

        Crashes.TrackError(ex, eventDetails, attachments);
    }

#endregion


#region Track Events

    public void TrackEvent( [CallerMemberName] string source = "" )
    {
        ThrowIfNotEnabled();

        if ( !_Services.SendCrashes ) { return; }

        TrackEvent(AppState(), source);
    }

    protected void TrackEvent( Dictionary<string, string> eventDetails, [CallerMemberName] string source = "" )
    {
        ThrowIfNotEnabled();

        if ( !_Services.SendCrashes ) { return; }

        Analytics.TrackEvent(source, eventDetails);
    }

#endregion
}
