// Jakar.Extensions :: Jakar.Extensions.Xamarin.Forms
// 09/08/2022  11:16 AM

namespace Jakar.AppLogger.Common;


[SuppressMessage( "ReSharper", "CollectionNeverUpdated.Global" )]
public abstract class LoggingSettings : ObservableClass, IScopeID, ISessionID, IAsyncDisposable
{
    public const           long           DEFAULT_FILE_SIZE_LIMIT_BYTES = 1L * 1024 * 1024 * 1024; // 1GB
    public const           string         DEFAULT_OUTPUT_TEMPLATE       = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public static readonly string         EmptyGuid                     = Guid.Empty.ToString();
    public const           string         INSTALL_ID                    = nameof(INSTALL_ID);
    private                bool           _enableAnalytics;
    private                bool           _enableApi;
    private                bool           _enableCrashes;
    private                bool           _includeAppStateOnError;
    private                bool           _includeDeviceInfoOnError;
    private                bool           _includeEventDetailsOnError;
    private                bool           _includeHwInfo;
    private                bool           _includeRequestsOnError;
    private                bool           _includeUserIDOnError;
    private                bool           _takeScreenshotOnError;
    private                Guid           _installID;
    private                Guid           _sessionID;
    private                Guid?          _scopeID;
    private                IScope?        _scope;
    private                LogLevel       _logLevel;
    private                string         _appName = string.Empty;
    private                string?        _userID;


    public                 DateTimeOffset AppLaunchTimestamp { get; init; } = DateTimeOffset.UtcNow;
    public string AppName
    {
        get => _appName;
        set => SetProperty( ref _appName, value );
    }
    public string? AppUserID
    {
        get => _userID;
        set => SetProperty( ref _userID, value );
    }
    [JsonIgnore] public ConcurrentBag<IAttachmentProvider> AttachmentProviders { get; init; } = new();
    public              DeviceDescriptor                   Device              { get; init; }
    public bool EnableAnalytics
    {
        get => _enableAnalytics;
        set => SetProperty( ref _enableAnalytics, value );
    }
    public bool EnableApi
    {
        get => _enableApi;
        set => SetProperty( ref _enableApi, value );
    }
    public bool EnableCrashes
    {
        get => _enableCrashes;
        set => SetProperty( ref _enableCrashes, value );
    }
    public bool IncludeAppStateOnError
    {
        get => _includeAppStateOnError;
        set => SetProperty( ref _includeAppStateOnError, value );
    }
    public bool IncludeDeviceInfoOnError
    {
        get => _includeDeviceInfoOnError;
        set => SetProperty( ref _includeDeviceInfoOnError, value );
    }
    public bool IncludeEventDetailsOnError
    {
        get => _includeEventDetailsOnError;
        set => SetProperty( ref _includeEventDetailsOnError, value );
    }
    public bool IncludeHwInfo
    {
        get => _includeHwInfo;
        set => SetProperty( ref _includeHwInfo, value );
    }
    public bool IncludeRequestsOnError
    {
        get => _includeRequestsOnError;
        set => SetProperty( ref _includeRequestsOnError, value );
    }
    public bool IncludeUserIDOnError
    {
        get => _includeUserIDOnError;
        set => SetProperty( ref _includeUserIDOnError, value );
    }
    public Guid InstallID
    {
        get => _installID;
        set => SetProperty( ref _installID, value );
    }
    public LogLevel LogLevel
    {
        get => _logLevel;
        set => SetProperty( ref _logLevel, value );
    }
    public Guid? ScopeID
    {
        get => _scopeID;
        set => SetProperty( ref _scopeID, value );
    }
    public Guid SessionID
    {
        get => _sessionID;
        set => SetProperty( ref _sessionID, value );
    }
    public bool TakeScreenshotOnError
    {
        get => _takeScreenshotOnError;
        set => SetProperty( ref _takeScreenshotOnError, value );
    }


    public AppVersion Version { get; init; }


    protected LoggingSettings( AppVersion version, DeviceDescriptor device )
    {
        Device  = device;
        Version = version;
    }


    /// <summary>
    ///     Must initialize the following:
    ///     <para>
    ///         <list type="bullet">
    ///             <item>
    ///                 <see cref="InstallID"/>
    ///             </item>
    ///             <item>
    ///                 <see cref="Device"/>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </summary>
    public abstract ValueTask InitAsync();


    protected sealed override bool SetProperty<T>( ref T backingStore, T value, [CallerMemberName] string? propertyName = null ) => base.SetProperty( ref backingStore, value, EqualityComparer<T>.Default, propertyName );
    protected sealed override bool SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null )
    {
        if ( propertyName is null ) { throw new ArgumentNullException( nameof(propertyName) ); }

        if ( !base.SetProperty( ref backingStore, value, comparer, propertyName ) ) { return false; }

        HandleValue( value, propertyName );
        return true;
    }
    protected abstract void HandleValue<T>( T value, string propertyName );


    public virtual IDictionary<string, JToken?>? GetAppState() => IncludeAppStateOnError
                                                                      ? new Dictionary<string, JToken?>
                                                                        {
                                                                            [nameof(AppName)]                      = AppName,
                                                                            [nameof(IDevice.DeviceID)]             = Device.DeviceID,
                                                                            [nameof(AppVersion)]                   = Device.AppVersion.ToString(),
                                                                            [nameof(LanguageApi.SelectedLanguage)] = CultureInfo.CurrentCulture.DisplayName,
                                                                            [nameof(DateTime)]                     = DateTimeOffset.UtcNow,
                                                                        }
                                                                      : default;
    public abstract ValueTask<byte[]?> TakeScreenshot();


    protected internal LoggingSettings SetScope( IScope scope )
    {
        _scope?.Dispose();
        _scope = scope;
        return this;
    }
    public IScope CreateScope()
    {
        _scope?.Dispose();
        return _scope = new AppLoggerScope( this );
    }
    public virtual async ValueTask DisposeAsync()
    {
        _scope?.Dispose();
        _scope = default;

        foreach ( IAttachmentProvider provider in AttachmentProviders ) { await provider.DisposeAsync(); }

        GC.SuppressFinalize( this );
    }
}
