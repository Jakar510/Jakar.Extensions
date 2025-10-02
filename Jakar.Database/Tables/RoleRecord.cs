using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using ValueOf;



namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
public sealed record RoleRecord( [property: StringLength(1024)] string NameOfRole,
                                 [property: StringLength(1024)] string NormalizedName,
                                 [property: StringLength(4096)] string ConcurrencyStamp,
                                 string                                Rights,
                                 RecordID<RoleRecord>                  ID,
                                 RecordID<UserRecord>?                 CreatedBy,
                                 DateTimeOffset                        DateCreated,
                                 DateTimeOffset?                       LastModified = null ) : OwnedTableRecord<RoleRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), ITableRecord<RoleRecord>, IRoleModel<Guid>
{
    public const                                string                     TABLE_NAME = "roles";
    public static                               string                     TableName     { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TABLE_NAME; }
    public static                               JsonSerializerContext      JsonContext   => JakarDatabaseContext.Default;
    public static                               JsonTypeInfo<RoleRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.RoleRecord;
    public static                               JsonTypeInfo<RoleRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.RoleRecordArray;
    [StringLength(IUserRights.MAX_SIZE)] public string                     Rights        { get; set; } = Rights;


    public static ImmutableDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<RoleRecord>.Create()
                                                                                                              .WithColumn<string>(nameof(NameOfRole),       length: 1024)
                                                                                                              .WithColumn<string>(nameof(NormalizedName),   length: 1024)
                                                                                                              .WithColumn<string>(nameof(ConcurrencyStamp), length: 4096)
                                                                                                              .WithColumn<string>(nameof(Rights),           length: UNICODE_CAPACITY)
                                                                                                              .With_CreatedBy()
                                                                                                              .Build();


    public RoleRecord( IdentityRole role, UserRecord? caller                     = null ) : this(role.Name ?? string.Empty, role.NormalizedName ?? string.Empty, role.ConcurrencyStamp ?? string.Empty, caller) { }
    public RoleRecord( IdentityRole role, string      rights, UserRecord? caller = null ) : this(role.Name ?? string.Empty, role.NormalizedName ?? string.Empty, role.ConcurrencyStamp ?? string.Empty, rights, caller) { }
    public RoleRecord( string       name, UserRecord? caller                                                                               = null ) : this(name, name, caller) { }
    public RoleRecord( string       name, string      normalizedName, UserRecord? caller                                                   = null ) : this(name, normalizedName, string.Empty, string.Empty, RecordID<RoleRecord>.New(), caller?.ID, DateTimeOffset.UtcNow) { }
    public RoleRecord( string       name, string      normalizedName, string      concurrencyStamp, UserRecord? caller                     = null ) : this(name, normalizedName, concurrencyStamp, string.Empty, RecordID<RoleRecord>.New(), caller?.ID, DateTimeOffset.UtcNow) { }
    public RoleRecord( string       name, string      normalizedName, string      concurrencyStamp, string      rights, UserRecord? caller = null ) : this(name, normalizedName, concurrencyStamp, rights, RecordID<RoleRecord>.New(), caller?.ID, DateTimeOffset.UtcNow) { }
    public RoleModel ToRoleModel() => new(this);
    public TRoleModel ToRoleModel<TRoleModel>()
        where TRoleModel : class, IRoleModel<TRoleModel, Guid> => TRoleModel.Create(this);

    public RoleRecord WithRights<TEnum>( scoped in UserRights<TEnum> rights )
        where TEnum : struct, Enum
    {
        Rights = rights.ToString();
        return this;
    }

    [Pure] public override PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(NameOfRole),       NameOfRole);
        parameters.Add(nameof(NormalizedName),   NormalizedName);
        parameters.Add(nameof(ConcurrencyStamp), ConcurrencyStamp);
        parameters.Add(nameof(Rights),           Rights);
        return parameters;
    }
    [Pure] public static RoleRecord Create( DbDataReader reader )
    {
        string                rights           = reader.GetFieldValue<string>(nameof(Rights));
        string                name             = reader.GetFieldValue<string>(nameof(NameOfRole));
        string                normalizedName   = reader.GetFieldValue<string>(nameof(NormalizedName));
        string                concurrencyStamp = reader.GetFieldValue<string>(nameof(ConcurrencyStamp));
        DateTimeOffset        dateCreated      = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?       lastModified     = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRecord>? ownerUserID      = RecordID<UserRecord>.CreatedBy(reader);
        RecordID<RoleRecord>  id               = RecordID<RoleRecord>.ID(reader);
        RoleRecord            record           = new(name, normalizedName, concurrencyStamp, rights, id, ownerUserID, dateCreated, lastModified);
        return record.Validate();
    }

    [Pure] public IAsyncEnumerable<UserRecord> GetUsers( NpgsqlConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => UserRoleRecord.Where(connection, transaction, db.Users, this, token);


    [Pure] public IdentityRole ToIdentityRole() => new()
                                                   {
                                                       Name             = NameOfRole,
                                                       NormalizedName   = NormalizedName,
                                                       ConcurrencyStamp = ConcurrencyStamp
                                                   };


    public static MigrationRecord CreateTable( ulong migrationID )
    {
        string tableID = TABLE_NAME.SqlColumnName();

        return MigrationRecord.Create<RoleRecord>(migrationID,
                                                  $"create {tableID} table",
                                                  $"""
                                                   CREATE TABLE IF NOT EXISTS {tableID}
                                                   (  
                                                   {nameof(NameOfRole).SqlColumnName()}       varchar(1024) NOT NULL, 
                                                   {nameof(NormalizedName).SqlColumnName()}   varchar(1024) NOT NULL, 
                                                   {nameof(ConcurrencyStamp).SqlColumnName()} varchar(1024) NOT NULL, 
                                                   {nameof(Rights).SqlColumnName()}           varchar(1024) NOT NULL, 
                                                   {nameof(ID).SqlColumnName()}               uuid          PRIMARY KEY,
                                                   {nameof(DateCreated).SqlColumnName()}      timestamptz   NOT NULL,
                                                   {nameof(LastModified).SqlColumnName()}     timestamptz   NULL,
                                                   {nameof(AdditionalData).SqlColumnName()}   json          NULL
                                                   );
                                                   
                                                   CREATE TRIGGER {nameof(MigrationRecord.SetLastModified).SqlColumnName()}
                                                   BEFORE INSERT OR UPDATE ON {tableID}
                                                   FOR EACH ROW
                                                   EXECUTE FUNCTION {nameof(MigrationRecord.SetLastModified).SqlColumnName()}();
                                                   """);
    }


    public override bool Equals( RoleRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && string.Equals(NameOfRole, other.NameOfRole, StringComparison.InvariantCultureIgnoreCase) && string.Equals(NormalizedName, other.NormalizedName, StringComparison.InvariantCultureIgnoreCase);
    }
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), NameOfRole, NormalizedName, Rights);
    public override int CompareTo( RoleRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int nameComparison = string.Compare(NameOfRole, other.NameOfRole, StringComparison.Ordinal);
        if ( nameComparison != 0 ) { return nameComparison; }

        int normalizedNameComparison = string.Compare(NormalizedName, other.NormalizedName, StringComparison.Ordinal);
        if ( normalizedNameComparison != 0 ) { return normalizedNameComparison; }

        int concurrencyComparison = string.Compare(ConcurrencyStamp, other.ConcurrencyStamp, StringComparison.Ordinal);
        if ( concurrencyComparison != 0 ) { return concurrencyComparison; }

        return base.CompareTo(other);
    }


    public static bool operator >( RoleRecord  left, RoleRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( RoleRecord left, RoleRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( RoleRecord  left, RoleRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( RoleRecord left, RoleRecord right ) => left.CompareTo(right) <= 0;
}
