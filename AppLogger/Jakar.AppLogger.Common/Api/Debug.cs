#nullable enable
namespace Jakar.AppLogger.Common;


[SuppressMessage( "ReSharper", "MemberCanBeMadeStatic.Global" )]
public sealed class Debug : ObservableClass
{
    private readonly BaseFileSystemApi    _fileSystemApi;
    private readonly IAppLogger           _logger;
    private          object?              _appState;
    private          object?              _incoming;
    private          object?              _outgoing;
    private          ReadOnlyMemory<byte> _screenShot;


    public object? AppState
    {
        get => _appState;
        set => SetProperty( ref _appState, value );
    }
    internal LocalFile FeedBack => _fileSystemApi.FeedBackFile;
    public object? Incoming
    {
        get => _incoming;
        set => SetProperty( ref _incoming, value );
    }
    public Guid InstallID => _logger.Config.InstallID;
    public object? Outgoing
    {
        get => _outgoing;
        set => SetProperty( ref _outgoing, value );
    }
    public ReadOnlyMemory<byte> ScreenShot
    {
        get => _screenShot;
        set => SetProperty( ref _screenShot, value );
    }


    public Debug( BaseFileSystemApi api, IAppLogger logger )
    {
        _fileSystemApi = api;
        _logger        = logger;
    }


    public void HandleException( Exception e ) => Task.Run( () => HandleExceptionAsync( e ) );
    public async ValueTask HandleExceptionAsync( Exception e )
    {
        ScreenShot = await _logger.TryTakeScreenShot();
        TrackError( e );
    }
    public async Task SaveFeedBack( object? feedback )
    {
        if ( feedback is null ) { FeedBack.Delete(); }
        else { await FeedBack.WriteAsync( feedback.ToPrettyJson() ); }
    }
    public async Task SaveFeedBack( string? feedback )
    {
        if ( feedback is null ) { FeedBack.Delete(); }
        else { await FeedBack.WriteAsync( feedback ); }
    }
    public async Task SaveFeedBack( ReadOnlyMemory<byte> feedback, CancellationToken token ) => await FeedBack.WriteAsync( feedback, token );


    public void TrackError( Exception e, EventId? eventId = default )
    {
        if ( !_logger.Config.IncludeAppStateOnError )
        {
            TrackError( e, eventId );
            return;
        }


        e.Details( out Dictionary<string, string?> dict );
        var eventDetails = new Dictionary<string, JToken?>();
        foreach ( (string? key, string? value) in dict ) { eventDetails.Add( key, value ); }

        TrackError( e, eventDetails, eventId );
    }
    public void TrackError( Exception ex, IDictionary<string, JToken?>? eventDetails, EventId? eventId = default )
    {
        List<Attachment> attachments = GetAttachments( ex, eventDetails );

        if ( FeedBack.Exists ) { attachments.Add( Attachment.Create( FeedBack ) ); }

        _logger.TrackError( ex, eventId ?? new EventId( ex.Message.GetHashCode(), ex.Message ), attachments, eventDetails );
        ScreenShot = default;
    }
    public async ValueTask TrackErrorAsync( Exception ex, IDictionary<string, JToken?>? eventDetails, EventId? eventId = default )
    {
        List<Attachment> attachments = GetAttachments( ex, eventDetails );

        if ( FeedBack.Exists ) { attachments.Add( await Attachment.CreateAsync( FeedBack ) ); }

        _logger.TrackError( ex, eventId ?? new EventId( ex.Message.GetHashCode(), ex.Message ), attachments, eventDetails );
        ScreenShot = default;
    }
    private List<Attachment> GetAttachments( Exception ex, IDictionary<string, JToken?>? eventDetails )
    {
        var attachments = new List<Attachment>( 10 )
                          {
                              Attachment.Create( ex.ToString(),
                                                 ex.GetType()
                                                   .FullName ),
                          };


        if ( _logger.Config.IncludeAppStateOnError && AppState is not null ) { attachments.Add( Attachment.Create( AppState.ToPrettyJson(), _fileSystemApi.AppStateFile.Name ) ); }

        if ( _logger.Config.IncludeEventDetailsOnError && eventDetails is not null ) { attachments.Add( Attachment.Create( eventDetails.ToPrettyJson(), _fileSystemApi.DebugFile.Name ) ); }


        if ( _logger.Config.IncludeRequestsOnError )
        {
            if ( Incoming is not null ) { attachments.Add( Attachment.Create( Incoming.ToPrettyJson(), _fileSystemApi.IncomingFile.Name ) ); }

            if ( Outgoing is not null ) { attachments.Add( Attachment.Create( Outgoing.ToPrettyJson(), _fileSystemApi.OutgoingFile.Name ) ); }
        }


        if ( _logger.Config.TakeScreenshotOnError && !ScreenShot.IsEmpty ) { attachments.Add( Attachment.Create( ScreenShot, "ScreenShot.jpeg", "image/jpeg" ) ); }

        return attachments;
    }


    public void TrackEvent<T>( T cls, LogLevel                      level = LogLevel.Trace, [CallerMemberName] string? source = default ) => TrackEvent( cls, _logger.Config.GetAppState(), level, source );
    public void TrackEvent<T>( T cls, IDictionary<string, JToken?>? eventDetails,           LogLevel                   level  = LogLevel.Trace, [CallerMemberName] string? source = default ) => _logger.TrackEvent( cls, level, eventDetails, source );
}
