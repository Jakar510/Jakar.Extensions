namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record RoleRecord( [property: StringLength( 1024 )] string NameOfRole,
                                 [property: StringLength( 1024 )] string NormalizedName,
                                 [property: StringLength( 4096 )] string ConcurrencyStamp,
                                 string                                  Rights,
                                 RecordID<RoleRecord>                    ID,
                                 RecordID<UserRecord>?                   OwnerUserID,
                                 DateTimeOffset                          DateCreated,
                                 DateTimeOffset?                         LastModified = default ) : OwnedTableRecord<RoleRecord>( ID, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<RoleRecord>, IRoleModel<Guid>
{
    public const                                  string TABLE_NAME = "Roles";
    public static                                 string TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }
    [StringLength( IUserRights.MAX_SIZE )] public string Rights    { get; set; } = Rights;


    public RoleRecord( IdentityRole role, UserRecord? caller                     = default ) : this( role.Name ?? string.Empty, role.NormalizedName ?? string.Empty, role.ConcurrencyStamp ?? string.Empty, caller ) { }
    public RoleRecord( IdentityRole role, string      rights, UserRecord? caller = default ) : this( role.Name ?? string.Empty, role.NormalizedName ?? string.Empty, role.ConcurrencyStamp ?? string.Empty, rights, caller ) { }
    public RoleRecord( string       name, UserRecord? caller                                                                               = default ) : this( name, name, caller ) { }
    public RoleRecord( string       name, string      normalizedName, UserRecord? caller                                                   = default ) : this( name, normalizedName, string.Empty, string.Empty, RecordID<RoleRecord>.New(), caller?.ID, DateTimeOffset.UtcNow ) { }
    public RoleRecord( string       name, string      normalizedName, string      concurrencyStamp, UserRecord? caller                     = default ) : this( name, normalizedName, concurrencyStamp, string.Empty, RecordID<RoleRecord>.New(), caller?.ID, DateTimeOffset.UtcNow ) { }
    public RoleRecord( string       name, string      normalizedName, string      concurrencyStamp, string      rights, UserRecord? caller = default ) : this( name, normalizedName, concurrencyStamp, rights, RecordID<RoleRecord>.New(), caller?.ID, DateTimeOffset.UtcNow ) { }
    public RoleModel<Guid> ToRoleModel() => new(this);
    public TRoleModel ToRoleModel<TRoleModel>()
        where TRoleModel : IRoleModel<TRoleModel, Guid> => TRoleModel.Create( this );

    public RoleRecord WithRights<TEnum>( scoped in UserRights<TEnum> rights )
        where TEnum : struct, Enum
    {
        Rights = rights.ToString();
        return this;
    }

    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add( nameof(NameOfRole),       NameOfRole );
        parameters.Add( nameof(NormalizedName),   NormalizedName );
        parameters.Add( nameof(ConcurrencyStamp), ConcurrencyStamp );
        parameters.Add( nameof(Rights),           Rights );
        return parameters;
    }
    [Pure]
    public static RoleRecord Create( DbDataReader reader )
    {
        string                rights           = reader.GetFieldValue<string>( nameof(Rights) );
        string                name             = reader.GetFieldValue<string>( nameof(NameOfRole) );
        string                normalizedName   = reader.GetFieldValue<string>( nameof(NormalizedName) );
        string                concurrencyStamp = reader.GetFieldValue<string>( nameof(ConcurrencyStamp) );
        DateTimeOffset        dateCreated      = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?       lastModified     = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<UserRecord>? ownerUserID      = RecordID<UserRecord>.OwnerUserID( reader );
        RecordID<RoleRecord>  id               = RecordID<RoleRecord>.ID( reader );
        RoleRecord            record           = new RoleRecord( name, normalizedName, concurrencyStamp, rights, id, ownerUserID, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [Pure]
    public static async IAsyncEnumerable<RoleRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }

    [Pure] public IAsyncEnumerable<UserRecord> GetUsers( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => UserRoleRecord.Where( connection, transaction, db.Users, this, token );


    [Pure]
    public IdentityRole ToIdentityRole() => new()
                                            {
                                                Name             = NameOfRole,
                                                NormalizedName   = NormalizedName,
                                                ConcurrencyStamp = ConcurrencyStamp
                                            };


    public override int CompareTo( RoleRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int nameComparison = string.Compare( NameOfRole, other.NameOfRole, StringComparison.Ordinal );
        if ( nameComparison != 0 ) { return nameComparison; }

        int normalizedNameComparison = string.Compare( NormalizedName, other.NormalizedName, StringComparison.Ordinal );
        if ( normalizedNameComparison != 0 ) { return normalizedNameComparison; }

        int concurrencyComparison = string.Compare( ConcurrencyStamp, other.ConcurrencyStamp, StringComparison.Ordinal );
        if ( concurrencyComparison != 0 ) { return concurrencyComparison; }

        return base.CompareTo( other );
    }
}
