namespace Jakar.Extensions.Models.Base.Records;


[Serializable]
public abstract record BaseModelRecord<T> : BaseCollectionsRecord<T>, IDataBaseID, IEquatable<T>, IComparable<T>, IComparable where T : BaseModelRecord<T>
{
    public abstract bool Equals( T? other );

    public abstract int CompareTo( T? other );


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is T value
                   ? CompareTo(value)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(T));
    }



    public sealed class EqualityComparer : IEqualityComparer<T>
    {
        public static EqualityComparer Instance { get; } = new();

        public bool Equals( T? x, T? y )
        {
            if ( ReferenceEquals(x, y) ) { return true; }

            if ( ReferenceEquals(x, null) ) { return false; }

            if ( ReferenceEquals(y, null) ) { return false; }

            if ( x.GetType() != y.GetType() ) { return false; }

            return x.Equals(y);
        }

        public int GetHashCode( T obj ) => obj.GetHashCode();
    }



    public sealed class RelationalComparer : IComparer<T>, IComparer
    {
        public static RelationalComparer Instance { get; } = new();

        public int Compare( T? x, T? y )
        {
            if ( ReferenceEquals(x, y) ) { return 0; }

            if ( ReferenceEquals(null, y) ) { return 1; }

            if ( ReferenceEquals(null, x) ) { return -1; }

            return x.CompareTo(y);
        }


        public int Compare( object? x, object? y )
        {
            if ( x is not T left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(T)); }

            if ( y is not T right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(T)); }

            return Compare(left, right);
        }
    }
}
