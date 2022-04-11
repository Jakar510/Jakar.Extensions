// Jakar.Extensions :: Jakar.Extensions
// 04/11/2022  9:54 AM

using System.Linq.Expressions;



namespace Jakar.Extensions.Models;


/// <summary>
/// Inspired by <see cref="ValueOf{TValue,TThis}"/>
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TThis"></typeparam>
public abstract class ValidValueOf<TValue, TThis> : IComparable<ValidValueOf<TValue, TThis>>, IEquatable<ValidValueOf<TValue, TThis>>, IComparable where TThis : ValidValueOf<TValue, TThis>, new()
                                                                                                                                                   where TValue : IComparable<TValue>, IEquatable<TValue>
{
    protected static readonly Func<TThis> _factory = (Func<TThis>)Expression.Lambda(typeof(Func<TThis>), Expression.New(typeof(TThis).GetTypeInfo().DeclaredConstructors.First<ConstructorInfo>(), Array.Empty<Expression>())).Compile();


    public TValue Value { get; protected set; }


    public static TThis From( TValue item )
    {
        TThis @this = _factory();
        @this.Value = item;
        @this.Validate();
        return @this;
    }

    public static bool TryFrom( TValue item, out TThis? thisValue )
    {
        TThis? @this = _factory();
        @this.Value = item;

        thisValue = @this.TryValidate()
                        ? @this
                        : default;

        return thisValue is not null;
    }


    [DoesNotReturn] protected virtual void ThrowError() => throw new FormatException($"Provided Value was in the wrong format: '{Value}'");

    protected void Validate()
    {
        if ( !TryValidate() ) { ThrowError(); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns><see langword="true"/> if <see cref="Value"/> is valid, otherwise <see langword="false"/></returns>
    protected abstract bool TryValidate();


    public virtual bool Equals( ValidValueOf<TValue, TThis> other ) => Value.Equals(other.Value);

    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return other is ValidValueOf<TValue, TThis> value && Equals(value);
    }

    public override int GetHashCode() => Value.GetHashCode();


    public int CompareTo( ValidValueOf<TValue, TThis> other ) => Comparer<TValue>.Default.Compare(Value, other.Value);
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is ValidValueOf<TValue, TThis> value
                   ? CompareTo(value)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(ValidValueOf<TValue, TThis>));
    }


    public static bool operator ==( ValidValueOf<TValue, TThis>  left, ValidValueOf<TValue, TThis>  right ) => Equalizer.Instance.Equals(left, right);
    public static bool operator !=( ValidValueOf<TValue, TThis>  left, ValidValueOf<TValue, TThis>  right ) => !Equalizer.Instance.Equals(left, right);
    public static bool operator <( ValidValueOf<TValue, TThis>?  left, ValidValueOf<TValue, TThis>? right ) => Sorter.Instance.Compare(left, right) < 0;
    public static bool operator >( ValidValueOf<TValue, TThis>?  left, ValidValueOf<TValue, TThis>? right ) => Sorter.Instance.Compare(left, right) > 0;
    public static bool operator <=( ValidValueOf<TValue, TThis>? left, ValidValueOf<TValue, TThis>? right ) => Sorter.Instance.Compare(left, right) <= 0;
    public static bool operator >=( ValidValueOf<TValue, TThis>? left, ValidValueOf<TValue, TThis>? right ) => Sorter.Instance.Compare(left, right) >= 0;


    public override string ToString() => Value.ToString();



    public class Sorter : Sorter<ValidValueOf<TValue, TThis>> { }



    public class Equalizer : Equalizer<ValidValueOf<TValue, TThis>> { }
}
