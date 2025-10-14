// Jakar.Extensions :: Jakar.Database
// 4/2/2024  17:43


using TelemetrySpan = Jakar.Extensions.TelemetrySpan;



namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
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
                                 DateTimeOffset?      LastModified = null ) : TableRecord<FileRecord>(in ID, in DateCreated, in LastModified), ITableRecord<FileRecord>, IFileData<Guid>, IFileMetaData
{
    public const  string                     TABLE_NAME = "files";
    public static string                     TableName     { get => TABLE_NAME; }
    public static JsonSerializerContext      JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<FileRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.FileRecord;
    public static JsonTypeInfo<FileRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.FileRecordArray;


    public static ImmutableDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<FileRecord>.Create()
                                                                                                              .WithColumn<string?>(nameof(FileName),        ColumnOptions.Nullable, length: 256)
                                                                                                              .WithColumn<string?>(nameof(FileDescription), ColumnOptions.Nullable, length: 1024)
                                                                                                              .WithColumn<string?>(nameof(FileType),        ColumnOptions.Nullable, length: 256)
                                                                                                              .WithColumn<long>(nameof(FileSize))
                                                                                                              .WithColumn<string>(nameof(Hash),        length: MAX_FIXED)
                                                                                                              .WithColumn<MimeType?>(nameof(MimeType), ColumnOptions.Nullable)
                                                                                                              .WithColumn<string>(nameof(FullPath),    length: MAX_FIXED)
                                                                                                              .Build();


    public FileRecord( IFileData<Guid, FileMetaData>               data, LocalFile?    file                      = null ) : this(data, data.MetaData, file) { }
    private FileRecord( IFileData<Guid>                            data, IFileMetaData metaData, LocalFile? file = null ) : this(metaData.FileName, metaData.FileDescription, metaData.FileType, data.FileSize, data.Hash, metaData.MimeType, data.Payload, file?.FullPath, RecordID<FileRecord>.New(), DateTimeOffset.UtcNow) { }
    public static FileRecord Create( IFileData<Guid, FileMetaData> data, LocalFile?    file = null ) => new(data, file);
    public static FileRecord Create<TFileMetaData>( IFileData<Guid, TFileMetaData> data, LocalFile? file = null )
        where TFileMetaData : class, IFileMetaData<TFileMetaData> => new(data, data.MetaData, file);
    public TFileData ToFileData<TFileData, TFileMetaData>()
        where TFileData : class, IFileData<TFileData, Guid, TFileMetaData>
        where TFileMetaData : class, IFileMetaData<TFileMetaData> => TFileData.Create(this, TFileMetaData.Create(this));


    [Pure] public async ValueTask<OneOf<byte[], string, FileData>> Read( CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        if ( string.IsNullOrWhiteSpace(FullPath) ) { return new FileData(this, FileMetaData.Create(this)); }

        LocalFile file = FullPath;
        if ( MimeType != file.Mime ) { throw new InvalidOperationException($"{nameof(MimeType)} mismatch. Got {file.Mime} but expected {MimeType}"); }

        return file.Mime.IsText()
                   ? await file.ReadAsync()
                               .AsString(token)
                   : await file.ReadAsync()
                               .AsBytes(token);
    }


    [Pure] public async ValueTask<ErrorOrResult<FileData>> ToFileData( CancellationToken token = default )
    {
        OneOf<byte[], string, FileData> data = await Read(token);
        if ( data.IsT2 ) { return data.AsT2; }

        if ( data.IsT0 )
        {
            byte[] content = data.AsT0;
            string hash    = content.GetHash();
            if ( FileSize != content.Length ) { return Error.Conflict($"{nameof(FileSize)} mismatch. Got {content.Length} but expected {FileSize}"); }

            if ( !string.Equals(Hash, hash, StringComparison.Ordinal) ) { return Error.Conflict($"{nameof(Hash)} mismatch: {Hash} != {hash}"); }

            return new FileData(FileSize, Hash, Convert.ToBase64String(content), FileMetaData.Create(this));
        }
        else
        {
            string content = data.AsT1;
            string hash    = content.GetHash();
            if ( FileSize != content.Length ) { return Error.Conflict($"{nameof(FileSize)} mismatch. Got {content.Length} but expected {FileSize}"); }

            if ( !string.Equals(Hash, hash, StringComparison.Ordinal) ) { return Error.Conflict($"{nameof(Hash)} mismatch: {Hash} != {hash}"); }

            return new FileData(FileSize, Hash, content, FileMetaData.Create(this));
        }
    }


    [Pure] public async ValueTask<FileRecord> Update( LocalFile file, CancellationToken token = default )
    {
        if ( FullPath != file.FullPath ) { throw new InvalidOperationException($"{nameof(FullPath)} mismatch. Got {file.FullPath} but expected {FullPath}"); }

        ( long fileSize, string? hash, string payload, _, FileMetaData? metaData ) = await FileData.Create(file, token);

        return new FileRecord(metaData.FileName,
                              metaData.FileDescription,
                              metaData.FileType,
                              fileSize,
                              hash,
                              metaData.MimeType,
                              payload,
                              file.FullPath,
                              ID,
                              DateCreated,
                              DateTimeOffset.UtcNow);
    }


    [Pure] public override PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(FileName),        FileName);
        parameters.Add(nameof(FileDescription), FileDescription);
        parameters.Add(nameof(FileType),        FileType);
        parameters.Add(nameof(FileSize),        FileSize);
        parameters.Add(nameof(Hash),            Hash);
        parameters.Add(nameof(MimeType),        MimeType);
        parameters.Add(nameof(Payload),         Payload);
        parameters.Add(nameof(FullPath),        FullPath);
        parameters.Add(nameof(ID),              ID);
        parameters.Add(nameof(DateCreated),     DateCreated);
        parameters.Add(nameof(LastModified),    LastModified);
        return parameters;
    }


    [Pure] public static FileRecord Create( DbDataReader reader )
    {
        string?              name         = reader.GetFieldValue<string?>(nameof(FileName));
        string?              description  = reader.GetFieldValue<string?>(nameof(FileDescription));
        string?              fileType     = reader.GetFieldValue<string?>(nameof(FileType));
        long                 size         = reader.GetFieldValue<long>(nameof(FileSize));
        string               hash         = reader.GetFieldValue<string>(nameof(Hash));
        MimeType             mime         = reader.GetFieldValue<MimeType>(nameof(MimeType));
        string               payload      = reader.GetString(nameof(Payload));
        string?              fullPath     = reader.GetValue<string?>(nameof(FullPath));
        DateTimeOffset       dateCreated  = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?      lastModified = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<FileRecord> id           = RecordID<FileRecord>.ID(reader);

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


    public override int CompareTo( FileRecord? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int fileTypeComparison = string.Compare(FileType, other.FileType, StringComparison.Ordinal);
        if ( fileTypeComparison != 0 ) { return fileTypeComparison; }

        int fileNameComparison = string.Compare(FileName, other.FileName, StringComparison.Ordinal);
        if ( fileNameComparison != 0 ) { return fileNameComparison; }

        int fileDescriptionComparison = string.Compare(FileDescription, other.FileDescription, StringComparison.Ordinal);
        if ( fileDescriptionComparison != 0 ) { return fileDescriptionComparison; }

        int fileSizeComparison = FileSize.CompareTo(other.FileSize);
        if ( fileSizeComparison != 0 ) { return fileSizeComparison; }

        int hashComparison = string.Compare(Hash, other.Hash, StringComparison.Ordinal);
        if ( hashComparison != 0 ) { return hashComparison; }

        int mimeTypeComparison = Nullable.Compare(MimeType, other.MimeType);
        if ( mimeTypeComparison != 0 ) { return mimeTypeComparison; }

        int payloadComparison = string.Compare(Payload, other.Payload, StringComparison.Ordinal);
        if ( payloadComparison != 0 ) { return payloadComparison; }

        return string.Compare(FullPath, other.FullPath, StringComparison.Ordinal);
    }
    public override bool Equals( FileRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && Nullable.Equals(MimeType, other.MimeType) && string.Equals(FileType, other.FileType, StringComparison.InvariantCultureIgnoreCase) && string.Equals(Hash, other.Hash, StringComparison.Ordinal) && string.Equals(FullPath, other.FullPath, StringComparison.OrdinalIgnoreCase);
    }
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(AdditionalData);
        hashCode.Add(FileName);
        hashCode.Add(FileDescription);
        hashCode.Add(FileType);
        hashCode.Add(FileSize);
        hashCode.Add(Hash);
        hashCode.Add(MimeType);
        hashCode.Add(Payload);
        hashCode.Add(FullPath);
        return hashCode.ToHashCode();
    }


    public static bool operator >( FileRecord  left, FileRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( FileRecord left, FileRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( FileRecord  left, FileRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( FileRecord left, FileRecord right ) => left.CompareTo(right) <= 0;
    public static MigrationRecord CreateTable( ulong migrationID )
    {
        return MigrationRecord.Create<FileRecord>(5,
                                                  $"create {TABLE_NAME} table",
                                                  $"""
                                                   CREATE TABLE IF NOT EXISTS {TABLE_NAME}
                                                   (
                                                   {nameof(FileName).SqlColumnName()}        varchar({FILE_NAME})  NULL UNIQUE,
                                                   {nameof(FileDescription).SqlColumnName()} varchar({MAX_FIXED})  NULL,
                                                   {nameof(FileType).SqlColumnName()}        varchar(TYPE)         NULL,
                                                   {nameof(FullPath).SqlColumnName()}        varchar({MAX_FIXED})  NULL UNIQUE,
                                                   {nameof(FileSize).SqlColumnName()}        bigint                NOT NULL,
                                                   {nameof(Hash).SqlColumnName()}             varchar({MAX_FIXED}) NOT NULL,
                                                   {nameof(MimeType).SqlColumnName()}        varchar(TYPE)         NULL,
                                                   {nameof(Payload).SqlColumnName()}          text                 NOT NULL,
                                                   {nameof(ID).SqlColumnName()}               uuid                 NOT NULL PRIMARY KEY,
                                                   {nameof(DateCreated).SqlColumnName()}     timestamptz           NOT NULL DEFAULT SYSUTCDATETIME(),
                                                   {nameof(LastModified).SqlColumnName()}    timestamptz           NULL,
                                                   FOREIGN KEY({nameof(MimeType).SqlColumnName()}) REFERENCES {nameof(MimeType).SqlColumnName()}(id) ON DELETE SET NULL
                                                   );

                                                   CREATE TRIGGER {nameof(MigrationRecord.SetLastModified).SqlColumnName()}
                                                   BEFORE INSERT OR UPDATE ON {TABLE_NAME}
                                                   FOR EACH ROW
                                                   EXECUTE FUNCTION {nameof(MigrationRecord.SetLastModified).SqlColumnName()}();
                                                   """);
    }
}
