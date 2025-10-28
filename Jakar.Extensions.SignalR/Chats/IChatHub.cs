// Jakar.Extensions :: Jakar.Extensions.SignalR
// 09/25/2025  10:20

namespace Jakar.Extensions.SignalR.Chats;


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
