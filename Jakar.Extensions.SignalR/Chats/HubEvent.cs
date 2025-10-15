// Jakar.Extensions :: Jakar.Extensions.SignalR
// 09/25/2025  10:19

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;



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



public sealed class HubEvent : BaseClass<HubEvent>, IJsonModel<HubEvent>
{
    public static                                 JsonSerializerContext    JsonContext   => JakarSignalRContext.Default;
    public static                                 JsonTypeInfo<HubEvent>   JsonTypeInfo  => JakarSignalRContext.Default.HubEvent;
    public static                                 JsonTypeInfo<HubEvent[]> JsonArrayInfo => JakarSignalRContext.Default.HubEventArray;
    [StringLength(CONNECTION_ID)] public required string                   ConnectionID  { get; init; }
    [StringLength(NAME)]          public required string                   Group         { get; init; }
    public required                               HubEventType             Type          { get; init; }
    public required                               ChatUser                 User          { get; init; }
    public                                        InstantMessage?          Message       { get; init; }


    public HubEvent() { }
    [SetsRequiredMembers] public HubEvent( string connectionID, string group, HubEventType type, ChatUser user, InstantMessage? message = null )
    {
        ConnectionID = connectionID;
        Group        = group;
        Type         = type;
        User         = user;
        Message      = message;
    }

    public override int CompareTo( HubEvent? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int groupComparison = string.Compare(Group, other.Group, StringComparison.InvariantCultureIgnoreCase);
        if ( groupComparison != 0 ) { return groupComparison; }

        int userComparison = User.CompareTo(other.User);
        if ( userComparison != 0 ) { return userComparison; }

        int connectionIDComparison = string.Compare(ConnectionID, other.ConnectionID, StringComparison.InvariantCultureIgnoreCase);
        if ( connectionIDComparison != 0 ) { return connectionIDComparison; }

        int typeComparison = Type.CompareTo(other.Type);
        if ( typeComparison != 0 ) { return typeComparison; }

        return Comparer<InstantMessage?>.Default.Compare(Message, other.Message);
    }
    public override bool Equals( HubEvent? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return string.Equals(Group, other.Group, StringComparison.InvariantCultureIgnoreCase) && User.Equals(other.User) && Type == other.Type && Equals(Message, other.Message) && string.Equals(ConnectionID, other.ConnectionID, StringComparison.InvariantCultureIgnoreCase);
    }
    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(Group, StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(User);
        hashCode.Add((int)Type);
        hashCode.Add(Message);
        hashCode.Add(ConnectionID, StringComparer.InvariantCultureIgnoreCase);
        return hashCode.ToHashCode();
    }


    public static bool operator <( HubEvent?  left, HubEvent? right ) => Comparer<HubEvent>.Default.Compare(left, right) < 0;
    public static bool operator >( HubEvent?  left, HubEvent? right ) => Comparer<HubEvent>.Default.Compare(left, right) > 0;
    public static bool operator <=( HubEvent? left, HubEvent? right ) => Comparer<HubEvent>.Default.Compare(left, right) <= 0;
    public static bool operator >=( HubEvent? left, HubEvent? right ) => Comparer<HubEvent>.Default.Compare(left, right) >= 0;
    public static bool operator ==( HubEvent? left, HubEvent? right ) => EqualityComparer<HubEvent>.Default.Equals(left, right);
    public static bool operator !=( HubEvent? left, HubEvent? right ) => !EqualityComparer<HubEvent>.Default.Equals(left, right);
}
