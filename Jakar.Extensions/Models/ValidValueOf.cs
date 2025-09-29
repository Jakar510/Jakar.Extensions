// Jakar.Extensions :: Jakar.Extensions
// 04/11/2022  9:54 AM

namespace Jakar.Extensions;


/// <summary> Inspired by <see cref="ValueOf{TValue,TThis}"/> </summary>
/// <typeparam name="TValue"> </typeparam>
/// <typeparam name="TClass"> </typeparam>
[SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
public abstract class ValidValueOf<TClass, TValue> : IComparable<ValidValueOf<TClass, TValue>>, IEquatable<ValidValueOf<TClass, TValue>>, IComparable, IValidator
    where TClass : ValidValueOf<TClass, TValue>, new()
    where TValue : IComparable<TValue>, IEquatable<TValue>
{
    bool IValidator.IsValid => IsValid();
    public TValue   Value   { get; protected set; } = default!;


    public static bool TryCreate( TValue value, [NotNullWhen(true)] out TClass? thisValue )
    {
        TClass self = new() { Value = value };

        thisValue = self.IsValid()
                        ? self
                        : null;

        return thisValue is not null;
    }


    public static TClass Create( TValue item )
    {
        TClass self = new() { Value = item };

        self.Validate();
        return self;
    }

    /// <summary> </summary>
    /// <returns> <see langword="true"/> if <see cref="Value"/> is valid, otherwise <see langword="false"/> </returns>
    protected abstract bool IsValid();
    protected virtual bool IsValid( [NotNullWhen(true)] out TValue? value )
    {
        value = IsValid()
                    ? Value
                    : default;

        return value is not null;
    }
    public override int     GetHashCode() => HashCode.Combine(Value);
    public override string? ToString()    => Value?.ToString();


    [DoesNotReturn] protected virtual void ThrowError() => throw new FormatException($"Provided Value was in the wrong format: '{Value}'");

    protected virtual void Validate()
    {
        if ( !IsValid() ) { ThrowError(); }
    }


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is ValidValueOf<TClass, TValue> value
                   ? CompareTo(value)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(ValidValueOf<TClass, TValue>));
    }
    public int CompareTo( ValidValueOf<TClass, TValue>? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return Compare(Value, other.Value);
    }
    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return other is ValidValueOf<TClass, TValue> value && Equals(value);
    }
    public virtual bool Equals( ValidValueOf<TClass, TValue>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Equals(Value, other.Value);
    }


    private static bool Equals( TValue? left, TValue? right )
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if ( left is null && right is null ) { return true; }

        if ( left is null ) { return false; }

        if ( right is null ) { return false; }

        if ( typeof(TValue).IsByRef && ReferenceEquals(left, right) ) { return true; }

        return left.Equals(right);
    }
    private static int Compare( TValue? left, TValue? right )
    {
        if ( left is null ) { return 1; }

        if ( right is null ) { return NOT_FOUND; }

        if ( typeof(TValue).IsByRef && ReferenceEquals(left, right) ) { return 0; }

        return left.CompareTo(right);
    }


    public static bool operator ==( ValidValueOf<TClass, TValue> left, ValidValueOf<TClass, TValue> right ) => EqualityComparer<TValue>.Default.Equals(left.Value, right.Value);
    public static bool operator !=( ValidValueOf<TClass, TValue> left, ValidValueOf<TClass, TValue> right ) => !EqualityComparer<TValue>.Default.Equals(left.Value, right.Value);
    public static bool operator >( ValidValueOf<TClass, TValue>  left, ValidValueOf<TClass, TValue> right ) => Comparer<TValue>.Default.Compare(left.Value, right.Value) > 0;
    public static bool operator >=( ValidValueOf<TClass, TValue> left, ValidValueOf<TClass, TValue> right ) => Comparer<TValue>.Default.Compare(left.Value, right.Value) >= 0;
    public static bool operator <( ValidValueOf<TClass, TValue>  left, ValidValueOf<TClass, TValue> right ) => Comparer<TValue>.Default.Compare(left.Value, right.Value) < 0;
    public static bool operator <=( ValidValueOf<TClass, TValue> left, ValidValueOf<TClass, TValue> right ) => Comparer<TValue>.Default.Compare(left.Value, right.Value) <= 0;
}
