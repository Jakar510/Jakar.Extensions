using Jakar.Extensions.Xamarin.Forms.Statics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;



namespace Jakar.Extensions.Xamarin.Forms;


public class Debug : ObservableClass
{
    private readonly BaseFileSystemApi? _fileSystemApi;
    private readonly IAppSettings       _settings;
    private          Guid               _installID;
    private readonly Synchronized<bool> _apiEnabled = new(false);


    protected readonly string _appName;
    public virtual     bool   CanDebug      => Debugger.IsAttached;
    public virtual     bool   UseDebugLogin => CanDebug;
    public             bool   SendCrashes   => _settings.SendCrashes;


    public Guid InstallID
    {
        get => _installID;
        protected set => SetProperty(ref _installID, value);
    }

    public bool ApiEnabled
    {
        get => _apiEnabled;
        protected internal set
        {
            _apiEnabled.Value = value;
            OnPropertyChanged();
        }
    }


    public Debug( BaseFileSystemApi api, IAppSettings settings, string appName )
    {
        _fileSystemApi = api;
        _settings      = settings;
        _appName       = appName;
    }


    public async Task InitAsync( string app_center_id, params Type[] appCenterServices )
    {
        try
        {
            VersionTracking.Track();
            AppCenter.Start($"ios={app_center_id};android={app_center_id}", appCenterServices);

            AppCenter.LogLevel = CanDebug
                                     ? LogLevel.Verbose
                                     : LogLevel.Error; //AppCenter.LogLevel = LogLevel.Debug;


            Guid? id = await AppCenter.GetInstallIdAsync().ConfigureAwait(false);

            if ( id is null )
            {
                id = Guid.NewGuid();
                AppCenter.SetUserId(id.ToString());
            }

            InstallID                  = id.Value;
            _settings.DeviceID         = InstallID;
            _settings.CrashDataPending = await Crashes.HasCrashedInLastSessionAsync().ConfigureAwait(false);
            ApiEnabled                 = true;
        }
        catch ( Exception e )
        {
            e.WriteToDebug();
            throw;
        }
    }


    protected void ThrowIfNotEnabled()
    {
        if ( ApiEnabled ) { return; }

        // if ( _services is null ) { throw new ApiDisabledException($"Must call {nameof(InitAsync)} first.", new NullReferenceException(nameof(_services))); }

        if ( _fileSystemApi is null ) { throw new ApiDisabledException($"Must call {nameof(InitAsync)} first.", new NullReferenceException(nameof(_fileSystemApi))); }

        throw new ApiDisabledException($"Must call {nameof(InitAsync)} first.");
    }


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

        if ( !SendCrashes ) { return; }

        ReadOnlyMemory<byte> screenShot = await AppShare.TakeScreenShot().ConfigureAwait(false);

        await TrackError(e, screenShot).ConfigureAwait(false);
    }



    #region AppStates

    protected async Task Save( ExceptionDetails payload )
    {
        if ( _fileSystemApi is null ) { throw new NullReferenceException(nameof(_fileSystemApi)); }

        using var file = new LocalFile(_fileSystemApi.AppStateFileName);
        await file.WriteAsync(payload.ToPrettyJson()).ConfigureAwait(false);
    }

    protected async Task Save( Dictionary<string, object?> payload )
    {
        if ( _fileSystemApi is null ) { throw new NullReferenceException(nameof(_fileSystemApi)); }

        using var file = new LocalFile(_fileSystemApi.FeedBackFileName);
        await file.WriteAsync(payload.ToPrettyJson()).ConfigureAwait(false);
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
                                                                   [nameof(_appName)]                     = _appName ?? throw new NullReferenceException(nameof(_appName)),
                                                                   [nameof(DateTime)]                     = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                                                                   [nameof(AppDeviceInfo.DeviceId)]       = AppDeviceInfo.DeviceId,
                                                                   [nameof(AppDeviceInfo.VersionNumber)]  = AppDeviceInfo.VersionNumber,
                                                                   [nameof(LanguageApi.SelectedLanguage)] = CultureInfo.CurrentCulture.DisplayName
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
    public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails ) => await TrackError(ex,                                     eventDetails, exceptionDetails: null).ConfigureAwait(false);
    public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails, ExceptionDetails? exceptionDetails ) => await TrackError(ex, eventDetails, exceptionDetails, null, null).ConfigureAwait(false);
    public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails, ExceptionDetails? exceptionDetails, ReadOnlyMemory<byte> screenShot ) =>
        await TrackError(ex, eventDetails, exceptionDetails, null, null, screenShot).ConfigureAwait(false);
    public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails, ExceptionDetails? exceptionDetails, string? incomingText, string? outgoingText ) =>
        await TrackError(ex, eventDetails, exceptionDetails, incomingText, outgoingText, null).ConfigureAwait(false);

    public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails, ExceptionDetails? exceptionDetails, string? incomingText, string? outgoingText, ReadOnlyMemory<byte>? screenShot )
    {
        ThrowIfNotEnabled();

        if ( !SendCrashes ) { return; }

        if ( exceptionDetails is not null ) { await Save(exceptionDetails).ConfigureAwait(false); }

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

        if ( !SendCrashes ) { return; }

        if ( ex is null ) { throw new ArgumentNullException(nameof(ex)); }

        Crashes.TrackError(ex, eventDetails, attachments);
    }

    #endregion



    #region Track Events

    public void TrackEvent( [CallerMemberName] string? source = default )
    {
        ThrowIfNotEnabled();

        if ( !SendCrashes ) { return; }

        TrackEvent(AppState(), source);
    }

    protected void TrackEvent( Dictionary<string, string> eventDetails, [CallerMemberName] string? source = default )
    {
        ThrowIfNotEnabled();

        if ( !SendCrashes ) { return; }

        Analytics.TrackEvent(source, eventDetails);
    }

    #endregion
}
