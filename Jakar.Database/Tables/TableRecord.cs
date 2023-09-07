// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


public interface IRecordPair : IUniqueID<Guid> // where TID : IComparable<TID>, IEquatable<TID>
{
    public DateTimeOffset DateCreated { get; }
}



public interface ITableRecord : IRecordPair
{
    public RecordID<UserRecord>? CreatedBy    { get; }
    public DateTimeOffset?       LastModified { get; }
    public Guid?                 OwnerUserID  { get; }
}



public interface IDbReaderMapping<out T> where T : TableRecord<T>
{
    public abstract static T Create( DbDataReader                        reader );
    public abstract static IAsyncEnumerable<T> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default );
}



[Serializable]
public abstract record TableRecord<TRecord> : ObservableRecord<TRecord>, ITableRecord where TRecord : TableRecord<TRecord>
{
    private DateTimeOffset? _lastModified;


    public static string                TableName   { get; } = typeof(TRecord).GetTableName();
    public        RecordID<UserRecord>? CreatedBy   { get; init; }
    public        DateTimeOffset        DateCreated { get; init; }
    [Key] public  RecordID<TRecord>     ID          { get; init; }
    public DateTimeOffset? LastModified
    {
        get => _lastModified;
        set => SetProperty( ref _lastModified, value );
    }
    public Guid?         OwnerUserID { get; init; }
    Guid IUniqueID<Guid>.ID          => ID.Value;


    protected TableRecord() : base() { }
    protected TableRecord( UserRecord? owner ) : this( RecordID<TRecord>.New(), owner ) { }
    protected TableRecord( RecordID<TRecord> id, UserRecord? owner = default )
    {
        ID          = id;
        CreatedBy   = owner?.ID;
        OwnerUserID = owner?.UserID;
        DateCreated = DateTimeOffset.UtcNow;
    }


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


    public override int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int lastModifiedComparison = Nullable.Compare( _lastModified, other._lastModified );
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        int createdByComparison = Nullable.Compare( CreatedBy, other.CreatedBy );
        if ( createdByComparison != 0 ) { return createdByComparison; }

        int userIDComparison = Nullable.Compare( OwnerUserID, other.OwnerUserID );
        if ( userIDComparison != 0 ) { return userIDComparison; }

        return DateCreated.CompareTo( other.DateCreated );
    }
    public override bool Equals( TRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return ID == other.ID && CreatedBy == other.CreatedBy && OwnerUserID.Equals( other.OwnerUserID ) && DateCreated.Equals( other.DateCreated );
    }
    public override int GetHashCode() => HashCode.Combine( CreatedBy, LastModified, OwnerUserID, DateCreated );
}
