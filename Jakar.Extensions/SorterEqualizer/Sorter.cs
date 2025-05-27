namespace Jakar.Extensions;


public sealed class ValueSorter<TValue> : IComparer<TValue?>, IComparer<TValue>, IComparer
    where TValue : struct, IComparable<TValue>
{
    public static readonly ValueSorter<TValue> Default = new();


    public int Compare( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.CompareTo( right );
    }
    public int Compare( TValue? left, TValue? right ) => left.HasValue
                                                             ? right.HasValue
                                                                   ? Compare( left.Value, right.Value )
                                                                   : 1
                                                             : right.HasValue
                                                                 ? -1
                                                                 : 0;
    public int Compare( TValue left, TValue right ) => left.CompareTo( right );
}



public sealed class Sorter<TValue> : IComparer<TValue>, IComparer
    where TValue : class, IComparable<TValue>
{
    public static readonly Sorter<TValue> Default = new();


    public int Compare( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.CompareTo( right );
    }
    public int Compare( TValue? left, TValue? right )
    {
        if ( left is null ) { return 1; }

        if ( right is null ) { return NOT_FOUND; }

        if ( ReferenceEquals( left, right ) ) { return 0; }

        return left.CompareTo( right );
    }
}
