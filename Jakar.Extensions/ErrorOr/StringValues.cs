// Jakar.Extensions :: Jakar.Extensions
// 09/19/2025  18:42

using ZLinq;
using ZLinq.Linq;



namespace Jakar.Extensions;


[DefaultValue(nameof(Empty))]
[method: JsonConstructor]
public readonly struct StringTags( Pair[]? tags, string[]? values ) : IValueEnumerable<FromArray<string>, string>, IValueEnumerable<FromArray<Pair>, Pair>, IEquatable<StringTags>
{
    public static readonly StringTags Empty  = new(null, null);
    public readonly        Pair[]     Tags   = tags   ?? [];
    public readonly        string[]   Values = values ?? [];


    public StringTags( Pair     pair ) : this([pair], null) { }
    public StringTags( Pair[]   pairs ) : this(pairs, null) { }
    public StringTags( string[] values ) : this(null, values) { }
    public StringTags( string   value ) : this(null, [value]) { }


    public static implicit operator StringTags( Pair     value )  => new([value]);
    public static implicit operator StringTags( Pair[]   values ) => new(values);
    public static implicit operator StringTags( string   value )  => new([value]);
    public static implicit operator StringTags( string[] values ) => new(values);
    public static implicit operator string[]( StringTags value )  => value.Values;
    public static implicit operator Pair[]( StringTags   value )  => value.Tags;


    public ValueEnumerable<FromArray<string>, string>                                      EnumerateValues()   => new(new FromArray<string>(Values ?? []));
    public ValueEnumerable<FromArray<Pair>, Pair>                                          EnumerateTags()     => new(new FromArray<Pair>(Tags     ?? []));
    ValueEnumerable<FromArray<string>, string> IValueEnumerable<FromArray<string>, string>.AsValueEnumerable() => EnumerateValues();
    ValueEnumerable<FromArray<Pair>, Pair> IValueEnumerable<FromArray<Pair>, Pair>.        AsValueEnumerable() => EnumerateTags();


    public          bool Equals( StringTags other ) => Tags.Equals(other.Tags) && Values.Equals(other.Values);
    public override bool Equals( object?    obj )   => obj is StringTags other && Equals(other);
    public override int  GetHashCode()              { return HashCode.Combine(Tags, Values); }


    public static bool operator ==( StringTags left, StringTags right ) => Equals(left, right);
    public static bool operator !=( StringTags left, StringTags right ) => !Equals(left, right);
}
