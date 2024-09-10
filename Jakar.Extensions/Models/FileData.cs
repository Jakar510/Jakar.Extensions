// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:10

namespace Jakar.Extensions;


public interface IFileMetaData<out TID> : IUniqueID<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    string? FileDescription { get; }
    string? FileName        { get; }
    string? FileType        { get; }
}



public interface IFileData<out TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    long                       FileSize { get; }
    string                     Hash     { get; }
    MimeType                   MimeType { get; }
    string                     Payload  { get; }
    public IFileMetaData<TID>? MetaData { get; }


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



public interface IFileData<out TID, out TMetaData> : IFileData<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TMetaData : IFileMetaData<TMetaData, TID>, IComparable<TMetaData>, IEquatable<TMetaData>
{
    public new TMetaData? MetaData { get; }
}



[SuppressMessage( "ReSharper", "TypeParameterCanBeVariant" )]
public interface IFileData<TClass, TID, TMetaData> : IFileData<TID, TMetaData>, IComparable<TClass>, IEquatable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TMetaData : IFileMetaData<TMetaData, TID>, IComparable<TMetaData>, IEquatable<TMetaData>
    where TClass : IFileData<TClass, TID, TMetaData>
{
    public abstract static TClass            Create( IFileData<TID, TMetaData>                                        data );
    public abstract static TClass?           TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData<TID, TMetaData>? data );
    public abstract static ValueTask<TClass> Create( LocalFile                                                        file,   CancellationToken token                                              = default );
    public abstract static ValueTask<TClass> Create( Stream                                                           stream, MimeType          mime, TMetaData? metaData, CancellationToken token = default );
    public abstract static TClass            Create( MemoryStream                                                     stream, MimeType          mime, TMetaData? metaData );
    public abstract static TClass            Create( ReadOnlyMemory<byte>                                             data,   MimeType          mime, TMetaData? metaData );
    public abstract static TClass            Create( scoped in ReadOnlySpan<byte>                                     data,   MimeType          mime, TMetaData? metaData );
    public abstract static TClass            Create( string                                                           data,   MimeType          mime, TMetaData? metaData, Encoding? encoding = null );
}



[SuppressMessage( "ReSharper", "TypeParameterCanBeVariant" )]
public interface IFileMetaData<TClass, TID> : IFileMetaData<TID>, IComparable<TClass>, IEquatable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : IFileMetaData<TClass, TID>
{
    public abstract static TClass  Create( IFileMetaData<TID>                                        data );
    public abstract static TClass? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileMetaData<TID>? data );
}



[Serializable]
[SuppressMessage( "ReSharper", "RedundantExplicitPositionalPropertyDeclaration" )]
public abstract record FileMetaData<TClass, TID>( string? FileName, string? FileType, string? FileDescription = null, TID ID = default ) : BaseRecord<TClass, TID>( ID ), IFileMetaData<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : FileMetaData<TClass, TID>, IFileMetaData<TClass, TID>
{
    [StringLength( UNICODE_CAPACITY )] public string? FileName        { get; init; } = FileName;
    [StringLength( UNICODE_CAPACITY )] public string? FileType        { get; init; } = FileType;
    [StringLength( UNICODE_CAPACITY )] public string? FileDescription { get; init; } = FileDescription;


    protected FileMetaData( IFileMetaData<TID> value ) : this( value.FileName, value.FileType, value.FileDescription, value.ID ) { }
    protected FileMetaData( LocalFile          value ) : this( value.Name, value.ContentType ) { }


    public override int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int fileNameComparison = string.Compare( FileName, other.FileName, StringComparison.Ordinal );
        if ( fileNameComparison != 0 ) { return fileNameComparison; }

        int fileTypeComparison = string.Compare( FileType, other.FileType, StringComparison.Ordinal );
        if ( fileTypeComparison != 0 ) { return fileTypeComparison; }

        return string.Compare( FileDescription, other.FileDescription, StringComparison.Ordinal );
    }
    public override bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return FileName == other.FileName && FileType == other.FileType && FileDescription == other.FileDescription;
    }
    public override int GetHashCode() => HashCode.Combine( FileName, FileType, FileDescription );
}



[Serializable, SuppressMessage( "ReSharper", "InconsistentNaming" )]
[SuppressMessage(               "ReSharper", "RedundantExplicitPositionalPropertyDeclaration" )]
public abstract record FileData<TClass, TID, TMetaData>( MimeType MimeType, long FileSize, string Hash, string Payload, TMetaData? MetaData, TID ID = default ) : BaseRecord<TClass, TID>( ID ), IFileData<TID, TMetaData>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : FileData<TClass, TID, TMetaData>, IFileData<TClass, TID, TMetaData>
    where TMetaData : class, IFileMetaData<TMetaData, TID>
{
    IFileMetaData<TID>? IFileData<TID>.                  MetaData => MetaData;
    public                                    MimeType   MimeType { get; init; } = MimeType;
    public                                    long       FileSize { get; init; } = FileSize;
    [StringLength( UNICODE_CAPACITY )] public string     Hash     { get; init; } = Hash;
    [StringLength( UNICODE_CAPACITY )] public string     Payload  { get; init; } = Payload;
    public                                    TMetaData? MetaData { get; init; } = MetaData;


    protected FileData( IFileData<TID, TMetaData> file ) : this( file, TMetaData.TryCreate( file.MetaData ) ) { }
    protected FileData( IFileData<TID>               file,    TMetaData? metaData ) : this( file.MimeType, file.FileSize, file.Hash, file.Payload, metaData ) { }
    protected FileData( scoped in ReadOnlySpan<byte> content, MimeType   mime, TMetaData? metaData ) : this( mime, content.Length, Hashes.GetHash( content ), Convert.ToBase64String( content ), metaData ) { }


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

        return Sorter<TMetaData>.Default.Compare( MetaData, other.MetaData );
    }
    public override bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return MimeType == other.MimeType && FileSize == other.FileSize && Hash == other.Hash && Payload == other.Payload && Equalizer<TMetaData>.Default.Equals( MetaData, other.MetaData );
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
