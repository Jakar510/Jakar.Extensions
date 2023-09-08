// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


public interface IRecordPair : IUniqueID<Guid> // where TID : IComparable<TID>, IEquatable<TID>
{
    public DateTimeOffset DateCreated { get; }
}



public interface IDbReaderMapping<out TRecord> where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public abstract static TRecord Create( DbDataReader                        reader );
    public abstract static IAsyncEnumerable<TRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default );
}



public interface ITableRecord<TRecord> : IRecordPair where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public new             RecordID<TRecord>     ID           { get; }
    public                 RecordID<UserRecord>? CreatedBy    { get; }
    public                 DateTimeOffset?       LastModified { get; }
    public                 Guid?                 OwnerUserID  { get; }
    public abstract static string                TableName    { get; }
    Guid IUniqueID<Guid>.                        ID           => ID.Value;
}



[Serializable]
public abstract record TableRecord<TRecord>( [property: Key] RecordID<TRecord> ID, RecordID<UserRecord>? CreatedBy, Guid? OwnerUserID, DateTimeOffset DateCreated, DateTimeOffset? LastModified ) : ITableRecord<TRecord>
    where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public static string TableName { get; } = typeof(TRecord).GetTableName();


    protected TableRecord( UserRecord?       owner ) : this( RecordID<TRecord>.New(), owner ) { }
    protected TableRecord( RecordID<TRecord> id, UserRecord? owner = default ) : this( id, owner?.ID, owner?.UserID, DateTimeOffset.UtcNow, default ) { }



    public static DynamicParameters GetDynamicParameters( UserRecord user )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(CreatedBy),   user.ID );
        parameters.Add( nameof(OwnerUserID), user.UserID );
        return parameters;
    }
    protected static DynamicParameters GetDynamicParameters( TableRecord<TRecord> record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(OwnerUserID), record.OwnerUserID );
        parameters.Add( nameof(CreatedBy),   record.CreatedBy );
        return parameters;
    }


    public async ValueTask<UserRecord?> GetUser( DbConnection           connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, true, GetDynamicParameters( this ), token );
    public async ValueTask<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, CreatedBy?.Value, token );


    public TRecord WithOwner( UserRecord user ) => (TRecord)(this with
                                                             {
                                                                 OwnerUserID = user.UserID
                                                             });
    internal TRecord NewID( Guid id ) => NewID( new RecordID<TRecord>( id ) );
    public TRecord NewID( RecordID<TRecord> id ) => (TRecord)(this with
                                                              {
                                                                  ID = id
                                                              });


    public bool Owns( UserRecord       record ) => record.CreatedBy == record.ID;
    public bool DoesNotOwn( UserRecord record ) => record.CreatedBy != record.ID;


    public virtual int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int lastModifiedComparison = Nullable.Compare( LastModified, other.LastModified );
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        int createdByComparison = Nullable.Compare( CreatedBy, other.CreatedBy );
        if ( createdByComparison != 0 ) { return createdByComparison; }

        int userIDComparison = Nullable.Compare( OwnerUserID, other.OwnerUserID );
        if ( userIDComparison != 0 ) { return userIDComparison; }

        return DateCreated.CompareTo( other.DateCreated );
    }
}
