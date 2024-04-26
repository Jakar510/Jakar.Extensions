// Jakar.Extensions :: Jakar.Database
// 4/2/2024  17:43

namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record FileRecord( string?              FileName,
                                 string?              FileDescription,
                                 string?              FileType,
                                 long                 FileSize,
                                 string               Hash,
                                 MimeType             MimeType,
                                 string               Payload,
                                 string?              FullPath,
                                 RecordID<FileRecord> ID,
                                 DateTimeOffset       DateCreated,
                                 DateTimeOffset?      LastModified = default ) : TableRecord<FileRecord>( ID, DateCreated, LastModified ), IDbReaderMapping<FileRecord>, IFileMetaData<Guid>, IFileData<Guid>
{
    public const  string                TABLE_NAME = "Files";
    public static string                TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }
    IFileMetaData<Guid> IFileData<Guid>.MetaData  => FileMetaData.Create( this );


    public FileRecord( IFileData<Guid>               data, LocalFile?           file                      = null ) : this( data, data.MetaData, file ) { }
    private FileRecord( IFileData<Guid>              data, IFileMetaData<Guid>? metaData, LocalFile? file = null ) : this( metaData?.FileName, metaData?.FileDescription, metaData?.FileType, data.FileSize, data.Hash, data.MimeType, data.Payload, file?.FullPath, RecordID<FileRecord>.New(), DateTimeOffset.UtcNow ) { }
    public static FileRecord Create( IFileData<Guid> data, LocalFile?           file = null ) => new(data, file);
    public static FileRecord Create<TFileMetaData>( IFileData<Guid, TFileMetaData> data, LocalFile? file = null )
        where TFileMetaData : IFileMetaData<TFileMetaData, Guid> => new(data, data.MetaData, file);


    [Pure]
    public async ValueTask<OneOf<byte[], string, FileData>> Read( CancellationToken token = default )
    {
        if ( string.IsNullOrWhiteSpace( FullPath ) ) { return new FileData( this, FileMetaData.Create( this ) ); }

        LocalFile file = FullPath;
        if ( MimeType != file.Mime ) { throw new InvalidOperationException( $"{nameof(MimeType)} mismatch. Got {file.Mime} but expected {MimeType}" ); }

        return file.Mime.IsText()
                   ? await file.ReadAsync().AsString( token )
                   : await file.ReadAsync().AsBytes( token );
    }


    [Pure]
    public async ValueTask<ErrorOrResult<FileData>> ToFileData( CancellationToken token = default )
    {
        OneOf<byte[], string, FileData> data = await Read( token );
        if ( data.IsT2 ) { return data.AsT2; }

        if ( data.IsT0 )
        {
            byte[] content = data.AsT0;
            string hash    = IFileData<Guid>.GetHash( content );
            if ( FileSize != content.Length ) { return Error.Conflict( $"{nameof(FileSize)} mismatch. Got {content.Length} but expected {FileSize}" ); }

            if ( string.Equals( Hash, hash, StringComparison.Ordinal ) is false ) { return Error.Conflict( $"{nameof(Hash)} mismatch: {Hash} != {hash}" ); }

            return new FileData( MimeType, FileSize, Hash, Convert.ToBase64String( content ), FileMetaData.Create( this ) );
        }
        else
        {
            string content = data.AsT1;
            string hash    = IFileData<Guid>.GetHash( content );
            if ( FileSize != content.Length ) { return Error.Conflict( $"{nameof(FileSize)} mismatch. Got {content.Length} but expected {FileSize}" ); }

            if ( string.Equals( Hash, hash, StringComparison.Ordinal ) is false ) { return Error.Conflict( $"{nameof(Hash)} mismatch: {Hash} != {hash}" ); }

            return new FileData( MimeType, FileSize, Hash, content, FileMetaData.Create( this ) );
        }
    }


    [Pure]
    public async ValueTask<FileRecord> Update( LocalFile file, CancellationToken token = default )
    {
        FileData      data     = await FileData.Create( file, token );
        FileMetaData? metaData = data.MetaData;

        return new FileRecord( metaData?.FileName,
                               metaData?.FileDescription,
                               metaData?.FileType,
                               data.FileSize,
                               data.Hash,
                               data.MimeType,
                               string.Empty,
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

        var record = new FileRecord( name,
                                     description,
                                     fileType,
                                     size,
                                     hash,
                                     mime,
                                     payload,
                                     fullPath,
                                     id,
                                     dateCreated,
                                     lastModified );

        record.Validate();
        return record;
    }


    [Pure]
    public static async IAsyncEnumerable<FileRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
