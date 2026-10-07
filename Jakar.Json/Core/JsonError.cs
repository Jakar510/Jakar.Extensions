// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


public enum JsonErrorKind : byte
{
    None,
    UnexpectedEnd,
    UnexpectedToken,
    InvalidNumber,
    NumberOverflow,
    InvalidString,
    InvalidEscape,
    InvalidUtf8,
    DepthExceeded,
    DuplicateMember,
    UnknownMember,
    MissingRequired,
    UnknownDiscriminator,
    NonFiniteNumber,
    DocumentTooLarge,
    TrailingData,
    BufferTooSmall,
    InvalidValue, // a well-formed token the target type can't hold: an unknown enum name, a malformed Guid or date, a null for a non-nullable member
    Converter
}



/// <summary> Where and why reading or writing JSON failed. No references: free to copy and store. </summary>
/// <param name="Kind"> What went wrong; <see cref="JsonErrorKind.None"/> for no error. </param>
/// <param name="Position"> The char (UTF-16 input) or byte (UTF-8 input) offset. </param>
/// <param name="Line"> 1-based; 0 when there is no input (writing, document size). </param>
/// <param name="Column"> 1-based, in chars or bytes like <paramref name="Position"/>; 0 when there is no input. </param>
public readonly record struct JsonError( JsonErrorKind Kind, int Position, int Line, int Column )
{
    public bool IsError => Kind != JsonErrorKind.None;

    public override string ToString() => Line > 0
                                             ? $"{Kind} at line {Line}, column {Column} (offset {Position})"
                                             : $"{Kind} at offset {Position}";
}



/// <summary> Malformed JSON, or JSON that doesn't match the target type. A <see cref="FormatException"/>, so <see cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)"/> callers catch the type they expect. </summary>
public sealed class JsonReadException : FormatException
{
    public JsonError Error { get; }

    /// <summary> The JSON path of the failure, e.g. <c>$.lines[3].price</c>; <c>$</c> when it isn't known. </summary>
    public string Path { get; }


    public JsonReadException( JsonError error, string? path = null ) : base(Describe(error, path))
    {
        Error = error;
        Path  = path ?? "$";
    }


    private static string Describe( JsonError error, string? path ) => path is null or "$"
                                                                           ? $"Invalid JSON: {error}."
                                                                           : $"Invalid JSON: {error}, at {path}.";
}



/// <summary> A value JSON can't represent (NaN, too deep) or a writer used out of order. </summary>
public sealed class JsonWriteException( JsonError error, string message ) : InvalidOperationException(message)
{
    public JsonError Error { get; } = error;
}
