// Jakar.Json
// 10/07/2026

using System.Text.Json;



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



/// <summary>
///     Malformed JSON, or JSON that doesn't match the target type. A System.Text.Json <see cref="JsonException"/>, so code that catches that keeps working whichever serializer
///     read the JSON (a <c>[JsonModel]</c> type's <c>IJsonModel</c> members may run on either). <see cref="JsonException.Path"/>, <see cref="JsonException.LineNumber"/> and
///     <see cref="JsonException.BytePositionInLine"/> are set (zero-based, as System.Text.Json reports them); <see cref="Error"/> has the kind and the one-based position.
/// </summary>
/// <remarks> Not a <see cref="FormatException"/>: <see cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)"/> on a generated type throws this. </remarks>
public sealed class JsonReadException( JsonError error, string? path = null ) : JsonException(Describe(error, path), path ?? "$", Math.Max(0, error.Line - 1), Math.Max(0, error.Column - 1), null)
{
    public JsonError Error { get; } = error;

    /// <summary> The JSON path of the failure, e.g. <c>$.lines[3].price</c>; <c>$</c> when it isn't known. </summary>
    public new string Path => base.Path ?? "$";


    private static string Describe( JsonError error, string? path ) => path is null or "$"
                                                                           ? $"Invalid JSON: {error}."
                                                                           : $"Invalid JSON: {error}, at {path}.";
}



/// <summary> A value JSON can't represent (NaN, too deep) or a writer used out of order. </summary>
public sealed class JsonWriteException( JsonError error, string message ) : InvalidOperationException(message)
{
    public JsonError Error { get; } = error;
}
