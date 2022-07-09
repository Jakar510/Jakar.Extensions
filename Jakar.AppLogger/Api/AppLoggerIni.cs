// Jakar.Extensions :: Jakar.AppLogger
// 07/06/2022  1:34 PM

using System.Globalization;



namespace Jakar.AppLogger;


public sealed class AppLoggerIni : IAppLoggerConfig
{
    public const string CONFIG_FILE_NAME = "AppCenter.ini";


    private          LocalFile? _file;
    private readonly IniConfig  _config = new();
    private          Guid       _installID;
    private          Guid?      _sessionID;
    private          LogLevel   _logLevel;
    private          DateTime   _appLaunchTimestamp;
    private          string?    _userID;

    private  string            _appName = string.Empty;
    internal IniConfig.Section Section => _config[nameof(AppLogger)];

    public Device Device { get; set; }


    public string AppName
    {
        get => _appName;
        set
        {
            _appName                 = value;
            Section[nameof(AppName)] = value;
        }
    }

    public LogLevel LogLevel
    {
        get => _logLevel;
        set
        {
            _logLevel                 = value;
            Section[nameof(LogLevel)] = value.ToString();
        }
    }


    public DateTime AppLaunchTimestamp
    {
        get => _appLaunchTimestamp;
        set
        {
            _appLaunchTimestamp                 = value;
            Section[nameof(AppLaunchTimestamp)] = value.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
        }
    }

    public string? UserID
    {
        get => _userID;
        set
        {
            _userID                 = value;
            Section[nameof(UserID)] = value ?? string.Empty;
        }
    }

    public Guid InstallID
    {
        get => _installID;
        set
        {
            _installID                 = value;
            Section[nameof(InstallID)] = value.ToString();
        }
    }

    public Guid? SessionID
    {
        get => _sessionID;
        set
        {
            _sessionID                 = value;
            Section[nameof(SessionID)] = value?.ToString() ?? string.Empty;
        }
    }

    internal LocalFile Path
    {
        get => _file ??= LocalDirectory.CurrentDirectory.Join(CONFIG_FILE_NAME);
        set => _file = value;
    }


    internal AppLoggerIni( LocalDirectory directory ) : this(directory.Join(CONFIG_FILE_NAME)) { }
    internal AppLoggerIni( LocalFile file )
    {
        Path   = file;
        Device = new Device();
    }


    public static AppLoggerIni Create( LocalDirectory directory ) => Create(directory.Join(CONFIG_FILE_NAME));
    public static AppLoggerIni Create( LocalFile file )
    {
        IniConfig? ini = IniConfig.ReadFromFile(file);
        return new AppLoggerIni(file).Refresh(ini);
    }
    public static Task<AppLoggerIni> CreateAsync( LocalDirectory directory ) => CreateAsync(directory.Join(CONFIG_FILE_NAME));
    public static async Task<AppLoggerIni> CreateAsync( LocalFile file )
    {
        IniConfig? ini = await IniConfig.ReadFromFileAsync(file);
        return new AppLoggerIni(file).Refresh(ini);
    }


    private AppLoggerIni Refresh( IniConfig? config )
    {
        if ( config is null ) { return this; }

        IniConfig.Section section = config[nameof(AppLogger)];

        foreach ( ( string key, string value ) in section )
        {
            switch ( key )
            {
                case nameof(AppName):
                    AppName = value;
                    break;

                case nameof(InstallID):
                    InstallID = Guid.Parse(value);
                    break;

                case nameof(SessionID):
                    SessionID = Guid.TryParse(value, out Guid result)
                                    ? result
                                    : default;

                    break;
            }
        }

        return this;
    }


    public void Refresh()
    {
        IniConfig? ini = IniConfig.ReadFromFile(Path);
        Refresh(ini);
    }
    public Task RefreshAsync() => Task.Run(Refresh);


    public async Task SaveAsync() => await _config.WriteToFile(Path);
}
