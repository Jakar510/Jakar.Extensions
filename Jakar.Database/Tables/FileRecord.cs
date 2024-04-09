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
                                 RecordID<FileRecord> ID,
                                 DateTimeOffset       DateCreated,
                                 DateTimeOffset?      LastModified = default ) : TableRecord<FileRecord>( ID, DateCreated, LastModified ), IDbReaderMapping<FileRecord>, IFileMetaData<Guid>, IFileData<Guid>
{
    public const  string                TABLE_NAME = "Files";
    public static string                TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }
    IFileMetaData<Guid> IFileData<Guid>.MetaData  => FileMetaData.Create( this );


    public FileRecord( IFileData<Guid>               file ) : this( file, file.MetaData ) { }
    private FileRecord( IFileData<Guid>              file, IFileMetaData<Guid>? data ) : this( data?.FileName, data?.FileDescription, data?.FileType, file.FileSize, file.Hash, file.MimeType, file.Payload, RecordID<FileRecord>.New(), DateTimeOffset.UtcNow ) { }
    public static FileRecord Create( IFileData<Guid> file ) => new(file);
    public static FileRecord Create<TFileMetaData>( IFileData<Guid, TFileMetaData> file )
        where TFileMetaData : IFileMetaData<TFileMetaData, Guid> => new(file.MetaData?.FileName, file.MetaData?.FileDescription, file.MetaData?.FileType, file.FileSize, file.Hash, file.MimeType, file.Payload, RecordID<FileRecord>.New(), DateTimeOffset.UtcNow);


    public FileData ToFileData() => new(this, FileMetaData.Create( this ));


    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add( nameof(FileName),        FileName );
        parameters.Add( nameof(FileDescription), FileDescription );
        parameters.Add( nameof(FileType),        FileType );
        parameters.Add( nameof(FileSize),        FileSize );
        parameters.Add( nameof(Hash),            Hash );
        parameters.Add( nameof(MimeType),        MimeType );
        parameters.Add( nameof(Payload),         Payload );
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
        string?              type         = reader.GetFieldValue<string?>( nameof(FileType) );
        long                 size         = reader.GetFieldValue<long>( nameof(FileSize) );
        string               hash         = reader.GetFieldValue<string>( nameof(Hash) );
        MimeType             mime         = reader.GetFieldValue<MimeType>( nameof(MimeType) );
        string               payload      = reader.GetFieldValue<string>( nameof(Payload) );
        DateTimeOffset       dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?      lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<FileRecord> id           = RecordID<FileRecord>.ID( reader );
        FileRecord           record       = new FileRecord( name, description, type, size, hash, mime, payload, id, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [Pure]
    public static async IAsyncEnumerable<FileRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
