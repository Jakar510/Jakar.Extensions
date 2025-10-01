// Jakar.Extensions :: Jakar.Database
// 09/30/2025  20:32

namespace Jakar.Database;


[Serializable]
public sealed record MigrationRecord( string Description, long ID, DateTimeOffset DateCreated ) : BaseRecord<MigrationRecord>, ITableRecord<MigrationRecord>
{
    internal static readonly PropertyInfo[]                  Properties = typeof(MigrationRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    public const             string                          TABLE_NAME = "migrations";
    public static            JsonSerializerContext           JsonContext     => JakarDatabaseContext.Default;
    public static            JsonTypeInfo<MigrationRecord>   JsonTypeInfo    => JakarDatabaseContext.Default.MigrationRecord;
    public static            JsonTypeInfo<MigrationRecord[]> JsonArrayInfo   => JakarDatabaseContext.Default.MigrationRecordArray;
    public static            string                          TableName       => TABLE_NAME;
    public static            int                             PropertyCount   => Properties.Length;
    public static            IEnumerable<MigrationRecord>    Migrations      { [Pure] get; }
    public static            ReadOnlyMemory<PropertyInfo>    ClassProperties => Properties;
    RecordID<MigrationRecord> IRecordPair<MigrationRecord>.  ID              => RecordID<MigrationRecord>.Create(ID.AsGuid());


    public static ImmutableDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<MigrationRecord>.Create()
                                                                                                                   .WithColumn<string?>(nameof(Description), length: UNICODE_CAPACITY)
                                                                                                                   .With_AdditionalData()
                                                                                                                   .Build();


    public static MigrationRecord Create( DbDataReader reader )
    {
        string          description = reader.GetFieldValue<string>(nameof(Description));
        DateTimeOffset  dateCreated = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        long            id          = reader.GetFieldValue<long>(nameof(ID));
        MigrationRecord record      = new(description, id, dateCreated);
        return record.Validate();
    }


    public DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(ID),             ID);
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
    public override int  GetHashCode()                       => HashCode.Combine(Description, ID, DateCreated, AdditionalData);


    public static bool operator >( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) > 0;
    public static bool operator >=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) >= 0;
    public static bool operator <( MigrationRecord  left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) < 0;
    public static bool operator <=( MigrationRecord left, MigrationRecord right ) => Comparer<MigrationRecord>.Default.Compare(left, right) <= 0;
}
