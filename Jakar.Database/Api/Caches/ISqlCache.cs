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
    public string         CreatedBy    { get; }
    public string         DateCreated  { get; }
    public string         IdColumnName { get; }
    public DbTypeInstance Instance     { get; }
    public string         LastModified { get; }
    public string         RandomMethod { get; }


    void       Reset();
    SqlCommand All();
    SqlCommand First();
    SqlCommand Last();
    SqlCommand SortedIDs();
    SqlCommand Delete( ref readonly RecordID<TRecord>              id );
    SqlCommand Delete( ref readonly IEnumerable<RecordID<TRecord>> ids );
    SqlCommand Delete( ref readonly bool                           matchAll, ref readonly DynamicParameters parameters );
    SqlCommand Next( ref readonly   RecordPair<TRecord>            pair );
    SqlCommand Next( ref readonly   RecordID<TRecord>              id, ref readonly DateTimeOffset dateCreated );
    SqlCommand NextID( ref readonly RecordPair<TRecord>            pair );
    SqlCommand NextID( ref readonly RecordID<TRecord>              id, ref readonly DateTimeOffset dateCreated );
    SqlCommand Count();
    SqlCommand Random();
    SqlCommand Random( ref readonly          int                            count );
    SqlCommand Random( ref readonly          Guid?                          userID, ref readonly int count );
    SqlCommand Random( ref readonly          RecordID<UserRecord>           id,     ref readonly int count );
    SqlCommand Single( ref readonly          RecordID<TRecord>              id );
    SqlCommand Insert( ref readonly          TRecord                        record );
    SqlCommand TryInsert( ref readonly       TRecord                        record, ref readonly bool matchAll, ref readonly DynamicParameters parameters );
    SqlCommand InsertOrUpdate( ref readonly  TRecord                        record, ref readonly bool matchAll, ref readonly DynamicParameters parameters );
    SqlCommand Update( ref readonly          TRecord                        record );
    SqlCommand Where<TValue>( ref readonly   string                         columnName, ref readonly TValue?           value );
    SqlCommand Where( ref readonly           bool                           matchAll,   ref readonly DynamicParameters parameters );
    SqlCommand WhereID<TValue>( ref readonly string                         columnName, ref readonly TValue?           value );
    SqlCommand Exists( ref readonly          bool                           matchAll,   ref readonly DynamicParameters parameters );
    SqlCommand Get( ref readonly             bool                           matchAll,   ref readonly DynamicParameters parameters );
    SqlCommand Get( ref readonly             RecordID<TRecord>              id );
    SqlCommand Get( ref readonly             IEnumerable<RecordID<TRecord>> ids );
}
