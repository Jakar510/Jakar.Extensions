// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:10

namespace Jakar.Extensions;


public interface IFileMetaData : IUniqueID<Guid>
{
    public const int DESCRIPTION_SIZE_LIMIT = 2048;
    public const int NAME_SIZE_LIMIT        = 256;
    public const int TYPE_SIZE_LIMIT        = 2048;

    [property: MaxLength( DESCRIPTION_SIZE_LIMIT )] string? FileDescription { get; }
    [property: MaxLength( NAME_SIZE_LIMIT )]        string? FileName        { get; }
    [property: MaxLength( TYPE_SIZE_LIMIT )]        string? FileType        { get; }
}



public sealed record FileMetaData( [property: MaxLength( IFileMetaData.NAME_SIZE_LIMIT )] string? FileName, [property: MaxLength( IFileMetaData.TYPE_SIZE_LIMIT )] string? FileType, [property: MaxLength( IFileMetaData.DESCRIPTION_SIZE_LIMIT )] string? FileDescription = null, Guid ID = default ) : IFileMetaData
{
    public FileMetaData( IFileMetaData value ) : this( value.FileName, value.FileType, value.FileDescription, value.ID ) { }
    public FileMetaData( LocalFile     value ) : this( value.Name, value.ContentType ) { }
}



public interface IFileData
{
    public const int FILE_SIZE_LIMIT = 0x3FFFFFDF; // 1 GB -- from string.MaxSize
    public const int HASH_SIZE_LIMIT = 4096;
    long             FileSize { get; }

    [property: MaxLength( HASH_SIZE_LIMIT )] string Hash     { get; }
    IFileMetaData?                                  MetaData { get; }
    MimeType                                        MimeType { get; }
    [property: MaxLength( FILE_SIZE_LIMIT )] string Payload  { get; }


    public static OneOf<byte[], string> GetData( string data )
    {
        try { return Convert.FromBase64String( data ); }
        catch ( FormatException ) { return data; }
    }
    public static string GetHash( in OneOf<byte[], string>                       data ) => data.Match( GetHash, GetHash );
    public static string GetHash( in OneOf<ReadOnlyMemory<byte>, byte[], string> data ) => data.Match( GetHash, GetHash, GetHash );
    public static string GetHash( string                                         data ) => GetHash( data, Encoding.Default );
    public static string GetHash( string data, Encoding encoding )
    {
        using IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent( encoding.GetByteCount( data ) );

        encoding.GetBytes( data, owner.Memory.Span );
        return GetHash( owner.Memory );
    }
    static        string GetHash( byte[]               x )    => Hashes.Hash_SHA256( x );
    public static string GetHash( ReadOnlyMemory<byte> data ) => GetHash( data.Span );
    public static string GetHash( ReadOnlySpan<byte> data )
    {
        Debug.Assert( data.Length > 0 );
        using var  hasher = SHA256.Create();
        Span<byte> span   = stackalloc byte[HASH_SIZE_LIMIT];
        hasher.TryComputeHash( data, span, out int bytesWritten );
        Debug.Assert( bytesWritten > 0 );

        span = span[..bytesWritten];
        Span<char>   hexChars     = stackalloc char[span.Length * 2];
        const string HEX_ALPHABET = "0123456789ABCDEF";

        for ( int i = 0; i < span.Length; i++ )
        {
            hexChars[i * 2]     = HEX_ALPHABET[span[i] >> 4];
            hexChars[i * 2 + 1] = HEX_ALPHABET[span[i] & 0x0F];
        }

        return hexChars.ToString();
    }
}



[Serializable]
public record FileData<TFileMetaData>( long FileSize, MimeType MimeType, [property: MaxLength( IFileData.HASH_SIZE_LIMIT )] string Hash, [property: MaxLength( IFileData.FILE_SIZE_LIMIT )] string Payload, TFileMetaData? FileMetaData ) : IFileData
    where TFileMetaData : IFileMetaData
{
    IFileMetaData? IFileData.MetaData => FileMetaData;


    public FileData( ReadOnlySpan<byte>   content, MimeType mime, TFileMetaData? metaData ) : this( content.Length, mime, IFileData.GetHash( content ), Convert.ToBase64String( content ), metaData ) { }
    public FileData( ReadOnlyMemory<byte> content, MimeType mime, TFileMetaData? metaData ) : this( content.Span, mime, metaData ) { }


    public static async ValueTask<FileData<TFileMetaData>> Create( Stream stream, MimeType mime, TFileMetaData? metaData, CancellationToken token = default )
    {
        stream.Seek( 0, SeekOrigin.Begin );
        using MemoryStream memory = new((int)stream.Length);
        await stream.CopyToAsync( memory, token );
        return Create( memory, mime, metaData );
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData<TFileMetaData> Create( MemoryStream         stream, MimeType mime, TFileMetaData? metaData ) => new(new ReadOnlyMemory<byte>( stream.GetBuffer() ), mime, metaData);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData<TFileMetaData> Create( ReadOnlyMemory<byte> data,   MimeType mime, TFileMetaData? metaData ) => new(data, mime, metaData);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData<TFileMetaData> Create( ReadOnlySpan<byte>   data,   MimeType mime, TFileMetaData? metaData ) => new(data, mime, metaData);
}



[Serializable]
public record FileData : FileData<FileMetaData>
{
    public static async ValueTask<FileData> Create( LocalFile file, CancellationToken token = default )
    {
        ReadOnlyMemory<byte> content = await file.ReadAsync().AsBytes( token );
        return new FileData( content, file.Mime, new FileMetaData( null, file.Name, file.ContentType ) );
    }
    public FileData( ReadOnlySpan<byte>   content,  MimeType mime,     FileMetaData? metaData ) : base( content, mime, metaData ) { }
    public FileData( ReadOnlyMemory<byte> content,  MimeType mime,     FileMetaData? metaData ) : base( content, mime, metaData ) { }
    public FileData( long                 FileSize, MimeType MimeType, string        Hash, string Payload, FileMetaData? metaData ) : base( FileSize, MimeType, Hash, Payload, metaData ) { }
}



public static class FileDataExtensions
{
    public static Stream GetStream( this IFileData data ) => data.GetStream( Encoding.Default );
    public static Stream GetStream( this IFileData data, Encoding encoding )
    {
        OneOf<byte[], string> one = data.GetData();

        return one.IsT0
                   ? new MemoryStream( one.AsT0 )
                   : new MemoryStream( encoding.GetBytes( one.AsT1 ) );
    }
    public static OneOf<byte[], string> GetData( this IFileData data ) => data.MimeType.IsText()
                                                                              ? data.Payload
                                                                              : IFileData.GetData( data.Payload );
}
