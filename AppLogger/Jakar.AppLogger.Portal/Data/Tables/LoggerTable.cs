// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/26/2022  2:48 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


public interface ILoggerTable : JsonModels.IJsonModel
{
    public   bool IsActive    { get; set; }
    internal bool IsNotActive => !IsActive;
}



public abstract record LoggerTable<TRecord>( RecordID<TRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified ) : TableRecord<TRecord>( ID, DateCreated, LastModified ), ILoggerTable
    where TRecord : LoggerTable<TRecord>, IDbReaderMapping<TRecord>
{
    [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
    public                       bool                          IsActive       { get; set; } = true;
    internal                     bool                          IsNotActive    => !IsActive;
    bool ILoggerTable.                                         IsNotActive    => IsNotActive;


    protected LoggerTable( RecordID<TRecord> id ) : this( id, DateTimeOffset.UtcNow, null ) { }
}



public abstract record OwnedLoggerTable<TRecord>
    ( RecordID<TRecord> ID, RecordID<UserRecord>? CreatedBy, Guid? OwnerUserID, DateTimeOffset DateCreated, DateTimeOffset? LastModified ) : OwnedTableRecord<TRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), ILoggerTable
    where TRecord : OwnedLoggerTable<TRecord>, IDbReaderMapping<TRecord>
{
    [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
    public                       bool                          IsActive       { get; set; } = true;
    internal                     bool                          IsNotActive    => !IsActive;
    bool ILoggerTable.                                         IsNotActive    => IsNotActive;


    protected OwnedLoggerTable( UserRecord?       owner ) : this( RecordID<TRecord>.New(), owner ) { }
    protected OwnedLoggerTable( RecordID<TRecord> id, UserRecord? owner = default ) : this( id, owner?.ID, owner?.UserID, DateTimeOffset.UtcNow, null ) { }
}
