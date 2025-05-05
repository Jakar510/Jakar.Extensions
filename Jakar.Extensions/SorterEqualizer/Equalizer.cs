namespace Jakar.Extensions;


public sealed class ValueEqualizer<TValue> : IEqualityComparer<TValue?>, IEqualityComparer<TValue>, IEqualityComparer
    where TValue : struct, IEquatable<TValue>
{
    public static readonly ValueEqualizer<TValue> Default = new();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.Equals( right );
    }
    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    public bool Equals( TValue?      left, TValue? right ) => Nullable.Equals( left, right );
    public int  GetHashCode( TValue? obj )                => obj.GetHashCode();
    public bool Equals( TValue       left, TValue right ) => left.Equals( right );
    public int  GetHashCode( TValue  obj ) => obj.GetHashCode();
}



public sealed class Equalizer<TValue> : IEqualityComparer<TValue>, IEqualityComparer
    where TValue : class, IEquatable<TValue>
{
    public static readonly Equalizer<TValue> Default = new();


    bool IEqualityComparer.Equals( object? x, object? y )
    {
        if ( x is not TValue left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(TValue) ); }

        if ( y is not TValue right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(TValue) ); }

        return left.Equals( right );
    }
    int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();


    public bool Equals( TValue? left, TValue? right )
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if ( left is null && right is null ) { return true; }

        if ( left is null || right is null ) { return false; }

        return ReferenceEquals( left, right ) || left.Equals( right );
    }


    public int GetHashCode( TValue obj ) => obj.GetHashCode();
}
