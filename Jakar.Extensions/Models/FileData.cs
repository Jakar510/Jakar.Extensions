// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:10

namespace Jakar.Extensions;


public interface IFileMetaData : IJsonModel
{
    string?   FileDescription { get; }
    string?   FileName        { get; }
    string?   FileType        { get; }
    MimeType? MimeType        { get; }
}



public interface IFileMetaData<TFileMetaData> : IFileMetaData, IJsonModel<TFileMetaData>, IEqualComparable<TFileMetaData>
    where TFileMetaData : class, IFileMetaData<TFileMetaData>
{
    public abstract static TFileMetaData  Create( IFileMetaData                                      data );
    public abstract static TFileMetaData? TryCreate( [NotNullIfNotNull(nameof(data))] IFileMetaData? data );
    public abstract static TFileMetaData  Create( LocalFile                                          file );
    public abstract static TFileMetaData  Create( string?                                            fileName, MimeType mimeType, string?   fileDescription                   = null );
    public abstract static TFileMetaData  Create( string?                                            fileName, string?  fileType, MimeType? mimeType, string? fileDescription = null );
}



public interface IFileData<out TID> : IUniqueID<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    long   FileSize { get; }
    string Hash     { get; }
    string Payload  { get; }
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



[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IFileData<TSelf, TID, TFileMetaData> : IFileData<TID, TFileMetaData>, IJsonModel<TSelf>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TFileMetaData : class, IFileMetaData<TFileMetaData>
    where TSelf : class, IFileData<TSelf, TID, TFileMetaData>
{
    public abstract static TSelf  Create( IFileData<TID, TFileMetaData>                                      data );
    public abstract static TSelf  Create( IFileData<TID>                                                     data, TFileMetaData metaData );
    public abstract static TSelf? TryCreate( [NotNullIfNotNull(nameof(data))] IFileData<TID, TFileMetaData>? data );
    public abstract static TSelf? TryCreate( [NotNullIfNotNull(nameof(data))] IFileData<TID>?                data, TFileMetaData metaData );


    public abstract static ValueTask<TSelf> Create( LocalFile     file,     CancellationToken                 token                            = default );
    public abstract static ValueTask<TSelf> Create( TFileMetaData metaData, Stream                            content, CancellationToken token = default );
    public abstract static TSelf            Create( TFileMetaData metaData, MemoryStream                      content );
    public abstract static TSelf            Create( TFileMetaData metaData, ref readonly ReadOnlyMemory<byte> content );
    public abstract static TSelf            Create( TFileMetaData metaData, params       ReadOnlySpan<byte>   content );
    public abstract static TSelf            Create( TFileMetaData metaData, string                            content, Encoding? encoding = null );
    public abstract static TSelf            Create( long          fileSize, string                            hash,    string    payload, TID id, TFileMetaData metaData );
}



[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "RedundantExplicitPositionalPropertyDeclaration")]
public abstract class FileData<TSelf, TID, TFileMetaData>( long fileSize, string hash, string payload, TID id, TFileMetaData metaData ) : BaseClass<TSelf>, IFileData<TID, TFileMetaData>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TFileMetaData : class, IFileMetaData<TFileMetaData>
    where TSelf : FileData<TSelf, TID, TFileMetaData>, IFileData<TSelf, TID, TFileMetaData>, IJsonModel<TSelf>, IEqualComparable<TSelf>
{
    public                          long          FileSize { get; init; } = fileSize;
    [StringLength(HASH)] public     string        Hash     { get; init; } = hash;
    public                          TID           ID       { get; init; } = id;
    public                          TFileMetaData MetaData { get; init; } = metaData;
    protected internal              MimeType      Mime     => MetaData.MimeType ?? MimeType.Text;
    [StringLength(MAX_SIZE)] public string        Payload  { get; init; } = payload;


    public virtual LocalFile GetFile( LocalDirectory directory )
    {
        string extension = Mime.ToExtension();
        string name      = Path.GetFileName(MetaData.FileName) ?? Guids.NewBase64();
        string fileName  = $"{name}.{extension}";
        return directory.Join(fileName);
    }
    public async Task WriteToAsync( LocalDirectory directory, CancellationToken token ) => await WriteToAsync(GetFile(directory), token)
                                                                                              .ConfigureAwait(false);
    public async Task WriteToAsync( LocalFile file, CancellationToken token )
    {
        await using FileStream stream = file.OpenWrite(FileMode.OpenOrCreate);

        await WriteToAsync(stream, token)
           .ConfigureAwait(false);
    }
    public async Task WriteToAsync( Stream stream, CancellationToken token )
    {
        if ( !stream.CanWrite ) { throw new ArgumentException("Stream is not writable"); }

        if ( !stream.CanSeek ) { throw new ArgumentException("Stream is not seekable"); }

        stream.Seek(0, SeekOrigin.Begin);
        OneOf<byte[], string> data = GetData();

        if ( data.IsT1 )
        {
            await using StreamWriter writer = new(stream);

            await writer.WriteAsync(data.AsT1)
                        .ConfigureAwait(false);

            return;
        }

        ReadOnlyMemory<byte> payload = data.AsT0;

        await stream.WriteAsync(payload, token)
                    .ConfigureAwait(false);
    }
    public void WriteTo( Stream stream )
    {
        if ( !stream.CanWrite ) { throw new ArgumentException("Stream is not writable"); }

        if ( !stream.CanSeek ) { throw new ArgumentException("Stream is not seekable"); }

        stream.Seek(0, SeekOrigin.Begin);
        OneOf<byte[], string> data = GetData();

        if ( data.IsT1 )
        {
            using StreamWriter writer = new(stream);
            writer.Write(data.AsT1);
            return;
        }

        stream.Write(data.AsT0);
    }
    public MemoryStream GetPayloadAsStream()
    {
        MemoryStream stream = new(Payload.Length);
        WriteTo(stream);
        return stream;
    }
    public OneOf<byte[], string> GetData() => Mime.IsText()
                                                  ? Payload
                                                  : Payload.TryGetData();


    public static TSelf Create( IFileData<TID, TFileMetaData> data )                         => Create(data, data.MetaData);
    public static TSelf Create( IFileData<TID>                data, TFileMetaData metaData ) => TSelf.Create(data.FileSize, data.Hash, data.Payload, data.ID, metaData);
    public static TSelf Create( TFileMetaData metaData, MemoryStream stream ) => Create(metaData,
                                                                                        stream.AsReadOnlyMemory()
                                                                                              .Span);
    public static TSelf Create( TFileMetaData metaData, ref readonly ReadOnlyMemory<byte> content )                            => Create(metaData, content.Span);
    public static TSelf Create( TFileMetaData metaData, params       ReadOnlySpan<byte>   content )                            => TSelf.Create(content.Length, content.Hash_SHA512(),                             Convert.ToBase64String(content), default, metaData);
    public static TSelf Create( TFileMetaData metaData, string                            content, Encoding? encoding = null ) => TSelf.Create(content.Length, content.Hash_SHA512(encoding ?? Encoding.Default), content,                         default, metaData);


    public static TSelf? TryCreate( [NotNullIfNotNull(nameof(content))] IFileData<TID, TFileMetaData>? content ) => content is not null
                                                                                                                        ? Create(content)
                                                                                                                        : null;
    public static TSelf? TryCreate( [NotNullIfNotNull(nameof(content))] IFileData<TID>? content, TFileMetaData metaData ) => content is not null
                                                                                                                                 ? Create(content, metaData)
                                                                                                                                 : null;
    public static async ValueTask<TSelf> Create( LocalFile file, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        ReadOnlyMemory<byte> content = await file.ReadAsync()
                                                 .AsMemory(token);

        return Create(TFileMetaData.Create(file), content.Span);
    }
    public static async ValueTask<TSelf> Create( TFileMetaData metaData, Stream stream, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        stream.Seek(0, SeekOrigin.Begin);
        using MemoryStream memory = await stream.ToMemoryStream();
        return Create(metaData, memory);
    }


    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int fileSizeComparison = FileSize.CompareTo(other.FileSize);
        if ( fileSizeComparison != 0 ) { return fileSizeComparison; }

        int hashComparison = string.Compare(Hash, other.Hash, StringComparison.Ordinal);
        if ( hashComparison != 0 ) { return hashComparison; }

        int payloadComparison = string.Compare(Payload, other.Payload, StringComparison.Ordinal);
        if ( payloadComparison != 0 ) { return payloadComparison; }

        return Comparer<TFileMetaData>.Default.Compare(MetaData, other.MetaData);
    }
    public override bool Equals( TSelf? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return FileSize == other.FileSize && Hash == other.Hash && Payload == other.Payload && EqualityComparer<TFileMetaData>.Default.Equals(MetaData, other.MetaData);
    }
    public override int GetHashCode() => HashCode.Combine(FileSize, Hash, Payload, MetaData);
    public void Deconstruct( out long fileSize, out string hash, out string payload, out TID id, out TFileMetaData metaData )
    {
        fileSize = FileSize;
        hash     = Hash;
        payload  = Payload;
        id       = ID;
        metaData = MetaData;
    }
}



[Serializable]
[SuppressMessage("ReSharper", "RedundantExplicitPositionalPropertyDeclaration")]
[method: JsonConstructor]
public sealed class FileMetaData( string? fileName, string? fileType, MimeType? mimeType, string? fileDescription = null ) : BaseClass<FileMetaData>, IFileMetaData<FileMetaData>
{
    public static                      JsonSerializerContext        JsonContext     => JakarExtensionsContext.Default;
    public static                      JsonTypeInfo<FileMetaData>   JsonTypeInfo    => JakarExtensionsContext.Default.FileMetaData;
    public static                      JsonTypeInfo<FileMetaData[]> JsonArrayInfo   => JakarExtensionsContext.Default.FileMetaDataArray;
    [StringLength(DESCRIPTION)] public string?                      FileDescription { get; set; }  = fileDescription;
    [StringLength(NAME)]        public string?                      FileName        { get; init; } = fileName;
    [StringLength(TYPE)]        public string?                      FileType        { get; init; } = fileType;
    public                             MimeType?                    MimeType        { get; init; } = mimeType;


    public FileMetaData( IFileMetaData value ) : this(value.FileName, value.FileType, value.MimeType, value.FileDescription)
    {
        if ( value.AdditionalData is not null ) { AdditionalData = new JsonObject(value.AdditionalData); }
    }
    public FileMetaData( LocalFile value ) : this(value.Name, value.ContentType, value.Mime) { }


    public static FileMetaData Create( IFileMetaData data )                                                                            => new(data);
    public static FileMetaData Create( LocalFile     file )                                                                            => new(file);
    public static FileMetaData Create( string?       fileName, MimeType mimeType, string?   fileDescription                   = null ) => new(fileName, mimeType.ToContentType(), mimeType, fileDescription);
    public static FileMetaData Create( string?       fileName, string?  fileType, MimeType? mimeType, string? fileDescription = null ) => new(fileName, fileType, mimeType, fileDescription);
    public static FileMetaData? TryCreate( [NotNullIfNotNull(nameof(data))] IFileMetaData? data ) => data is null
                                                                                                         ? null
                                                                                                         : new FileMetaData(data);
    public static FileMetaData? TryCreate( [NotNullIfNotNull(nameof(data))] LocalFile? data ) => data is null
                                                                                                     ? null
                                                                                                     : new FileMetaData(data);


    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals(this, other) || ( other is FileMetaData data && Equals(data) );
    }
    public override bool Equals( FileMetaData? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return FileName == other.FileName && FileType == other.FileType && FileDescription == other.FileDescription;
    }
    public override int CompareTo( FileMetaData? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int fileNameComparison = string.Compare(FileName, other.FileName, StringComparison.Ordinal);
        if ( fileNameComparison != 0 ) { return fileNameComparison; }

        int fileTypeComparison = string.Compare(FileType, other.FileType, StringComparison.Ordinal);
        if ( fileTypeComparison != 0 ) { return fileTypeComparison; }

        return string.Compare(FileDescription, other.FileDescription, StringComparison.Ordinal);
    }
    public override int GetHashCode() => HashCode.Combine(FileName, FileType, MimeType);


    public static bool operator ==( FileMetaData? left, FileMetaData? right ) => EqualityComparer<FileMetaData>.Default.Equals(left, right);
    public static bool operator !=( FileMetaData? left, FileMetaData? right ) => !EqualityComparer<FileMetaData>.Default.Equals(left, right);
    public static bool operator >( FileMetaData   left, FileMetaData  right ) => Comparer<FileMetaData>.Default.Compare(left, right) > 0;
    public static bool operator >=( FileMetaData  left, FileMetaData  right ) => Comparer<FileMetaData>.Default.Compare(left, right) >= 0;
    public static bool operator <( FileMetaData   left, FileMetaData  right ) => Comparer<FileMetaData>.Default.Compare(left, right) < 0;
    public static bool operator <=( FileMetaData  left, FileMetaData  right ) => Comparer<FileMetaData>.Default.Compare(left, right) <= 0;
}
