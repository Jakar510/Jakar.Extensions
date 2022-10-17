// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

using Jakar.Database.Implementations;



namespace Jakar.Database;


[Serializable]
public abstract record TableRecord<TRecord> : BaseCollectionsRecord<TRecord, long> where TRecord : TableRecord<TRecord>
{
    private       DateTimeOffset? _lastModified;
    private       long            _createdBy;
    public static string          TableName { get; } = typeof(TRecord).GetTableName();


    public DateTimeOffset DateCreated { get; init; }
    public DateTimeOffset? LastModified
    {
        get => _lastModified;
        set => SetProperty( ref _lastModified, value );
    }

    public Guid UserID { get; init; }
    public long CreatedBy
    {
        get => _createdBy;
        set => SetProperty( ref _createdBy, value );
    }


    protected TableRecord() { }
    protected TableRecord( UserRecord user )
    {
        UserID      = user.UserID;
        CreatedBy   = user.ID;
        DateCreated = DateTimeOffset.Now;
    }
    protected TableRecord( long id ) : base( id ) => DateCreated = DateTimeOffset.Now;


    public override int CompareTo( TRecord? other )
    {
        if (other is null) { return 1; }

        if (ReferenceEquals( this, other )) { return 0; }

        int createdByComparison = _createdBy.CompareTo( other._createdBy );
        if (createdByComparison != 0) { return createdByComparison; }

        int userIDComparison = UserID.CompareTo( other.UserID );
        if (userIDComparison != 0) { return userIDComparison; }

        int lastModifiedComparison = Nullable.Compare( _lastModified, other._lastModified );
        if (lastModifiedComparison != 0) { return lastModifiedComparison; }

        return DateCreated.CompareTo( other.DateCreated );
    }
    public override bool Equals( TRecord? other )
    {
        if (other is null) { return false; }

        if (ReferenceEquals( this, other )) { return true; }

        return base.Equals( other ) && _createdBy == other._createdBy && Nullable.Equals( _lastModified, other._lastModified ) && UserID.Equals( other.UserID ) && DateCreated.Equals( other.DateCreated );
    }


    public static DynamicParameters GetDynamicParameters( TableRecord<TRecord> tableRecord )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserID), tableRecord.UserID );
        return parameters;
    }


    public override int GetHashCode() => HashCode.Combine( base.GetHashCode(), _createdBy, _lastModified, UserID, DateCreated );


    public async ValueTask<UserRecord?> GetUser( DbConnection connection, DbTransaction? transaction, DbTableBase<UserRecord> table, CancellationToken token ) => await table.Get( connection, transaction, true, GetDynamicParameters( this ), token );
    public async ValueTask<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, DbTableBase<UserRecord> table, CancellationToken token ) => await table.Get( connection, transaction, CreatedBy, token );


    public TRecord NewID( in long id ) => (TRecord)(this with
                                                    {
                                                        ID = id
                                                    });
}
