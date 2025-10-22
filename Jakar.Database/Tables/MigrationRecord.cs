// Jakar.Extensions :: Jakar.Database
// 09/30/2025  20:32

using ZLinq.Linq;



namespace Jakar.Database;


[Serializable]
public sealed record MigrationRecord : BaseRecord<MigrationRecord>, ITableRecord<MigrationRecord>
{
    public const             string         TABLE_NAME = "migrations";
    internal static readonly PropertyInfo[] Properties = typeof(MigrationRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    public static readonly   string         SelectSql  = $"SELECT * FROM {MigrationRecord.TABLE_NAME} ORDER BY {nameof(MigrationRecord.MigrationID).SqlColumnName()}";
    public static readonly string ApplySql = $"""
                                              INSERT INTO {TABLE_NAME} 
                                              (
                                              {nameof(MigrationID).SqlColumnName()},
                                              {nameof(TableID).SqlColumnName()},
                                              {nameof(Description).SqlColumnName()},
                                              {nameof(AppliedOn).SqlColumnName()},
                                              {nameof(AdditionalData).SqlColumnName()}
                                              ) 
                                              VALUES 
                                              (
                                              @{nameof(MigrationID).SqlColumnName()},
                                              @{nameof(TableID).SqlColumnName()},
                                              @{nameof(Description).SqlColumnName()},
                                              @{nameof(AppliedOn).SqlColumnName()},
                                              @{nameof(AdditionalData).SqlColumnName()}
                                              )
                                              """;


    public static ReadOnlyMemory<PropertyInfo>    ClassProperties => Properties;
    public static JsonTypeInfo<MigrationRecord[]> JsonArrayInfo   => JakarDatabaseContext.Default.MigrationRecordArray;
    public static JsonSerializerContext           JsonContext     => JakarDatabaseContext.Default;
    public static JsonTypeInfo<MigrationRecord>   JsonTypeInfo    => JakarDatabaseContext.Default.MigrationRecord;


    public static int PropertyCount => Properties.Length;

    public static FrozenDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<MigrationRecord>.Empty.WithColumn<ulong>(nameof(MigrationID))
                                                                                                                .WithColumn<string>(nameof(TableID),     length: 256)
                                                                                                                .WithColumn<string>(nameof(Description), length: MAX_FIXED)
                                                                                                                .WithColumn(ColumnMetaData.DateCreated)
                                                                                                                .With_AdditionalData()
                                                                                                                .Build();

    public static string                                   TableName   => TABLE_NAME;
    public        DateTimeOffset                           AppliedOn   { get; init; } = DateTimeOffset.UtcNow;
    DateTimeOffset IDateCreated.                           DateCreated => AppliedOn;
    [StringLength(MAX_FIXED)] public required string       Description { get; init; }
    RecordID<MigrationRecord> IRecordPair<MigrationRecord>.ID          => RecordID<MigrationRecord>.Create(MigrationID.AsGuid());
    [Key] public required      ulong                       MigrationID { get; init; }
    internal                   string                      SQL         { get; init; } = EMPTY;
    [StringLength(256)] public string?                     TableID     { get; init; }


    [SetsRequiredMembers] public MigrationRecord( ulong migrationID, string description, string? TABLE_NAME = null ) : base()
    {
        this.Description = description;
        this.MigrationID = migrationID;
        this.TableID     = TABLE_NAME;
    }


    public static MigrationRecord CreateTable( ulong migrationID )
    {
        return Create<MigrationRecord>(migrationID,
                                       $"create {TABLE_NAME} table",
                                       $"""
                                        CREATE TABLE IF NOT EXISTS {TABLE_NAME}
                                        (
                                        {nameof(MigrationID).SqlColumnName()}    bigint        PRIMARY KEY,
                                        {nameof(TableID).SqlColumnName()}        varchar(256)  NOT NULL,
                                        {nameof(Description).SqlColumnName()}    varchar(4096) UNIQUE NOT NULL,
                                        {nameof(AppliedOn).SqlColumnName()}      timestamptz   NOT NULL DEFAULT SYSUTCDATETIME(),
                                        {nameof(AdditionalData).SqlColumnName()} json          NULL
                                        );

                                        CREATE TRIGGER {nameof(SetLastModified).SqlColumnName()}
                                        BEFORE INSERT OR UPDATE ON {TABLE_NAME}
                                        FOR EACH ROW
                                        EXECUTE FUNCTION {nameof(SetLastModified).SqlColumnName()}();
                                        """);
    }
    public static MigrationRecord SetLastModified( ulong migrationID )
    {
        string name = nameof(SetLastModified)
           .SqlColumnName();

        return new MigrationRecord(migrationID, $"create {name} function")
               {
                   SQL = $"""
                          CREATE OR REPLACE FUNCTION {name}()
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
        where TEnum : unmanaged, Enum
    {
        string[]                                   values     = Enum.GetNames(typeof(TEnum));
        string                                     tableName  = typeof(TEnum).Name.SqlColumnName();
        ValueEnumerable<FromArray<string>, string> enumerable = values.AsValueEnumerable();
        int                                        length     = enumerable.Max(static x => x.Length);

        MigrationRecord record = new(migrationID, $"create {tableName} table")
                                 {
                                     TableID = tableName,
                                     SQL = $"""
                                            CREATE TABLE IF NOT EXISTS {tableName}
                                            (
                                            id   bigint            PRIMARY KEY,
                                            name varchar({length}) UNIQUE NOT NULL,
                                            );


                                            CREATE TRIGGER {nameof(MigrationRecord.SetLastModified).SqlColumnName()}
                                            BEFORE INSERT OR UPDATE ON {tableName}
                                            FOR EACH ROW
                                            EXECUTE FUNCTION {nameof(MigrationRecord.SetLastModified).SqlColumnName()}();


                                            -- Insert values if they do not exist with explicit ids (enum order)
                                            INSERT INTO {tableName} (id, name)
                                            SELECT v.id, v.name
                                            FROM (VALUES
                                                {string.Join(",\n", values.Select(( v, i ) => $"    ({i}, '{v}')"))}
                                            ) AS v(id, name)
                                            WHERE NOT EXISTS (
                                            SELECT 1 FROM mime_types m WHERE m.id = v.id OR m.name = v.name
                                            )
                                            );
                                            """
                                 };

        return record.Validate();
    }
    public static MigrationRecord Create<TSelf>( ulong migrationID, string description, string sql )
        where TSelf : ITableRecord<TSelf>
    {
        MigrationRecord record = new MigrationRecord(migrationID, description)
                                 {
                                     TableID = TSelf.TableName,
                                     SQL     = sql
                                 };

        return record.Validate();
    }
    public static MigrationRecord Create( DbDataReader reader )
    {
        string         description = reader.GetFieldValue<string>(nameof(Description));
        string?        tableName   = reader.GetFieldValue<string?>(nameof(TableID));
        DateTimeOffset appliedOn   = reader.GetFieldValue<DateTimeOffset>(nameof(AppliedOn));
        ulong          id          = reader.GetFieldValue<ulong>(nameof(MigrationID));

        MigrationRecord record = new(id, description)
                                 {
                                     TableID   = tableName,
                                     AppliedOn = appliedOn

                                     // SQL = sql
                                 };

        return record.Validate();
    }


    public PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = PostgresParameters.Create<MigrationRecord>();
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
    public override int  GetHashCode()                       => HashCode.Combine(MigrationID, Description);


    public static bool operator >( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) > 0;
    public static bool operator >=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) >= 0;
    public static bool operator <( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) < 0;
    public static bool operator <=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) <= 0;
}
