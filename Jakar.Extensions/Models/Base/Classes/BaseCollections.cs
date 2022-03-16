namespace Jakar.Extensions.Models.Base.Classes;


[Serializable]
public abstract class BaseCollections<T> : BaseNotifyPropertyModel where T : BaseCollections<T>
{
    [Serializable]
    public class Collection : ObservableCollection<T>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<T> items ) : base(items) { }
    }



    [Serializable]
    public class ConcurrentCollection : ConcurrentObservableCollection<T>
    {
        public ConcurrentCollection() : base() { }
        public ConcurrentCollection( IEnumerable<T> items ) : base(items) { }
    }



    [Serializable]
    public class Queue : MultiQueue<T>
    {
        public Queue() : base() { }
        public Queue( IEnumerable<T> items ) : base(items) { }
    }



    [Serializable]
    public class Deque : MultiDeque<T>
    {
        public Deque() : base() { }
        public Deque( IEnumerable<T> items ) : base(items) { }
    }



    [Serializable]
    public class Items : List<T>
    {
        public Items() : base() { }
        public Items( int            capacity ) : base(capacity) { }
        public Items( IEnumerable<T> items ) : base(items) { }
    }



    [Serializable]
    public class Set : HashSet<T>
    {
        public Set() : base() { }
        public Set( int            capacity ) : base(capacity) { }
        public Set( IEnumerable<T> items ) : base(items) { }
    }
}
