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
