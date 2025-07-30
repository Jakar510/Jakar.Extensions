namespace Jakar.Extensions;


public sealed class ValueSorter<TValue> : IComparer<TValue?>, IComparer<TValue>, IComparer, IEqualityComparer<TValue?>, IEqualityComparer<TValue>, IEqualityComparer
    where TValue : struct, IComparable<TValue>, IEquatable<TValue>
{
    public static readonly ValueSorter<TValue> Default = new();


    public int Compare( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.CompareTo( right );
    }
    public int Compare( TValue? left, TValue? right )
    {
        if ( left is IStructuralComparable comparable ) { return comparable.CompareTo( right, this ); }

        return left.HasValue
                   ? right.HasValue
                         ? Compare( left.Value, right.Value )
                         : 1
                   : right.HasValue
                       ? -1
                       : 0;
    }
    public int Compare( TValue left, TValue right ) => left.CompareTo( right );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool LessThan( TValue              left, TValue right ) => Compare( left, right ) < 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool LessThanOrEqualTo( TValue     left, TValue right ) => Compare( left, right ) <= 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool GreaterThan( TValue           left, TValue right ) => Compare( left, right ) > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool GreaterThanOrEqualTo( TValue  left, TValue right ) => Compare( left, right ) >= 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int  GetHashCode( TValue?          obj )                => obj.GetHashCode();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Equals( TValue                left, TValue right ) => left.Equals( right );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int  GetHashCode( TValue           obj )                               => obj.GetHashCode();
    public                                                      bool Equals( ReadOnlySpan<TValue>  left, ReadOnlySpan<TValue>  right ) => left.SequenceEqual( right );
    public                                                      bool Equals( ReadOnlySpan<TValue?> left, ReadOnlySpan<TValue?> right ) => left.SequenceEqual( right );

    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.Equals( right );
    }
    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool DoesNotEqual( TValue  left, TValue  right ) => Equals( left, right ) is false;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool DoesNotEqual( TValue? left, TValue? right ) => Equals( left, right ) is false;
    public bool Equals( TValue? left, TValue? right )
    {
        if ( left is IStructuralEquatable comparable ) { return comparable.Equals( right, this ); }

        return left.HasValue
                   ? right.HasValue && Equals( left.Value, right.Value )
                   : right is null;
    }
}