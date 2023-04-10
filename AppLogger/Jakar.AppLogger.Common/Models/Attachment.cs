// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/08/2022  1:50 PM

namespace Jakar.AppLogger.Common;


public interface IAttachment
{
    public string  Content     { get; init; }
    public string? Description { get; init; }
    public string? FileName    { get; init; }
    public bool    IsBinary    { get; init; }
    public long    Length      { get; init; }
    public string? Type        { get; init; }

    public byte[]? GetData();
}



[Serializable]
public sealed class Attachment : BaseClass
{
    public const int MAX_SIZE = 0x3FFFFFDF; // 1GB


    public static Attachment   Default     { get; }       = new();
    public static Attachment[] Empty       { get; }       = Array.Empty<Attachment>();
    public        string       Content     { get; init; } = string.Empty;
    public        string?      Description { get; init; }
    public        string?      FileName    { get; init; }
    public        bool         IsBinary    { get; init; }
    public        long         Length      { get; init; }
    public        string?      Type        { get; init; }


    public Attachment() { }
    public Attachment( IAttachment attachment )
    {
        Length      = attachment.Length;
        Content     = attachment.Content;
        Description = attachment.Description;
        Type        = attachment.Type;
        FileName    = attachment.FileName;
        IsBinary    = attachment.IsBinary;
    }
    public Attachment( MemoryStream content, string? fileName, string? description = default, string? type = default ) : this( content.GetBuffer()
                                                                                                                                      .AsSpan(),
                                                                                                                               fileName,
                                                                                                                               description,
                                                                                                                               type ) { }
    public Attachment( ReadOnlyMemory<byte> content, string? fileName, string? description = default, string? type = default ) : this( content.Span, fileName, description, type ) { }
    public Attachment( ReadOnlySpan<byte> content, string? fileName, string? description = default, string? type = default ) :
        this( Convert.ToBase64String( content ), content.Length, fileName, description, type, !string.IsNullOrEmpty( fileName ) ) { }
    public Attachment( byte[] content, string? fileName,              string? description = default, string? type = default ) : this( Convert.ToBase64String( content ), content.Length, fileName, description, type, true ) { }
    public Attachment( string content, string? description = default, string? type        = default ) : this( content, content.Length, default, description, type, false ) { }
    private Attachment( string content, long length, string? fileName, string? description, string? type, bool isBinary )
    {
        if ( length > MAX_SIZE ) { ThrowTooLong(); }

        Length      = length;
        FileName    = fileName;
        Content     = content;
        Description = description;
        Type        = type;
        IsBinary    = isBinary;
    }


    public byte[]? GetData() => IsBinary
                                    ? Convert.FromBase64String( Content )
                                    : default;


    [DoesNotReturn] public static void ThrowTooLong() => throw new ArgumentException( $"{nameof(Content)}.{nameof(Length)} is too long; Must be < {MAX_SIZE}." );
    public static Attachment Create( ReadOnlySpan<byte>   content, string? description = default, string? type = default ) => new(content, description, type);
    public static Attachment Create( ReadOnlyMemory<byte> content, string? description = default, string? type = default ) => new(content, description, type);
    public static Attachment Create( string               content, string? description = default, string? type = default ) => new(content, description, type);
    public static Attachment Create( Stream stream, string? fileName, string? description = default, string? type = default )
    {
        using var ms = new MemoryStream();
        stream.CopyTo( ms );

        return new Attachment( ms, fileName, description, type );
    }
    public static async ValueTask<Attachment> CreateAsync( Stream stream, string? fileName, string? description = default, string? type = default, CancellationToken token = default )
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync( ms, token );

        return new Attachment( ms, fileName, description, type );
    }
    public static Attachment Create( LocalFile file, string? description = default, string? type = default )
    {
        byte[] content = file.Read()
                             .AsBytes();

        return new Attachment( content, file.Name, description ?? file.Name, type ?? file.ContentType );
    }
    public static async ValueTask<Attachment> CreateAsync( LocalFile file, string? description = default, string? type = default, CancellationToken token = default )
    {
        byte[] content = await file.ReadAsync()
                                   .AsBytes( token );

        return new Attachment( content, file.Name, description ?? file.Name, type ?? file.ContentType );
    }
}
