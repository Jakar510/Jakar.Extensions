// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  16:01

using Jakar.Extensions.UserGuid;



namespace Jakar.Extensions.Telemetry;


[Serializable]
public sealed record TelemetryBaggage( MimeType MimeType, long FileSize, string Hash, string Payload, FileMetaData? MetaData, Guid ID = default ) : FileData<TelemetryBaggage, Guid, FileMetaData>( MimeType, FileSize, Hash, Payload, AssertIsValid( ID ), MetaData ), IFileData<TelemetryBaggage, Guid, FileMetaData>
{
    public TelemetryBaggage( IFileData<Guid, FileMetaData> file ) : this( file, file.MetaData ) { }
    public TelemetryBaggage( IFileData<Guid>               file, FileMetaData? metaData ) : this( file.MimeType, file.FileSize, file.Hash, file.Payload, metaData ) { }
    public TelemetryBaggage( MimeType                      mime, FileMetaData? metaData, params ReadOnlySpan<byte> content ) : this( mime, content.Length, content.GetHash(), Convert.ToBase64String( content ), metaData ) { }


    public static implicit operator TelemetryBaggage( FileData data ) => new(data.MimeType, data.FileSize, data.Hash, data.Payload, data.MetaData, data.ID);


    public static TelemetryBaggage Create( MimeType mime, long fileSize, string hash, string payload, Guid id, FileMetaData? metaData ) => new(mime, fileSize, hash, payload, metaData, id);


    private static Guid AssertIsValid( Guid id ) => id.IsNotValidID()
                                                        ? Guid.CreateVersion7()
                                                        : id;



    public sealed class Collection() : LinkedList<TelemetryBaggage>()
    {
        public static Collection Create() => new();
        public Collection( IEnumerable<TelemetryBaggage> values ) : this()
        {
            foreach ( TelemetryBaggage tag in values ) { AddLast( tag ); }
        }
    }
}
