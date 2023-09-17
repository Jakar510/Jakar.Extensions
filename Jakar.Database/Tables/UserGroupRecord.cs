﻿// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:39 PM

namespace Jakar.Database;


[ Serializable, Table( "UserGroups" ) ]
public sealed record UserGroupRecord : Mapping<UserGroupRecord, UserRecord, GroupRecord>, ICreateMapping<UserGroupRecord, UserRecord, GroupRecord>, IDbReaderMapping<UserGroupRecord>
{
    public UserGroupRecord( UserRecord owner, GroupRecord value, UserRecord? caller = default ) : base( owner, value, caller ) { }
    private UserGroupRecord( RecordID<UserRecord> key, RecordID<GroupRecord> value, RecordID<UserGroupRecord> id, RecordID<UserRecord> createdBy, Guid ownerUserID, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) :
        base( key, value, id, createdBy, ownerUserID, dateCreated, lastModified ) { }
    [ RequiresPreviewFeatures ] public static UserGroupRecord Create( UserRecord owner, GroupRecord value, UserRecord? caller = default ) => new(owner, value, caller);

    public static UserGroupRecord Create( DbDataReader reader )
    {
        var                       key          = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var                       value        = new RecordID<GroupRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        DateTimeOffset            dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?           lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        Guid                      ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>      createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<UserGroupRecord> id           = new RecordID<UserGroupRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new UserGroupRecord( key, value, id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<UserGroupRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
