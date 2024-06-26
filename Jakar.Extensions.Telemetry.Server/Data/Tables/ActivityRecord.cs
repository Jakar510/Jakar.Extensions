﻿// Jakar.Extensions :: Jakar.Extensions.Telemetry.Server
// 06/25/2024  22:06

namespace Jakar.Extensions.Telemetry.Server.Data.Tables;


[Table( TABLE_NAME )]
public record ActivityRecord( string ApplicationName, RecordID<ApplicationRecord>? ApplicationID, RecordID<UserRecord>? CreatedBy, RecordID<ActivityRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<ActivityRecord>( CreatedBy, ID, DateCreated, LastModified ), IDbReaderMapping<ActivityRecord>
{
    private const string TABLE_NAME = "Applications";
    public static string TableName => TABLE_NAME;


    // public static ActivityRecord Create( string applicationName, in RecordID<UserRecord>? createdBy, in RecordID<ActivityRecord> id ) => new(applicationName, createdBy, id, DateTimeOffset.UtcNow);
    public static ActivityRecord Create( DbDataReader reader )
    {
        string                      applicationName = reader.GetFieldValue<string>( nameof(ApplicationName) );
        RecordID<ApplicationRecord> applicationID   = RecordID<ApplicationRecord>.Create( reader, nameof(ApplicationID) );
        DateTimeOffset              dateCreated     = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?             lastModified    = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<UserRecord>?       ownerUserID     = RecordID<UserRecord>.CreatedBy( reader );
        RecordID<ActivityRecord>    id              = RecordID<ActivityRecord>.ID( reader );
        ActivityRecord              record          = new(applicationName, applicationID, ownerUserID, id, dateCreated, lastModified);
        record.Validate();
        return record;
    }
    public static async IAsyncEnumerable<ActivityRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
