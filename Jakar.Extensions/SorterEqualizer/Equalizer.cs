namespace Jakar.Extensions;


public sealed class ValueEqualizer<T> : IEqualityComparer<T?>, IEqualityComparer<T>, IEqualityComparer
    where T : struct, IEquatable<T>
{
    public static readonly ValueEqualizer<T> Default = new();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not T left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(T) ); }

        if ( y is not T right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(T) ); }

        return left.Equals( right );
    }
    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    public bool Equals( T?      left, T? right ) => Nullable.Equals( left, right );
    public int  GetHashCode( T? obj )           => obj.GetHashCode();
    public bool Equals( T       left, T right ) => left.Equals( right );
    public int  GetHashCode( T  obj ) => obj.GetHashCode();
}



public sealed class Equalizer<T> : IEqualityComparer<T>, IEqualityComparer
    where T : class, IEquatable<T>
{
    public static readonly Equalizer<T> Default = new();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not T left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(T) ); }

        if ( y is not T right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(T) ); }

        return left.Equals( right );
    }
    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    public bool Equals( T? left, T? right )
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if ( left is null && right is null ) { return true; }

        if ( left is null || right is null ) { return false; }

        return ReferenceEquals( left, right ) || left.Equals( right );
    }


    public int GetHashCode( T obj ) => obj.GetHashCode();
}
