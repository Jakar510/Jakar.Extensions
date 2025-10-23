using Newtonsoft.Json.Linq;
using ValueOf;



namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed record GroupRecord( [property: StringLength(GroupRecord.MAX_SIZE)] string NameOfGroup, string? NormalizedName, UserRights Rights, RecordID<GroupRecord> ID, RecordID<UserRecord>? CreatedBy, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<GroupRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), ITableRecord<GroupRecord>, IGroupModel<Guid>
{
    public const  int                         MAX_SIZE   = 1024;
    public const  string                      TABLE_NAME = "groups";
    public static JsonTypeInfo<GroupRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.GroupRecordArray;
    public static JsonSerializerContext       JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<GroupRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.GroupRecord;


    public static FrozenDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<GroupRecord>.Default.WithColumn<string?>(nameof(NormalizedName), length: MAX_SIZE)
                                                                                                            .WithColumn<string>(nameof(NameOfGroup), length: MAX_SIZE, checks: $"{nameof(NameOfGroup)} > 0")
                                                                                                            .WithColumn<string>(nameof(Rights),      checks: $"{nameof(Rights)} > 0")
                                                                                                            .With_CreatedBy()
                                                                                                            .Build();


    public static string                                   TableName      { get => TABLE_NAME; }
    Guid? ICreatedByUser<Guid>.                            CreatedBy      => CreatedBy?.Value;
    Guid? IGroupModel<Guid>.                               OwnerID        => CreatedBy?.Value;
    [StringLength(RIGHTS)]               public UserRights Rights         { get; set; } = Rights;
    [StringLength(GroupRecord.MAX_SIZE)] public string?    NormalizedName { get; set; } = NormalizedName;


    public GroupRecord( string nameOfGroup, UserRights rights, RecordID<UserRecord>? owner = null, string? normalizedName = null ) : this(nameOfGroup, normalizedName, EMPTY, RecordID<GroupRecord>.New(), owner, DateTimeOffset.UtcNow) { }


    [Pure] public static GroupRecord Create<TEnum>( string name, [HandlesResourceDisposal] scoped Permissions<TEnum> rights, string? normalizedName = null, RecordID<UserRecord>? caller = null )
        where TEnum : unmanaged, Enum => new(name, normalizedName, rights.ToStringAndDispose(), RecordID<GroupRecord>.New(), caller, DateTimeOffset.UtcNow);


    public GroupModel ToGroupModel() => new(this);
    public TGroupModel ToGroupModel<TGroupModel>()
        where TGroupModel : class, IGroupModel<TGroupModel, Guid> => TGroupModel.Create(this);


    public static MigrationRecord CreateTable( ulong migrationID )
    {
        return MigrationRecord.Create<RoleRecord>(migrationID,
                                                  $"create {TABLE_NAME} table",
                                                  $"""
                                                   CREATE TABLE IF NOT EXISTS {TABLE_NAME}
                                                   (  
                                                   {nameof(NameOfGroup).SqlColumnName()}    varchar(1024) NOT NULL, 
                                                   {nameof(NormalizedName).SqlColumnName()}     varchar(1024) NOT NULL,  
                                                   {nameof(Rights).SqlColumnName()}         varchar(1024) NOT NULL, 
                                                   {nameof(ID).SqlColumnName()}             uuid          PRIMARY KEY,
                                                   {nameof(DateCreated).SqlColumnName()}    timestamptz   NOT NULL,
                                                   {nameof(LastModified).SqlColumnName()}   timestamptz   NULL,
                                                   {nameof(AdditionalData).SqlColumnName()} json          NULL
                                                   );

                                                   CREATE TRIGGER {nameof(MigrationRecord.SetLastModified).SqlColumnName()}
                                                   BEFORE INSERT OR UPDATE ON {TABLE_NAME}
                                                   FOR EACH ROW
                                                   EXECUTE FUNCTION {nameof(MigrationRecord.SetLastModified).SqlColumnName()}();
                                                   """);
    }


    public override int CompareTo( GroupRecord? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int nameOfGroupComparison = string.Compare(NameOfGroup, other.NameOfGroup, StringComparison.Ordinal);
        if ( nameOfGroupComparison != 0 ) { return nameOfGroupComparison; }

        int normalizedNameComparison = string.Compare(NormalizedName, other.NormalizedName, StringComparison.Ordinal);
        if ( normalizedNameComparison != 0 ) { return normalizedNameComparison; }

        int lastModifiedComparison = Nullable.Compare(LastModified, other.LastModified);
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        return DateCreated.CompareTo(other.DateCreated);
    }
    public override bool Equals( GroupRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && NameOfGroup == other.NameOfGroup && Rights == other.Rights && NormalizedName == other.NormalizedName;
    }
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), NameOfGroup, Rights, NormalizedName);


    [Pure] public override PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(NormalizedName), NormalizedName);
        parameters.Add(nameof(NameOfGroup),    NameOfGroup);
        parameters.Add(nameof(CreatedBy),      CreatedBy);
        parameters.Add(nameof(Rights),         Rights);
        return parameters;
    }


    [Pure] public static GroupRecord Create( DbDataReader reader )
    {
        string                normalizedName = reader.GetFieldValue<string>(nameof(NormalizedName));
        string                nameOfGroup    = reader.GetFieldValue<string>(nameof(NameOfGroup));
        string                rights         = reader.GetFieldValue<string>(nameof(Rights));
        DateTimeOffset        dateCreated    = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?       lastModified   = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRecord>? ownerUserID    = RecordID<UserRecord>.CreatedBy(reader);
        RecordID<GroupRecord> id             = RecordID<GroupRecord>.ID(reader);
        GroupRecord           record         = new(nameOfGroup, normalizedName, rights, id, ownerUserID, dateCreated, lastModified);
        return record.Validate();
    }


    [Pure] public async ValueTask<ErrorOrResult<UserRecord>> GetOwner( NpgsqlConnection connection, NpgsqlTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get(connection, transaction, CreatedBy, token);
    [Pure] public       IAsyncEnumerable<UserRecord>         GetUsers( NpgsqlConnection connection, NpgsqlTransaction? transaction, Database db, CancellationToken token ) => UserGroupRecord.Where(connection, transaction, db.Users, ID, token);


    public static bool operator >( GroupRecord  left, GroupRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( GroupRecord left, GroupRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( GroupRecord  left, GroupRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( GroupRecord left, GroupRecord right ) => left.CompareTo(right) <= 0;
}
