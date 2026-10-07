// Jakar.SystemTextJson.Tests
// 10/02/2026

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Jakar.Extensions;



namespace Jakar.SystemTextJson.Tests;


[JsonSourceGenerationOptions(AllowTrailingCommas = true, ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Invoice))]
[JsonSerializable(typeof(Point))]
[JsonSerializable(typeof(Outer.Note))]
[JsonSerializable(typeof(Bag))]
[JsonSerializable(typeof(Code))]
[JsonSerializable(typeof(Dictionary<string, Invoice>))]
[JsonSerializable(typeof(Dictionary<Code, int>))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(string))]
public sealed partial class TestJsonContext : JsonSerializerContext;



[JsonModel(typeof(TestJsonContext))]
public sealed partial class Invoice : IEquatable<Invoice>
{
    public string  Id    { get; init; } = "";
    public decimal Total { get; init; }

    public bool          Equals( Invoice? other ) => other is not null && Id == other.Id && Total == other.Total;
    public override bool Equals( object?  obj )   => obj is Invoice other && Equals(other);
    public override int  GetHashCode()            => HashCode.Combine(Id, Total);
}



[JsonModel(typeof(TestJsonContext), GenerateToString = true)]
public readonly partial record struct Point( int X, int Y );



public static partial class Outer
{
    [JsonModel(typeof(TestJsonContext))]
    public sealed partial record Note( string Text );
}



/// <summary> Unknown members survive a round trip through <see cref="IJsonModel.AdditionalData"/>. </summary>
[JsonModel(typeof(TestJsonContext))]
public sealed partial class Bag : IJsonModel
{
    public                        string      Name           { get; set; } = "";
    [JsonExtensionData] public Dictionary<string, System.Text.Json.JsonElement>? AdditionalData { get; set; }
}



/// <summary> A value serialized as a string through <see cref="SerializeAsStringJsonConverter{T}"/>. </summary>
[JsonConverter(typeof(SerializeAsStringJsonConverter<Code>))]
public readonly record struct Code( int Value ) : ISpanParsable<Code>, ISpanFormattable
{
    public static Code Parse( string s, IFormatProvider? provider ) => Parse(s.AsSpan(), provider);
    public static Code Parse( ReadOnlySpan<char> s, IFormatProvider? provider ) => TryParse(s, provider, out Code result)
                                                                                       ? result
                                                                                       : throw new FormatException(s.ToString());

    public static bool TryParse( [NotNullWhen(true)] string? s, IFormatProvider? provider, out Code result ) => TryParse(s.AsSpan(), provider, out result);
    public static bool TryParse( ReadOnlySpan<char> s, IFormatProvider? provider, out Code result )
    {
        if ( s.StartsWith("C-") ) { s = s[2..]; }

        bool ok = int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value);
        result = new Code(value);
        return ok;
    }

    public string ToString( string? format, IFormatProvider? formatProvider ) => $"C-{Value}";
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider ) => destination.TryWrite(CultureInfo.InvariantCulture, $"C-{Value}", out charsWritten);
}



/// <summary> Collection that serializes its own contents (here: everything, ignoring a view filter). </summary>
public sealed class FilteredList( IEnumerable<int> values, Func<int, bool> filter ) : IJsonArraySource<int>, IEnumerable<int>
{
    private readonly List<int> _values = [.. values];

    public void WriteJsonArray( System.Text.Json.Utf8JsonWriter writer, System.Text.Json.Serialization.Metadata.JsonTypeInfo<int> info ) => JsonModel.WriteArray(writer, _values, info);

    public IEnumerator<int> GetEnumerator() => _values.Where(filter).GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
