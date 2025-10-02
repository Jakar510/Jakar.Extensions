// Jakar.Extensions :: Jakar.Database
// 09/30/2025  20:32

namespace Jakar.Database;


[Serializable]
public sealed record MigrationRecord : BaseRecord<MigrationRecord>, ITableRecord<MigrationRecord>
{
    public const             string                          TABLE_NAME = "migrations";
    internal static readonly PropertyInfo[]                  Properties = typeof(MigrationRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    public static            ReadOnlyMemory<PropertyInfo>    ClassProperties => Properties;
    public static            JsonTypeInfo<MigrationRecord[]> JsonArrayInfo   => JakarDatabaseContext.Default.MigrationRecordArray;
    public static            JsonSerializerContext           JsonContext     => JakarDatabaseContext.Default;
    public static            JsonTypeInfo<MigrationRecord>   JsonTypeInfo    => JakarDatabaseContext.Default.MigrationRecord;
    public static            List<MigrationRecord>           Migrations      { [Pure] get; } = [..Jakar.Database.Migrations.BuiltIns()];


    public static int PropertyCount => Properties.Length;

    public static ImmutableDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<MigrationRecord>.Empty()
                                                                                                                   .WithColumn<ulong>(nameof(MigrationID))
                                                                                                                   .WithColumn<string>(nameof(TableID),     length: 256)
                                                                                                                   .WithColumn<string>(nameof(Description), length: UNICODE_CAPACITY)
                                                                                                                   .WithColumn(ColumnMetaData.DateCreated)
                                                                                                                   .With_AdditionalData()
                                                                                                                   .Build();

    public static string                                    TableName   => TABLE_NAME;
    public        DateTimeOffset                            AppliedOn   { get; init; } = DateTimeOffset.UtcNow;
    DateTimeOffset IDateCreated.                            DateCreated => AppliedOn;
    [StringLength(UNICODE_CAPACITY)] public required string Description { get; init; }
    RecordID<MigrationRecord> IRecordPair<MigrationRecord>. ID          => RecordID<MigrationRecord>.Create(MigrationID.AsGuid());
    [Key] public required      ulong                        MigrationID { get; init; }
    internal                   string                       SQL         { get; init; } = string.Empty;
    [StringLength(256)] public string?                      TableID     { get; init; }


    [SetsRequiredMembers] public MigrationRecord( ulong migrationID, string description, string? tableID = null ) : base()
    {
        this.Description = description;
        this.MigrationID = migrationID;
        this.TableID     = tableID;
    }


    public static MigrationRecord CreateTable( ulong migrationID )
    {
        string tableID = TABLE_NAME.SqlColumnName();

        return Create<MigrationRecord>(migrationID,
                                       $"create {tableID} table",
                                       $"""
                                        CREATE TABLE IF NOT EXISTS {tableID}
                                        (
                                        {nameof(MigrationID).SqlColumnName()}    bigint        NOT NULL,
                                        {nameof(TableID).SqlColumnName()}        varchar(256)  NOT NULL,
                                        {nameof(Description).SqlColumnName()}    varchar(4096) NOT NULL,
                                        {nameof(AppliedOn).SqlColumnName()}      timestamptz   NOT NULL DEFAULT SYSUTCDATETIME(),
                                        {nameof(AdditionalData).SqlColumnName()} json          NULL
                                        PRIMARY KEY({nameof(TableID).SqlColumnName()}, {nameof(MigrationID).SqlColumnName()})
                                        );
                                        
                                        CREATE TRIGGER {nameof(MigrationRecord.SetLastModified).SqlColumnName()}
                                        BEFORE INSERT OR UPDATE ON {tableID}
                                        FOR EACH ROW
                                        EXECUTE FUNCTION {nameof(MigrationRecord.SetLastModified).SqlColumnName()}();
                                        """);
    }
    public static MigrationRecord SetLastModified( ulong migrationID )
    {
        string tableID = nameof(SetLastModified)
           .SqlColumnName();

        return new MigrationRecord(migrationID, $"create {tableID} function")
               {
                   SQL = $"""
                          CREATE OR REPLACE FUNCTION {tableID}()
                          RETURNS TRIGGER AS $$
                          BEGIN
                              NEW.{nameof(ILastModified.LastModified).SqlColumnName()} = now();
                              RETURN NEW;
                          END;
                          $$ LANGUAGE plpgsql;
                          """
               };
    }
    public static MigrationRecord FromEnum<TEnum>( ulong migrationID )
        where TEnum : struct, Enum
    {
        string tableID = typeof(TEnum).Name.SqlColumnName();

        MigrationRecord record = new(migrationID, $"create {tableID} table")
                                 {
                                     TableID = tableID,
                                     SQL = $"""
                                            CREATE TABLE IF NOT EXISTS {tableID}
                                            (
                                            id    bigint        PRIMARY KEY,
                                            name  varchar(256)  UNIQUE NOT NULL,
                                            );
                                            
                                            CREATE TRIGGER {nameof(MigrationRecord.SetLastModified).SqlColumnName()}
                                            BEFORE INSERT OR UPDATE ON {tableID}
                                            FOR EACH ROW
                                            EXECUTE FUNCTION {nameof(MigrationRecord.SetLastModified).SqlColumnName()}();

                                            -- Insert values if they do not exist with explicit ids (enum order)
                                            INSERT INTO {tableID} (id, name)
                                            SELECT v.id, v.name
                                            FROM (VALUES
                                            {string.Join(",\n", Enum.GetValues<TEnum>().Select(( v, i ) => $"    ({i}, '{v}')"))}
                                            ) AS v(id, name)
                                            WHERE NOT EXISTS (
                                            SELECT 1 FROM mime_types m WHERE m.id = v.id OR m.name = v.name
                                            )
                                            );
                                            """
                                 };

        return record.Validate();
    }
    public static MigrationRecord Create<TClass>( ulong migrationID, string description, string sql )
        where TClass : ITableRecord<TClass>
    {
        MigrationRecord record = new MigrationRecord(migrationID, description)
                                 {
                                     TableID = TClass.TableName,
                                     SQL     = sql
                                 };

        return record.Validate();
    }
    public static MigrationRecord Create( DbDataReader reader )
    {
        string         description = reader.GetFieldValue<string>(nameof(Description));
        string?        tableID     = reader.GetFieldValue<string?>(nameof(TableID));
        DateTimeOffset appliedOn   = reader.GetFieldValue<DateTimeOffset>(nameof(AppliedOn));
        ulong          id          = reader.GetFieldValue<ulong>(nameof(MigrationID));

        MigrationRecord record = new(id, description)
                                 {
                                     TableID   = tableID,
                                     AppliedOn = appliedOn

                                     // SQL = sql
                                 };

        return record.Validate();
    }


    public PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(MigrationID),    MigrationID);
        parameters.Add(nameof(AppliedOn),      AppliedOn);
        parameters.Add(nameof(Description),    Description);
        parameters.Add(nameof(AdditionalData), AdditionalData);
        return parameters;
    }
    public RecordPair<MigrationRecord> ToPair()                              => new(( (IRecordPair<MigrationRecord>)this ).ID, AppliedOn);
    public MigrationRecord             NewID( RecordID<MigrationRecord> id ) => throw new NotImplementedException();
    public UInt128 GetHash()
    {
        ReadOnlySpan<char> span = ToString();
        return span.Hash128();
    }


    public override bool Equals( MigrationRecord?    other ) => ReferenceEquals(this, other) || string.Equals(Description, other?.Description);
    public override int  CompareTo( MigrationRecord? other ) => Nullable.Compare(AppliedOn, other?.AppliedOn);
    public override int  GetHashCode()                       => HashCode.Combine(Description, MigrationID, AppliedOn, AdditionalData);


    public static bool operator >( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) > 0;
    public static bool operator >=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) >= 0;
    public static bool operator <( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) < 0;
    public static bool operator <=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) <= 0;
}
