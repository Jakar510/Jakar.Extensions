// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/08/2022  1:50 PM

namespace Jakar.AppLogger.Common;


public interface ILoggerAttachment
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
public sealed record LoggerAttachment( string Content, long Length, string? FileName, string? Description, string? Type, bool IsBinary ) : BaseRecord
{
    public const     int  DESCRIPTION_SIZE = 1024;
    public const     int  FILE_NAME_SIZE   = 256;
    public const     int  MAX_SIZE         = 2 ^ 28; // ~ 268.4 MB
    public const     int  TYPE_SIZE        = 256;
    private readonly long _length          = Length;


    public static LoggerAttachment   Default { get; } = new(string.Empty, 0, default, default, default, false);
    public static LoggerAttachment[] Empty   { get; } = Array.Empty<LoggerAttachment>();
    public long Length
    {
        get => _length;
        init
        {
            _length = value;
            if ( value > MAX_SIZE ) { }
        }
    }


    public LoggerAttachment( ILoggerAttachment    attachment ) : this( attachment.Content, attachment.Length, attachment.FileName, attachment.Description, attachment.Type, attachment.IsBinary ) { }
    public LoggerAttachment( MemoryStream         content, string? fileName,              string? description = default, string? type = default ) : this( content.GetBuffer(), fileName, description, type ) { }
    public LoggerAttachment( ReadOnlyMemory<byte> content, string? fileName,              string? description = default, string? type = default ) : this( content.Span, fileName, description, type ) { }
    public LoggerAttachment( ReadOnlySpan<byte>   content, string? fileName,              string? description = default, string? type = default ) : this( Convert.ToBase64String( content ), content.Length, fileName, description, type, true ) { }
    public LoggerAttachment( byte[]               content, string? fileName,              string? description = default, string? type = default ) : this( Convert.ToBase64String( content ), content.Length, fileName, description, type, true ) { }
    public LoggerAttachment( string               content, string? description = default, string? type        = default ) : this( content, content.Length, default, description, type, false ) { }


    public byte[]? GetData() => IsBinary
                                    ? Convert.FromBase64String( Content )
                                    : default;


    [DoesNotReturn] public static void ThrowTooLong( in long    length ) => throw new ArgumentException( $"{nameof(Content)}.{nameof(Length)} is too long '{length}'; Must be < {MAX_SIZE}." );
    public static LoggerAttachment Create( ILoggerAttachment    attachment ) => new(attachment);
    public static LoggerAttachment Create( ReadOnlySpan<byte>   content, string? fileName,              string? description = default, string? type = default ) => new(content, fileName, description, type);
    public static LoggerAttachment Create( ReadOnlyMemory<byte> content, string? fileName,              string? description = default, string? type = default ) => new(content, fileName, description, type);
    public static LoggerAttachment Create( byte[]               content, string? fileName,              string? description = default, string? type = default ) => new(content, fileName, description, type);
    public static LoggerAttachment Create( MemoryStream         content, string? fileName,              string? description = default, string? type = default ) => new(content, fileName, description, type);
    public static LoggerAttachment Create( string               content, string? description = default, string? type        = default ) => new(content, description, type);


    public static LoggerAttachment Create( Stream stream, string? fileName = default, string? description = default, string? type = default )
    {
        using var ms = new MemoryStream();
        stream.CopyTo( ms );
        ReadOnlyMemory<byte> content = ms.GetBuffer();

        return new LoggerAttachment( content, fileName, description, type );
    }
    public static async ValueTask<LoggerAttachment> CreateAsync( Stream stream, string? fileName = default, string? description = default, string? type = default, CancellationToken token = default )
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync( ms, token );
        ReadOnlyMemory<byte> content = ms.GetBuffer();

        return new LoggerAttachment( content, fileName, description, type );
    }


    public static LoggerAttachment Create( LocalFile file, string? description = default, string? type = default )
    {
        ReadOnlyMemory<byte> content = file.Read()
                                           .AsMemory();

        return new LoggerAttachment( content, file.Name, description ?? file.Name, type ?? file.ContentType );
    }

    public static async ValueTask<LoggerAttachment> CreateAsync( LocalFile file, string? description = default, string? type = default, CancellationToken token = default )
    {
        ReadOnlyMemory<byte> content = await file.ReadAsync()
                                                 .AsMemory( token );

        return new LoggerAttachment( content, file.Name, description ?? file.Name, type ?? file.ContentType );
    }
}
