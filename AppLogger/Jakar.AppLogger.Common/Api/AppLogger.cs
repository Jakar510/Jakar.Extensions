// Jakar.AppLogger :: Jakar.AppLogger.Client
// 09/08/2022  1:54 PM

using Microsoft.Extensions.Options;



namespace Jakar.AppLogger.Common;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public sealed class AppLogger : Service, IAppLogger
{
    private readonly        ILogger<AppLogger> _logger;
    private readonly        ConcurrentBag<Log> _logs = new();
    private readonly        WebRequester       _requester;
    private static readonly TimeSpan           _delay  = TimeSpan.FromMilliseconds( 50 );
    public static readonly  EventId            EventID = new(1, nameof(AppLogger));


    public          AppLoggerOptions        Options     { get; }
    public override bool                    IsValid     => Options.IsValid && base.IsValid;
    internal        IEnumerable<Attachment> Attachments => Config.AttachmentProviders.Select( x => x.GetAttachment() );
    public          LoggingSettings         Config      => Options.Config ?? throw new InvalidOperationException( $"{nameof(AppLoggerOptions)}.{nameof(AppLoggerOptions.Config)} is not set" );
    internal        string                  ApiToken    => Options.APIToken;


    public AppLogger( IOptions<AppLoggerOptions> options, ILogger<AppLogger> logger ) : this( options.Value, logger ) { }
    internal AppLogger( AppLoggerOptions options, ILogger<AppLogger> logger )
    {
        _logger    = logger;
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


    public void Add( Log log ) => _logs.Add( log );
    public void Add( params Log[] logs )
    {
        foreach ( Log log in logs ) { _logs.Add( log ); }
    }
    public void Add( IEnumerable<Log> logs )
    {
        foreach ( Log log in logs ) { _logs.Add( log ); }
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
        WebResponse<string> reply = await _requester.Post( $"/Api/{nameof(StartSession)}", session, token )
                                                    .AsString();

        if ( !Guid.TryParse( reply.GetPayload(), out Guid result ) ) { return false; }

        Config.SessionID = result;
        return true;
    }
    private async ValueTask EndSession( CancellationToken token ) =>
        await _requester.Post( $"/Api/{nameof(EndSession)}", Config.SessionID.ToString(), token )
                        .AsString();
    public override async ValueTask DisposeAsync()
    {
        Dispose( true );
        await Config.DisposeAsync();
    }


    public async Task StartAsync( CancellationToken token )
    {
        IsAlive = true;

        try
        {
            if ( string.IsNullOrWhiteSpace( ApiToken ) ) { throw new ApiDisabledException( $"{nameof(ApiToken)} must be provided" ); }

            if ( !Options.IsValid ) { throw new ApiDisabledException( $"{nameof(Options)} must be provided" ); }

            await Config.InitAsync();

            using ( var source = new CancellationTokenSource( Options.TimeOut ) )
            {
                await using ( token.Register( source.Cancel ) ) { await StartSession( source.Token ); }
            }


            if ( !Config.SessionID.IsValidID() ) { throw new ApiDisabledException( $"{nameof(LoggingSettings.SessionID)} is not set." ); }


            using var timer = new PeriodicTimer( _delay );

            while ( !token.IsCancellationRequested )
            {
                await timer.WaitForNextTickAsync( token );

                try
                {
                    var       logs   = new HashSet<Log>( _logs.Select( x => x.Update( Config ) ) );
                    using var source = new CancellationTokenSource( Options.TimeOut );
                    await using ( token.Register( source.Cancel ) ) { await SendLog( logs, source.Token ); }
                }
                catch ( Exception ex ) { _logger.LogCritical( EventID, ex, "{Caller}", nameof(StartAsync) ); }
            }
        }
        finally { IsAlive = false; }
    }
    private async ValueTask<bool> SendLog( IEnumerable<Log> log, CancellationToken token )
    {
        if ( !IsValid ) { throw new ApiDisabledException(); }

        WebResponse<bool> reply = await _requester.Post( "/Api/Log", log, token )
                                                  .AsBool();

        return reply.GetPayload();
    }
    public async Task StopAsync( CancellationToken token )
    {
        var logs = new HashSet<Log>( _logs.Select( x => x.Update( Config ) ) );
        await SendLog( logs, token );
        await EndSession( token );
    }


    public async ValueTask<byte[]?> TryTakeScreenShot()
    {
        if ( !Config.TakeScreenshotOnError ) { return default; }

        try { return await Config.TakeScreenshot(); }
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


    public void TrackError( Exception e ) => TrackError( e,                                             default,      Array.Empty<Attachment>() );
    public void TrackError( Exception e, IDictionary<string, JToken?>? eventDetails ) => TrackError( e, eventDetails, Array.Empty<Attachment>() );
    public void TrackError( Exception e, params Attachment[]           attachments ) => TrackError( e,  default,      attachments );
    public void TrackError( Exception e, IDictionary<string, JToken?>? eventDetails, params Attachment[] attachments )
    {
        if ( !Config.EnableCrashes ) { return; }

        var log = new Log( Config, attachments.Concat( Attachments ), e )
                  {
                      AdditionalData = eventDetails
                  };

        _logs.Add( log );
    }
    public void TrackError( Exception e, IEnumerable<Attachment> attachments ) => TrackError( e, default, attachments );
    public void TrackError( Exception e, IDictionary<string, JToken?>? eventDetails, IEnumerable<Attachment> attachments )
    {
        if ( !Config.EnableCrashes ) { return; }

        var log = new Log( Config, attachments.Concat( Attachments ), e )
                  {
                      AdditionalData = eventDetails
                  };

        _logs.Add( log );
    }


    public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? e, Func<TState, Exception?, string> formatter )
    {
        if ( !IsEnabled( logLevel ) ) { return; }

        if ( !Config.EnableAnalytics && logLevel < LogLevel.Error ) { return; }

        if ( !Config.EnableCrashes && logLevel >= LogLevel.Error ) { return; }

        var log = new Log( Config, Attachments, e, eventId, formatter( state, e ), logLevel );
        _logs.Add( log );
    }
    public bool IsEnabled( LogLevel logLevel )
    {
        if ( logLevel is LogLevel.None ) { return false; }

        return logLevel >= Config.LogLevel;
    }


    public IDisposable BeginScope<TState>( TState state ) where TState : notnull => Config.CreateScope();
    public ILogger CreateLogger( string           categoryName ) => this;
}
