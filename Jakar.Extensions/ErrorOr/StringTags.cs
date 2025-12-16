// Jakar.Extensions :: Jakar.Extensions
// 09/19/2025  18:42

using Microsoft.Extensions.Primitives;
using ZLinq;
using ZLinq.Linq;



namespace Jakar.Extensions;


[DefaultValue(nameof(Empty))]
[method: JsonConstructor]
public readonly struct StringTags( Pair[] tags, string[] entries ) : IValueEnumerable<FromArray<string>, string>, IValueEnumerable<FromArray<Pair>, Pair>, IEquatable<StringTags>, ISpanFormattable
{
    public static readonly StringTags Empty   = new();
    public readonly        Pair[]     Tags    = tags;
    public readonly        string[]   Entries = entries;


    public bool IsEmpty { [MemberNotNullWhen(false, nameof(Tags))] [MemberNotNullWhen(false, nameof(Entries))] get => Tags?.Length is null or <= 0 && Entries?.Length is null or <= 0; }


    public StringTags() : this([], []) { }
    public StringTags( Pair     pair ) : this([pair], []) { }
    public StringTags( Pair[]   pairs ) : this(pairs, []) { }
    public StringTags( string[] entries ) : this([], entries) { }
    public StringTags( string   value ) : this([], [value]) { }
    public StringTags( ref readonly StringValues entries ) : this([],
                                                                  entries.Count > 1
                                                                      ? (string[])entries.ToArray()!
                                                                      : [entries.ToString()]) { }


    public static implicit operator StringTags( (Pair[] Tags, string[] Entries) pair )    => new(pair.Tags, pair.Entries);
    public static implicit operator StringTags( StringValues                    entries ) => new(in entries);
    public static implicit operator StringTags( Pair                            pair )    => new([pair]);
    public static implicit operator StringTags( Span<Pair>                      pair )    => new([..pair]);
    public static implicit operator StringTags( ReadOnlySpan<Pair>              pair )    => new([..pair]);
    public static implicit operator StringTags( Pair[]                          entries ) => new(entries);
    public static implicit operator StringTags( string                          entry )   => new([entry]);
    public static implicit operator StringTags( Span<string>                    entry )   => new([..entry]);
    public static implicit operator StringTags( ReadOnlySpan<string>            entry )   => new([..entry]);
    public static implicit operator StringTags( string[]                        entries ) => new(entries);
    public static implicit operator ReadOnlySpan<string>( StringTags            value )   => new(value.Entries);
    public static implicit operator ReadOnlySpan<Pair>( StringTags              value )   => new(value.Tags);
    public static implicit operator ReadOnlyMemory<string>( StringTags          value )   => new(value.Entries);
    public static implicit operator ReadOnlyMemory<Pair>( StringTags            value )   => new(value.Tags);


    public ValueEnumerable<FromArray<string>, string>                                      Values()            => new(new FromArray<string>(Entries ?? []));
    public ValueEnumerable<FromArray<Pair>, Pair>                                          Pairs()             => new(new FromArray<Pair>(Tags      ?? []));
    ValueEnumerable<FromArray<string>, string> IValueEnumerable<FromArray<string>, string>.AsValueEnumerable() => Values();
    ValueEnumerable<FromArray<Pair>, Pair> IValueEnumerable<FromArray<Pair>, Pair>.        AsValueEnumerable() => Pairs();


    [Pure] public StringTags With( in StringTags other )
    {
        if ( other.IsEmpty ) { return this; }

        Deconstruct(out ReadOnlySpan<Pair> tags1, out ReadOnlySpan<string> entries1);
        other.Deconstruct(out ReadOnlySpan<Pair> tags2, out ReadOnlySpan<string> entries2);

        return new StringTags([..tags1, ..tags2], [..entries1, ..entries2]);
    }
    public override string ToString() => $"{nameof(Tags)}: {Tags}, {nameof(Entries)}: {Entries}";
    public string ToString( string? format, IFormatProvider? formatProvider ) => string.Equals("json", format, StringComparison.OrdinalIgnoreCase)
                                                                                     ? this.ToJson()
                                                                                     : ToString();
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider ) => destination.TryWrite(provider, $"{nameof(Tags)}: {Tags}, {nameof(Entries)}: {Entries}", out charsWritten);
    public int GetBufferLength()
    {
        ReadOnlySpan<Pair>   tags    = Tags;
        ReadOnlySpan<string> entries = Entries;
        return 4 + tags.Sum(static x => 6 + x.Key.Length + ( x.Value?.Length ?? 0 )) + entries.Sum(static x => x.Length);
    }
    public int GetBufferLength( ReadOnlySpan<char> format )
    {
        ReadOnlySpan<Pair>   tags    = Tags;
        ReadOnlySpan<string> entries = Entries;
        return GetBufferLength() + ( format.Length * ( tags.Length + entries.Length ) );
    }


    public void Deconstruct( out ReadOnlySpan<Pair> tags, out ReadOnlySpan<string> entries )
    {
        tags    = Tags;
        entries = Entries;
    }
    public void Deconstruct( out Pair[] tags, out string[] entries )
    {
        tags    = Tags;
        entries = Entries;
    }
    public bool Equals( StringTags other ) => Tags.AsSpan()
                                                  .SequenceEqual(other.Tags) &&
                                              Entries.AsSpan()
                                                     .SequenceEqual(other.Entries);
    public override bool Equals( object? obj ) => obj is StringTags other && Equals(other);
    public override int  GetHashCode()         => HashCode.Combine(Tags, Entries);


    public static bool operator ==( StringTags left, StringTags right ) => Equals(left, right);
    public static bool operator !=( StringTags left, StringTags right ) => !Equals(left, right);
}
