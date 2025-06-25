namespace Jakar.Extensions.Serilog.Server.Data.Tables;


[Table( TABLE_NAME )]
public record ApplicationRecord( string ApplicationName, RecordID<UserRecord>? CreatedBy, RecordID<ApplicationRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<ApplicationRecord>( in CreatedBy, in ID, in DateCreated, in LastModified ), IDbReaderMapping<ApplicationRecord>
{
    private const string TABLE_NAME = "Applications";
    public static string TableName => TABLE_NAME;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ApplicationRecord Create( string applicationName, RecordID<UserRecord>? createdBy, RecordID<ApplicationRecord> id ) => new(applicationName, createdBy, id, DateTimeOffset.UtcNow);
    public static ApplicationRecord Create( DbDataReader reader )
    {
        string                      applicationName = reader.GetFieldValue<string>( nameof(ApplicationName) );
        DateTimeOffset              dateCreated     = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?             lastModified    = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<UserRecord>?       ownerUserID     = RecordID<UserRecord>.CreatedBy( reader );
        RecordID<ApplicationRecord> id              = RecordID<ApplicationRecord>.ID( reader );
        ApplicationRecord           record          = new(applicationName, ownerUserID, id, dateCreated, lastModified);
        return record.Validate();
    }
    public static async IAsyncEnumerable<ApplicationRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public static bool operator >( ApplicationRecord  left, ApplicationRecord right ) => Sorter.GreaterThan( left, right );
    public static bool operator >=( ApplicationRecord left, ApplicationRecord right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static bool operator <( ApplicationRecord  left, ApplicationRecord right ) => Sorter.LessThan( left, right );
    public static bool operator <=( ApplicationRecord left, ApplicationRecord right ) => Sorter.LessThanOrEqualTo( left, right );
}
