// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     Writes a <see cref="LocalFile"/> as <c> {"FullPath": "...", "FileEncoding": "utf-8"} </c>.
///     <para> Reads that shape (also the 10.x object shape, which carried <c> FullPath </c> among many computed properties; the others are ignored) or a plain path string. </para>
///     <para> The default contract would serialize every computed property: <see cref="LocalFile.Length"/> throws for a missing file, and reading the rest touches the disk. </para>
/// </summary>
public sealed class LocalFileJsonConverter : JsonConverter<LocalFile>
{
    public override LocalFile? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        switch ( reader.TokenType )
        {
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.String:
                return new LocalFile(reader.GetString() ?? throw new JsonException("A LocalFile path can't be null."));

            case JsonTokenType.StartObject:
            {
                string?   path     = null;
                Encoding? encoding = null;

                while ( reader.Read() && reader.TokenType != JsonTokenType.EndObject )
                {
                    string name = reader.GetString() ?? EMPTY;
                    reader.Read();

                    if ( name.Equals(nameof(LocalFile.FullPath), StringComparison.OrdinalIgnoreCase) || name.Equals(nameof(LocalFile.PhysicalPath), StringComparison.OrdinalIgnoreCase) ) { path ??= reader.GetString(); }
                    else if ( name.Equals(nameof(LocalFile.FileEncoding), StringComparison.OrdinalIgnoreCase) ) { encoding = EncodingConverter.Instance.Read(ref reader, typeof(Encoding), options); }
                    else { reader.Skip(); }
                }

                return string.IsNullOrWhiteSpace(path)
                           ? throw new JsonException($"A {nameof(LocalFile)} needs a {nameof(LocalFile.FullPath)}.")
                           : new LocalFile(path, encoding);
            }

            default:
                throw new JsonException($"Unexpected {reader.TokenType} when reading a {nameof(LocalFile)}.");
        }
    }


    public override void Write( Utf8JsonWriter writer, LocalFile value, JsonSerializerOptions options )
    {
        writer.WriteStartObject();
        writer.WriteString(nameof(LocalFile.FullPath), value.FullPath);
        writer.WriteString(nameof(LocalFile.FileEncoding), value.FileEncoding.WebName);
        writer.WriteEndObject();
    }
}



/// <summary> Writes a <see cref="LocalDirectory"/> as <c> {"FullPath": "..."} </c>; reads that (or the 10.x object shape, or a plain path string). The default contract would enumerate the directory's files and sub-folders. </summary>
public sealed class LocalDirectoryJsonConverter : JsonConverter<LocalDirectory>
{
    public override LocalDirectory? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        switch ( reader.TokenType )
        {
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.String:
                return new LocalDirectory(reader.GetString() ?? throw new JsonException("A LocalDirectory path can't be null."));

            case JsonTokenType.StartObject:
            {
                string? path = null;

                while ( reader.Read() && reader.TokenType != JsonTokenType.EndObject )
                {
                    string name = reader.GetString() ?? EMPTY;
                    reader.Read();

                    if ( name.Equals(nameof(LocalDirectory.FullPath), StringComparison.OrdinalIgnoreCase) ) { path = reader.GetString(); }
                    else { reader.Skip(); }
                }

                return string.IsNullOrWhiteSpace(path)
                           ? throw new JsonException($"A {nameof(LocalDirectory)} needs a {nameof(LocalDirectory.FullPath)}.")
                           : new LocalDirectory(path);
            }

            default:
                throw new JsonException($"Unexpected {reader.TokenType} when reading a {nameof(LocalDirectory)}.");
        }
    }


    public override void Write( Utf8JsonWriter writer, LocalDirectory value, JsonSerializerOptions options )
    {
        writer.WriteStartObject();
        writer.WriteString(nameof(LocalDirectory.FullPath), value.FullPath);
        writer.WriteEndObject();
    }
}
