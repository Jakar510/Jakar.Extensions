// Jakar.Extensions :: Jakar.Extensions
// 12/05/2023  1:18 PM

namespace Jakar.Extensions;


[ Serializable ]
public abstract record CollectionsRecord<T, TID> : ObservableRecord<T, TID> where T : CollectionsRecord<T, TID>
                                                                            where TID : struct, IComparable<TID>, IEquatable<TID>
{
    public static Equalizer<T> Equalizer => Equalizer<T>.Default;
    public static Sorter<T>    Sorter    => Sorter<T>.Default;


    protected CollectionsRecord() : base() { }
    protected CollectionsRecord( TID id ) : base( id ) { }



    [ Serializable ]
    public class Collection : ObservableCollection<T>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<T> items ) : base( items ) { }
    }



    [ Serializable ]
    public class ConcurrentCollection : ConcurrentObservableCollection<T>
    {
        public ConcurrentCollection() : base() { }
        public ConcurrentCollection( IEnumerable<T> items ) : base( items ) { }
    }



    [ Serializable ]
    public class Deque : MultiDeque<T>
    {
        public Deque() : base() { }
        public Deque( IEnumerable<T> items ) : base( items ) { }
    }



    [ Serializable ]
    public class Items : List<T>
    {
        public Items() : base() { }
        public Items( int            capacity ) : base( capacity ) { }
        public Items( IEnumerable<T> items ) : base( items ) { }
    }



    [ Serializable ]
    public class Queue : MultiQueue<T>
    {
        public Queue() : base() { }
        public Queue( IEnumerable<T> items ) : base( items ) { }
    }



    [ Serializable ]
    public class Set : HashSet<T>
    {
        public Set() : base() { }
        public Set( int            capacity ) : base( capacity ) { }
        public Set( IEnumerable<T> items ) : base( items ) { }
    }
}

[ Serializable ]
public abstract record CollectionsRecord<T> : CollectionsRecord<T, long> where T : CollectionsRecord<T, long>
{
    protected CollectionsRecord() : base() { }
    protected CollectionsRecord( long id ) : base( id ) { }
}
