﻿#nullable enable
namespace Jakar.AppLogger.Client;


[SuppressMessage( "ReSharper", "MemberCanBeMadeStatic.Global" )]
public sealed class Debug : ObservableClass
{
    private readonly BaseFileSystemApi    _fileSystemApi;
    private readonly IAppLogger           _logger;
    private          object?              _appState;
    private          object?              _incoming;
    private          object?              _outgoing;
    private          ReadOnlyMemory<byte> _screenShot;


    public   Guid      InstallID => _logger.Config.InstallID;
    internal LocalFile FeedBack  => _fileSystemApi.FeedBackFileName;
    public object? AppState
    {
        get => _appState;
        set => SetProperty( ref _appState, value );
    }
    public object? Incoming
    {
        get => _incoming;
        set => SetProperty( ref _incoming, value );
    }
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


    /// <summary>
    ///     <see cref = "Task.Run(Action)" />
    /// </summary>
    public void HandleException( Exception e ) => Task.Run( () => HandleExceptionAsync( e ) );


    public async ValueTask HandleExceptionAsync( Exception e )
    {
        ScreenShot = await _logger.TryTakeScreenShot();
        TrackError( e );
    }
    public async Task SaveFeedBack( object? feedback )
    {
        if (feedback is null) { FeedBack.Delete(); }
        else { await FeedBack.WriteAsync( feedback.ToPrettyJson() ); }
    }


    public void TrackError( Exception e )
    {
        if (!_logger.Config.IncludeAppStateOnError)
        {
            TrackError( e, default );
            return;
        }


        e.Details( out Dictionary<string, string?> dict );
        var eventDetails = new Dictionary<string, JToken?>();
        foreach ((string? key, string? value) in dict) { eventDetails.Add( key, value ); }

        TrackError( e, eventDetails );
    }
    public void TrackError( Exception ex, IDictionary<string, JToken?>? eventDetails )
    {
        List<Attachment> attachments = GetAttachments( ex, eventDetails );

        if (FeedBack.Exists) { attachments.Add( Attachment.Create( FeedBack ) ); }

        _logger.TrackError( ex, eventDetails, attachments );
        ScreenShot = default;
    }
    public async ValueTask TrackErrorAsync( Exception ex, IDictionary<string, JToken?>? eventDetails )
    {
        List<Attachment> attachments = GetAttachments( ex, eventDetails );

        if (FeedBack.Exists) { attachments.Add( await Attachment.CreateAsync( FeedBack ) ); }

        _logger.TrackError( ex, eventDetails, attachments );
        ScreenShot = default;
    }
    private List<Attachment> GetAttachments( Exception ex, IDictionary<string, JToken?>? eventDetails )
    {
        var attachments = new List<Attachment>( 10 )
                          {
                              Attachment.Create( ex.ToString(),
                                                 ex.GetType()
                                                   .FullName )
                          };


        if (_logger.Config.IncludeAppStateOnError && AppState is not null) { attachments.Add( Attachment.Create( AppState.ToPrettyJson(), _fileSystemApi.AppStateFileName ) ); }

        if (_logger.Config.IncludeEventDetailsOnError && eventDetails is not null) { attachments.Add( Attachment.Create( eventDetails.ToPrettyJson(), _fileSystemApi.DebugFileName ) ); }


        if (_logger.Config.IncludeRequestsOnError)
        {
            if (Incoming is not null) { attachments.Add( Attachment.Create( Incoming.ToPrettyJson(), _fileSystemApi.IncomingFileName ) ); }

            if (Outgoing is not null) { attachments.Add( Attachment.Create( Outgoing.ToPrettyJson(), _fileSystemApi.OutgoingFileName ) ); }
        }


        if (_logger.Config.TakeScreenshotOnError && !ScreenShot.IsEmpty) { attachments.Add( Attachment.Create( ScreenShot, "ScreenShot.jpeg", "image/jpeg" ) ); }

        return attachments;
    }


    public void TrackEvent<T>( T cls, LogLevel                      level = LogLevel.Trace, [CallerMemberName] string? source = default ) => TrackEvent( cls, _logger.Config.GetAppState(), level, source );
    public void TrackEvent<T>( T cls, IDictionary<string, JToken?>? eventDetails,           LogLevel                   level  = LogLevel.Trace, [CallerMemberName] string? source = default ) => _logger.TrackEvent( cls, level, eventDetails, source );
}