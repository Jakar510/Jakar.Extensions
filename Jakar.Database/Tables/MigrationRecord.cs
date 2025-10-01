// Jakar.Extensions :: Jakar.Database
// 09/30/2025  20:32

namespace Jakar.Database;


[Serializable]
public sealed record MigrationRecord( [StringLength(UNICODE_CAPACITY)] [property: StringLength(UNICODE_CAPACITY)] string Description, [property: Key] ulong MigrationID, DateTimeOffset DateCreated ) : BaseRecord<MigrationRecord>, ITableRecord<MigrationRecord>
{
    public const             string                          TABLE_NAME = "migrations";
    internal static readonly PropertyInfo[]                  Properties = typeof(MigrationRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    public static            ReadOnlyMemory<PropertyInfo>    ClassProperties => Properties;
    public static            JsonTypeInfo<MigrationRecord[]> JsonArrayInfo   => JakarDatabaseContext.Default.MigrationRecordArray;
    public static            JsonSerializerContext           JsonContext     => JakarDatabaseContext.Default;
    public static            JsonTypeInfo<MigrationRecord>   JsonTypeInfo    => JakarDatabaseContext.Default.MigrationRecord;
    public static            IEnumerable<MigrationRecord>    Migrations      { [Pure] get; } = [];
    public static            int                             PropertyCount   => Properties.Length;

    public static ImmutableDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<MigrationRecord>.Empty()
                                                                                                                   .WithColumn<ulong>(nameof(MigrationID))
                                                                                                                   .WithColumn<string>(nameof(Description), length: UNICODE_CAPACITY)
                                                                                                                   .WithColumn(ColumnMetaData.DateCreated)
                                                                                                                   .With_AdditionalData()
                                                                                                                   .Build();

    public static string                                   TableName => TABLE_NAME;
    RecordID<MigrationRecord> IRecordPair<MigrationRecord>.ID        => RecordID<MigrationRecord>.Create(MigrationID.AsGuid());
    public string                                          SQL       { get; init; } = string.Empty;


    public static MigrationRecord Create( DbDataReader reader )
    {
        string          description = reader.GetFieldValue<string>(nameof(Description));
        DateTimeOffset  dateCreated = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        ulong           id          = reader.GetFieldValue<ulong>(nameof(MigrationID));
        MigrationRecord record      = new(description, id, dateCreated);
        return record.Validate();
    }


    public DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
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
