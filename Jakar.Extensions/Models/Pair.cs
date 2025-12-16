// Jakar.Extensions :: Jakar.Extensions
// 03/13/2025  13:03

namespace Jakar.Extensions;


[method: JsonConstructor]
public readonly struct Pair( string key, string? value ) : IEquatable<Pair>, IComparable<Pair>, IComparable, ISpanFormattable
{
    public readonly string  Key   = key;
    public readonly string? Value = value;


    public bool HasValue { [MemberNotNullWhen(true, nameof(Value))] get => !string.IsNullOrWhiteSpace(Value); }


    public static implicit operator KeyValuePair<string, string?>( Pair tag ) => new(tag.Key, tag.Value);
    public static implicit operator (string Key, string? Value)( Pair   tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair( KeyValuePair<string, string?> tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair( (string Key, string? Value)   tag ) => new(tag.Key, tag.Value);


    public int CompareTo( Pair other )
    {
        int keyComparison = string.Compare(Key, other.Key, StringComparison.Ordinal);
        if ( keyComparison != 0 ) { return keyComparison; }

        return string.Compare(Value, other.Value, StringComparison.Ordinal);
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is Pair pair
                   ? CompareTo(pair)
                   : throw new ArgumentException($"Object must be of type {nameof(Pair)}");
    }
    public          bool Equals( Pair    other ) => string.Equals(Key, other.Key, StringComparison.Ordinal) && string.Equals(Value, other.Value, StringComparison.Ordinal);
    public override bool Equals( object? obj )   => obj is Pair other                                       && Equals(other);
    public override int  GetHashCode()           => HashCode.Combine(Key, Value);


    public static bool operator ==( Pair? left, Pair? right ) => Nullable.Equals(left, right);
    public static bool operator !=( Pair? left, Pair? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( Pair  left, Pair  right ) => left.Equals(right);
    public static bool operator !=( Pair  left, Pair  right ) => !left.Equals(right);
    public static bool operator <( Pair   left, Pair  right ) => left.CompareTo(right) < 0;
    public static bool operator >( Pair   left, Pair  right ) => left.CompareTo(right) > 0;
    public static bool operator <=( Pair  left, Pair  right ) => left.CompareTo(right) <= 0;
    public static bool operator >=( Pair  left, Pair  right ) => left.CompareTo(right) >= 0;


    public void Deconstruct( out string key, out string? value )
    {
        key   = Key;
        value = Value;
    }
    public override string ToString() => $"{nameof(Key)}: {Key}, {nameof(Value)}: {Value}";
    public string ToString( string? format, IFormatProvider? formatProvider ) => string.Equals("json", format, StringComparison.OrdinalIgnoreCase)
                                                                                     ? this.ToJson()
                                                                                     : ToString();
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider ) => destination.TryWrite(provider, $"{nameof(Key)}: {Key}, {nameof(Value)}: {Value}", out charsWritten);
}



[method: JsonConstructor]
public readonly struct Pair<TValue>( string key, TValue value ) : IEquatable<Pair<TValue>>, IComparable<Pair<TValue>>, IComparable
    where TValue : IComparable<TValue>, IEquatable<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    public readonly string Key   = key;
    public readonly TValue Value = value;


    public bool HasValue { [MemberNotNullWhen(true, nameof(Value))] get => Value is not null; }


    public static implicit operator KeyValuePair<string, TValue>( Pair<TValue> tag ) => new(tag.Key, tag.Value);
    public static implicit operator (string Key, TValue Value)( Pair<TValue>   tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair<TValue>( KeyValuePair<string, TValue> tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair<TValue>( (string Key, TValue Value)   tag ) => new(tag.Key, tag.Value);


    public void Deconstruct( out string key, out TValue value )
    {
        key   = Key;
        value = Value;
    }
    public int CompareTo( Pair<TValue> other )
    {
        int keyComparison = string.Compare(Key, other.Key, StringComparison.Ordinal);
        if ( keyComparison != 0 ) { return keyComparison; }

        return Value.CompareTo(other.Value);
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is Pair<TValue> pair
                   ? CompareTo(pair)
                   : throw new ArgumentException($"Object must be of type {nameof(Pair)}");
    }
    public          bool Equals( Pair<TValue> other ) => string.Equals(Key, other.Key, StringComparison.Ordinal) && Value.Equals(other.Value);
    public override bool Equals( object?      obj )   => obj is Pair<TValue> other                               && Equals(other);
    public override int  GetHashCode()                => HashCode.Combine(Key, Value);


    public static bool operator ==( Pair<TValue>? left, Pair<TValue>? right ) => Nullable.Equals(left, right);
    public static bool operator !=( Pair<TValue>? left, Pair<TValue>? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( Pair<TValue>  left, Pair<TValue>  right ) => left.Equals(right);
    public static bool operator !=( Pair<TValue>  left, Pair<TValue>  right ) => !left.Equals(right);
    public static bool operator <( Pair<TValue>   left, Pair<TValue>  right ) => left.CompareTo(right) < 0;
    public static bool operator >( Pair<TValue>   left, Pair<TValue>  right ) => left.CompareTo(right) > 0;
    public static bool operator <=( Pair<TValue>  left, Pair<TValue>  right ) => left.CompareTo(right) <= 0;
    public static bool operator >=( Pair<TValue>  left, Pair<TValue>  right ) => left.CompareTo(right) >= 0;
}
