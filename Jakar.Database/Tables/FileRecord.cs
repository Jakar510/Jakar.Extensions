// Jakar.Extensions :: Jakar.Database
// 4/2/2024  17:43


namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record FileRecord( string?              FileName,
                                 string?              FileDescription,
                                 string?              FileType,
                                 long                 FileSize,
                                 string               Hash,
                                 MimeType?            MimeType,
                                 string               Payload,
                                 string?              FullPath,
                                 RecordID<FileRecord> ID,
                                 DateTimeOffset       DateCreated,
                                 DateTimeOffset?      LastModified = null ) : TableRecord<FileRecord>( in ID, in DateCreated, in LastModified ), IDbReaderMapping<FileRecord>, IFileData<Guid>, IFileMetaData
{
    public const               string                        TABLE_NAME = "Files";
    public static              string                        TableName      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }


    public FileRecord( IFileData<Guid, FileMetaData>               data, LocalFile?    file                      = null ) : this( data, data.MetaData, file ) { }
    private FileRecord( IFileData<Guid>                            data, IFileMetaData metaData, LocalFile? file = null ) : this( metaData.FileName, metaData.FileDescription, metaData.FileType, data.FileSize, data.Hash, metaData.MimeType, data.Payload, file?.FullPath, RecordID<FileRecord>.New(), DateTimeOffset.UtcNow ) { }
    public static FileRecord Create( IFileData<Guid, FileMetaData> data, LocalFile?    file = null ) => new(data, file);
    public static FileRecord Create<TFileMetaData>( IFileData<Guid, TFileMetaData> data, LocalFile? file = null )
        where TFileMetaData : class, IFileMetaData<TFileMetaData> => new(data, data.MetaData, file);
    public TFileData ToFileData<TFileData, TFileMetaData>()
        where TFileData : class, IFileData<TFileData, Guid, TFileMetaData>
        where TFileMetaData : class, IFileMetaData<TFileMetaData> => TFileData.Create( this, TFileMetaData.Create( this ) );


    [Pure]
    public async ValueTask<OneOf<byte[], string, FileData>> Read( CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        if ( string.IsNullOrWhiteSpace( FullPath ) ) { return new FileData( this, FileMetaData.Create( this ) ); }

        LocalFile file = FullPath;
        if ( MimeType != file.Mime ) { throw new InvalidOperationException( $"{nameof(MimeType)} mismatch. Got {file.Mime} but expected {MimeType}" ); }

        return file.Mime.IsText()
                   ? await file.ReadAsync().AsString(telemetrySpan, token)
                   : await file.ReadAsync().AsBytes(telemetrySpan, token);
    }


    [Pure]
    public async ValueTask<ErrorOrResult<FileData>> ToFileData( CancellationToken token = default )
    {
        OneOf<byte[], string, FileData> data = await Read( token );
        if ( data.IsT2 ) { return data.AsT2; }

        if ( data.IsT0 )
        {
            byte[] content = data.AsT0;
            string hash    = Hashes.GetHash( content );
            if ( FileSize != content.Length ) { return Error.Conflict( $"{nameof(FileSize)} mismatch. Got {content.Length} but expected {FileSize}" ); }

            if ( string.Equals( Hash, hash, StringComparison.Ordinal ) is false ) { return Error.Conflict( $"{nameof(Hash)} mismatch: {Hash} != {hash}" ); }

            return new FileData( FileSize, Hash, Convert.ToBase64String( content ), FileMetaData.Create( this ) );
        }
        else
        {
            string content = data.AsT1;
            string hash    = Hashes.GetHash( content );
            if ( FileSize != content.Length ) { return Error.Conflict( $"{nameof(FileSize)} mismatch. Got {content.Length} but expected {FileSize}" ); }

            if ( string.Equals( Hash, hash, StringComparison.Ordinal ) is false ) { return Error.Conflict( $"{nameof(Hash)} mismatch: {Hash} != {hash}" ); }

            return new FileData( FileSize, Hash, content, FileMetaData.Create( this ) );
        }
    }


    [Pure]
    public async ValueTask<FileRecord> Update( LocalFile file, CancellationToken token = default )
    {
        if ( FullPath != file.FullPath ) { throw new InvalidOperationException( $"{nameof(FullPath)} mismatch. Got {file.FullPath} but expected {FullPath}" ); }

        (long fileSize, string? hash, string payload, _, FileMetaData? metaData) = await FileData.Create( file, token );

        return new FileRecord( metaData.FileName,
                               metaData.FileDescription,
                               metaData.FileType,
                               fileSize,
                               hash,
                               metaData.MimeType,
                               payload,
                               file.FullPath,
                               ID,
                               DateCreated,
                               DateTimeOffset.UtcNow );
    }


    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(FileName),        FileName );
        parameters.Add( nameof(FileDescription), FileDescription );
        parameters.Add( nameof(FileType),        FileType );
        parameters.Add( nameof(FileSize),        FileSize );
        parameters.Add( nameof(Hash),            Hash );
        parameters.Add( nameof(MimeType),        MimeType );
        parameters.Add( nameof(Payload),         Payload );
        parameters.Add( nameof(FullPath),        FullPath );
        parameters.Add( nameof(ID),              ID );
        parameters.Add( nameof(DateCreated),     DateCreated );
        parameters.Add( nameof(LastModified),    LastModified );
        return parameters;
    }


    [Pure]
    public static FileRecord Create( DbDataReader reader )
    {
        string?              name         = reader.GetFieldValue<string?>( nameof(FileName) );
        string?              description  = reader.GetFieldValue<string?>( nameof(FileDescription) );
        string?              fileType     = reader.GetFieldValue<string?>( nameof(FileType) );
        long                 size         = reader.GetFieldValue<long>( nameof(FileSize) );
        string               hash         = reader.GetFieldValue<string>( nameof(Hash) );
        var                  mime         = reader.GetFieldValue<MimeType>( nameof(MimeType) );
        string               payload      = reader.GetString( nameof(Payload) );
        string?              fullPath     = reader.GetValue<string?>( nameof(FullPath) );
        var                  dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var                  lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<FileRecord> id           = RecordID<FileRecord>.ID( reader );

        FileRecord record = new(name,
                                description,
                                fileType,
                                size,
                                hash,
                                mime,
                                payload,
                                fullPath,
                                id,
                                dateCreated,
                                lastModified);

        return record.Validate();
    }


    [Pure]
    public static async IAsyncEnumerable<FileRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
