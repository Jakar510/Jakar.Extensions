// Jakar.Extensions :: Jakar.Extensions
// 04/11/2022  9:54 AM

namespace Jakar.Extensions;


/// <summary> Inspired by <see cref="ValueOf{TValue,TThis}"/> </summary>
/// <typeparam name="TValue"> </typeparam>
/// <typeparam name="TThis"> </typeparam>
[SuppressMessage( "ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract" )]
public abstract class ValidValueOf<TValue, TThis> : IComparable<ValidValueOf<TValue, TThis>>, IEquatable<ValidValueOf<TValue, TThis>>, IComparable, IValidator
    where TThis : ValidValueOf<TValue, TThis>, new()
    where TValue : IComparable<TValue>, IEquatable<TValue>
{
    bool IValidator.IsValid => IsValid();
    public TValue   Value   { get; protected set; } = default!;


    public static bool TryCreate( TValue value, [NotNullWhen( true )] out TThis? thisValue )
    {
        TThis self = new TThis { Value = value };

        thisValue = self.IsValid()
                        ? self
                        : default;

        return thisValue is not null;
    }


    public static TThis Create( TValue item )
    {
        TThis self = new TThis { Value = item };

        self.Validate();
        return self;
    }
    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return other is ValidValueOf<TValue, TThis> value && Equals( value );
    }

    /// <summary> </summary>
    /// <returns> <see langword="true"/> if <see cref="Value"/> is valid, otherwise <see langword="false"/> </returns>
    protected abstract bool IsValid();

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public override string? ToString() => Value?.ToString();


    [DoesNotReturn] protected virtual void ThrowError() => throw new FormatException( $"Provided Value was in the wrong format: '{Value}'" );

    protected void Validate()
    {
        if ( !IsValid() ) { ThrowError(); }
    }


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is ValidValueOf<TValue, TThis> value
                   ? CompareTo( value )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(ValidValueOf<TValue, TThis>) );
    }


    public int CompareTo( ValidValueOf<TValue, TThis>? other )
    {
        if ( other is null ) { return 1; }

        return Compare( Value, other.Value );
    }
    public virtual bool Equals( ValidValueOf<TValue, TThis>? other )
    {
        if ( other is null ) { return false; }

        return Equals( Value, other.Value );
    }


    private static bool Equals( TValue? left, TValue? right )
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if ( left is null && right is null ) { return true; }

        if ( left is null ) { return false; }

        if ( right is null ) { return false; }

        if ( ReferenceEquals( left, right ) ) { return true; }

        return left.Equals( right );
    }

    private static int Compare( TValue? left, TValue? right )
    {
        if ( left is null ) { return 1; }

        if ( right is null ) { return NOT_FOUND; }

        if ( ReferenceEquals( left, right ) ) { return 0; }

        return left.CompareTo( right );
    }
    public static bool operator ==( ValidValueOf<TValue, TThis> left, ValidValueOf<TValue, TThis> right ) => Equals( left, right );
    public static bool operator >( ValidValueOf<TValue, TThis>  left, ValidValueOf<TValue, TThis> right ) => Compare( left.Value, right.Value ) > 0;
    public static bool operator >=( ValidValueOf<TValue, TThis> left, ValidValueOf<TValue, TThis> right ) => Compare( left.Value, right.Value ) >= 0;
    public static bool operator !=( ValidValueOf<TValue, TThis> left, ValidValueOf<TValue, TThis> right ) => !Equals( left, right );
    public static bool operator <( ValidValueOf<TValue, TThis>  left, ValidValueOf<TValue, TThis> right ) => Compare( left.Value, right.Value ) < 0;
    public static bool operator <=( ValidValueOf<TValue, TThis> left, ValidValueOf<TValue, TThis> right ) => Compare( left.Value, right.Value ) <= 0;
}
