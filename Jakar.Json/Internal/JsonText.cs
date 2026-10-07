// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


internal static partial class JsonText
{
    /// <summary> Parses a value from text: a static interface, so <see cref="TryRead{TReader,T,TParser}"/> is specialized per parser with no delegate. </summary>
    public interface IParser<T>
    {
        static abstract bool TryParse( scoped ReadOnlySpan<char> text, [MaybeNullWhen(false)] out T value );
    }



    public readonly struct Parsable<T> : IParser<T>
        where T : ISpanParsable<T>
    {
        public static bool TryParse( scoped ReadOnlySpan<char> text, [MaybeNullWhen(false)] out T value ) => T.TryParse(text, CultureInfo.InvariantCulture, out value);
    }



    /// <summary> Keeps the kind: <c>Z</c> → UTC, an offset → local (converted), none → unspecified. Never culture- or machine-dependent parsing. </summary>
    public readonly struct DateTimeText : IParser<DateTime>
    {
        public static bool TryParse( scoped ReadOnlySpan<char> text, out DateTime value ) => DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out value);
    }



    /// <summary> A value without an offset is read as UTC, never as the machine's local time. </summary>
    public readonly struct DateTimeOffsetText : IParser<DateTimeOffset>
    {
        public static bool TryParse( scoped ReadOnlySpan<char> text, out DateTimeOffset value ) => DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out value);
    }



    public readonly struct VersionText : IParser<Version>
    {
        public static bool TryParse( scoped ReadOnlySpan<char> text, [MaybeNullWhen(false)] out Version value ) => Version.TryParse(text, out value);
    }



    /// <summary> Reads a JSON string and parses it with <typeparamref name="TParser"/>, without allocating (UTF-8 is decoded into a stack buffer). </summary>
    public static bool TryRead<TReader, T, TParser>( ref TReader reader, [MaybeNullWhen(false)] out T value )
        where TReader : IJsonReader, allows ref struct
        where TParser : IParser<T>
    {
        value = default;
        JsonReaderCheckpoint mark = reader.Checkpoint();
        if ( !reader.TryReadStringSpan(out JsonSpan text) ) { return false; }

        if ( text.IsUtf8
                 ? TryParseUtf8<T, TParser>(text.Utf8, out value)
                 : TParser.TryParse(text.Utf16, out value) ) { return true; }

        reader.Rewind(mark);
        return reader.Fail(JsonErrorKind.InvalidValue);
    }

    public static bool TryParseUtf8<T, TParser>( scoped ReadOnlySpan<byte> utf8, [MaybeNullWhen(false)] out T value )
        where TParser : IParser<T>
    {
        char[]? rented = null;

        Span<char> buffer = utf8.Length <= 256
                                ? stackalloc char[256]
                                : rented = ArrayPool<char>.Shared.Rent(utf8.Length);

        try
        {
            Utf8.ToUtf16(utf8, buffer, out _, out int written);
            return TParser.TryParse(buffer[..written], out value);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<char>.Shared.Return(rented); }
        }
    }


    /// <summary> Decodes a <see cref="JsonSpan"/> (either encoding) to chars and hands it to <typeparamref name="TParser"/>. </summary>
    public static bool TryParse<T, TParser>( scoped in JsonSpan text, [MaybeNullWhen(false)] out T value )
        where TParser : IParser<T> => text.IsUtf8
                                          ? TryParseUtf8<T, TParser>(text.Utf8, out value)
                                          : TParser.TryParse(text.Utf16, out value);
}
