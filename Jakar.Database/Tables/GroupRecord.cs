namespace Jakar.Database;


[Serializable]
[Table( "Groups" )]
public sealed record GroupRecord : TableRecord<GroupRecord>
{
    private string? _ownerID;
    private string? _customerID;
    private string  _nameOfGroup = string.Empty;


    [MaxLength( 256 )]
    public string? OwnerID
    {
        get => _ownerID;
        init => SetProperty( ref _ownerID, value );
    }


    [MaxLength( 256 )]
    public string? CustomerID
    {
        get => _customerID;
        init => SetProperty( ref _customerID, value );
    }


    [MaxLength( 1024 )]
    public string NameOfGroup
    {
        get => _nameOfGroup;
        set => SetProperty( ref _nameOfGroup, value );
    }


    public GroupRecord() { }
    public GroupRecord( UserRecord owner, string nameOfGroup, string? customerID, UserRecord caller ) : base( caller )
    {
        CreatedBy   = caller.ID;
        OwnerID     = owner.ID;
        NameOfGroup = nameOfGroup;
        CustomerID  = customerID;
    }
    public override int GetHashCode() => HashCode.Combine( base.GetHashCode(), NameOfGroup, OwnerID, CustomerID );


    public async Task<UserRecord?> GetOwner( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) =>
        !string.IsNullOrEmpty( OwnerID )
            ? await db.Users.Get( connection, transaction, OwnerID, token )
            : default;

    public async ValueTask<UserRecord[]> GetUsers( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) =>
        await UserGroupRecord.Where( connection, transaction, db.UserGroups, db.Users, this, token );


    public override bool Equals( GroupRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return base.Equals( other ) && _nameOfGroup == other._nameOfGroup && _ownerID == other._ownerID && _customerID == other._customerID;
    }
}
