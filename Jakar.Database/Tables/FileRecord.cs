// Jakar.Extensions :: Jakar.Database
// 4/2/2024  17:43

namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record FileRecord( RecordID<FileRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = default ) : TableRecord<FileRecord>( ID, DateCreated, LastModified ), IDbReaderMapping<FileRecord>
{
    public const  string                       TABLE_NAME = "Files";
    public static string                       TableName                                                             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }
    public static FileRecord                   Create( DbDataReader      reader )                                    => null;
    public static IAsyncEnumerable<FileRecord> CreateAsync( DbDataReader reader, CancellationToken token = default ) => null;
}
