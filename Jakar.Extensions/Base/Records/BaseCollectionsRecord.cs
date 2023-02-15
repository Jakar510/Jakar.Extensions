#nullable enable
namespace Jakar.Extensions;


[Serializable]
public abstract record BaseCollectionsRecord<T, TID> : ObservableRecord<T, TID> where T : BaseCollectionsRecord<T, TID>
                                                                                where TID : struct, IComparable<TID>, IEquatable<TID>
{
    protected BaseCollectionsRecord() : base() { }
    protected BaseCollectionsRecord( TID id ) : base( id ) { }



    [Serializable]
    public class Collection : ObservableCollection<T>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<T> items ) : base( items ) { }
    }



    [Serializable]
    public class ConcurrentCollection : ConcurrentObservableCollection<T>
    {
        public ConcurrentCollection() : base() { }
        public ConcurrentCollection( IEnumerable<T> items ) : base( items ) { }
    }



    [Serializable]
    public class Deque : MultiDeque<T>
    {
        public Deque() : base() { }
        public Deque( IEnumerable<T> items ) : base( items ) { }
    }



    [Serializable]
    public class Items : List<T>
    {
        public Items() : base() { }
        public Items( int            capacity ) : base( capacity ) { }
        public Items( IEnumerable<T> items ) : base( items ) { }
    }



    [Serializable]
    public class Queue : MultiQueue<T>
    {
        public Queue() : base() { }
        public Queue( IEnumerable<T> items ) : base( items ) { }
    }



    [Serializable]
    public class Set : HashSet<T>
    {
        public Set() : base() { }
        public Set( int            capacity ) : base( capacity ) { }
        public Set( IEnumerable<T> items ) : base( items ) { }
    }



    public sealed class Equalizer : Equalizer<T> { }



    public sealed class Sorter : Sorter<T> { }
}



[Serializable]
public abstract record BaseCollectionsRecord<T> : BaseCollectionsRecord<T, long> where T : BaseCollectionsRecord<T, long>
{
    protected BaseCollectionsRecord() : base() { }
    protected BaseCollectionsRecord( long id ) : base( id ) { }
}
