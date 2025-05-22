// TrueLogic :: TrueLogic.Common
// 05/16/2022  2:09 PM

namespace Jakar.Extensions.SignalR.Chats;


public interface IChatUser : IUserID
{
    [StringLength( UNICODE_CAPACITY )] public string FullName { get; }
    [StringLength( UNICODE_CAPACITY )] public string UserName { get; }
}



public sealed record ChatUser( [property: StringLength( UNICODE_CAPACITY )] string FullName, [property: StringLength( UNICODE_CAPACITY )] string UserName, Guid UserID ) : BaseRecord, IChatUser
{
    public static readonly ChatUser Empty = new(string.Empty, string.Empty, Guid.Empty);
    public ChatUser( IChatUser               data ) : this( data.FullName, data.UserName, data.UserID ) { }
    public static ChatUser Create( IChatUser data ) => new(data.FullName, data.UserName, data.UserID);



    public sealed class Collection() : ConcurrentObservableCollection<ChatUser>( DEFAULT_CAPACITY )
    {
        public Collection( params ReadOnlySpan<ChatUser> enumerable ) : this() => Add( enumerable );
    }
}



public sealed record HubEvent( [property: StringLength( UNICODE_CAPACITY )] string ConnectionID, [property: StringLength( UNICODE_CAPACITY )] string Group, HubEventType Type, ChatUser User, InstantMessage? Message = null ) : BaseRecord;



public enum HubEventType
{
    Login,
    Logout,
    Send,
    Typing,
    JoinRoom,
    LeaveRoom,
    Reconnection,
    Disconnection
}



public interface IChatHub
{
    ChatUser Sender { get; set; }


    Task                   Disconnection( HubEvent value, CancellationToken token                         = default );
    Task                   Reconnection( HubEvent  value, CancellationToken token                         = default );
    Task                   Login( HubEvent         value, CancellationToken token                         = default );
    Task                   Logout( HubEvent        value, CancellationToken token                         = default );
    Task                   Typing( HubEvent        value, CancellationToken token                         = default );
    Task                   JoinRoom( HubEvent      value, CancellationToken token                         = default );
    Task                   LeaveRoom( HubEvent     value, CancellationToken token                         = default );
    Task                   Send( HubEvent          value, CancellationToken token                         = default );
    Task<InstantMessage[]> History( string         group, ChatUser          user, CancellationToken token = default );
}



public interface IChatClientService : IHostedService, IChatHub, INotifyPropertyChanged
{
    public const string                                    PATH = "/Chat/hub";
    public       ConcurrentObservableCollection<IChatRoom> Rooms       { get; }
    public       long                                      UnreadChats { get; }


    /// <summary> Occurs when the <see cref="HubConnection"/> starts reconnecting after losing its underlying connection. </summary>
    /// <remarks> The <see cref="Exception"/> that occurred will be passed in as the sole argument to this handler. </remarks>
    /// <example>
    ///     The following example attaches a handler to the <see cref="Reconnecting"/> event, and checks the provided argument to log the error.
    ///     <code>
    /// connection.Reconnecting += (exception) =>
    /// {
    ///     Console.WriteLine($"Connection started reconnecting due to an error: {exception}");
    /// };
    /// </code>
    /// </example>
    public event Func<Exception?, Task>? Reconnecting;


    /// <summary> Occurs when the <see cref="HubConnection"/> successfully reconnects after losing its underlying connection. </summary>
    /// <remarks> The <see cref="string"/> parameter will be the <see cref="HubConnection"/>'s new ConnectionId or null if negotiation was skipped. </remarks>
    /// <example>
    ///     The following example attaches a handler to the <see cref="Reconnected"/> event, and checks the provided argument to log the ConnectionId.
    ///     <code>
    /// connection.Reconnected += (connectionId) =>
    /// {
    ///     Console.WriteLine($"Connection successfully reconnected. The ConnectionId is now: {connectionId}");
    /// };
    /// </code>
    /// </example>
    public event Func<string?, Task>? Reconnected;


    public event EventHandler<HubEvent>? OnEvent;


    public ValueTask Login( IChatRoom room, CancellationToken token                            = default );
    public ValueTask Send( IChatRoom  room, InstantMessage    message, CancellationToken token = default );


    public ValueTask<InstantMessage[]> History( IChatRoom room, CancellationToken token = default );


    public ValueTask JoinRoom( IEnumerable<IChatRoom> rooms, CancellationToken token = default );
    public ValueTask JoinRoom( IChatRoom              room,  CancellationToken token = default );


    public ValueTask LeaveRoom( IEnumerable<IChatRoom> rooms, CancellationToken token = default );
    public ValueTask LeaveRoom( IChatRoom              room,  CancellationToken token = default );


    public ValueTask Logout( IChatRoom room, CancellationToken token = default );
    public ValueTask Typing( IChatRoom room, CancellationToken token = default );


    public void     SendEvent( HubEvent    value );
    public HubEvent GetHubEvent( IChatRoom room, HubEventType type, InstantMessage? message = null );
}



public interface IChatRoom : IEquatable<IChatRoom>, IComparable<IChatRoom>, INotifyPropertyChanged
{
    string                    Group       { get; }
    InstantMessage.Collection Messages    { get; }
    int                       UnreadChats { get; }
    ChatUser.Collection       Users       { get; }
    public bool               Active      { get; set; }


    public Task Join( CancellationToken   appToken = default );
    public Task Leave( CancellationToken  appToken = default );
    public Task Login( CancellationToken  token    = default );
    public Task Logout( CancellationToken token    = default );
    public Task Typing( CancellationToken token    = default );
}
