// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


public interface ITableRecord
{
    public DateTimeOffset  DateCreated  { get; }
    public DateTimeOffset? LastModified { get; }
    public string          ID           { get; }
    public string?         CreatedBy    { get; }
}



[Serializable]
public abstract record TableRecord<TRecord> : ObservableRecord<TRecord>, ITableRecord, IUniqueID<string> where TRecord : TableRecord<TRecord>
{
    private DateTimeOffset? _lastModified;


    public static string         TableName   { get; } = typeof(TRecord).GetTableName();
    public        DateTimeOffset DateCreated { get; init; }
    public DateTimeOffset? LastModified
    {
        get => _lastModified;
        set => SetProperty( ref _lastModified, value );
    }
    public                          Guid    UserID    { get; init; }
    [MaxLength( 256 )] [Key] public string  ID        { get; init; } = string.Empty;
    [MaxLength( 256 )]       public string? CreatedBy { get; init; }


    protected TableRecord() : base() { }
    protected TableRecord( string id ) : base()
    {
        ID          = Validate.ThrowIfNull( id, nameof(id) );
        DateCreated = DateTimeOffset.UtcNow;
    }
    protected TableRecord( Guid       id ) : this( id.ToBase64() ) { }
    protected TableRecord( UserRecord user ) : this( Guid.NewGuid(), user ) { }
    protected TableRecord( Guid       id, UserRecord user ) : this( id.ToBase64(), user ) { }
    protected TableRecord( string id, UserRecord user ) : this( id )
    {
        UserID    = user.UserID;
        CreatedBy = user.ID;
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


    public bool DoesNotOwn( UserRecord record ) => record.CreatedBy != record.ID;
    public override bool Equals( TRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( ID, other.ID, StringComparison.Ordinal ) && CreatedBy == other.CreatedBy && UserID.Equals( other.UserID ) && DateCreated.Equals( other.DateCreated );
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
        parameters.Add( nameof(UserID),    record.UserID );
        parameters.Add( nameof(CreatedBy), record.CreatedBy );
        return parameters;
    }
    public override int GetHashCode() => HashCode.Combine( CreatedBy, LastModified, UserID, DateCreated );


    public async ValueTask<UserRecord?> GetUser( DbConnection           connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, true,      GetDynamicParameters( this ), token );
    public async ValueTask<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, CreatedBy, token );


    public TRecord NewID( Guid id ) => NewID( id.ToBase64() );
    public TRecord NewID( string id ) => (TRecord)(this with
                                                   {
                                                       ID = id
                                                   });
    public bool Owns( UserRecord record ) => record.CreatedBy == record.ID;
}
