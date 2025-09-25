// TrueLogic :: TrueLogic.Common
// 05/16/2022  2:09 PM

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Extensions.SignalR.Chats;


public interface IChatUser : IUserID
{
    [StringLength(UNICODE_CAPACITY)] public string FullName { get; }
    [StringLength(UNICODE_CAPACITY)] public string UserName { get; }
}



public sealed class ChatUser( string fullName, string userName, Guid userID ) : BaseClass<ChatUser>, IChatUser, IJsonModel<ChatUser>
{
    public static readonly                            ChatUser                 Empty = new(string.Empty, string.Empty, Guid.Empty);
    public static                                     JsonTypeInfo<ChatUser[]> JsonArrayInfo => JakarSignalRContext.Default.ChatUserArray;
    public static                                     JsonSerializerContext    JsonContext   => JakarSignalRContext.Default;
    public static                                     JsonTypeInfo<ChatUser>   JsonTypeInfo  => JakarSignalRContext.Default.ChatUser;
    [property: StringLength(UNICODE_CAPACITY)] public string                   FullName      { get; init; } = fullName;
    public                                            Guid                     UserID        { get; init; } = userID;
    [property: StringLength(UNICODE_CAPACITY)] public string                   UserName      { get; init; } = userName;


    public ChatUser( IChatUser               data ) : this(data.FullName, data.UserName, data.UserID) { }
    public static ChatUser Create( IChatUser data ) => new(data.FullName, data.UserName, data.UserID);
    public override bool Equals( ChatUser? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals(this, other) || string.Equals(FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase) && UserID.Equals(other.UserID) && string.Equals(UserName, other.UserName, StringComparison.InvariantCultureIgnoreCase);
    }
    public override bool Equals( object? obj ) => ReferenceEquals(this, obj) || obj is ChatUser other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(FullName, StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(UserID);
        hashCode.Add(UserName, StringComparer.InvariantCultureIgnoreCase);
        return hashCode.ToHashCode();
    }
    public override int CompareTo( ChatUser? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int userNameComparison = string.Compare(UserName, other.UserName, StringComparison.InvariantCultureIgnoreCase);
        if ( userNameComparison != 0 ) { return userNameComparison; }

        return string.Compare(FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase);
    }


    public static bool operator ==( ChatUser? left, ChatUser? right ) => Equals(left, right);
    public static bool operator !=( ChatUser? left, ChatUser? right ) => !Equals(left, right);
    public static bool operator <( ChatUser?  left, ChatUser? right ) => Comparer<ChatUser>.Default.Compare(left, right) < 0;
    public static bool operator >( ChatUser?  left, ChatUser? right ) => Comparer<ChatUser>.Default.Compare(left, right) > 0;
    public static bool operator <=( ChatUser? left, ChatUser? right ) => Comparer<ChatUser>.Default.Compare(left, right) <= 0;
    public static bool operator >=( ChatUser? left, ChatUser? right ) => Comparer<ChatUser>.Default.Compare(left, right) >= 0;
}



public sealed class ChatUserCollection : ConcurrentObservableCollection<ChatUserCollection, ChatUser>, ICollectionAlerts<ChatUserCollection, ChatUser>
{
    public static JsonTypeInfo<ChatUserCollection[]> JsonArrayInfo => JakarSignalRContext.Default.ChatUserCollectionArray;
    public static JsonSerializerContext              JsonContext   => JakarSignalRContext.Default;
    public static JsonTypeInfo<ChatUserCollection>   JsonTypeInfo  => JakarSignalRContext.Default.ChatUserCollection;


    public ChatUserCollection() : this(DEFAULT_CAPACITY) { }
    public ChatUserCollection( int                           capacity ) : base(capacity) { }
    public ChatUserCollection( IEnumerable<ChatUser>         enumerable ) : base(enumerable) { }
    public ChatUserCollection( params ReadOnlySpan<ChatUser> enumerable ) : base(enumerable) { }


    public static bool operator ==( ChatUserCollection? left, ChatUserCollection? right ) => EqualityComparer<ChatUserCollection>.Default.Equals(left, right);
    public static bool operator !=( ChatUserCollection? left, ChatUserCollection? right ) => !EqualityComparer<ChatUserCollection>.Default.Equals(left, right);
    public static bool operator >( ChatUserCollection   left, ChatUserCollection  right ) => Comparer<ChatUserCollection>.Default.Compare(left, right) > 0;
    public static bool operator >=( ChatUserCollection  left, ChatUserCollection  right ) => Comparer<ChatUserCollection>.Default.Compare(left, right) >= 0;
    public static bool operator <( ChatUserCollection   left, ChatUserCollection  right ) => Comparer<ChatUserCollection>.Default.Compare(left, right) < 0;
    public static bool operator <=( ChatUserCollection  left, ChatUserCollection  right ) => Comparer<ChatUserCollection>.Default.Compare(left, right) <= 0;


    public static implicit operator ChatUserCollection( List<ChatUser>           values ) => new(values);
    public static implicit operator ChatUserCollection( HashSet<ChatUser>        values ) => new(values);
    public static implicit operator ChatUserCollection( ConcurrentBag<ChatUser>  values ) => new(values);
    public static implicit operator ChatUserCollection( Collection<ChatUser>     values ) => new(values);
    public static implicit operator ChatUserCollection( ChatUser[]               values ) => new(values.AsSpan());
    public static implicit operator ChatUserCollection( ImmutableArray<ChatUser> values ) => new(values.AsSpan());
    public static implicit operator ChatUserCollection( ReadOnlyMemory<ChatUser> values ) => new(values.Span);
    public static implicit operator ChatUserCollection( ReadOnlySpan<ChatUser>   values ) => new(values);
}



public sealed record HubEvent( [property: StringLength(UNICODE_CAPACITY)] string ConnectionID, [property: StringLength(UNICODE_CAPACITY)] string Group, HubEventType Type, ChatUser User, InstantMessage? Message = null ) : BaseRecord;



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
    public bool              Active      { get; set; }
    string                   Group       { get; }
    InstantMessageCollection Messages    { get; }
    int                      UnreadChats { get; }
    ChatUserCollection       Users       { get; }


    public Task Join( CancellationToken   appToken = default );
    public Task Leave( CancellationToken  appToken = default );
    public Task Login( CancellationToken  token    = default );
    public Task Logout( CancellationToken token    = default );
    public Task Typing( CancellationToken token    = default );
}
