// Jakar.Extensions :: Jakar.Extensions
// 05/21/2025  16:51

using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;



namespace Jakar.Extensions.SignalR.Chats;


/// <summary>
///     Represents a service for managing chat client operations, including handling chat rooms, user interactions, and communication events. This service extends <see cref="BackgroundService"/> and implements
///     <see
///         cref="IChatClientService"/>
///     .
/// </summary>
/// <remarks> The <see cref="ChatClientService"/> provides functionality to manage chat rooms, send and receive messages, handle user login/logout, and track unread chats. It also supports event handling for reconnection, disconnection, and property changes. </remarks>
public abstract class ChatClientService : BackgroundService, IChatClientService
{
    public const           string                                    PATH          = "/Chat/hub";
    public static readonly InstantMessage[]                          EmptyMessages = [];
    private readonly       Disposables                               __disposables = [];
    protected              ChatUser                                  _user         = ChatUser.Empty;
    protected              ConcurrentObservableCollection<IChatRoom> _rooms        = [];
    protected              HubConnection?                            _connection; // SignalR.Client
    protected              Tokens?                                   _tokens;
    private                bool                                      __isDisposed;
    public abstract        Uri                                       HostInfo { get; }


    public ConcurrentObservableCollection<IChatRoom> Rooms
    {
        get => _rooms;
        set
        {
            _rooms = value;
            OnPropertyChanged();
        }
    }
    public virtual Uri TargetHost => new(HostInfo, IChatClientService.PATH);
    public long UnreadChats
    {
        get
        {
            ReadOnlySpan<IChatRoom> span = Rooms.AsSpan();
            return span.Sum(room => room.UnreadChats);
        }
    }
    public virtual ChatUser Sender
    {
        get => _user;
        set
        {
            if ( _user.Equals(value) ) { return; }

            _user = value;
            OnPropertyChanged();
        }
    }


    public event Func<Exception?, Task>? Reconnecting
    {
        add
        {
            if ( _connection is not null ) { _connection.Reconnecting += value; }
        }
        remove
        {
            if ( _connection is not null ) { _connection.Reconnecting -= value; }
        }
    }

    public event Func<string?, Task>? Reconnected
    {
        add
        {
            if ( _connection is not null ) { _connection.Reconnected += value; }
        }
        remove
        {
            if ( _connection is not null ) { _connection.Reconnected -= value; }
        }
    }

    public event EventHandler<HubEvent>?      OnEvent;
    public event PropertyChangedEventHandler? PropertyChanged;

    public override void Dispose()
    {
        base.Dispose();
        __isDisposed = true;
        GC.SuppressFinalize(this);
    }


    public    void SendEvent( HubEvent                           value )               => OnEvent?.Invoke(this, value);
    protected void OnPropertyChanged( [CallerMemberName] string? propertyName = null ) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public HubEvent GetHubEvent( IChatRoom room, HubEventType type, InstantMessage? message = null )
    {
        ObjectDisposedException.ThrowIf(__isDisposed, this);
        string connectionID = _connection?.ConnectionId ?? throw new InvalidOperationException($"{nameof(_connection)} is null. Make sure to call {nameof(StartAsync)} first.");
        return new HubEvent(connectionID, room.Group, type, Sender, message);
    }


    protected override async Task ExecuteAsync( CancellationToken token )
    {
        if ( _connection is not null ) { await _connection.DisposeAsync(); }

        IOptions<HttpConnectionOptions> options;
        await StopAsync(token);
        _connection = CreateConnection();

        __disposables.Add(_connection.On<HubEvent>(nameof(IChatHub.Send),          SendEvent));
        __disposables.Add(_connection.On<HubEvent>(nameof(IChatHub.JoinRoom),      SendEvent));
        __disposables.Add(_connection.On<HubEvent>(nameof(IChatHub.LeaveRoom),     SendEvent));
        __disposables.Add(_connection.On<HubEvent>(nameof(IChatHub.Disconnection), SendEvent));
        __disposables.Add(_connection.On<HubEvent>(nameof(IChatHub.Reconnection),  SendEvent));
        __disposables.Add(_connection.On<HubEvent>(nameof(IChatHub.Login),         SendEvent));
        __disposables.Add(_connection.On<HubEvent>(nameof(IChatHub.Logout),        SendEvent));
        __disposables.Add(_connection.On<HubEvent>(nameof(IChatHub.Typing),        SendEvent));
        await _connection.StartAsync(token);
    }
    public override async Task StopAsync( CancellationToken cancellationToken )
    {
        await base.StopAsync(cancellationToken);
        __disposables.Dispose();
        _connection = null;
    }
    protected virtual  void          ConfigureHttpConnection( HttpConnectionOptions options ) => options.AccessTokenProvider = OptionsAccessTokenProvider;
    protected virtual  HubConnection CreateConnection()                                       => new HubConnectionBuilder().AddJsonProtocol().WithAutomaticReconnect().WithUrl(TargetHost, ConfigureHttpConnection).Build();
    protected abstract Task<string?> OptionsAccessTokenProvider();


    public async ValueTask Login( IChatRoom room, CancellationToken token = default )
    {
        HubEvent hubEvent = GetHubEvent(room, HubEventType.Login);
        await Login(hubEvent, token);
    }
    public async ValueTask Send( IChatRoom room, InstantMessage message, CancellationToken token = default )
    {
        HubEvent hubEvent = GetHubEvent(room, HubEventType.Send, message);
        await Send(hubEvent, token);
    }


    public async ValueTask<InstantMessage[]> History( IChatRoom room, CancellationToken token = default ) { return await History(room.Group, Sender, token); }


    public async ValueTask JoinRoom( IEnumerable<IChatRoom> rooms, CancellationToken token = default )
    {
        foreach ( IChatRoom room in rooms ) { await JoinRoom(room, token); }
    }
    public async ValueTask JoinRoom( IChatRoom room, CancellationToken token = default )
    {
        HubEvent hubEvent = GetHubEvent(room, HubEventType.JoinRoom);
        await JoinRoom(hubEvent, token);
    }


    public async ValueTask LeaveRoom( IEnumerable<IChatRoom> rooms, CancellationToken token = default )
    {
        foreach ( IChatRoom room in rooms ) { await LeaveRoom(room, token); }
    }
    public async ValueTask LeaveRoom( IChatRoom room, CancellationToken token = default )
    {
        HubEvent hubEvent = GetHubEvent(room, HubEventType.LeaveRoom);
        await LeaveRoom(hubEvent, token);
    }


    public async ValueTask Logout( IChatRoom room, CancellationToken token = default )
    {
        HubEvent hubEvent = GetHubEvent(room, HubEventType.Logout);
        await Logout(hubEvent, token);
    }
    public async ValueTask Typing( IChatRoom room, CancellationToken token = default )
    {
        HubEvent hubEvent = GetHubEvent(room, HubEventType.Typing);
        await Typing(hubEvent, token);
    }


    public async Task Disconnection( HubEvent value, CancellationToken token = default )
    {
        if ( _connection is not null )
        {
            await _connection.SendAsync(nameof(Disconnection), value, token);
            SendEvent(value);
        }
    }
    public async Task Reconnection( HubEvent value, CancellationToken token = default )
    {
        if ( _connection is not null )
        {
            await _connection.SendAsync(nameof(Reconnection), value, token);
            SendEvent(value);
        }
    }
    public async Task Login( HubEvent value, CancellationToken token = default )
    {
        if ( _connection is not null )
        {
            await _connection.SendAsync(nameof(Login), value, token);
            SendEvent(value);
        }
    }
    public async Task Logout( HubEvent value, CancellationToken token = default )
    {
        if ( _connection is not null )
        {
            await _connection.SendAsync(nameof(Login), value, token);
            SendEvent(value);
        }
    }
    public async Task Typing( HubEvent value, CancellationToken token = default )
    {
        if ( _connection is not null )
        {
            await _connection.SendAsync(nameof(Typing), value, token);
            SendEvent(value);
        }
    }
    public async Task JoinRoom( HubEvent value, CancellationToken token = default )
    {
        if ( _connection is not null )
        {
            await _connection.SendAsync(nameof(JoinRoom), value, token);
            SendEvent(value);
        }
    }
    public async Task LeaveRoom( HubEvent value, CancellationToken token = default )
    {
        if ( _connection is not null )
        {
            await _connection.SendAsync(nameof(LeaveRoom), value, token);
            SendEvent(value);
        }
    }
    public async Task Send( HubEvent value, CancellationToken token = default )
    {
        if ( _connection is not null )
        {
            await _connection.SendAsync(nameof(Send), value, token);
            SendEvent(value);
        }
    }
    public async Task<InstantMessage[]> History( string group, ChatUser user, CancellationToken token = default ) =>
        _connection is null
            ? EmptyMessages
            : await _connection.InvokeAsync<InstantMessage[]>(nameof(History), group, user, token);
}
