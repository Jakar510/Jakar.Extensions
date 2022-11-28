// Jakar.AppLogger :: Jakar.AppLogger.Client
// 09/08/2022  1:54 PM

namespace Jakar.AppLogger.Client;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public sealed class AppLogger : Service, IAppLogger
{
    private readonly MultiDeque<Log> _logs = new();
    private readonly WebRequester    _requester;


    public          AppLoggerOptions        Options     { get; }
    public override bool                    IsValid     => Options.IsValid && base.IsValid;
    internal        IEnumerable<Attachment> Attachments => Options.Config.AttachmentProviders.Select( x => x.GetAttachment() );
    public          LoggingSettings         Config      => Options.Config;
    internal        string                  ApiToken    => Options.APIToken;


    public AppLogger( IOptions<AppLoggerOptions> options ) : this( options.Value ) { }
    internal AppLogger( AppLoggerOptions options )
    {
        Options    = options;
        _requester = options.CreateWebRequester();
    }
    protected override void Dispose( bool disposing )
    {
        if ( !disposing ) { return; }

        _logs.Clear();
    }
    internal void ThrowIfNotEnabled()
    {
        if ( !IsValid ) { throw new ApiDisabledException( $"Must call {nameof(AppLogger)}.{nameof(StartAsync)} first." ); }
    }


    public static AppLogger Create( string apiToken, string appName, AppVersion version, Uri baseHost, bool includeHwInfo = true )
    {
    #if __WINDOWS__ || __MACOS__ || __ANDROID__ || __IOS__
        var config = new AppLoggerConfig( version, DeviceDescriptor.Create( includeHwInfo, version ), appName );
    #else
        var config = AppLoggerIni.Create(version, appName, includeHwInfo);
    #endif

        return new AppLogger( new AppLoggerOptions( apiToken, baseHost, config ) );
    }


    public void Add( Log log ) => _logs.Add( log );


    private async ValueTask Log( Log log, CancellationToken token )
    {
        if ( !IsValid ) { throw new ApiDisabledException(); }

        await _requester.Post( $"/Api/{nameof(Log)}", log, token )
                        .AsString();
    }
    private async ValueTask StartSession( CancellationToken token )
    {
        var session = new StartSession
                      {
                          AppLoggerSecret = ApiToken,
                          Device          = Config.Device
                      };

        while ( token.ShouldContinue() && !await StartSession( session, token ) ) { }
    }
    private async ValueTask<bool> StartSession( StartSession session, CancellationToken token )
    {
        WebResponse<string> id = await _requester.Post( $"/Api/{nameof(StartSession)}", session, token )
                                                 .AsString();

        if ( !Guid.TryParse( id.Payload, out Guid result ) ) { return false; }

        Config.SessionID = result;
        return true;
    }
    private async ValueTask EndSession( CancellationToken token ) =>
        await _requester.Post( $"/Api/{nameof(EndSession)}", Config.SessionID.ToString(), token )
                        .AsString();
    public override ValueTask DisposeAsync()
    {
        Dispose( true );
        return default;
    }


    public async Task StartAsync( CancellationToken token )
    {
        IsAlive = true;

        try
        {
            if ( string.IsNullOrWhiteSpace( ApiToken ) ) { throw new ApiDisabledException( $"{nameof(ApiToken)} must be provided" ); }

            await Config.InitAsync();

            using ( var source = new CancellationTokenSource( TimeSpan.FromSeconds( 15 ) ) )
            {
                await using ( token.Register( source.Cancel ) ) { await StartSession( source.Token ); }
            }


            if ( !Config.SessionID.IsValidID() ) { throw new ApiDisabledException( $"{nameof(LoggingSettings.SessionID)} is not set." ); }

            while ( !token.IsCancellationRequested )
            {
                if ( _logs.Remove( out Log? log ) ) { await Log( log.Update( Config ), token ); }

                await Delay( 1, token );
            }
        }
        finally { IsAlive = false; }
    }
    public async Task StopAsync( CancellationToken token )
    {
        while ( _logs.Remove( out Log? log ) ) { await Log( log, token ); }

        await EndSession( token );
    }


    public async ValueTask<byte[]?> TryTakeScreenShot()
    {
        if ( !Config.TakeScreenshotOnError ) { return default; }

        try { return await LoggingSettings.TakeScreenshot(); }
        catch ( Exception ) { return default; }
    }


    public void TrackEvent( string? message, LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = default )
    {
        if ( !Config.EnableAnalytics ) { return; }

        if ( string.IsNullOrWhiteSpace( message ) ) { return; }

        Add( new Log( Config, level, Attachments )
             {
                 Message        = message,
                 AdditionalData = eventDetails
             } );
    }
    public void TrackEvent<T>( LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = null, [CallerMemberName] string? caller = null ) => TrackEvent( $"{typeof(T).Name}.{caller}", level, eventDetails );
    public void TrackEvent<T>( T        _, LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = null, [CallerMemberName] string? caller = null ) => TrackEvent( $"{typeof(T).Name}.{caller}", level, eventDetails );


    public void TrackError( Exception exception ) => TrackError( exception,                                             default,      Array.Empty<Attachment>() );
    public void TrackError( Exception exception, IDictionary<string, JToken?>? eventDetails ) => TrackError( exception, eventDetails, Array.Empty<Attachment>() );
    public void TrackError( Exception exception, params Attachment[]           attachments ) => TrackError( exception,  default,      attachments );
    public void TrackError( Exception exception, IDictionary<string, JToken?>? eventDetails, params Attachment[] attachments )
    {
        if ( !Config.EnableCrashes ) { return; }

        var log = new Log( Config, attachments.Concat( Attachments ), exception )
                  {
                      AdditionalData = eventDetails
                  };

        _logs.Add( log );
    }
    public void TrackError( Exception exception, IEnumerable<Attachment> attachments ) => TrackError( exception, default, attachments );
    public void TrackError( Exception exception, IDictionary<string, JToken?>? eventDetails, IEnumerable<Attachment> attachments )
    {
        if ( !Config.EnableCrashes ) { return; }

        var log = new Log( Config, attachments.Concat( Attachments ), exception )
                  {
                      AdditionalData = eventDetails
                  };

        _logs.Add( log );
    }


    public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
    {
        if ( !IsEnabled( logLevel ) ) { return; }

        if ( !Config.EnableAnalytics && logLevel < LogLevel.Error ) { return; }

        if ( !Config.EnableCrashes && logLevel >= LogLevel.Error ) { return; }

        var log = new Log( Config, Attachments, exception, eventId, formatter( state, exception ), logLevel );
        _logs.Add( log );
    }
    public bool IsEnabled( LogLevel logLevel )
    {
        if ( logLevel is LogLevel.None ) { return false; }

        return logLevel >= Options.Config.LogLevel;
    }


    public IDisposable BeginScope<TState>( TState state ) where TState : notnull => Config.CreateScope();
    public ILogger CreateLogger( string           categoryName ) => this;
}
