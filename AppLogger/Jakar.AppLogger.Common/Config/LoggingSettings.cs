// Jakar.Extensions :: Jakar.Extensions.Xamarin.Forms
// 09/08/2022  11:16 AM

namespace Jakar.AppLogger.Common;


[SuppressMessage( "ReSharper", "CollectionNeverUpdated.Global" )]
public abstract class LoggingSettings : ObservableClass, ISessionID, IAsyncDisposable
{
    public const           string                             DEFAULT_OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public static readonly string                             EmptyGuid               = Guid.Empty.ToString();
    private readonly       ConcurrentDictionary<Guid, IScope> _scopes                 = new();


    private bool               _enableAnalytics;
    private bool               _enableApi;
    private bool               _enableCrashes;
    private bool               _includeAppStateOnError;
    private bool               _includeDeviceInfoOnError;
    private bool               _includeEventDetailsOnError;
    private bool               _includeHwInfo;
    private bool               _includeRequestsOnError;
    private bool               _includeUserIDOnError;
    private bool               _takeScreenshotOnError;
    private Guid               _installID;
    private LogLevel           _logLevel;
    private StartSessionReply? _session;
    private string             _appName = string.Empty;
    private string?            _userID;


    public DateTimeOffset AppLaunchTimeStamp { get; init; } = DateTimeOffset.UtcNow;
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
    public DeviceDescriptor Device { get; init; }
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
    [JsonIgnore] public ConcurrentBag<ILoggerAttachmentProvider> LoggerAttachmentProviders { get; init; } = new();
    public LogLevel LogLevel
    {
        get => _logLevel;
        set => SetProperty( ref _logLevel, value );
    }
    public HashSet<Guid> ScopeIDs { get; init; } = new();
    public StartSessionReply? Session
    {
        get => _session;
        set => SetProperty( ref _session, value );
    }
    Guid? ISessionID.SessionID => Session?.SessionID;
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
    ///         <list type="bullet"> <item> <see cref="InstallID"/> </item> <item> <see cref="Device"/> </item> </list>
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
                                                                            [nameof(DateTime)]                     = DateTimeOffset.UtcNow
                                                                        }
                                                                      : default;
    public abstract ValueTask<byte[]?> TakeScreenshot();


    public T AddScope<T>( in T scope ) where T : IScope
    {
        _scopes.TryAdd( scope.ScopeID, scope );
        return scope;
    }
    public bool RemoveScope<T>( in T                          scope ) where T : IScope => _scopes.TryRemove( scope.ScopeID, out _ );
    public AppLoggerScope<TState> CreateScope<TState>( TState state ) => AddScope( new AppLoggerScope<TState>( this, state ) );
    public virtual async ValueTask DisposeAsync()
    {
        foreach ( IScope? scope in _scopes.Values ) { scope.Dispose(); }

        _scopes.Clear();
        foreach ( ILoggerAttachmentProvider provider in LoggerAttachmentProviders ) { await provider.DisposeAsync(); }

        GC.SuppressFinalize( this );
    }
}
