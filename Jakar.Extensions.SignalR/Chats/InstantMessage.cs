// TrueLogic :: TrueLogic.Common
// 05/11/2022  12:24 PM

namespace Jakar.Extensions.SignalR.Chats;


public interface IInstantMessage
{
    [StringLength( UNICODE_CAPACITY )] public string         GroupName   { get; }
    public                                    bool           HasBeenRead { get; }
    [StringLength( UNICODE_CAPACITY )] public string         Message     { get; }
    public                                    DateTimeOffset TimeStamp   { get; }
}



[Serializable]
public sealed class InstantMessage : ObservableClass<InstantMessage>, IInstantMessage
{
    private bool        _hasBeenRead;
    private string      _groupName = string.Empty;
    private string      _message   = string.Empty;
    private FileData[]? _data;


    public                                             FileData[]?    Data        { get => _data;        set => SetProperty( ref _data,        value ); }
    [StringLength( UNICODE_CAPACITY )] public required string         GroupName   { get => _groupName;   set => SetProperty( ref _groupName,   value ); }
    public                                             bool           HasBeenRead { get => _hasBeenRead; set => SetProperty( ref _hasBeenRead, value ); }
    [StringLength( UNICODE_CAPACITY )] public required string         Message     { get => _message;     set => SetProperty( ref _message,     value ); }
    public                                             DateTimeOffset TimeStamp   { get;                 init; }


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

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int groupComparison = string.Compare( GroupName, other.GroupName, StringComparison.OrdinalIgnoreCase );
        if ( groupComparison != 0 ) { return groupComparison; }

        return TimeStamp.CompareTo( other.TimeStamp );
    }
    public override bool Equals( InstantMessage? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return _hasBeenRead == other._hasBeenRead && _groupName == other._groupName && _message == other._message && Equals( _data, other._data ) && TimeStamp.Equals( other.TimeStamp );
    }
    protected override int GetHashCodeInternal()
    {
        HashCode hashCode = new();
        hashCode.Add( base.GetHashCode() );
        hashCode.Add( _hasBeenRead );
        hashCode.Add( _groupName );
        hashCode.Add( _message );
        hashCode.Add( _data );
        hashCode.Add( TimeStamp );
        return hashCode.ToHashCode();
    }



    public sealed class Collection() : ConcurrentObservableCollection<InstantMessage>( DEFAULT_CAPACITY )
    {
        public Collection( params ReadOnlySpan<InstantMessage> enumerable ) : this() => Add( enumerable );
    }
}
