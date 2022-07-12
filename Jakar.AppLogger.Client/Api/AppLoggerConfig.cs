namespace Jakar.AppLogger.Client;


public sealed class AppLoggerConfig : IAppLoggerConfig
{
    private          Guid     _installID = Guid.Parse(Preferences.Get(nameof(InstallID), AppLoggerApi.EmptyGuid));
    private          Guid?    _sessionID = Guid.Parse(Preferences.Get(nameof(SessionID), AppLoggerApi.EmptyGuid));
    private          string   _appName   = Preferences.Get(nameof(AppName), string.Empty);
    private          string?  _userID    = Preferences.Get(nameof(UserID),  string.Empty);
    private readonly Device   _device    = Preferences.Get(nameof(Device),  string.Empty).FromJson<Device>();
    private          LogLevel _logLevel  = (LogLevel)Preferences.Get(nameof(LogLevel), LogLevel.Debug.AsInt());


    public DateTime AppLaunchTimestamp { get; init; } = DateTime.UtcNow;


    public LogLevel LogLevel
    {
        get => _logLevel;
        set
        {
            _logLevel = value;
            Preferences.Set(nameof(LogLevel), value.AsInt());
        }
    }

    public Device Device
    {
        get => _device;
        init
        {
            _device = value;
            Preferences.Set(nameof(Device), value.ToJson());
        }
    }

    public string? UserID
    {
        get => _userID;
        set
        {
            _userID = value;
            Preferences.Set(nameof(UserID), value);
        }
    }

    public string AppName
    {
        get => _appName;
        set
        {
            _appName = value;
            Preferences.Set(nameof(AppName), value);
        }
    }

    public Guid InstallID
    {
        get => _installID;
        set
        {
            _installID = value;
            Preferences.Set(nameof(InstallID), value.ToString());
        }
    }
    
    public Guid? SessionID
    {
        get => _sessionID;
        set
        {
            _sessionID = value;
            Preferences.Set(nameof(SessionID), value.ToString());
        }
    }


    internal AppLoggerConfig() { }
}
