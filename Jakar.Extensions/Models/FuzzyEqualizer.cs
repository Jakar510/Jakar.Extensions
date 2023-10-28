namespace Jakar.Extensions;


public interface IFuzzyEquals<T> : IEquatable<T>
{
    public bool FuzzyEquals( T other );
}



public sealed class ValueFuzzyEqualizer<T> : IEqualityComparer<T?>, IEqualityComparer<T>, IEqualityComparer where T : struct, IFuzzyEquals<T>
{
    public static ValueFuzzyEqualizer<T> Default { get; } = new();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not T left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(T) ); }

        if ( y is not T right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(T) ); }

        return left.Equals( right );
    }

    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    public bool Equals( T? left, T? right )
    {
        if ( left.HasValue && right.HasValue ) { left.Value.FuzzyEquals( right.Value ); }

        if ( left is null && right is null ) { return true; }

        return false;
    }
    public int GetHashCode( T? obj ) => obj.GetHashCode();

    public bool Equals( T left, T right ) => left.FuzzyEquals( right );


    public int GetHashCode( T obj ) => obj.GetHashCode();
}



public sealed class FuzzyEqualizer<T> : IEqualityComparer<T>, IEqualityComparer where T : class, IFuzzyEquals<T>
{
    public static FuzzyEqualizer<T> Default { get; } = new();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not T left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(T) ); }

        if ( y is not T right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(T) ); }

        return left.Equals( right );
    }

    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    public bool Equals( T? left, T? right )
    {
        if ( left is null && right is null ) { return true; }

        if ( left is null || right is null ) { return false; }

        return left.FuzzyEquals( right );
    }


    public int GetHashCode( T obj ) => obj.GetHashCode();
}
