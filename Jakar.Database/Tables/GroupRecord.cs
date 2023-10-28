namespace Jakar.Database;


[ Serializable, Table( "Groups" ) ]
public sealed record GroupRecord( [ MaxLength( 256 ) ]                                                      string?              CustomerID,
                                  [ MaxLength( 1024 ) ]                                                     string               NameOfGroup,
                                  [ MaxLength( 256 ) ]                                                      RecordID<UserRecord> OwnerID,
                                  [ MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string               Rights,
                                  RecordID<GroupRecord>                                                                          ID,
                                  RecordID<UserRecord>?                                                                          CreatedBy,
                                  Guid?                                                                                          OwnerUserID,
                                  DateTimeOffset                                                                                 DateCreated,
                                  DateTimeOffset?                                                                                LastModified = default
) : OwnedTableRecord<GroupRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<GroupRecord>, UserRights.IRights
{
    public static string TableName { get; } = typeof(GroupRecord).GetTableName();


    public GroupRecord( UserRecord owner, string nameOfGroup, string? customerID, UserRecord? caller = default ) : this( customerID,
                                                                                                                         nameOfGroup,
                                                                                                                         owner.ID,
                                                                                                                         string.Empty,
                                                                                                                         RecordID<GroupRecord>.New(),
                                                                                                                         caller?.ID,
                                                                                                                         caller?.UserID,
                                                                                                                         DateTimeOffset.UtcNow ) { }
    public GroupRecord( UserRecord owner, string nameOfGroup, string? customerID, UserRights rights, UserRecord? caller = default ) : this( customerID,
                                                                                                                                            nameOfGroup,
                                                                                                                                            owner.ID,
                                                                                                                                            rights.ToString(),
                                                                                                                                            RecordID<GroupRecord>.New(),
                                                                                                                                            caller?.ID,
                                                                                                                                            caller?.UserID,
                                                                                                                                            DateTimeOffset.UtcNow ) { }

    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(CustomerID),  CustomerID );
        parameters.Add( nameof(NameOfGroup), NameOfGroup );
        parameters.Add( nameof(OwnerID),     OwnerID );
        parameters.Add( nameof(Rights),      Rights );
        return parameters;
    }

    public static GroupRecord Create( DbDataReader reader )
    {
        string                customerID   = reader.GetString( nameof(CustomerID) );
        string                nameOfGroup  = reader.GetString( nameof(NameOfGroup) );
        var                   ownerID      = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(OwnerID) ) );
        string                rights       = reader.GetString( nameof(Rights) );
        var                   dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var                   lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var                   ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>? createdBy    = RecordID<UserRecord>.CreatedBy( reader );
        RecordID<GroupRecord> id           = RecordID<GroupRecord>.ID( reader );
        var                   record       = new GroupRecord( customerID, nameOfGroup, ownerID, rights, id, createdBy, ownerUserID, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    public static async IAsyncEnumerable<GroupRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public async ValueTask<UserRecord?>       GetOwner( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, OwnerID, token );
    public       IAsyncEnumerable<UserRecord> GetUsers( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => UserGroupRecord.Where( connection, transaction, db.UserGroups, db.Users, this, token );
    public       UserRights                   GetRights() => UserRights.Create( this );
}
