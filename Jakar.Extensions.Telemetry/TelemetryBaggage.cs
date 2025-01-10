// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  16:01

using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Jakar.Extensions.UserGuid;



namespace Jakar.Extensions.Telemetry;


[Serializable]
public sealed record TelemetryBaggage( MimeType MimeType, long FileSize, string Hash, string Payload, FileMetaData? MetaData, Guid ID = default ) : FileData<TelemetryBaggage, Guid, FileMetaData>( MimeType, FileSize, Hash, Payload, AssertIsValid( ID ), MetaData ), IFileData<TelemetryBaggage, Guid, FileMetaData>
{
    public TelemetryBaggage( IFileData<Guid, FileMetaData>   file ) : this( file, file.MetaData ) { }
    public TelemetryBaggage( IFileData<Guid>                 file,    FileMetaData? metaData ) : this( file.MimeType, file.FileSize, file.Hash, file.Payload, metaData ) { }
    public TelemetryBaggage( ref readonly ReadOnlySpan<byte> content, MimeType      mime, FileMetaData? metaData ) : this( mime, content.Length, content.GetHash(), Convert.ToBase64String( content ), metaData ) { }


    public static implicit operator TelemetryBaggage( FileData data ) => new(data.MimeType, data.FileSize, data.Hash, data.Payload, data.MetaData, data.ID);

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TelemetryBaggage Create( string                        data,    MimeType mime, FileMetaData? metaData, Encoding? encoding = null ) => new(mime, data.Length, Hashes.Hash_SHA256( data, encoding ?? Encoding.Default ), data, metaData);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TelemetryBaggage Create( MemoryStream                  stream,  MimeType mime, FileMetaData? metaData ) => Create( stream.AsReadOnlyMemory(), mime, metaData );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TelemetryBaggage Create( ReadOnlyMemory<byte>          data,    MimeType mime, FileMetaData? metaData ) => Create( data.Span,                 mime, metaData );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TelemetryBaggage Create( scoped in ReadOnlySpan<byte>  content, MimeType mime, FileMetaData? metaData ) => new(in content, mime, metaData);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TelemetryBaggage Create( IFileData<Guid, FileMetaData> data )                         => new(data, data.MetaData);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TelemetryBaggage Create( IFileData<Guid>               data, FileMetaData? metaData ) => new(data, metaData);


    private static Guid AssertIsValid( Guid id ) => id.IsNotValidID()
                                                        ? Guid.CreateVersion7()
                                                        : id;
    public static TelemetryBaggage? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData<Guid>? data, FileMetaData? metaData ) => data is not null
                                                                                                                                         ? Create( data, metaData )
                                                                                                                                         : null;
    public static TelemetryBaggage? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData<Guid, FileMetaData>? data ) => data is not null
                                                                                                                               ? Create( data )
                                                                                                                               : null;
    public static async ValueTask<TelemetryBaggage> Create( LocalFile file, CancellationToken token = default )
    {
        ReadOnlyMemory<byte> content = await file.ReadAsync().AsBytes( token );
        ReadOnlySpan<byte>   span    = content.Span;
        return new TelemetryBaggage( in span, file.Mime, new FileMetaData( null, file.Name, file.ContentType ) );
    }
    public static async ValueTask<TelemetryBaggage> Create( Stream stream, MimeType mime, FileMetaData? metaData, CancellationToken token = default )
    {
        stream.Seek( 0, SeekOrigin.Begin );
        using MemoryStream memory = new((int)stream.Length);
        await stream.CopyToAsync( memory, token );
        return Create( memory, mime, metaData );
    }

    
    public sealed class Collection() : LinkedList<TelemetryBaggage>()
    {
        public static Collection Create() => new();
        public Collection( IEnumerable<TelemetryBaggage> values ) : this()
        {
            foreach ( var tag in values ) { AddLast( tag ); }
        }
    }
}
