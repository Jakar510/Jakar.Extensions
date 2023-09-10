// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/26/2022  2:48 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


public abstract record LoggerTable<TRecord>
    ( RecordID<TRecord> ID, RecordID<UserRecord>? CreatedBy, Guid? OwnerUserID, DateTimeOffset DateCreated, DateTimeOffset? LastModified ) : TableRecord<TRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ),
                                                                                                                                                               JsonModels.IJsonStringModel where TRecord : LoggerTable<TRecord>
{
    [ MaxLength( int.MaxValue ) ] public string? AdditionalData { get; set; }


    public   bool IsActive    { get; set; } = true;
    internal bool IsNotActive => !IsActive;

    
    protected LoggerTable( UserRecord?       owner ) : this( RecordID<TRecord>.New(), owner ) { }
    protected LoggerTable( RecordID<TRecord> id, UserRecord? owner = default ) : this( id, owner?.ID, owner?.UserID, DateTimeOffset.UtcNow, null ) { }

}
