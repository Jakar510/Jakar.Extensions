namespace Jakar.Extensions;


public sealed class ValueSorter<T> : IComparer<T?>, IComparer<T>, IComparer
    where T : struct, IComparable<T>
{
    public static ValueSorter<T> Default { get; } = new();


    public int Compare( object? x, object? y )
    {
        if ( x is not T left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(T) ); }

        if ( y is not T right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(T) ); }

        return left.CompareTo( right );
    }
    public int Compare( T? left, T? right ) => Nullable.Compare( left, right );
    public int Compare( T  left, T  right ) => left.CompareTo( right );
}



public sealed class Sorter<T> : IComparer<T>, IComparer
    where T : class, IComparable<T>
{
    public static Sorter<T> Default { get; } = new();


    public int Compare( object? x, object? y )
    {
        if ( x is not T left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(T) ); }

        if ( y is not T right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(T) ); }

        return left.CompareTo( right );
    }
    public int Compare( T? left, T? right )
    {
        if ( left is null ) { return 1; }

        if ( right is null ) { return NOT_FOUND; }

        if ( ReferenceEquals( left, right ) ) { return 0; }

        return left.CompareTo( right );
    }
}
