﻿// Jakar.AppLogger :: Jakar.AppLogger.Client
// 09/08/2022  1:54 PM

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;



namespace Jakar.AppLogger.Common;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public sealed class AppLogger : Service, IAppLogger
{
    public static readonly EventId               EventID = new(1, nameof(AppLogger));
    private readonly       ConcurrentBag<AppLog> _logs   = new();
    private readonly       ILogger               _logger;
    private readonly       ILoggerFactory        _factory;
    private readonly       string?               _categoryName;
    private readonly       WebRequester          _requester;
    private                bool                  _disposed;


    internal        string                  ApiToken     => Options.APIToken;
    internal        IEnumerable<Attachment> Attachments  => Config.AttachmentProviders.Select( x => x.GetAttachment() );
    public          string                  CategoryName => _categoryName ?? Options.Config?.AppName ?? string.Empty;
    public          LoggingSettings         Config       => Options.Config ?? throw new InvalidOperationException( $"{nameof(AppLoggerOptions)}.{nameof(AppLoggerOptions.Config)} is not set" );
    public override bool                    IsValid      => Options.IsValid && base.IsValid;
    public          AppLoggerOptions        Options      { get; }


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
        Options       = logger.Options;
        _logger       = _factory.CreateLogger( categoryName );
        _requester    = logger._requester;
    }
    public override async ValueTask DisposeAsync()
    {
        if ( _disposed ) { ThrowDisposed(); }

        _logs.Clear();
        await Config.DisposeAsync();
        _disposed = true;
    }
    public void Dispose() => DisposeAsync()
       .CallSynchronously();


    public void Add( AppLog log )
    {
        if ( _disposed ) { ThrowDisposed(); }

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
        if ( _disposed ) { ThrowDisposed(); }

        using var source = new CancellationTokenSource( Options.TimeOut );

        var session = new StartSession
                      {
                          AppLoggerSecret = ApiToken,
                          Device          = Config.Device,
                      };

        await using ( token.Register( source.Cancel ) )
        {
            while ( token.ShouldContinue() )
            {
                WebResponse<string> reply = await _requester.Post( "/Api/StartSession", session, token )
                                                            .AsString();

                if ( Guid.TryParse( reply.GetPayload(), out Guid result ) )
                {
                    Config.SessionID = result;
                    break;
                }
            }

            if ( !Config.SessionID.IsValidID() ) { throw new ApiDisabledException( $"{nameof(LoggingSettings.SessionID)} is not set." ); }
        }
    }
    private async ValueTask EndSession( CancellationToken token )
    {
        if ( _disposed ) { ThrowDisposed(); }

        WebResponse<string> reply = await _requester.Post( $"/Api/{nameof(EndSession)}", Config.SessionID.ToString(), token )
                                                    .AsString();

        reply.GetPayload();
    }


    public async Task StartAsync( CancellationToken token )
    {
        if ( _disposed ) { ThrowDisposed(); }

        if ( IsAlive ) { return; }

        try
        {
            IsAlive = true;
            if ( string.IsNullOrWhiteSpace( ApiToken ) ) { throw new ApiDisabledException( $"{nameof(ApiToken)} must be provided" ); }

            if ( !Options.IsValid ) { throw new ApiDisabledException( $"{nameof(Options)} must be provided" ); }

            await Config.InitAsync();
            await StartSession( token );

        #if !NETSTANDARD2_1
            using var timer = new PeriodicTimer( TimeSpan.FromMilliseconds( 50 ) );
        #endif
            while ( !token.IsCancellationRequested )
            {
            #if !NETSTANDARD2_1
                await timer.WaitForNextTickAsync( token );
            #else
                await TimeSpan.FromMilliseconds( 50 )
                              .Delay( token );
            #endif

                try
                {
                    var logs = new HashSet<AppLog>( _logs.Count );
                    while ( _logs.TryTake( out AppLog? log ) ) { logs.Add( log ); }

                    using var source = new CancellationTokenSource( Options.TimeOut );
                    await using ( token.Register( source.Cancel ) ) { await SendLog( logs, source.Token ); }
                }
                catch ( Exception ex ) { _logger.LogCritical( EventID, ex, "{Caller}", nameof(StartAsync) ); }
            }
        }
        finally { IsAlive = false; }
    }
    private async ValueTask<bool> SendLog( IEnumerable<AppLog> log, CancellationToken token )
    {
        if ( !IsValid ) { throw new ApiDisabledException(); }

        WebResponse<bool> reply = await _requester.Post( "/Api/Log", log, token )
                                                  .AsBool();

        return reply.GetPayload();
    }
    public async Task StopAsync( CancellationToken token )
    {
        if ( _disposed ) { ThrowDisposed(); }

        var logs = new HashSet<AppLog>( _logs.Select( x => x.Update( Config ) ) );
        await SendLog( logs, token );
        await EndSession( token );
    }


    public async ValueTask<byte[]?> TryTakeScreenShot()
    {
        if ( _disposed ) { ThrowDisposed(); }

        if ( !Config.TakeScreenshotOnError ) { return default; }

        try { return await Config.TakeScreenshot(); }
        catch ( Exception ) { return default; }
    }


    public void TrackEvent<T>( LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = null, [CallerMemberName] string? caller = null ) =>
        TrackEvent( $"{typeof(T).Name}.{caller}", level, eventDetails );
    public void TrackEvent<T>( T _, LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = null, [CallerMemberName] string? caller = null ) =>
        TrackEvent( $"{typeof(T).Name}.{caller}", level, eventDetails );
    public void TrackEvent( string message, LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = default )
    {
        if ( !IsEnabled( level ) ) { return; }

        if ( string.IsNullOrWhiteSpace( message ) ) { return; }

        var eventID = new EventId( message.GetHashCode(), message );
        var log     = new AppLog( this, level, eventID, message, Attachments, eventDetails );
        Add( log );
    }


    public void TrackError( Exception e, EventId? eventId = default ) =>
        TrackError( e, eventId, default, Attachment.Empty );
    public void TrackError( Exception e, EventId? eventId, IDictionary<string, JToken?>? eventDetails ) =>
        TrackError( e, eventId, eventDetails, Attachment.Empty );
    public void TrackError( Exception e, EventId? eventId, params Attachment[] attachments ) =>
        TrackError( e, eventId, default, attachments );
    public void TrackError( Exception e, EventId? eventId, IDictionary<string, JToken?>? eventDetails, params Attachment[] attachments ) =>
        TrackError( e, eventId ?? new EventId( e.HResult, e.Source ), attachments, eventDetails );
    public void TrackError( Exception e, EventId eventId, IEnumerable<Attachment> attachments, IDictionary<string, JToken?>? eventDetails = default )
    {
        if ( !IsEnabled( LogLevel.Error ) ) { return; }

        AppLog log = AppLog.Create( this, LogLevel.Error, eventId, e.Message, e, Attachments.Concat( attachments ), eventDetails );
        _logs.Add( log );
    }


    public void Log<TState>( LogLevel level, EventId eventId, TState state, Exception? e, Func<TState, Exception?, string> formatter )
    {
        if ( !IsEnabled( level ) ) { return; }

        AppLog log = AppLog.Create( this, level, eventId, state, e, formatter, Attachments );
        _logs.Add( log );
    }
    public bool IsEnabled( LogLevel level )
    {
        if ( !Config.EnableAnalytics && level < LogLevel.Error ) { return false; }

        if ( !Config.EnableCrashes && level >= LogLevel.Error ) { return false; }

        return level is not LogLevel.None && level >= Config.LogLevel;
    }


    public IDisposable BeginScope<TState>( TState state ) where TState : notnull => Config.CreateScope(state);
    public AppLogger CreateLogger( string         categoryName ) => new(this, categoryName);
    ILogger ILoggerProvider.CreateLogger( string  categoryName ) => CreateLogger( categoryName );
}
