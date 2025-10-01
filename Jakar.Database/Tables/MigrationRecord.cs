// Jakar.Extensions :: Jakar.Database
// 09/30/2025  20:32

using Org.BouncyCastle.Tls;



namespace Jakar.Database.DbMigrations;


[Serializable]
public sealed record MigrationRecord( string Description, RecordID<MigrationRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified, JsonObject? AdditionalData = null ) : TableRecord<MigrationRecord>(in ID, in DateCreated, in LastModified, AdditionalData), ITableRecord<MigrationRecord>
{
    public const  string                          TABLE_NAME = "migrations";
    public static JsonSerializerContext           JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<MigrationRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.MigrationRecord;
    public static JsonTypeInfo<MigrationRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.MigrationRecordArray;
    public static string                          TableName     => TABLE_NAME;

    public static ImmutableDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<MigrationRecord>.Create()
                                                                                                                   .WithColumn<string?>(nameof(Description), length: UNICODE_CAPACITY)
                                                                                                                   .Build();


    public static MigrationRecord Create( DbDataReader reader )
    {
        string                    description  = reader.GetFieldValue<string>(nameof(Description));
        DateTimeOffset            dateCreated  = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?           lastModified = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<MigrationRecord> id           = RecordID<MigrationRecord>.ID(reader);
        MigrationRecord           record       = new(description, id, dateCreated, lastModified);
        return record.Validate();
    }


    public override int CompareTo( MigrationRecord?   other )                       => Nullable.Compare(DateCreated, other?.DateCreated);
    public static   bool operator >( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) > 0;
    public static   bool operator >=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) >= 0;
    public static   bool operator <( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) < 0;
    public static   bool operator <=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) <= 0;
}
