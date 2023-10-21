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
    TryInsert,
    InsertOrUpdate,
    Update,
    GetID,
    GetIDs
}



public interface ISqlCache<TRecord> where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    SqlCommand All();
    SqlCommand First();
    SqlCommand Last();
    SqlCommand SortedIDs();
    SqlCommand Delete( in RecordID<TRecord>              id );
    SqlCommand Delete( in IEnumerable<RecordID<TRecord>> ids );
    SqlCommand Delete( in bool                           matchAll, in DynamicParameters parameters );
    SqlCommand Next( in   RecordPair<TRecord>            pair );
    SqlCommand Next( in   RecordID<TRecord>              id, in DateTimeOffset dateCreated );
    SqlCommand NextID( in RecordPair<TRecord>            pair );
    SqlCommand NextID( in RecordID<TRecord>              id, in DateTimeOffset dateCreated );
    SqlCommand Count();
    SqlCommand Random();
    SqlCommand Random( in int                  count );
    SqlCommand Random( in Guid?                userID, in int count );
    SqlCommand Random( in RecordID<UserRecord> id,     in int count );
    SqlCommand Single();
    SqlCommand Insert( in          TRecord                        record );
    SqlCommand TryInsert( in       TRecord                        record, in bool matchAll, in DynamicParameters parameters );
    SqlCommand InsertOrUpdate( in  TRecord                        record, in bool matchAll, in DynamicParameters parameters );
    SqlCommand Update( in          TRecord                        record );
    SqlCommand Where<TValue>( in   string                         columnName, in TValue?           value );
    SqlCommand Where( in           bool                           matchAll,   in DynamicParameters parameters );
    SqlCommand WhereID<TValue>( in string                         columnName, in TValue?           value );
    SqlCommand Exists( in          bool                           matchAll,   in DynamicParameters parameters );
    SqlCommand Get( in             bool                           matchAll,   in DynamicParameters parameters );
    SqlCommand Get( in             RecordID<TRecord>              id );
    SqlCommand Get( in             IEnumerable<RecordID<TRecord>> ids );
}
