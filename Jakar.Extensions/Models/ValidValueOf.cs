﻿// Jakar.Extensions :: Jakar.Extensions
// 04/11/2022  9:54 AM

#nullable enable
using System.Linq.Expressions;



namespace Jakar.Extensions;


/// <summary> Inspired by <see cref="ValueOf{TValue,TThis}"/> </summary>
/// <typeparam name="TValue"> </typeparam>
/// <typeparam name="TThis"> </typeparam>
[SuppressMessage( "ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract" )]
public abstract class ValidValueOf<TValue, TThis> : IComparable<ValidValueOf<TValue, TThis>>, IEquatable<ValidValueOf<TValue, TThis>>, IComparable where TThis : ValidValueOf<TValue, TThis>, new()
                                                                                                                                                   where TValue : IComparable<TValue>, IEquatable<TValue>
{
    public TValue Value { get; protected set; }

    // ReSharper disable once NullableWarningSuppressionIsUsed
    protected ValidValueOf() => Value = default!;
    private static bool Equals( TValue? left, TValue? right )
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if ( left is null && right is null ) { return true; }

        if ( left is null ) { return false; }

        if ( right is null ) { return false; }

        if ( ReferenceEquals( left, right ) ) { return true; }

        return left.Equals( right );
    }


    public static bool operator ==( ValidValueOf<TValue, TThis> left, ValidValueOf<TValue, TThis> right ) => Equals( left, right );
    public static bool operator >( ValidValueOf<TValue, TThis>  left, ValidValueOf<TValue, TThis> right ) => Compare( left.Value, right.Value ) > 0;
    public static bool operator >=( ValidValueOf<TValue, TThis> left, ValidValueOf<TValue, TThis> right ) => Compare( left.Value, right.Value ) >= 0;
    public static bool operator !=( ValidValueOf<TValue, TThis> left, ValidValueOf<TValue, TThis> right ) => !Equals( left, right );
    public static bool operator <( ValidValueOf<TValue, TThis>  left, ValidValueOf<TValue, TThis> right ) => Compare( left.Value, right.Value ) < 0;
    public static bool operator <=( ValidValueOf<TValue, TThis> left, ValidValueOf<TValue, TThis> right ) => Compare( left.Value, right.Value ) <= 0;

    public static bool TryCreate( TValue item, [NotNullWhen( true )] out TThis? thisValue )
    {
        TThis self = _factory();
        self.Value = item;

        thisValue = self.IsValid()
                        ? self
                        : default;

        return thisValue is not null;
    }
    protected static readonly Func<TThis> _factory = (Func<TThis>)Expression.Lambda( typeof(Func<TThis>),
                                                                                     Expression.New( typeof(TThis).GetTypeInfo()
                                                                                                                  .DeclaredConstructors.First( x => x.GetParameters()
                                                                                                                                                     .IsEmpty() ),
                                                                                                     Array.Empty<Expression>() ) )
                                                                            .Compile();

    private static int Compare( TValue? left, TValue? right )
    {
        if ( left is null ) { return 1; }

        if ( right is null ) { return -1; }

        if ( ReferenceEquals( left, right ) ) { return 0; }

        return left.CompareTo( right );
    }


    public static TThis Create( TValue item )
    {
        TThis self = _factory();
        self.Value = item;
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
}
