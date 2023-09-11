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
) : TableRecord<GroupRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<GroupRecord>, UserRights.IRights
{
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

    
    public static GroupRecord Create( DbDataReader reader )
    {
        DateTimeOffset           dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset           lastModified = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        Guid                     ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>     createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<GroupRecord> id           = new RecordID<GroupRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new GroupRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<GroupRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public UserRights GetRights() => new(this);
    public async ValueTask<UserRecord?> GetOwner( DbConnection             connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, OwnerID, token );
    public async ValueTask<IEnumerable<UserRecord>> GetUsers( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await UserGroupRecord.Where( connection, transaction, db.UserGroups, db.Users, this, token );
}
