// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


[Serializable]
public abstract record TableRecord<TRecord> : BaseCollectionsRecord<TRecord, long> where TRecord : TableRecord<TRecord>
{
    private       DateTimeOffset? _lastModified;
    public static string          TableName { get; } = typeof(TRecord).GetTableName();


    public DateTimeOffset DateCreated { get; init; }
    public DateTimeOffset? LastModified
    {
        get => _lastModified;
        set => SetProperty( ref _lastModified, value );
    }
    public Guid UserID    { get; init; }
    public long CreatedBy { get; init; }


    protected TableRecord() : base() { }
    protected TableRecord( long id ) : base( id ) => DateCreated = DateTimeOffset.Now;
    protected TableRecord( UserRecord user )
    {
        DateCreated = DateTimeOffset.Now;
        UserID      = user.UserID;
        CreatedBy   = user.ID;
    }


    public static DynamicParameters GetDynamicParameters( UserRecord record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserID), record.UserID );
        return parameters;
    }
    protected static DynamicParameters GetDynamicParameters( TableRecord<TRecord> record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserID), record.UserID );
        return parameters;
    }
    public override bool Equals( TRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return base.Equals( other ) && CreatedBy == other.CreatedBy && Nullable.Equals( _lastModified, other._lastModified ) && UserID.Equals( other.UserID ) && DateCreated.Equals( other.DateCreated );
    }


    public override int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int createdByComparison = CreatedBy.CompareTo( other.CreatedBy );
        if ( createdByComparison != 0 ) { return createdByComparison; }

        int userIDComparison = UserID.CompareTo( other.UserID );
        if ( userIDComparison != 0 ) { return userIDComparison; }

        int lastModifiedComparison = Nullable.Compare( _lastModified, other._lastModified );
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        return DateCreated.CompareTo( other.DateCreated );
    }


    public override int GetHashCode() => HashCode.Combine( CreatedBy, LastModified, UserID, DateCreated );


    public TRecord NewID( in long id ) => (TRecord)(this with
                                                    {
                                                        ID = id,
                                                    });


    public async ValueTask<UserRecord?> GetUser( DbConnection           connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, true,      GetDynamicParameters( this ), token );
    public async ValueTask<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, CreatedBy, token );
}
