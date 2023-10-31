// Jakar.AppLogger :: Jakar.AppLogger.Client
// 09/08/2022  1:54 PM

using System.Collections.Immutable;



namespace Jakar.AppLogger.Common;


[ SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" ) ]
public sealed class AppLogger : IValidator, IAppLogger
{
    private static readonly TimeSpan              _delay  = TimeSpan.FromMilliseconds( 50 );
    public static readonly  EventID               EventID = new(510, nameof(AppLogger));
    private readonly        ConcurrentBag<AppLog> _logs   = new();
    private readonly        ILogger               _logger;
    private readonly        ILoggerFactory        _factory;
    private readonly        string?               _categoryName;
    private readonly        Synchronized<bool>    _isAlive = new(false);
    private readonly        WebRequester          _requester;
    private                 bool                  _disposed;


    internal string          ApiToken     => Options.APIToken;
    public   string          CategoryName => _categoryName  ?? Options.Config?.AppName ?? string.Empty;
    public   LoggingSettings Config       => Options.Config ?? throw new InvalidOperationException( $"{nameof(AppLoggerOptions)}.{nameof(AppLoggerOptions.Config)} is not set" );
    public bool IsAlive
    {
        get => _isAlive.Value;
        set => _isAlive.Value = value;
    }
    public   bool                          IsValid           => Options.IsValid;
    internal IEnumerable<LoggerAttachment> LoggerAttachments => Config.LoggerAttachmentProviders.Select( x => x.GetLoggerAttachment() );
    public   AppLoggerOptions              Options           { get; }


    public AppLogger( IOptions<AppLoggerOptions> options, ILoggerFactory factory ) : this( options.Value, factory ) { }
    internal AppLogger( AppLoggerOptions options, ILoggerFactory factory )
    {
        _factory   = factory;
        Options    = options;
        _logger    = factory.CreateLogger<AppLogger>();
        _requester = options.CreateWebRequester();
    }
    private AppLogger( AppLogger logger, string categoryName )
    {
        _categoryName = categoryName;
        _factory      = logger._factory;
        _requester    = logger._requester;
        Options       = logger.Options;
        _logger       = _factory.CreateLogger( categoryName );
    }
    public async ValueTask DisposeAsync()
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(AppLogger) ); }

        _logs.Clear();
        await Config.DisposeAsync();
        _disposed = true;
    }
    public void Dispose() => DisposeAsync().CallSynchronously();


    public void Add( AppLog log )
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(AppLogger) ); }

        _logs.Add( log );
    }
    public void Add( params AppLog[] logs )
    {
        foreach ( AppLog log in logs ) { _logs.Add( log ); }
    }
    public void Add( IEnumerable<AppLog> logs )
    {
        foreach ( AppLog log in logs ) { _logs.Add( log ); }
    }


    private async ValueTask StartSession( CancellationToken token )
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(AppLogger) ); }

        using var source  = new CancellationTokenSource( Options.TimeOut );
        var       session = new StartSession( ApiToken, Config.AppLaunchTimeStamp, Config.Device );

        await using ( token.Register( source.Cancel ) )
        {
            while ( token.ShouldContinue() )
            {
                WebResponse<StartSessionReply> reply = await _requester.Post( "/Api/StartSession", session, token ).AsJson<StartSessionReply>();

                Config.Session = reply.GetPayload();
            }

            if ( Config.Session?.SessionID.IsValidID() is not true ) { throw new ApiDisabledException( $"{nameof(LoggingSettings.Session)} is not set." ); }
        }
    }
    private async ValueTask EndSession( CancellationToken token )
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(AppLogger) ); }

        if ( Config.Session is null ) { return; }

        WebResponse<string> reply = await _requester.Post( $"/Api/{nameof(EndSession)}", Config.Session, token ).AsString();

        reply.GetPayload();
    }


    private ImmutableArray<AppLog> GetLogs() => _logs.ToImmutableArray();
    private void ClearLogs( in ImmutableArray<AppLog> logs )
    {
        int count = 0;
        while ( count < logs.Length && _logs.TryTake( out AppLog? _ ) ) { count++; }
    }
    public async Task StartAsync( CancellationToken token )
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(AppLogger) ); }

        if ( IsAlive ) { return; }

        try
        {
            IsAlive = true;
            if ( string.IsNullOrWhiteSpace( ApiToken ) ) { throw new ApiDisabledException( $"{nameof(ApiToken)} must be provided" ); }

            if ( !Options.IsValid ) { throw new ApiDisabledException( $"{nameof(Options)} must be provided" ); }

            await Config.InitAsync();
            await StartSession( token );

        #if NET6_0_OR_GREATER
            using var timer = new PeriodicTimer( _delay );
        #endif
            while ( token.IsCancellationRequested is false )
            {
            #if NET6_0_OR_GREATER
                await timer.WaitForNextTickAsync( token );
            #else
                await _delay.Delay( token );
            #endif

                try
                {
                    ImmutableArray<AppLog> logs   = GetLogs();
                    using var              source = new CancellationTokenSource( Options.TimeOut );
                    await using ( token.Register( source.Cancel ) ) { await SendLog( logs, source.Token ); }

                    ClearLogs( logs );
                }
                catch ( Exception ex ) { _logger.LogCritical( EventID, ex, "{Caller}", nameof(StartAsync) ); }
            }
        }
        finally { IsAlive = false; }
    }
    public async Task StopAsync( CancellationToken token )
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(AppLogger) ); }

        ImmutableArray<AppLog> logs = GetLogs();
        await SendLog( logs, token );
        await EndSession( token );
        ClearLogs( logs );
    }
    private async ValueTask<bool> SendLog( ImmutableArray<AppLog> logs, CancellationToken token )
    {
        if ( !IsValid ) { throw new ApiDisabledException(); }

        WebResponse<bool> reply = await _requester.Post( "/Api/Log", logs, token ).AsBool();

        return reply.GetPayload();
    }


    public async ValueTask<byte[]?> TryTakeScreenShot()
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(AppLogger) ); }

        if ( !Config.TakeScreenshotOnError ) { return default; }

        try { return await Config.TakeScreenshot(); }
        catch ( Exception ) { return default; }
    }


    public void TrackEvent<T>( LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = null, [ CallerMemberName ] string? caller = null ) =>
        TrackEvent( $"{typeof(T).Name}.{caller}", level, eventDetails );
    public void TrackEvent<T>( T _, LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = null, [ CallerMemberName ] string? caller = null ) =>
        TrackEvent( $"{typeof(T).Name}.{caller}", level, eventDetails );
    public void TrackEvent( string message, LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = default )
    {
        if ( !IsEnabled( level ) ) { return; }

        if ( string.IsNullOrWhiteSpace( message ) ) { return; }

        var eventID = new EventID( message.GetHashCode(), message );
        var log     = AppLog.Create( this, level, eventID, message, LoggerAttachments, eventDetails );
        Add( log );
    }


    public void TrackError( Exception e, EventID? eventId = default ) =>
        TrackError( e, eventId, default, LoggerAttachment.Empty );
    public void TrackError( Exception e, EventID? eventId, IDictionary<string, JToken?>? eventDetails ) =>
        TrackError( e, eventId, eventDetails, LoggerAttachment.Empty );
    public void TrackError( Exception e, EventID? eventId, params LoggerAttachment[] attachments ) =>
        TrackError( e, eventId, default, attachments );
    public void TrackError( Exception e, EventID? eventId, IDictionary<string, JToken?>? eventDetails, params LoggerAttachment[] attachments ) =>
        TrackError( e, eventId ?? new EventID( e.HResult, e.Source ), attachments, eventDetails );
    public void TrackError( Exception e, EventID eventId, IEnumerable<LoggerAttachment> attachments, IDictionary<string, JToken?>? eventDetails = default )
    {
        if ( !IsEnabled( LogLevel.Error ) ) { return; }

        var log = AppLog.Create( this, LogLevel.Error, eventId, e.Message, e, LoggerAttachments.Concat( attachments ), eventDetails );
        _logs.Add( log );
    }


    void ILogger.Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
    {
        EventID id = eventId;
        Log( logLevel, id, state, exception, formatter );
    }
    public void Log<TState>( LogLevel level, EventID eventId, TState state, Exception? e, Func<TState, Exception?, string> formatter )
    {
        if ( !IsEnabled( level ) ) { return; }

        var log = AppLog.Create( this, level, eventId, state, e, formatter, LoggerAttachments );
        _logs.Add( log );
    }
    public bool IsEnabled( LogLevel level )
    {
        if ( !Config.EnableAnalytics && level < LogLevel.Error ) { return false; }

        if ( !Config.EnableCrashes && level >= LogLevel.Error ) { return false; }

        return level is not LogLevel.None && level >= Config.LogLevel;
    }


    public IDisposable      BeginScope<TState>( TState state ) where TState : notnull => Config.CreateScope( state );
    public AppLogger        CreateLogger( string       categoryName )                 => new(this, categoryName);
    ILogger ILoggerProvider.CreateLogger( string       categoryName )                 => CreateLogger( categoryName );


    private         void   Log( LogLevel logLevel, string? message, params object?[] args ) => Log( logLevel, 0, null, message, args );
    private         void   Log( LogLevel logLevel, EventID eventID, string? message, params object?[] args ) => Log( logLevel, eventID, null, message, args );
    private         void   Log( LogLevel logLevel, Exception? exception, string? message, params object?[] args ) => Log( logLevel, 0, exception, message, args );
    private         void   Log( LogLevel logLevel, EventID eventID, Exception? exception, string? message, params object?[] args ) => Log( logLevel, eventID, new FormattedLogValues( message, args ), exception, MessageFormatter );
    internal static string MessageFormatter( FormattedLogValues log, Exception? exception ) => log.ToString();


    public void Debug( EventID    eventID,   Exception?       exception, string?          message, params object?[] args ) => Log( LogLevel.Debug, eventID, exception, message, args );
    public void Debug( EventID    eventID,   string?          message,   params object?[] args ) => Log( LogLevel.Debug, eventID,   message, args );
    public void Debug( Exception? exception, string?          message,   params object?[] args ) => Log( LogLevel.Debug, exception, message, args );
    public void Debug( string?    message,   params object?[] args ) => Log( LogLevel.Debug, message, args );


    public void Trace( EventID    eventID,   Exception?       exception, string?          message, params object?[] args ) => Log( LogLevel.Trace, eventID, exception, message, args );
    public void Trace( EventID    eventID,   string?          message,   params object?[] args ) => Log( LogLevel.Trace, eventID,   message, args );
    public void Trace( Exception? exception, string?          message,   params object?[] args ) => Log( LogLevel.Trace, exception, message, args );
    public void Trace( string?    message,   params object?[] args ) => Log( LogLevel.Trace, message, args );


    public void Information( EventID    eventID,   Exception?       exception, string?          message, params object?[] args ) => Log( LogLevel.Information, eventID, exception, message, args );
    public void Information( EventID    eventID,   string?          message,   params object?[] args ) => Log( LogLevel.Information, eventID,   message, args );
    public void Information( Exception? exception, string?          message,   params object?[] args ) => Log( LogLevel.Information, exception, message, args );
    public void Information( string?    message,   params object?[] args ) => Log( LogLevel.Information, message, args );


    public void Warning( EventID    eventID,   Exception?       exception, string?          message, params object?[] args ) => Log( LogLevel.Warning, eventID, exception, message, args );
    public void Warning( EventID    eventID,   string?          message,   params object?[] args ) => Log( LogLevel.Warning, eventID,   message, args );
    public void Warning( Exception? exception, string?          message,   params object?[] args ) => Log( LogLevel.Warning, exception, message, args );
    public void Warning( string?    message,   params object?[] args ) => Log( LogLevel.Warning, message, args );


    public void Error( EventID    eventID,   Exception?       exception, string?          message, params object?[] args ) => Log( LogLevel.Error, eventID, exception, message, args );
    public void Error( EventID    eventID,   string?          message,   params object?[] args ) => Log( LogLevel.Error, eventID,   message, args );
    public void Error( Exception? exception, string?          message,   params object?[] args ) => Log( LogLevel.Error, exception, message, args );
    public void Error( string?    message,   params object?[] args ) => Log( LogLevel.Error, message, args );


    public void Critical( EventID    eventID,   Exception?       exception, string?          message, params object?[] args ) => Log( LogLevel.Critical, eventID, exception, message, args );
    public void Critical( EventID    eventID,   string?          message,   params object?[] args ) => Log( LogLevel.Critical, eventID,   message, args );
    public void Critical( Exception? exception, string?          message,   params object?[] args ) => Log( LogLevel.Critical, exception, message, args );
    public void Critical( string?    message,   params object?[] args ) => Log( LogLevel.Critical, message, args );
}
