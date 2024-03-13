namespace Jakar.Database;


[Serializable]
[Table( TABLE_NAME )]
public sealed record GroupRecord( string?                                CustomerID,
                                  string                                 NameOfGroup,
                                  RecordID<UserRecord>                   OwnerID,
                                  [MaxLength( IRights.MAX_SIZE )] string Rights,
                                  RecordID<GroupRecord>                  ID,
                                  RecordID<UserRecord>?                  CreatedBy,
                                  Guid?                                  OwnerUserID,
                                  DateTimeOffset                         DateCreated,
                                  DateTimeOffset?                        LastModified = default ) : OwnedTableRecord<GroupRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<GroupRecord>, IRights
{
    public const  string TABLE_NAME = "Groups";
    public static string TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }


    public GroupRecord( UserRecord owner, string nameOfGroup, string? customerID, UserRecord? caller                     = default ) : this( customerID, nameOfGroup, owner.ID, string.Empty, RecordID<GroupRecord>.New(), caller?.ID, caller?.UserID, DateTimeOffset.UtcNow ) { }
    public GroupRecord( UserRecord owner, string nameOfGroup, string? customerID, IUserRights rights, UserRecord? caller = default ) : this( customerID, nameOfGroup, owner.ID, rights.ToString(), RecordID<GroupRecord>.New(), caller?.ID, caller?.UserID, DateTimeOffset.UtcNow ) { }


    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(CustomerID),  CustomerID );
        parameters.Add( nameof(NameOfGroup), NameOfGroup );
        parameters.Add( nameof(OwnerID),     OwnerID );
        parameters.Add( nameof(Rights),      Rights );
        return parameters;
    }
    [Pure]
    public static GroupRecord Create( DbDataReader reader )
    {
        string                customerID   = reader.GetFieldValue<string>( nameof(CustomerID) );
        string                nameOfGroup  = reader.GetFieldValue<string>( nameof(NameOfGroup) );
        var                   ownerID      = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(OwnerID) ) );
        string                rights       = reader.GetFieldValue<string>( nameof(Rights) );
        var                   dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var                   lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var                   ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>? createdBy    = RecordID<UserRecord>.CreatedBy( reader );
        RecordID<GroupRecord> id           = RecordID<GroupRecord>.ID( reader );
        var                   record       = new GroupRecord( customerID, nameOfGroup, ownerID, rights, id, createdBy, ownerUserID, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [Pure]
    public static async IAsyncEnumerable<GroupRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }

    [Pure] public async ValueTask<UserRecord?>       GetOwner( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, OwnerID, token );
    [Pure] public       IAsyncEnumerable<UserRecord> GetUsers( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => UserGroupRecord.Where( connection, transaction, db.UserGroups, db.Users, this, token );
}
