namespace Jakar.Extensions;


public interface IFuzzyEquals<TValue> : IEquatable<TValue>
{
    public bool FuzzyEquals( TValue other );
}



public sealed class ValueFuzzyEqualizer<TValue> : IEqualityComparer<TValue?>, IEqualityComparer<TValue>, IEqualityComparer
    where TValue : struct, IFuzzyEquals<TValue>
{
    public static readonly ValueFuzzyEqualizer<TValue> Default = new();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.Equals( right );
    }

    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    public bool Equals( TValue? left, TValue? right )
    {
        if ( left.HasValue && right.HasValue ) { left.Value.FuzzyEquals( right.Value ); }

        if ( left is null && right is null ) { return true; }

        return false;
    }
    public int GetHashCode( TValue? obj ) => obj.GetHashCode();

    public bool Equals( TValue left, TValue right ) => left.FuzzyEquals( right );


    public int GetHashCode( TValue obj ) => obj.GetHashCode();
}



public sealed class FuzzyEqualizer<TValue> : IEqualityComparer<TValue>, IEqualityComparer
    where TValue : class, IFuzzyEquals<TValue>
{
    public static readonly FuzzyEqualizer<TValue> Default = new();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.Equals( right );
    }

    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    public bool Equals( TValue? left, TValue? right )
    {
        if ( left is null && right is null ) { return true; }

        if ( left is null || right is null ) { return false; }

        return left.FuzzyEquals( right );
    }


    public int GetHashCode( TValue obj ) => obj.GetHashCode();
}
