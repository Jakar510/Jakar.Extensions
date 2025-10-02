// Jakar.Extensions :: Jakar.Database
// 09/30/2025  20:32

namespace Jakar.Database;


[Serializable]
public sealed record MigrationRecord( [StringLength(UNICODE_CAPACITY)] [property: StringLength(UNICODE_CAPACITY)] string Description, [StringLength(256)] [property: StringLength(256)] string TableID, [property: Key] ulong MigrationID, DateTimeOffset DateCreated ) : BaseRecord<MigrationRecord>, ITableRecord<MigrationRecord>
{
    public const             string                          TABLE_NAME = "migrations";
    internal static readonly PropertyInfo[]                  Properties = typeof(MigrationRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    public static            ReadOnlyMemory<PropertyInfo>    ClassProperties => Properties;
    public static            JsonTypeInfo<MigrationRecord[]> JsonArrayInfo   => JakarDatabaseContext.Default.MigrationRecordArray;
    public static            JsonSerializerContext           JsonContext     => JakarDatabaseContext.Default;
    public static            JsonTypeInfo<MigrationRecord>   JsonTypeInfo    => JakarDatabaseContext.Default.MigrationRecord;

    public static List<MigrationRecord> Migrations { [Pure] get; } = [..Jakar.Database.Migrations.BuiltIns()];


    public static int PropertyCount => Properties.Length;

    public static ImmutableDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<MigrationRecord>.Empty()
                                                                                                                   .WithColumn<ulong>(nameof(MigrationID))
                                                                                                                   .WithColumn<string>(nameof(TableID),     length: 256)
                                                                                                                   .WithColumn<string>(nameof(Description), length: UNICODE_CAPACITY)
                                                                                                                   .WithColumn(ColumnMetaData.DateCreated)
                                                                                                                   .With_AdditionalData()
                                                                                                                   .Build();

    public static string                                   TableName => TABLE_NAME;
    RecordID<MigrationRecord> IRecordPair<MigrationRecord>.ID        => RecordID<MigrationRecord>.Create(MigrationID.AsGuid());
    public string                                          SQL       { get; init; } = string.Empty;


    public static MigrationRecord FromEnum<TEnum>( ulong migrationID, DateTimeOffset dateCreated )
        where TEnum : struct, Enum
    {
        string tableID = typeof(TEnum).Name.SqlColumnName();

        MigrationRecord record = new($"create {tableID} table", tableID, migrationID, dateCreated)
                                 {
                                     SQL = $"""
                                            CREATE TABLE IF NOT EXISTS {tableID}
                                            (
                                            id    bigint        PRIMARY KEY,
                                            name  varchar(256)  UNIQUE NOT NULL,
                                            );

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
    public static MigrationRecord Create<TClass>( ulong migrationID, DateTimeOffset dateCreated, [StringLength(UNICODE_CAPACITY)] string description, [StringLength(UNICODE_CAPACITY)] string sql )
        where TClass : ITableRecord<TClass>
    {
        MigrationRecord record = new MigrationRecord(description, TClass.TableName, migrationID, dateCreated) { SQL = sql };
        return record.Validate();
    }
    public static MigrationRecord Create( DbDataReader reader )
    {
        string          description = reader.GetFieldValue<string>(nameof(Description));
        string          tableID     = reader.GetFieldValue<string>(nameof(TableID));
        DateTimeOffset  dateCreated = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        ulong           id          = reader.GetFieldValue<ulong>(nameof(MigrationID));
        MigrationRecord record      = new(description, tableID, id, dateCreated);
        return record.Validate();
    }


    public PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(MigrationID),    MigrationID);
        parameters.Add(nameof(DateCreated),    DateCreated);
        parameters.Add(nameof(Description),    Description);
        parameters.Add(nameof(AdditionalData), AdditionalData);
        return parameters;
    }
    public RecordPair<MigrationRecord> ToPair()                              => new(( (IRecordPair<MigrationRecord>)this ).ID, DateCreated);
    public MigrationRecord             NewID( RecordID<MigrationRecord> id ) => throw new NotImplementedException();
    public UInt128 GetHash()
    {
        ReadOnlySpan<char> span = ToString();
        return span.Hash128();
    }


    public override bool Equals( MigrationRecord?    other ) => ReferenceEquals(this, other) || string.Equals(Description, other?.Description);
    public override int  CompareTo( MigrationRecord? other ) => Nullable.Compare(DateCreated, other?.DateCreated);
    public override int  GetHashCode()                       => HashCode.Combine(Description, MigrationID, DateCreated, AdditionalData);


    public static bool operator >( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) > 0;
    public static bool operator >=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) >= 0;
    public static bool operator <( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) < 0;
    public static bool operator <=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) <= 0;
}
