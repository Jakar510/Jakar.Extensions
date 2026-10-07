// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> How a writer lays out and escapes JSON. Generated <c>ToJson</c> helpers default to the type's resolved settings (<c>T.DefaultWriterOptions</c>); callers may pass their own. </summary>
/// <remarks> <see langword="default"/> is valid: compact, tab indentation when indented, minimal escaping, depth 64. </remarks>
public readonly record struct JsonWriterOptions()
{
    /// <summary> The deepest nesting any reader or writer accepts. Readers and writers track nesting in fixed inline storage, so nothing allocates for depth. </summary>
    public const int MAX_DEPTH_LIMIT   = 1024;
    public const int DEFAULT_MAX_DEPTH = 64;


    public static JsonWriterOptions Default => new()
                                               {
                                                   MaxDepth   = DEFAULT_MAX_DEPTH,
                                                   Escaping   = JsonEscaping.Inherit,
                                                   IndentChar = JsonIndentChar.Tab,
                                                   IndentSize = 1,
                                                   Indented   = false
                                               };

    public static JsonWriterOptions Indent => new()
                                              {
                                                  MaxDepth   = DEFAULT_MAX_DEPTH,
                                                  Escaping   = JsonEscaping.Inherit,
                                                  IndentChar = JsonIndentChar.Tab,
                                                  IndentSize = 1,
                                                  Indented   = true
                                              };


    /// <summary> <see cref="IndentChar"/>s per nesting level (default 1). </summary>
    /// <remarks> Normalized in the getter, not the setter: a value that never sets it (<see langword="default"/>, or an initializer that leaves it out) must still read as 1. </remarks>
    public int IndentSize
    {
        get => field <= 0
                   ? 1
                   : field;
        init;
    }
    public JsonEscaping   Escaping   { get; init; }
    public bool           Indented   { get; init; }
    public JsonIndentChar IndentChar { get; init; }

    /// <summary> Default 64, at most <see cref="MAX_DEPTH_LIMIT"/>. Deeper output (including a cyclic graph) fails with <see cref="JsonErrorKind.DepthExceeded"/>. </summary>
    /// <remarks> Normalized in the getter, like <see cref="IndentSize"/>: an unset (0) depth means the default, never "no nesting allowed". </remarks>
    public int MaxDepth
    {
        get => field <= 0
                   ? DEFAULT_MAX_DEPTH
                   : Math.Min(field, MAX_DEPTH_LIMIT);
        init;
    }

    internal char IndentCharacter => IndentChar is JsonIndentChar.Space
                                         ? ' '
                                         : '\t';


    /// <summary> The options a <see cref="IFormattable"/> format string selects: <c>null</c>/<c>""</c> → <paramref name="defaults"/>, <c>"c"</c> → compact, <c>"i"</c> → indented. </summary>
    /// <exception cref="FormatException"> Any other format. </exception>
    public static JsonWriterOptions FromFormat( scoped ReadOnlySpan<char> format, in JsonWriterOptions defaults )
    {
        if ( format.IsEmpty ) { return defaults; }

        if ( format.Length == 1 )
        {
            switch ( format[0] )
            {
                case 'c' or 'C':
                    return defaults with { Indented = false };

                case 'i' or 'I':
                    return defaults with { Indented = true };
            }
        }

        throw new FormatException($"'{format}' isn't a JSON format: use \"c\" (compact), \"i\" (indented) or none.");
    }
}



/// <summary> What a reader accepts. <see langword="default"/> is strict RFC 8259 with depth 64 and a 64 MB stream limit. </summary>
public readonly record struct JsonReaderOptions
{
    public const int DEFAULT_MAX_DOCUMENT_BYTES = 64 * 1024 * 1024;


    public static JsonReaderOptions Default => default;


    /// <summary> Default 64, at most <see cref="JsonWriterOptions.MAX_DEPTH_LIMIT"/>. </summary>
    public int MaxDepth
    {
        get => field <= 0
                   ? JsonWriterOptions.DEFAULT_MAX_DEPTH
                   : Math.Min(field, JsonWriterOptions.MAX_DEPTH_LIMIT);
        init;
    }

    /// <summary> <c>//</c> and <c>/* */</c> comments (JSONC). </summary>
    public bool AllowComments { get; init; }

    public bool AllowTrailingCommas { get; init; }
    /// <summary> The most a stream read buffers (default 64 MB); also the longest NDJSON line. </summary>
    public int MaxDocumentBytes
    {
        get => field <= 0
                   ? DEFAULT_MAX_DOCUMENT_BYTES
                   : field;
        init;
    }
    /// <summary> The longest string token, in chars or bytes as written (default unlimited). </summary>
    public int MaxStringLength
    {
        get => field <= 0
                   ? int.MaxValue
                   : field;
        init;
    }
}
