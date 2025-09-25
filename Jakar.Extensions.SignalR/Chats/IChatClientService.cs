// Jakar.Extensions :: Jakar.Extensions.SignalR
// 09/25/2025  10:19

namespace Jakar.Extensions.SignalR.Chats;


public interface IChatClientService : IHostedService, IChatHub, INotifyPropertyChanged
{
    public const string    PATH = "/Chat/hub";
    public       ChatRooms Rooms       { get; }
    public       long      UnreadChats { get; }


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
