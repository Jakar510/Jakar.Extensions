namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record GroupRecord( [property: StringLength( GroupRecord.MAX_SIZE )] string? CustomerID,
                                  [property: StringLength( GroupRecord.MAX_SIZE )] string  NameOfGroup,
                                  string                                                   Rights,
                                  RecordID<GroupRecord>                                    ID,
                                  RecordID<UserRecord>?                                    CreatedBy,
                                  Guid?                                                    OwnerUserID,
                                  DateTimeOffset                                           DateCreated,
                                  DateTimeOffset?                                          LastModified = default ) : OwnedTableRecord<GroupRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<GroupRecord>, IGroupModel<Guid>
{
    public const  int                                    MAX_SIZE   = 1024;
    public const  string                                 TABLE_NAME = "Groups";
    public static string                                 TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }
    Guid? ICreatedByUser<Guid>.                          CreatedBy => CreatedBy?.Value;
    Guid? IGroupModel<Guid>.                             OwnerID   => OwnerUserID;
    [StringLength( IUserRights.MAX_SIZE )] public string Rights    { get; set; } = Rights;


    public GroupRecord( UserRecord owner, string nameOfGroup, string? customerID ) : this( customerID, nameOfGroup, string.Empty, RecordID<GroupRecord>.New(), owner?.ID, owner?.UserID, DateTimeOffset.UtcNow ) { }
    public GroupRecord( UserRecord owner, string nameOfGroup, string? customerID, IUserRights rights ) : this( customerID, nameOfGroup, rights.ToString(), RecordID<GroupRecord>.New(), owner?.ID, owner?.UserID, DateTimeOffset.UtcNow ) { }
    public GroupModel<Guid> ToGroupModel() => new(this);
    public TGroupModel ToGroupModel<TGroupModel>()
        where TGroupModel : IGroupModel<TGroupModel, Guid> => TGroupModel.Create( this );


    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(CustomerID),  CustomerID );
        parameters.Add( nameof(NameOfGroup), NameOfGroup );
        parameters.Add( nameof(OwnerUserID), OwnerUserID );
        parameters.Add( nameof(Rights),      Rights );
        return parameters;
    }
    [Pure]
    public static GroupRecord Create( DbDataReader reader )
    {
        string                customerID   = reader.GetFieldValue<string>( nameof(CustomerID) );
        string                nameOfGroup  = reader.GetFieldValue<string>( nameof(NameOfGroup) );
        string                rights       = reader.GetFieldValue<string>( nameof(Rights) );
        var                   dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var                   lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var                   ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>? createdBy    = RecordID<UserRecord>.CreatedBy( reader );
        RecordID<GroupRecord> id           = RecordID<GroupRecord>.ID( reader );
        var                   record       = new GroupRecord( customerID, nameOfGroup, rights, id, createdBy, ownerUserID, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [Pure]
    public static async IAsyncEnumerable<GroupRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }

    [Pure] public async ValueTask<UserRecord?>       GetOwner( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, RecordID<UserRecord>.TryCreate( OwnerUserID ), token );
    [Pure] public       IAsyncEnumerable<UserRecord> GetUsers( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => UserGroupRecord.Where( connection, transaction, db.Users, this, token );
}
