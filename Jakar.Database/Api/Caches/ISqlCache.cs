// Jakar.Extensions :: Jakar.Database
// 10/16/2023  9:11 PM

namespace Jakar.Database;


public enum SqlCacheType
{
    All,
    First,
    Last,
    SortedIDs,
    DeleteRecord,
    DeleteRecords,
    Next,
    NextID,
    Count,
    Random,
    RandomCount,
    RandomUserIDCount,
    RandomUserCount,
    Single,
    Insert,
    Update,
    GetID,
    GetIDs
}



public interface ISqlCache<TRecord>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    void       Reset();
    SqlCommand All();
    SqlCommand First();
    SqlCommand Last();
    SqlCommand SortedIDs();
    SqlCommand Delete( scoped in RecordID<TRecord>              id );
    SqlCommand Delete( in        IEnumerable<RecordID<TRecord>> ids );
    SqlCommand Delete( in        bool                           matchAll, in DynamicParameters parameters );
    SqlCommand Next( scoped in   RecordPair<TRecord>            pair );
    SqlCommand Next( scoped in   RecordID<TRecord>              id, scoped in DateTimeOffset dateCreated );
    SqlCommand NextID( scoped in RecordPair<TRecord>            pair );
    SqlCommand NextID( scoped in RecordID<TRecord>              id, scoped in DateTimeOffset dateCreated );
    SqlCommand Count();
    SqlCommand Random();
    SqlCommand Random( in          int                            count );
    SqlCommand Random( scoped in   Guid?                          userID, in int count );
    SqlCommand Random( scoped in   RecordID<UserRecord>           id,     in int count );
    SqlCommand Single( scoped in   RecordID<TRecord>              id );
    SqlCommand Insert( in          TRecord                        record );
    SqlCommand TryInsert( in       TRecord                        record, in bool matchAll, in DynamicParameters parameters );
    SqlCommand InsertOrUpdate( in  TRecord                        record, in bool matchAll, in DynamicParameters parameters );
    SqlCommand Update( in          TRecord                        record );
    SqlCommand Where<TValue>( in   string                         columnName, in TValue?           value );
    SqlCommand Where( in           bool                           matchAll,   in DynamicParameters parameters );
    SqlCommand WhereID<TValue>( in string                         columnName, in TValue?           value );
    SqlCommand Exists( in          bool                           matchAll,   in DynamicParameters parameters );
    SqlCommand Get( in             bool                           matchAll,   in DynamicParameters parameters );
    SqlCommand Get( scoped in      RecordID<TRecord>              id );
    SqlCommand Get( in             IEnumerable<RecordID<TRecord>> ids );
}
