#nullable enable
namespace Jakar.Extensions;


[Serializable]
[SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
public abstract class BaseCollections<T> : ObservableClass, IEquatable<T>, IComparable<T>, IComparable where T : BaseCollections<T>
{
    public sealed override bool Equals( object? other ) => ReferenceEquals(this, other) || ( other is T file && Equals(file) );
    public override int GetHashCode() => base.GetHashCode();


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is T value
                   ? CompareTo(value)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(T));
    }

    public abstract int CompareTo( T? other );
    public abstract bool Equals( T?   other );


    public string ToJson() => JsonNet.ToJson(this);
    public string ToPrettyJson() => this.ToJson(Formatting.Indented);


    public static T? FromJson( [NotNullIfNotNull("json")] string? json ) => json?.FromJson<T>();



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



    public sealed class Sorter : Sorter<T> { }



    public sealed class Equalizer : Equalizer<T> { }
}
