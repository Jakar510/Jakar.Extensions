#nullable enable
namespace Jakar.Extensions;


[Serializable]
public abstract record BaseCollectionsRecord<T> : ObservableRecord, IEquatable<T>, IComparable<T>, IComparable where T : BaseCollectionsRecord<T>
{
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



    public sealed class Sorter : IComparer<T>, IComparer
    {
        public static Sorter Instance { get; } = new();
        private Sorter() { }


        public int Compare( object? x, object? y )
        {
            if ( x is not T left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(T)); }

            if ( y is not T right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(T)); }


            return Compare(left, right);
        }
        public int Compare( T? x, T? y )
        {
            if ( ReferenceEquals(x, y) ) { return 0; }

            if ( y is null ) { return 1; }

            if ( x is null ) { return -1; }

            return x.CompareTo(y);
        }
    }



    public sealed class Equalizer : IEqualityComparer<T>
    {
        public static Equalizer Instance { get; } = new();
        private Equalizer() { }


        public bool Equals( T? left, T? right )
        {
            if ( left is null && right is null ) { return true; }

            if ( left is null || right is null ) { return false; }

            return left.Equals(right);
        }
        public int GetHashCode( T value ) => value.GetHashCode();
    }
}
