// TrueLogic :: TrueLogic.Common
// 05/11/2022  12:24 PM

namespace Jakar.Extensions.SignalR.Chats;


public interface IInstantMessage
{
    [StringLength(UNICODE_CAPACITY)] public string         GroupName   { get; }
    public                                  bool           HasBeenRead { get; }
    [StringLength(UNICODE_CAPACITY)] public string         Message     { get; }
    public                                  DateTimeOffset TimeStamp   { get; }
}



[Serializable]
public sealed class InstantMessage : ObservableClass<InstantMessage>, IInstantMessage, IEqualComparable<InstantMessage>
{
    private bool        __hasBeenRead;
    private FileData[]? __data;
    private string      __groupName = string.Empty;
    private string      __message   = string.Empty;


    public                                           FileData[]?    Data        { get => __data;        set => SetProperty(ref __data,        value); }
    [StringLength(UNICODE_CAPACITY)] public required string         GroupName   { get => __groupName;   set => SetProperty(ref __groupName,   value); }
    public                                           bool           HasBeenRead { get => __hasBeenRead; set => SetProperty(ref __hasBeenRead, value); }
    [StringLength(UNICODE_CAPACITY)] public required string         Message     { get => __message;     set => SetProperty(ref __message,     value); }
    public                                           DateTimeOffset TimeStamp   { get;                  init; }


    public InstantMessage() { }

    [SetsRequiredMembers]
    public InstantMessage( string message, string groupName )
    {
        Message   = message;
        GroupName = groupName;
        TimeStamp = DateTimeOffset.UtcNow;
    }
    [SetsRequiredMembers]
    public InstantMessage( IInstantMessage message )
    {
        Message     = message.Message;
        TimeStamp   = message.TimeStamp;
        HasBeenRead = message.HasBeenRead;
        GroupName   = message.GroupName;
    }
    public static InstantMessage Create( IInstantMessage data ) => new(data);


    public override int CompareTo( InstantMessage? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int groupComparison = string.Compare(GroupName, other.GroupName, StringComparison.OrdinalIgnoreCase);
        if ( groupComparison != 0 ) { return groupComparison; }

        return TimeStamp.CompareTo(other.TimeStamp);
    }
    public override bool Equals( InstantMessage? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return __hasBeenRead == other.__hasBeenRead && __groupName == other.__groupName && __message == other.__message && Equals(__data, other.__data) && TimeStamp.Equals(other.TimeStamp);
    }
    public override bool Equals( object? other ) => ReferenceEquals(this, other) || ( other is InstantMessage message && Equals(message) );
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(__hasBeenRead);
        hashCode.Add(__groupName);
        hashCode.Add(__message);
        hashCode.Add(__data);
        hashCode.Add(TimeStamp);
        return hashCode.ToHashCode();
    }


    public static bool operator ==( InstantMessage? left, InstantMessage? right ) => EqualityComparer<InstantMessage>.Default.Equals(left, right);
    public static bool operator !=( InstantMessage? left, InstantMessage? right ) => !EqualityComparer<InstantMessage>.Default.Equals(left, right);
    public static bool operator >( InstantMessage   left, InstantMessage  right ) => Comparer<InstantMessage>.Default.Compare(left, right) > 0;
    public static bool operator >=( InstantMessage  left, InstantMessage  right ) => Comparer<InstantMessage>.Default.Compare(left, right) >= 0;
    public static bool operator <( InstantMessage   left, InstantMessage  right ) => Comparer<InstantMessage>.Default.Compare(left, right) < 0;
    public static bool operator <=( InstantMessage  left, InstantMessage  right ) => Comparer<InstantMessage>.Default.Compare(left, right) <= 0;



    public sealed class Collection() : ConcurrentObservableCollection<InstantMessage>(DEFAULT_CAPACITY)
    {
        public Collection( params ReadOnlySpan<InstantMessage> enumerable ) : this() => Add(enumerable);
    }
}
