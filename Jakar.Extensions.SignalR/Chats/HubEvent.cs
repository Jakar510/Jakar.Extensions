// Jakar.Extensions :: Jakar.Extensions.SignalR
// 09/25/2025  10:19

namespace Jakar.Extensions.SignalR.Chats;


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



public sealed record HubEvent( [property: StringLength(UNICODE_CAPACITY)] string ConnectionID, [property: StringLength(UNICODE_CAPACITY)] string Group, HubEventType Type, ChatUser User, InstantMessage? Message = null ) : BaseRecord;
