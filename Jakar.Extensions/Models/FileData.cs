// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:10

namespace Jakar.Extensions;


public interface IFileMetaData : IUniqueID<Guid>
{
    public const int DESCRIPTION_SIZE_LIMIT = Sizes.UNICODE_CAPACITY;
    public const int NAME_SIZE_LIMIT        = 256;
    public const int TYPE_SIZE_LIMIT        = Sizes.UNICODE_CAPACITY;

    string? FileDescription { get; }
    string? FileName        { get; }
    string? FileType        { get; }
}



public interface IFileData<out TFileMetaData>
#if NET8_0_OR_GREATER
    where TFileMetaData : IFileMetaData<TFileMetaData>
#else
    where TFileMetaData : IFileMetaData
#endif
{
    long                  FileSize { get; }
    string                Hash     { get; }
    MimeType              MimeType { get; }
    string                Payload  { get; }
    public TFileMetaData? MetaData { get; }
}



public interface IFileData : IFileData<FileMetaData>
{
    public const int FILE_SIZE_LIMIT = 0x3FFFFFDF; // 1 GB -- from string.MaxSize
    public const int HASH_SIZE_LIMIT = 4096;


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



#if NET8_0_OR_GREATER
public interface IFileData<T, out TFileMetaData> : IFileData<TFileMetaData>
    where TFileMetaData : IFileMetaData<TFileMetaData>
    where T : IFileData<T, TFileMetaData>
{
    public abstract static T            Create( IFileData                                        data );
    public abstract static T?           TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData? data );
    public abstract static ValueTask<T> Create( LocalFile                                        file,   CancellationToken token = default );
    public abstract static ValueTask<T> Create( Stream                                           stream, MimeType          mime, FileMetaData? metaData, CancellationToken token = default );
    public abstract static T            Create( MemoryStream                                     stream, MimeType          mime, FileMetaData? metaData );
    public abstract static T            Create( ReadOnlyMemory<byte>                             data,   MimeType          mime, FileMetaData? metaData );
    public abstract static T            Create( ReadOnlySpan<byte>                               data,   MimeType          mime, FileMetaData? metaData );
}



public interface IFileMetaData<out T> : IFileMetaData
    where T : IFileMetaData<T>
{
    public abstract static T  Create( IFileMetaData                                        data );
    public abstract static T? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileMetaData? data );
}
#endif



[Serializable, SuppressMessage( "ReSharper", "InconsistentNaming" )]
public sealed record FileData( MimeType MimeType, long FileSize, [property: StringLength( IFileData.HASH_SIZE_LIMIT )] string Hash, [property: StringLength( IFileData.FILE_SIZE_LIMIT )] string Payload, FileMetaData? MetaData ) :
#if NET8_0_OR_GREATER
    IFileData<FileData, FileMetaData>
#else
    IFileData<FileMetaData>
#endif
{
    public FileData( IFileData                    file ) : this( file, FileMetaData.TryCreate( file.MetaData ) ) { }
    public FileData( IFileData                    file,    FileMetaData? metaData ) : this( file.MimeType, file.FileSize, file.Hash, file.Payload, metaData ) { }
    public FileData( scoped in ReadOnlySpan<byte> content, MimeType      mime, FileMetaData? metaData ) : this( mime, content.Length, IFileData.GetHash( content ), Convert.ToBase64String( content ), metaData ) { }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData Create( MemoryStream         stream, MimeType mime, FileMetaData? metaData ) => Create( stream.AsReadOnlyMemory(), mime, metaData );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData Create( ReadOnlyMemory<byte> data,   MimeType mime, FileMetaData? metaData ) => Create( data.Span,                 mime, metaData );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData Create( ReadOnlySpan<byte>   data,   MimeType mime, FileMetaData? metaData ) => new(data, mime, metaData);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData Create( IFileData            data ) => new(data);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static FileData? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData? data ) => data is not null
                                                                                                   ? Create( data )
                                                                                                   : null;
    public static async ValueTask<FileData> Create( LocalFile file, CancellationToken token = default )
    {
        ReadOnlyMemory<byte> content = await file.ReadAsync().AsBytes( token );
        return new FileData( content.Span, file.Mime, new FileMetaData( null, file.Name, file.ContentType ) );
    }
    public static async ValueTask<FileData> Create( Stream stream, MimeType mime, FileMetaData? metaData, CancellationToken token = default )
    {
        stream.Seek( 0, SeekOrigin.Begin );
        using MemoryStream memory = new((int)stream.Length);
        await stream.CopyToAsync( memory, token );
        return Create( memory, mime, metaData );
    }
}



[Serializable]
public sealed record FileMetaData( [property: StringLength( IFileMetaData.NAME_SIZE_LIMIT )] string? FileName, [property: StringLength( IFileMetaData.TYPE_SIZE_LIMIT )] string? FileType, [property: StringLength( IFileMetaData.DESCRIPTION_SIZE_LIMIT )] string? FileDescription = null, Guid ID = default ) :
#if NET8_0_OR_GREATER
    IFileMetaData<FileMetaData>
#else
    IFileMetaData
#endif
{
    public FileMetaData( IFileMetaData value ) : this( value.FileName, value.FileType, value.FileDescription, value.ID ) { }
    public FileMetaData( LocalFile     value ) : this( value.Name, value.ContentType ) { }


    public static FileMetaData Create( IFileMetaData data ) => new(data.FileName, data.FileType, data.FileDescription, data.ID);
    public static FileMetaData? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileMetaData? data ) => data is not null
                                                                                                           ? Create( data )
                                                                                                           : null;
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
