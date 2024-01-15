namespace Jakar.Database;


[ Serializable, Table( "Roles" ) ]
public sealed record RoleRecord( [ property: MaxLength( 1024 ) ] string Name, [ property: MaxLength( 1024 ) ] string NormalizedName, [ property: MaxLength( 4096 ) ] string ConcurrencyStamp, [ property: MaxLength( IRights.MAX_SIZE ) ] string Rights, RecordID<RoleRecord> ID, RecordID<UserRecord>? CreatedBy, Guid? OwnerUserID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = default )
    : OwnedTableRecord<RoleRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<RoleRecord>, IRights
{
    public static string TableName { get; } = typeof(RoleRecord).GetTableName();


    public RoleRecord( IdentityRole role, UserRecord?    caller                     = default ) : this( role.Name ?? string.Empty, role.NormalizedName ?? string.Empty, role.ConcurrencyStamp ?? string.Empty, caller ) { }
    public RoleRecord( IdentityRole role, in IUserRights rights, UserRecord? caller = default ) : this( role.Name ?? string.Empty, role.NormalizedName ?? string.Empty, role.ConcurrencyStamp ?? string.Empty, rights, caller ) { }
    public RoleRecord( string       name, UserRecord?    caller                                                                                  = default ) : this( name, name, caller ) { }
    public RoleRecord( string       name, string         normalizedName, UserRecord? caller                                                      = default ) : this( name, normalizedName, string.Empty, string.Empty, RecordID<RoleRecord>.New(), caller?.ID, caller?.UserID, DateTimeOffset.UtcNow ) { }
    public RoleRecord( string       name, string         normalizedName, string      concurrencyStamp, UserRecord?    caller                     = default ) : this( name, normalizedName, concurrencyStamp, string.Empty, RecordID<RoleRecord>.New(), caller?.ID, caller?.UserID, DateTimeOffset.UtcNow ) { }
    public RoleRecord( string       name, string         normalizedName, string      concurrencyStamp, in IUserRights rights, UserRecord? caller = default ) : this( name, normalizedName, concurrencyStamp, rights.ToString(), RecordID<RoleRecord>.New(), caller?.ID, caller?.UserID, DateTimeOffset.UtcNow ) { }


    [ Pure ]
    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(Name),             Name );
        parameters.Add( nameof(NormalizedName),   NormalizedName );
        parameters.Add( nameof(ConcurrencyStamp), ConcurrencyStamp );
        parameters.Add( nameof(Rights),           Rights );
        return parameters;
    }
    [ Pure ]
    public static RoleRecord Create( DbDataReader reader )
    {
        string                rights           = reader.GetFieldValue<string>( nameof(Rights) );
        string                name             = reader.GetFieldValue<string>( nameof(Name) );
        string                normalizedName   = reader.GetFieldValue<string>( nameof(NormalizedName) );
        string                concurrencyStamp = reader.GetFieldValue<string>( nameof(ConcurrencyStamp) );
        var                   dateCreated      = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var                   lastModified     = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var                   ownerUserID      = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>? createdBy        = RecordID<UserRecord>.CreatedBy( reader );
        RecordID<RoleRecord>  id               = RecordID<RoleRecord>.ID( reader );
        var                   record           = new RoleRecord( name, normalizedName, concurrencyStamp, rights, id, createdBy, ownerUserID, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [ Pure ]
    public static async IAsyncEnumerable<RoleRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }

    [ Pure ] public IAsyncEnumerable<UserRecord> GetUsers( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => UserRoleRecord.Where( connection, transaction, db.UserRoles, db.Users, this, token );
    
    
    [ Pure ]
    public IdentityRole ToIdentityRole() => new()
                                            {
                                                Name             = Name,
                                                NormalizedName   = NormalizedName,
                                                ConcurrencyStamp = ConcurrencyStamp
                                            };


    public override int CompareTo( RoleRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int nameComparison = string.Compare( Name, other.Name, StringComparison.Ordinal );
        if ( nameComparison != 0 ) { return nameComparison; }

        int normalizedNameComparison = string.Compare( NormalizedName, other.NormalizedName, StringComparison.Ordinal );
        if ( normalizedNameComparison != 0 ) { return normalizedNameComparison; }

        int concurrencyComparison = string.Compare( ConcurrencyStamp, other.ConcurrencyStamp, StringComparison.Ordinal );
        if ( concurrencyComparison != 0 ) { return concurrencyComparison; }

        return base.CompareTo( other );
    }
}
