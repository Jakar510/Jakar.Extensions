using Newtonsoft.Json.Linq;
using ValueOf;



namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed record GroupRecord( [property: StringLength(GroupRecord.MAX_SIZE)] string? CustomerID, [property: StringLength(GroupRecord.MAX_SIZE)] string NameOfGroup, UserRights Rights, RecordID<GroupRecord> ID, RecordID<UserRecord>? CreatedBy, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null )
    : OwnedTableRecord<GroupRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), ITableRecord<GroupRecord>, IGroupModel<Guid>
{
    public const  int                         MAX_SIZE   = 1024;
    public const  string                      TABLE_NAME = "groups";
    public static JsonTypeInfo<GroupRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.GroupRecordArray;
    public static JsonSerializerContext       JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<GroupRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.GroupRecord;


    public static FrozenDictionary<string, ColumnMetaData<GroupRecord>> PropertyMetaData { get; } = SqlTable<GroupRecord>.Default.WithColumn<string?>(nameof(CustomerID), length: MAX_SIZE)
                                                                                                                         .WithColumn<string>(nameof(NameOfGroup), length: MAX_SIZE, checks: $"{nameof(NameOfGroup)} > 0")
                                                                                                                         .WithColumn<string>(nameof(Rights),      checks: $"{nameof(Rights)} > 0")
                                                                                                                         .With_CreatedBy()
                                                                                                                         .Build();


    public static string                     TableName { get => TABLE_NAME; }
    Guid? ICreatedByUser<Guid>.              CreatedBy => CreatedBy?.Value;
    Guid? IGroupModel<Guid>.                 OwnerID   => CreatedBy?.Value;
    [StringLength(RIGHTS)] public UserRights Rights    { get; set; } = Rights;


    public GroupRecord( UserRecord? owner, string nameOfGroup, string? customerID ) : this(customerID, nameOfGroup, string.Empty, RecordID<GroupRecord>.New(), owner?.ID, DateTimeOffset.UtcNow) { }
    public GroupRecord( UserRecord? owner, string nameOfGroup, string? customerID, string rights ) : this(customerID, nameOfGroup, rights, RecordID<GroupRecord>.New(), owner?.ID, DateTimeOffset.UtcNow) { }
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
                                                   {nameof(CustomerID).SqlColumnName()}     varchar(1024) NOT NULL,  
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

        int customerIDComparison = string.Compare(CustomerID, other.CustomerID, StringComparison.Ordinal);
        if ( customerIDComparison != 0 ) { return customerIDComparison; }

        int lastModifiedComparison = Nullable.Compare(LastModified, other.LastModified);
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        return DateCreated.CompareTo(other.DateCreated);
    }
    public override bool Equals( GroupRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && NameOfGroup == other.NameOfGroup && Rights == other.Rights && CustomerID == other.CustomerID;
    }
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), NameOfGroup, Rights, CustomerID);


    [Pure] public override object ToDynamicParameters()
    {
        PostgresParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(CustomerID),  CustomerID);
        parameters.Add(nameof(NameOfGroup), NameOfGroup);
        parameters.Add(nameof(CreatedBy),   CreatedBy);
        parameters.Add(nameof(Rights),      Rights);
        return parameters;
    }


    [Pure] public static GroupRecord Create( DbDataReader reader )
    {
        string                customerID   = reader.GetFieldValue<string>(nameof(CustomerID));
        string                nameOfGroup  = reader.GetFieldValue<string>(nameof(NameOfGroup));
        string                rights       = reader.GetFieldValue<string>(nameof(Rights));
        DateTimeOffset        dateCreated  = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?       lastModified = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRecord>? ownerUserID  = RecordID<UserRecord>.CreatedBy(reader);
        RecordID<GroupRecord> id           = RecordID<GroupRecord>.ID(reader);
        GroupRecord           record       = new(customerID, nameOfGroup, rights, id, ownerUserID, dateCreated, lastModified);
        return record.Validate();
    }


    [Pure] public async ValueTask<UserRecord?>       GetOwner( NpgsqlConnection connection, NpgsqlTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get(connection, transaction, CreatedBy, token);
    [Pure] public       IAsyncEnumerable<UserRecord> GetUsers( NpgsqlConnection connection, NpgsqlTransaction? transaction, Database db, CancellationToken token ) => UserGroupRecord.Where(connection, transaction, db.Users, this, token);


    public static bool operator >( GroupRecord  left, GroupRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( GroupRecord left, GroupRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( GroupRecord  left, GroupRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( GroupRecord left, GroupRecord right ) => left.CompareTo(right) <= 0;
}
