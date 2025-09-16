// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  16:35

namespace Jakar.Extensions;


public sealed class EqualComparer<TValue> : IComparer<TValue>, IComparer, IEqualityComparer<TValue>, IEqualityComparer
    where TValue : IComparable<TValue>, IEquatable<TValue>
{
    public static readonly EqualComparer<TValue> Default = new();


    public int Compare( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.CompareTo( right );
    }

    public int Compare( TValue? left, TValue? right )
    {
        if ( left is IStructuralComparable comparable ) { return comparable.CompareTo( right, this ); }

        if ( typeof(TValue).IsByRef && ReferenceEquals( left, right ) ) { return 0; }

        if ( left is null && right is null ) { return 0; }

        if ( left is null ) { return 1; }

        if ( right is null ) { return -1; }

        return left.CompareTo( right );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining )] public bool LessThan( TValue?                  left, TValue? right ) => Compare( left, right ) < 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public bool LessThanOrEqualTo( TValue?         left, TValue? right ) => Compare( left, right ) <= 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public bool GreaterThan( TValue?               left, TValue? right ) => Compare( left, right ) > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public bool GreaterThanOrEqualTo( TValue?      left, TValue? right ) => Compare( left, right ) >= 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public int  GetHashCode( [DisallowNull] TValue obj )                 => obj.GetHashCode();
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public bool DoesNotEqual( TValue?              left, TValue? right ) => !Equals( left, right );
    public bool Equals( TValue? left, TValue? right )
    {
        if ( left is IStructuralEquatable comparable ) { return comparable.Equals( right, this ); }

        if ( typeof(TValue).IsByRef && ReferenceEquals( left, right ) ) { return true; }

        if ( left is null && right is null ) { return true; }

        if ( left is null || right is null ) { return false; }

        return left.Equals( right );
    }
    public bool Equals( ReadOnlySpan<TValue> left, ReadOnlySpan<TValue> right ) => left.SequenceEqual( right );


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return Equals( left, right );
    }

    int IEqualityComparer.GetHashCode( object obj )
    {
        if ( obj is not TValue value ) { throw new ExpectedValueTypeException( nameof(obj), obj, typeof(TValue) ); }

        return GetHashCode( value );
    }
}
