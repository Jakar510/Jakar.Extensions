// Jakar.AppLogger :: Jakar.AppLogger.Client
// 09/09/2022  7:43 PM

namespace Jakar.AppLogger.Client;


public sealed class AppLoggerOptions : BaseHostViewModel, IOptions<AppLoggerOptions>, IValidator, IHostInfo
{
    private         LoggingSettings _config   = DefaultConfig;
    private         string          _apiToken = string.Empty;
    internal static AppLoggerConfig DefaultConfig { get; } = new(default);


    AppLoggerOptions IOptions<AppLoggerOptions>.Value   => this;
    public bool                                 IsValid => !string.IsNullOrWhiteSpace( APIToken ) && !string.IsNullOrWhiteSpace( _config.AppName ) && !ReferenceEquals( Config, DefaultConfig );
    public LoggingSettings Config
    {
        get
        {
            lock (this) { return _config; }
        }
        set
        {
            lock (this) { SetProperty( ref _config, value ); }
        }
    }


    public string APIToken
    {
        get
        {
            lock (this) { return _apiToken; }
        }
        set
        {
            lock (this) { SetProperty( ref _apiToken, value ); }
        }
    }


    public AppLoggerOptions() : base( new Uri( "https://localhost:6969" ) ) { }
    public AppLoggerOptions( string apiToken, Uri baseHost, LoggingSettings settings ) : base( baseHost )
    {
        APIToken = apiToken;
        Config   = settings;
    }


    internal WebRequester CreateWebRequester() => WebRequester.Builder.Create( this )
                                                              .Build();
}
