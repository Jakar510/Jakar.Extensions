// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:10

namespace Jakar.Extensions;


[Serializable]
public sealed record UploadData( string Content, long Size, MimeType MimeType )
{
    public UploadData( ReadOnlySpan<byte> content, MimeType mime ) : this( Convert.ToBase64String( content ), content.Length, mime ) { }
}
