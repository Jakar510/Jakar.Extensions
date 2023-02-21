// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


public interface ITableRecord
{
    public DateTimeOffset  DateCreated  { get; }
    public DateTimeOffset? LastModified { get; }
    public Guid            ID           { get; }
    public Guid?           CreatedBy    { get; }
}



[Serializable]
public abstract record TableRecord<TRecord> : ObservableRecord<TRecord>, ITableRecord, IUniqueID<Guid> where TRecord : TableRecord<TRecord>
{
    private DateTimeOffset? _lastModified;


    public static string         TableName   { get; } = typeof(TRecord).GetTableName();
    public        DateTimeOffset DateCreated { get; init; }
    public DateTimeOffset? LastModified
    {
        get => _lastModified;
        set => SetProperty( ref _lastModified, value );
    }
    public                          Guid? OwnerUserID { get; init; }
    [MaxLength( 256 )] [Key] public Guid  ID          { get; init; }
    [MaxLength( 256 )]       public Guid? CreatedBy   { get; init; }


    protected TableRecord() : base() { }
    protected TableRecord( Guid id, UserRecord? user = default )
    {
        ID          = id;
        DateCreated = DateTimeOffset.UtcNow;
        OwnerUserID = user?.OwnerUserID;
        CreatedBy   = user?.CreatedBy;
    }
    public static string GenerateID() => Guid.NewGuid()
                                             .ToBase64();


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


    public bool DoesNotOwn( UserRecord record ) => record.CreatedBy != record.ID;
    public override bool Equals( TRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return ID == other.ID && CreatedBy == other.CreatedBy && OwnerUserID.Equals( other.OwnerUserID ) && DateCreated.Equals( other.DateCreated );
    }


    public static DynamicParameters GetDynamicParameters( UserRecord record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(OwnerUserID), record.OwnerUserID );
        return parameters;
    }
    protected static DynamicParameters GetDynamicParameters( TableRecord<TRecord> record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(OwnerUserID), record.OwnerUserID );
        parameters.Add( nameof(CreatedBy),   record.CreatedBy );
        return parameters;
    }
    public override int GetHashCode() => HashCode.Combine( CreatedBy, LastModified, OwnerUserID, DateCreated );


    public async ValueTask<UserRecord?> GetUser( DbConnection           connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, true,      GetDynamicParameters( this ), token );
    public async ValueTask<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, CreatedBy, token );


    public TRecord NewID( Guid id ) => (TRecord)(this with
                                                 {
                                                     ID = id
                                                 });
    public bool Owns( UserRecord record ) => record.CreatedBy == record.ID;
}
