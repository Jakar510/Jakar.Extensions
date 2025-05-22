// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:10

namespace Jakar.Extensions;


public interface IFileMetaData : JsonModels.IJsonModel
{
    string?   FileDescription { get; }
    string?   FileName        { get; }
    string?   FileType        { get; }
    MimeType? MimeType        { get; }
}



public interface IFileMetaData<TClass> : IFileMetaData, IEquatable<TClass>, IComparable<TClass>, IComparable
    where TClass : class, IFileMetaData<TClass>
{
    public abstract static Equalizer<TClass> Equalizer { get; }
    public abstract static Sorter<TClass>    Sorter    { get; }


    public abstract static TClass  Create( IFileMetaData                                        data );
    public abstract static TClass? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileMetaData? data );
    public abstract static TClass  Create( LocalFile                                            file );
    public abstract static TClass  Create( string?                                              fileName, MimeType mimeType, string?   fileDescription                   = null );
    public abstract static TClass  Create( string?                                              fileName, string?  fileType, MimeType? mimeType, string? fileDescription = null );
}



public interface IFileData<out TID> : IUniqueID<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    long   FileSize { get; }
    string Hash     { get; }
    string Payload  { get; }


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
    where TFileMetaData : class, IFileMetaData<TFileMetaData>
{
    public TFileMetaData MetaData { get; }


    public LocalFile GetFile( LocalDirectory      directory );
    public void      WriteTo( Stream              stream );
    public Task      WriteToAsync( LocalDirectory directory, CancellationToken token );
    public Task      WriteToAsync( LocalFile      file,      CancellationToken token );
    public Task      WriteToAsync( Stream         stream,    CancellationToken token );


    public MemoryStream          GetPayloadAsStream();
    public OneOf<byte[], string> GetData();
}



[SuppressMessage( "ReSharper", "TypeParameterCanBeVariant" )]
public interface IFileData<TClass, TID, TFileMetaData> : IFileData<TID, TFileMetaData>, IComparable<TClass>, IEquatable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TFileMetaData : class, IFileMetaData<TFileMetaData>
    where TClass : class, IFileData<TClass, TID, TFileMetaData>
{
    public abstract static Equalizer<TClass> Equalizer { get; }
    public abstract static Sorter<TClass>    Sorter    { get; }


    public abstract static TClass  Create( IFileData<TID, TFileMetaData>                                        data );
    public abstract static TClass  Create( IFileData<TID>                                                       data, TFileMetaData metaData );
    public abstract static TClass? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData<TID, TFileMetaData>? data );
    public abstract static TClass? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData<TID>?                data, TFileMetaData metaData );


    public abstract static ValueTask<TClass> Create( LocalFile     file,     TelemetrySpan                     parent = default, CancellationToken token  = default );
    public abstract static ValueTask<TClass> Create( TFileMetaData metaData, Stream                            content,          TelemetrySpan     parent = default, CancellationToken token = default );
    public abstract static TClass            Create( TFileMetaData metaData, MemoryStream                      content );
    public abstract static TClass            Create( TFileMetaData metaData, ref readonly ReadOnlyMemory<byte> content );
    public abstract static TClass            Create( TFileMetaData metaData, params       ReadOnlySpan<byte>   content );
    public abstract static TClass            Create( TFileMetaData metaData, string                            content, Encoding? encoding = null );
    public abstract static TClass            Create( long          fileSize, string                            hash,    string    payload, TID id, TFileMetaData metaData );
}



[Serializable, SuppressMessage( "ReSharper", "InconsistentNaming" ), SuppressMessage( "ReSharper", "RedundantExplicitPositionalPropertyDeclaration" )]
[method: SetsRequiredMembers]
public abstract class FileData<TClass, TID, TFileMetaData>( long fileSize, string hash, string payload, TID id, TFileMetaData metaData ) : BaseClass, IFileData<TID, TFileMetaData>, IComparable<TClass>, IEquatable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TFileMetaData : class, IFileMetaData<TFileMetaData>
    where TClass : FileData<TClass, TID, TFileMetaData>, IFileData<TClass, TID, TFileMetaData>
{
    public static                                      Equalizer<TClass> Equalizer { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Equalizer<TClass>.Default; }
    public static                                      Sorter<TClass>    Sorter    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<TClass>.Default; }
    public required                                    long              FileSize  { get; init; } = fileSize;
    [StringLength( UNICODE_CAPACITY )] public required string            Hash      { get; init; } = hash;
    public required                                    TID               ID        { get; init; } = id;
    public required                                    TFileMetaData     MetaData  { get; init; } = metaData;
    protected internal                                 MimeType          Mime      => MetaData.MimeType ?? MimeType.Text;
    [StringLength( UNICODE_CAPACITY )] public required string            Payload   { get; init; } = payload;


    public virtual LocalFile GetFile( LocalDirectory directory )
    {
        string extension = Mime.ToExtension();
        string name      = Path.GetFileName( MetaData.FileName ) ?? Guids.NewBase64();
        string fileName  = $"{name}.{extension}";
        return directory.Join( fileName );
    }
    public async Task WriteToAsync( LocalDirectory directory, CancellationToken token ) => await WriteToAsync( GetFile( directory ), token ).ConfigureAwait( false );
    public async Task WriteToAsync( LocalFile file, CancellationToken token )
    {
        await using FileStream stream = file.OpenWrite( FileMode.OpenOrCreate );
        await WriteToAsync( stream, token ).ConfigureAwait( false );
    }
    public async Task WriteToAsync( Stream stream, CancellationToken token )
    {
        if ( stream.CanWrite is false ) { throw new ArgumentException( "Stream is not writable" ); }

        if ( stream.CanSeek is false ) { throw new ArgumentException( "Stream is not seekable" ); }

        stream.Seek( 0, SeekOrigin.Begin );
        OneOf<byte[], string> data = GetData();

        if ( data.IsT1 )
        {
            await using StreamWriter writer = new(stream);
            await writer.WriteAsync( data.AsT1 ).ConfigureAwait( false );
            return;
        }

        ReadOnlyMemory<byte> payload = data.AsT0;
        await stream.WriteAsync( payload, token ).ConfigureAwait( false );
    }
    public void WriteTo( Stream stream )
    {
        if ( stream.CanWrite is false ) { throw new ArgumentException( "Stream is not writable" ); }

        if ( stream.CanSeek is false ) { throw new ArgumentException( "Stream is not seekable" ); }

        stream.Seek( 0, SeekOrigin.Begin );
        OneOf<byte[], string> data = GetData();

        if ( data.IsT1 )
        {
            using StreamWriter writer = new(stream);
            writer.Write( data.AsT1 );
            return;
        }

        stream.Write( data.AsT0 );
    }
    public MemoryStream GetPayloadAsStream()
    {
        MemoryStream stream = new(Payload.Length);
        WriteTo( stream );
        return stream;
    }
    public OneOf<byte[], string> GetData() => Mime.IsText()
                                                  ? Payload
                                                  : Payload.TryGetData();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TClass Create( IFileData<TID, TFileMetaData> data )                                                                           => Create( data, data.MetaData );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TClass Create( IFileData<TID>                data,     TFileMetaData                     metaData )                           => TClass.Create( data.FileSize, data.Hash, data.Payload, data.ID, metaData );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TClass Create( TFileMetaData                 metaData, MemoryStream                      stream )                             => Create( metaData, stream.AsReadOnlyMemory().Span );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TClass Create( TFileMetaData                 metaData, ref readonly ReadOnlyMemory<byte> content )                            => Create( metaData, content.Span );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TClass Create( TFileMetaData                 metaData, params       ReadOnlySpan<byte>   content )                            => TClass.Create( content.Length, content.Hash_SHA512(),                               Convert.ToBase64String( content ), default, metaData );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TClass Create( TFileMetaData                 metaData, string                            content, Encoding? encoding = null ) => TClass.Create( content.Length, content.Hash_SHA512( encoding ?? Encoding.Default ), content,                           default, metaData );


    public static TClass? TryCreate( [NotNullIfNotNull( nameof(content) )] IFileData<TID, TFileMetaData>? content ) => content is not null
                                                                                                                           ? Create( content )
                                                                                                                           : null;
    public static TClass? TryCreate( [NotNullIfNotNull( nameof(content) )] IFileData<TID>? content, TFileMetaData metaData ) => content is not null
                                                                                                                                    ? Create( content, metaData )
                                                                                                                                    : null;
    public static async ValueTask<TClass> Create( LocalFile file, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan  span    = parent.SubSpan();
        ReadOnlyMemory<byte> content = await file.ReadAsync().AsMemory( span, token );
        return Create( TFileMetaData.Create( file ), content.Span );
    }
    public static async ValueTask<TClass> Create( TFileMetaData metaData, Stream stream, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        stream.Seek( 0, SeekOrigin.Begin );
        using MemoryStream memory = new((int)stream.Length);
        await stream.CopyToAsync( memory, token );
        return Create( metaData, memory );
    }


    public virtual int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int fileSizeComparison = FileSize.CompareTo( other.FileSize );
        if ( fileSizeComparison != 0 ) { return fileSizeComparison; }

        int hashComparison = string.Compare( Hash, other.Hash, StringComparison.Ordinal );
        if ( hashComparison != 0 ) { return hashComparison; }

        int payloadComparison = string.Compare( Payload, other.Payload, StringComparison.Ordinal );
        if ( payloadComparison != 0 ) { return payloadComparison; }

        return TFileMetaData.Sorter.Compare( MetaData, other.MetaData );
    }
    public virtual bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return FileSize == other.FileSize && Hash == other.Hash && Payload == other.Payload && TFileMetaData.Equalizer.Equals( MetaData, other.MetaData );
    }
    public override int GetHashCode() => HashCode.Combine( FileSize, Hash, Payload, MetaData );
    public void Deconstruct( out long fileSize, out string hash, out string payload, out TID id, out TFileMetaData metaData )
    {
        fileSize = FileSize;
        hash     = Hash;
        payload  = Payload;
        id       = ID;
        metaData = MetaData;
    }
}



[Serializable, SuppressMessage( "ReSharper", "RedundantExplicitPositionalPropertyDeclaration" )]
public sealed class FileMetaData( string? fileName, string? fileType, MimeType? mimeType, string? fileDescription = null ) : IFileMetaData<FileMetaData>
{
    public static                             Equalizer<FileMetaData>       Equalizer       => Equalizer<FileMetaData>.Default;
    public static                             Sorter<FileMetaData>          Sorter          => Sorter<FileMetaData>.Default;
    [JsonExtensionData]                public IDictionary<string, JToken?>? AdditionalData  { get; set; }
    [StringLength( UNICODE_CAPACITY )] public string?                       FileDescription { get; set; }  = fileDescription;
    [StringLength( UNICODE_CAPACITY )] public string?                       FileName        { get; init; } = fileName;
    [StringLength( UNICODE_CAPACITY )] public string?                       FileType        { get; init; } = fileType;
    [StringLength( UNICODE_CAPACITY )] public MimeType?                     MimeType        { get; init; } = mimeType;


    public FileMetaData( IFileMetaData value ) : this( value.FileName, value.FileType, value.MimeType, value.FileDescription )
    {
        if ( value.AdditionalData is not null ) { AdditionalData = new Dictionary<string, JToken?>( value.AdditionalData ); }
    }
    public FileMetaData( LocalFile value ) : this( value.Name, value.ContentType, value.Mime ) { }


    public static FileMetaData Create( IFileMetaData data )                                                                            => new(data);
    public static FileMetaData Create( LocalFile     file )                                                                            => new(file);
    public static FileMetaData Create( string?       fileName, MimeType mimeType, string?   fileDescription                   = null ) => new(fileName, mimeType.ToContentType(), mimeType, fileDescription);
    public static FileMetaData Create( string?       fileName, string?  fileType, MimeType? mimeType, string? fileDescription = null ) => new(fileName, fileType, mimeType, fileDescription);
    public static FileMetaData? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileMetaData? data ) => data is null
                                                                                                           ? null
                                                                                                           : new FileMetaData( data );
    public static FileMetaData? TryCreate( [NotNullIfNotNull( nameof(data) )] LocalFile? data ) => data is null
                                                                                                       ? null
                                                                                                       : new FileMetaData( data );


    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals( this, other ) || other is FileMetaData data && Equals( data );
    }
    public bool Equals( FileMetaData? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return FileName == other.FileName && FileType == other.FileType && FileDescription == other.FileDescription;
    }
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
    public override int GetHashCode() => HashCode.Combine( FileName, FileType, MimeType );


    public static bool operator <( FileMetaData  left, FileMetaData right ) => Sorter.Compare( left, right ) < 0;
    public static bool operator >( FileMetaData  left, FileMetaData right ) => Sorter.Compare( left, right ) > 0;
    public static bool operator <=( FileMetaData left, FileMetaData right ) => Sorter.Compare( left, right ) <= 0;
    public static bool operator >=( FileMetaData left, FileMetaData right ) => Sorter.Compare( left, right ) >= 0;
    public static bool operator ==( FileMetaData left, FileMetaData right ) => Equalizer.Equals( left, right );
    public static bool operator !=( FileMetaData left, FileMetaData right ) => Equalizer.Equals( left, right ) is false;
}
