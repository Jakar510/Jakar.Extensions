// Jakar.Extensions :: Jakar.Extensions.SignalR
// 09/25/2025  10:19

namespace Jakar.Extensions.SignalR.Chats;


public interface IChatRoom : IEquatable<IChatRoom>, IComparable<IChatRoom>, INotifyPropertyChanged
{
    public bool              Active      { get; set; }
    string                   Group       { get; }
    InstantMessageCollection Messages    { get; }
    int                      UnreadChats { get; }
    ChatUserCollection       Users       { get; }


    public event Action? OnMessageReceived;


    public Task Join( CancellationToken   appToken = default );
    public Task Leave( CancellationToken  appToken = default );
    public Task Login( CancellationToken  token    = default );
    public Task Logout( CancellationToken token    = default );
    public Task Typing( CancellationToken token    = default );
}



public interface IChatRoom<TSelf> : IChatRoom, IJsonModel<TSelf>, IEqualComparable<TSelf>
    where TSelf : IChatRoom<TSelf>;



public abstract class ChatRooms<TSelf, TRoom> : ConcurrentObservableCollection<TSelf, TRoom>
    where TRoom : IChatRoom<TRoom>
    where TSelf : ChatRooms<TSelf, TRoom>, ICollectionAlerts<TSelf, TRoom>
{
    private bool __showAll;
    public  bool ShowAll { get => __showAll; set => SetProperty(ref __showAll, value); }


    protected ChatRooms() : this(DEFAULT_CAPACITY) { }
    protected ChatRooms( int                        capacity ) : base(capacity) { }
    protected ChatRooms( IEnumerable<TRoom>         enumerable ) : base(enumerable) { }
    protected ChatRooms( params ReadOnlySpan<TRoom> enumerable ) : base(enumerable) { }


    protected override bool Filter( int index, ref readonly TRoom? value ) => ShowAll || ( value is not null && value.Active );
}
