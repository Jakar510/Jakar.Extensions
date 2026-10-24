// Jakar.Extensions :: Jakar.Extensions
// 09/23/2025  18:44

namespace Jakar.Extensions;


/// <summary> Writes an <see cref="Encoding"/> as its <see cref="Encoding.WebName"/> (canonical, stable, lowercase) and reads it back with <see cref="Encoding.GetEncoding(string)"/>. <c> null </c> and <c> "" </c> read as <see langword="null"/>. </summary>
public readonly struct EncodingConverter : IJsonConverter<Encoding?>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in Encoding? value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteString(value.WebName); }
    }


    public static bool TryRead<TReader>( ref TReader reader, out Encoding? value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        if ( reader.TryReadNull() ) { return true; }

        JsonReaderCheckpoint mark = reader.Checkpoint();
        if ( !reader.TryReadString(out string? name) ) { return false; }

        if ( string.IsNullOrWhiteSpace(name) ) { return true; }

        try
        {
            value = Encoding.GetEncoding(name);
            return true;
        }
        catch ( ArgumentException )
        {
            reader.Rewind(mark);
            return reader.Fail(JsonErrorKind.InvalidValue);
        }
    }
}
