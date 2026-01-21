using System;



namespace Jakar.Extensions.Telemetry.Server.Data.Tables;


[Table(TABLE_NAME)]
public record ApplicationRecord( string ApplicationName, RecordID<UserRecord>? CreatedBy, RecordID<ApplicationRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<ApplicationRecord>(CreatedBy, ID, DateCreated, LastModified), ITableRecord<ApplicationRecord>
{
    private const string                                   TABLE_NAME = "Applications";
    public static FrozenDictionary<string, ColumnMetaData> PropertyMetaData { get; }
    public static JsonTypeInfo<ApplicationRecord[]>        JsonArrayInfo    { get; }
    public static JsonSerializerContext                    JsonContext      { get; }
    public static JsonTypeInfo<ApplicationRecord>          JsonTypeInfo     { get; }
    public static string                                   TableName        => TABLE_NAME;


    public static ApplicationRecord Create( string     applicationName, in RecordID<UserRecord>? createdBy, in RecordID<ApplicationRecord> id ) => new(applicationName, createdBy, id, DateTimeOffset.UtcNow);
    public static MigrationRecord   CreateTable( ulong migrationID ) => null;
    public static ApplicationRecord Create( DbDataReader reader )
    {
        string                      applicationName = reader.GetFieldValue<string>(nameof(ApplicationName));
        DateTimeOffset              dateCreated     = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?             lastModified    = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRecord>?       ownerUserID     = RecordID<UserRecord>.CreatedBy(reader);
        RecordID<ApplicationRecord> id              = RecordID<ApplicationRecord>.ID(reader);
        ApplicationRecord           record          = new(applicationName, ownerUserID, id, dateCreated, lastModified);
        record.Validate();
        return record;
    }
}
