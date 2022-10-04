// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


[Serializable]
public abstract record TableRecord<TRecord> : BaseCollectionsRecord<TRecord, long> where TRecord : TableRecord<TRecord>
{
    public static string TableName { get; } = typeof(TRecord).GetTableName();


    private long            _createdBy;
    private DateTimeOffset? _lastModified;

    public Guid UserID { get; init; }
    public long CreatedBy
    {
        get => _createdBy;
        set => SetProperty(ref _createdBy, value);
    }


    public DateTimeOffset DateCreated { get; init; }
    public DateTimeOffset? LastModified
    {
        get => _lastModified;
        set => SetProperty(ref _lastModified, value);
    }


    protected TableRecord() { }
    protected TableRecord( UserRecord user )
    {
        UserID      = user.UserID;
        CreatedBy   = user.ID;
        DateCreated = DateTimeOffset.Now;
    }
    protected TableRecord( long id ) : base(id) => DateCreated = DateTimeOffset.Now;


    public TRecord NewID( in long id ) => (TRecord)( this with
                                                     {
                                                         ID = id
                                                     } );

    protected virtual void VerifyAccess() { }


    public async Task<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, DbTable<UserRecord> table, CancellationToken token ) => await table.Get(connection, transaction, CreatedBy, token);


    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(UserID);
        hashCode.Add(CreatedBy);
        return hashCode.ToHashCode();
    }
}
