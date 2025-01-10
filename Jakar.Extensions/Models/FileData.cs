// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:10

namespace Jakar.Extensions;


public interface IFileMetaData
{
    string? FileDescription { get; }
    string? FileName        { get; }
    string? FileType        { get; }
}



public interface IFileData<out TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    long     FileSize { get; }
    string   Hash     { get; }
    MimeType MimeType { get; }
    string   Payload  { get; }


    /*
    public static string GetHash( scoped in ReadOnlySpan<byte>   data )
    {
        Debug.Assert( data.Length > 0 );
        using SHA256             hasher = SHA256.Create();
        using IMemoryOwner<byte> owner  = MemoryPool<byte>.Shared.Rent( BaseRecord.UNICODE_CAPACITY );
        Span<byte>               span   = owner.Memory.Span;
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
    */
}



public interface IFileData<out TID, out TFileMetaData> : IFileData<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TFileMetaData : IFileMetaData, IComparable<TFileMetaData>, IEquatable<TFileMetaData>
{
    public TFileMetaData? MetaData { get; }
}



[SuppressMessage( "ReSharper", "TypeParameterCanBeVariant" )]
public interface IFileData<TClass, TID, TFileMetaData> : IFileData<TID, TFileMetaData>, IComparable<TClass>, IEquatable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TFileMetaData : IFileMetaData, IComparable<TFileMetaData>, IEquatable<TFileMetaData>
    where TClass : IFileData<TClass, TID, TFileMetaData>
{
    public abstract static TClass            Create( IFileData<TID, TFileMetaData>                                        data );
    public abstract static TClass            Create( IFileData<TID>                                                       data, TFileMetaData? metaData );
    public abstract static TClass?           TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData<TID, TFileMetaData>? data );
    public abstract static TClass?           TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData<TID>?                data,   TFileMetaData?    metaData );
    public abstract static ValueTask<TClass> Create( LocalFile                                                            file,   CancellationToken token                                                 = default );
    public abstract static ValueTask<TClass> Create( Stream                                                               stream, MimeType          mime, FileMetaData? metaData, CancellationToken token = default );
    public abstract static TClass            Create( MemoryStream                                                         stream, MimeType          mime, FileMetaData? metaData );
    public abstract static TClass            Create( ReadOnlyMemory<byte>                                                 data,   MimeType          mime, FileMetaData? metaData );
    public abstract static TClass            Create( scoped in ReadOnlySpan<byte>                                         data,   MimeType          mime, FileMetaData? metaData );
    public abstract static TClass            Create( string                                                               data,   MimeType          mime, FileMetaData? metaData, Encoding? encoding = null );
}



[Serializable, SuppressMessage( "ReSharper", "InconsistentNaming" )]
[SuppressMessage(               "ReSharper", "RedundantExplicitPositionalPropertyDeclaration" )]
public abstract record FileData<TClass, TID, TFileMetaData>( MimeType MimeType, long FileSize, string Hash, string Payload, TID ID, TFileMetaData? MetaData ) : BaseRecord<TClass, TID>( ID ), IFileData<TID, TFileMetaData>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TFileMetaData : IFileMetaData, IComparable<TFileMetaData>, IEquatable<TFileMetaData>
    where TClass : FileData<TClass, TID, TFileMetaData>, IFileData<TClass, TID, TFileMetaData>
{
    public                                    MimeType       MimeType { get; init; } = MimeType;
    public                                    long           FileSize { get; init; } = FileSize;
    [StringLength( UNICODE_CAPACITY )] public string         Hash     { get; init; } = Hash;
    [StringLength( UNICODE_CAPACITY )] public string         Payload  { get; init; } = Payload;
    public                                    TFileMetaData? MetaData { get; init; } = MetaData;


    protected FileData( IFileData<TID, TFileMetaData> file ) : this( file, file.MetaData ) { }
    protected FileData( IFileData<TID>                file,    TFileMetaData? metaData ) : this( file.MimeType, file.FileSize, file.Hash, file.Payload, default, metaData ) { }
    protected FileData( scoped in ReadOnlySpan<byte>  content, MimeType       mime, TFileMetaData? metaData ) : this( mime, content.Length, content.GetHash(), Convert.ToBase64String( content ), default, metaData ) { }


    public override int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int mimeTypeComparison = MimeType.CompareTo( other.MimeType );
        if ( mimeTypeComparison != 0 ) { return mimeTypeComparison; }

        int fileSizeComparison = FileSize.CompareTo( other.FileSize );
        if ( fileSizeComparison != 0 ) { return fileSizeComparison; }

        int hashComparison = string.Compare( Hash, other.Hash, StringComparison.Ordinal );
        if ( hashComparison != 0 ) { return hashComparison; }

        int payloadComparison = string.Compare( Payload, other.Payload, StringComparison.Ordinal );
        if ( payloadComparison != 0 ) { return payloadComparison; }

        return Comparer<TFileMetaData>.Default.Compare( MetaData, other.MetaData );
    }
    public override bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return MimeType == other.MimeType && FileSize == other.FileSize && Hash == other.Hash && Payload == other.Payload && EqualityComparer<TFileMetaData>.Default.Equals( MetaData, other.MetaData );
    }
    public override int GetHashCode() => HashCode.Combine( MimeType, FileSize, Hash, Payload, MetaData );
}



public static class FileDataExtensions
{
    public static OneOf<byte[], string> TryGetData( this string data )
    {
        try { return Convert.FromBase64String( data ); }
        catch ( FormatException ) { return data; }
    }

    public static Stream GetStream<TID>( this IFileData<TID> data )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable => data.GetStream( Encoding.Default );


    public static Stream GetStream<TID>( this IFileData<TID> data, Encoding encoding )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    {
        OneOf<byte[], string> one = data.GetData();

        return one.IsT0
                   ? new MemoryStream( one.AsT0 )
                   : new MemoryStream( encoding.GetBytes( one.AsT1 ) );
    }


    public static OneOf<byte[], string> GetData<TID>( this IFileData<TID> data )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable => data.MimeType.IsText()
                                                                                                                                                               ? data.Payload
                                                                                                                                                               : data.Payload.TryGetData();
}



[Serializable]
[SuppressMessage( "ReSharper", "RedundantExplicitPositionalPropertyDeclaration" )]
public sealed class FileMetaData( string? fileName, string? fileType, string? fileDescription = null ) : IFileMetaData, IEquatable<FileMetaData>, IComparable<FileMetaData>, IComparable
{
    [StringLength( UNICODE_CAPACITY )] public string? FileName        { get; init; } = fileName;
    [StringLength( UNICODE_CAPACITY )] public string? FileType        { get; init; } = fileType;
    [StringLength( UNICODE_CAPACITY )] public string? FileDescription { get; init; } = fileDescription;


    public FileMetaData( IFileMetaData value ) : this( value.FileName, value.FileType, value.FileDescription ) { }
    public FileMetaData( LocalFile     value ) : this( value.Name, value.ContentType ) { }

    public static FileMetaData Create( IFileMetaData data ) => new(data);
    public static FileMetaData? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileMetaData? data ) => data is null
                                                                                                           ? null
                                                                                                           : new FileMetaData( data );

    public static FileMetaData? TryCreate( [NotNullIfNotNull( nameof(data) )] LocalFile? data ) => data is null
                                                                                                       ? null
                                                                                                       : new FileMetaData( data );


    public bool Equals( FileMetaData? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return FileName == other.FileName && FileType == other.FileType && FileDescription == other.FileDescription;
    }
    public override int GetHashCode() => HashCode.Combine( FileName, FileType, FileDescription );
    public int CompareTo( FileMetaData? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int fileNameComparison = string.Compare( FileName, other.FileName, StringComparison.Ordinal );
        if ( fileNameComparison != 0 ) { return fileNameComparison; }

        int fileTypeComparison = string.Compare( FileType, other.FileType, StringComparison.Ordinal );
        if ( fileTypeComparison != 0 ) { return fileTypeComparison; }

        return string.Compare( FileDescription, other.FileDescription, StringComparison.Ordinal );
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is FileMetaData other
                   ? CompareTo( other )
                   : throw new ArgumentException( $"Object must be of type {nameof(FileMetaData)}" );
    }
    public static bool operator <( FileMetaData  left, FileMetaData right ) => left.CompareTo( right ) < 0;
    public static bool operator >( FileMetaData  left, FileMetaData right ) => left.CompareTo( right ) > 0;
    public static bool operator <=( FileMetaData left, FileMetaData right ) => left.CompareTo( right ) <= 0;
    public static bool operator >=( FileMetaData left, FileMetaData right ) => left.CompareTo( right ) >= 0;
}
