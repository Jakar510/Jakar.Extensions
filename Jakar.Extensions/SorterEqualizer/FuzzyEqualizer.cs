namespace Jakar.Extensions;


public interface IFuzzyEquals<TValue> : IEqualComparable<TValue>
    where TValue : IFuzzyEquals<TValue>
{
    public abstract static FuzzyEqualizer<TValue> FuzzyEqualizer { get; }
    public                 bool                   FuzzyEquals( TValue other );
}



public sealed class FuzzyEqualizer<TValue> : IEqualityComparer<TValue>, IEqualityComparer
    where TValue : IFuzzyEquals<TValue>
{
    public static readonly  FuzzyEqualizer<TValue> Default    = new(); 


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.Equals( right );
    }

    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    public bool Equals( TValue? left, TValue? right )
    {
        if (  typeof(TValue).IsByRef && ReferenceEquals( left, right ) ) { return true; }

        if ( left is null && right is null ) { return true; }

        if ( left is null || right is null ) { return false; }

        return left.FuzzyEquals( right );
    }


    public int GetHashCode( TValue obj ) => obj.GetHashCode();
}
