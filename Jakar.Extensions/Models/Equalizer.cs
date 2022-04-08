namespace Jakar.Extensions.Models;


public class ValueEqualizer<T> : IEqualityComparer<T?>, IEqualityComparer<T>, IEqualityComparer where T : struct, IComparable<T>
{
    public static ValueEqualizer<T> Instance { get; } = new();


    public bool Equals( T? left, T? right ) => Nullable.Equals(left, right);
    public bool Equals( T  left, T  right ) => left.Equals(right);


    public int GetHashCode( T  obj ) => obj.GetHashCode();
    public int GetHashCode( T? obj ) => obj.GetHashCode();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not T left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(T)); }

        if ( y is not T right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(T)); }

        return left.Equals(right);
    }
    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();
}



public class Equalizer<T> : IEqualityComparer<T>, IEqualityComparer where T : class, IComparable<T>
{
    public static Equalizer<T> Instance { get; } = new();


    public bool Equals( T? left, T? right )
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if ( left is null && right is null ) { return true; }

        if ( left is null ) { return false; }

        if ( right is null ) { return false; }

        if ( ReferenceEquals(left, right) ) { return true; }

        return left.Equals(right);
    }


    public int GetHashCode( T obj ) => obj.GetHashCode();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not T left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(T)); }

        if ( y is not T right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(T)); }

        return left.Equals(right);
    }

    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();
}