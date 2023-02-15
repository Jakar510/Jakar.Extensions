#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
[SuppressMessage( "ReSharper", "MemberCanBeMadeStatic.Global" )]
[SuppressMessage( "ReSharper", "AsyncVoidLambda" )]
public class AppDebug : ObservableClass, ILogger
{
    protected readonly IAppSettings         _settings;
    protected readonly IFilePaths           _paths;
    private readonly   Synchronized<bool>   _apiEnabled = new(false);
    private readonly   Synchronized<Guid>   _installID  = new(Guid.Empty);
    private            DebugSettings?       _debugSettings;
    protected          ReadOnlyMemory<byte> _screenShot;


    public bool ApiDisabled => !ApiEnabled;
    public bool ApiEnabled
    {
        get => _apiEnabled;
        protected set
        {
            _apiEnabled.Value = value;
            OnPropertyChanged();
            OnPropertyChanged( nameof(ApiDisabled) );
        }
    }
    public virtual bool CanDebug      => Debugger.IsAttached;
    public virtual bool UseDebugLogin => CanDebug;
    public DebugSettings Settings
    {
        get
        {
            lock (this) { return _debugSettings ??= new DebugSettings(); }
        }
        set
        {
            lock (this) { SetProperty( ref _debugSettings, value ); }
        }
    }
    public Guid InstallID
    {
        get => _installID;
        protected set
        {
            _installID.Value = value;
            OnPropertyChanged();
        }
    }


    public AppDebug( IFilePaths paths, IAppSettings settings )
    {
        _paths    = paths;
        _settings = settings;
    }


    public static readonly ErrorAttachmentLog[] Empty = Array.Empty<ErrorAttachmentLog>();


    protected virtual Dictionary<string, string> AppState() => new()
                                                               {
                                                                   [nameof(IAppSettings.AppName)]         = _settings.AppName,
                                                                   [nameof(DateTime)]                     = DateTime.UtcNow.ToString( CultureInfo.InvariantCulture ),
                                                                   [nameof(AppDeviceInfo.DeviceId)]       = AppDeviceInfo.DeviceId,
                                                                   [nameof(AppDeviceInfo.VersionNumber)]  = AppDeviceInfo.VersionNumber,
                                                                   [nameof(LanguageApi.SelectedLanguage)] = CultureInfo.CurrentCulture.DisplayName
                                                               };
    protected virtual async ValueTask<EventDetails?> Handle_AppState( Exception e, ICollection<ErrorAttachmentLog> attachments, CancellationToken token = default )
    {
        if ( !Settings.IncludeAppStateOnError ) { return default; }


        await Handle_File( _paths.FeedBackFile, attachments, token );
        await Handle_File( _paths.AppStateFile, attachments, token );
        await Handle_File( _paths.DebugFile,    attachments, token );
        await Handle_File( _paths.IncomingFile, attachments, token );
        await Handle_File( _paths.OutgoingFile, attachments, token );
        await Handle_File( _paths.ZipFile,      attachments, token );


        var eventDetails = EventDetails.Create( e );
        attachments.Add( ErrorAttachmentLog.AttachmentWithText( eventDetails.ToPrettyJson(), nameof(EventDetails) ) );
        return eventDetails;
    }
    protected virtual void Handle_ExceptionDetails( Exception e, ICollection<ErrorAttachmentLog> attachments ) =>
        attachments.Add( ErrorAttachmentLog.AttachmentWithText( e.FullDetails()
                                                                 .ToPrettyJson(),
                                                                nameof(ExceptionDetails) ) );
    protected static async ValueTask Handle_File( LocalFile file, ICollection<ErrorAttachmentLog> attachments, CancellationToken token = default )
    {
        if ( file.DoesNotExist ) { return; }

        byte[] content = await file.ReadAsync()
                                   .AsBytes( token );

        attachments.Add( ErrorAttachmentLog.AttachmentWithBinary( content, file.Name, file.ContentType ) );
        file.Delete();
    }
    protected virtual async ValueTask Handle_ScreenShot( Exception e, ICollection<ErrorAttachmentLog> attachments, CancellationToken token = default )
    {
        if ( !Settings.TakeScreenshotOnError || _screenShot.IsEmpty ) { return; }

        attachments.Add( ErrorAttachmentLog.AttachmentWithBinary( _screenShot.ToArray(), "ScreenShot.jpeg", "image/jpeg" ) );

        if ( _settings.ScreenShotAddress is null || _settings.ScreenShotAddress.DoesNotExist ) { return; }

        await Handle_File( _settings.ScreenShotAddress, attachments, token );
    }


    public void HandleException( Exception           e ) => HandleException( e, Empty );
    public void HandleException( Exception           e, params ErrorAttachmentLog[] attachmentLogs ) => MainThread.BeginInvokeOnMainThread( async () => await HandleExceptionAsync( e, attachmentLogs ) );
    public ValueTask HandleExceptionAsync( Exception e ) => HandleExceptionAsync( e, Empty );
    public async ValueTask HandleExceptionAsync( Exception e, params ErrorAttachmentLog[] attachmentLogs )
    {
        if ( Settings.EnableApi ) { return; }

        if ( ApiDisabled ) { ThrowNotEnabled(); }

        _screenShot = Settings.TakeScreenshotOnError
                          ? await TakeScreenShot()
                          : default;

        await TrackError( e, attachmentLogs );
    }


    public void Init( string app_center_id, params Type[] appCenterServices ) => MainThread.BeginInvokeOnMainThread( async () => await InitAsync( app_center_id, appCenterServices ) );
    public async ValueTask InitAsync( string app_center_id, params Type[] appCenterServices )
    {
        try
        {
            VersionTracking.Track();
            AppCenter.Start( $"ios={app_center_id};android={app_center_id}", appCenterServices );

            Settings.LogLevel = CanDebug
                                    ? LogLevel.Debug
                                    : LogLevel.Info;


            Guid? id = await AppCenter.GetInstallIdAsync();

            if ( id is null )
            {
                id = Guid.NewGuid();
                AppCenter.SetUserId( id.ToString() );
            }

            _settings.DeviceID = InstallID = id.Value;
            ApiEnabled         = true;
        }
        catch ( Exception e )
        {
            e.WriteToConsole();
            Crashes.TrackError( e );
            throw;
        }
    }


    public async ValueTask SaveFeedBackAppState( Dictionary<string, string?> feedback, string key = "feedback" )
    {
        if ( ApiDisabled ) { ThrowNotEnabled(); }

        var result = new Dictionary<string, object?>
                     {
                         [key] = feedback
                     };

        if ( Settings.IncludeAppStateOnError ) { result[nameof(AppState)] = AppState(); }

        await _paths.FeedBackFile.WriteAsync( result.ToPrettyJson() );
    }


    [DoesNotReturn]
    [DebuggerHidden]
    protected void ThrowNotEnabled() => throw new ApiDisabledException( GetType()
                                                                           .Name );


    public async ValueTask TrackError( Exception e, params ErrorAttachmentLog[] attachmentLogs )
    {
        if ( ApiDisabled ) { ThrowNotEnabled(); }

        if ( !Settings.EnableCrashes ) { return; }

        var attachments = new List<ErrorAttachmentLog>( attachmentLogs.Length + 10 );
        attachments.AddRange( attachmentLogs );
        EventDetails? eventDetails = await Handle_AppState( e, attachments );
        Handle_ExceptionDetails( e, attachments );
        await Handle_ScreenShot( e, attachments );
        TrackError( e, eventDetails, attachments.ToArray() );
    }
    public void TrackError( Exception ex, EventDetails? eventDetails, params ErrorAttachmentLog[] attachments )
    {
        if ( ApiDisabled ) { ThrowNotEnabled(); }

        if ( !Settings.EnableCrashes ) { return; }

        Crashes.TrackError( ex, eventDetails, attachments );
    }
    public void TrackEvent( [CallerMemberName] string? source = default )
    {
        if ( ApiDisabled ) { ThrowNotEnabled(); }

        if ( !Settings.EnableAnalytics ) { return; }

        TrackEvent( AppState(), source );
    }
    protected void TrackEvent( Dictionary<string, string> eventDetails, [CallerMemberName] string? source = default )
    {
        if ( ApiDisabled ) { ThrowNotEnabled(); }

        if ( !Settings.EnableAnalytics ) { return; }

        Analytics.TrackEvent( source, eventDetails );
    }



    #region ScreenShots

    public async ValueTask BufferScreenShot() => _screenShot = await TakeScreenShot();
    public async ValueTask<LocalFile> GetScreenShot()
    {
        ReadOnlyMemory<byte> screenShot = await TakeScreenShot();
        return await WriteScreenShot( screenShot );
    }
    public static async ValueTask<ReadOnlyMemory<byte>> TakeScreenShot() => await MainThread.InvokeOnMainThreadAsync( CrossScreenshot.Current.CaptureAsync );
    public async ValueTask<LocalFile> WriteScreenShot( CancellationToken token = default ) => await WriteScreenShot( _screenShot.ToArray(), token );
    public async ValueTask<LocalFile> WriteScreenShot( ReadOnlyMemory<byte> screenShot, CancellationToken token = default )
    {
        LocalFile file = _paths.ScreenShot;
        await file.WriteAsync( screenShot, token );
        return file;
    }

    #endregion



    #region ILogger

    public void Log<TState>( MsLogLevel            logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter ) => Log( ConvertLevel( logLevel ), eventId, state, exception, formatter );
    public bool IsEnabled( MsLogLevel              logLevel ) => IsEnabled( ConvertLevel( logLevel ) );
    public IDisposable? BeginScope<TState>( TState state ) where TState : notnull => default;
    public bool IsEnabled( LogLevel                logLevel ) => logLevel != Settings.LogLevel;
    public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
    {
        if ( ApiDisabled ) { ThrowNotEnabled(); }

        if ( !IsEnabled( logLevel ) ) { return; }

        string? name = eventId.Name ?? state?.ToString();

        name = string.IsNullOrEmpty( name )
                   ? logLevel.ToString()
                   : $"{logLevel}_{name}";


        if ( exception is not null ) { HandleException( exception, ErrorAttachmentLog.AttachmentWithText( formatter( state, exception ), name ) ); }
        else
        {
            Analytics.TrackEvent( name,
                                  new Dictionary<string, string?>
                                  {
                                      [nameof(EventId)]  = eventId.ToJson(),
                                      ["State"]          = state?.ToString(),
                                      [nameof(LogLevel)] = logLevel.ToString()
                                  } );
        }
    }
    public static LogLevel ConvertLevel( MsLogLevel logLevel ) => logLevel switch
                                                                  {
                                                                      MsLogLevel.Trace       => LogLevel.Verbose,
                                                                      MsLogLevel.Debug       => LogLevel.Debug,
                                                                      MsLogLevel.Information => LogLevel.Info,
                                                                      MsLogLevel.Warning     => LogLevel.Warn,
                                                                      MsLogLevel.Error       => LogLevel.Error,
                                                                      MsLogLevel.Critical    => LogLevel.Error,
                                                                      MsLogLevel.None        => LogLevel.None,
                                                                      _                      => throw new OutOfRangeException( nameof(logLevel), logLevel )
                                                                  };

    #endregion
}



public sealed class EventDetails : Dictionary<string, string?>
{
    public EventDetails() { }
    public EventDetails( IDictionary<string, string?>               dictionary ) : base( dictionary ) { }
    public EventDetails( IDictionary<string, string?>               dictionary, IEqualityComparer<string>? comparer ) : base( dictionary, comparer ) { }
    public EventDetails( IEnumerable<KeyValuePair<string, string?>> collection ) : base( collection ) { }
    public EventDetails( IEnumerable<KeyValuePair<string, string?>> collection, IEqualityComparer<string>? comparer ) : base( collection, comparer ) { }
    public EventDetails( IEqualityComparer<string>?                 comparer ) : base( comparer ) { }
    public EventDetails( int                                        capacity ) : base( capacity ) { }
    public EventDetails( int                                        capacity, IEqualityComparer<string>? comparer ) : base( capacity, comparer ) { }


    public static EventDetails Create( Exception e )
    {
        e.Details( out Dictionary<string, string?> eventDetails );
        return new EventDetails( eventDetails );
    }
}
