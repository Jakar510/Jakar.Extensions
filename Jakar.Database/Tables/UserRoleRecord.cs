// Jakar.Extensions :: Jakar.Database
// 10/10/2022  4:57 PM

namespace Jakar.Database;


[Serializable]
[Table( "UserRoles" )]
public sealed record UserRoleRecord : TableRecord<UserRoleRecord>
{
    public string RoleID { get; init; } = string.Empty;


    public UserRoleRecord() { }
    public UserRoleRecord( UserRecord user, RoleRecord role ) : base( user ) => RoleID = role.ID;


    public static DynamicParameters GetDynamicParameters( RoleRecord record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(RoleID), record.ID );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( UserRecord user, RoleRecord record )
    {
        DynamicParameters parameters = GetDynamicParameters( user );
        parameters.Add( nameof(RoleID), record.ID );
        return parameters;
    }


    public override int CompareTo( UserRoleRecord? other )
    {
        if ( ReferenceEquals( this, other ) ) { return 0; }

        if ( other is null ) { return 1; }

        int userIDComparison = UserID.CompareTo( other.UserID );
        if ( userIDComparison != 0 ) { return userIDComparison; }

        return string.Compare( RoleID, other.RoleID, StringComparison.Ordinal );
    }
    public override int GetHashCode() => HashCode.Combine( base.GetHashCode(), UserID, CreatedBy, RoleID );


    public async ValueTask<RoleRecord?> GetRole( DbConnection connection, DbTransaction? transaction, DbTable<RoleRecord> table, CancellationToken token ) => await table.Get( connection, transaction, RoleID, token );
    public override bool Equals( UserRoleRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return base.Equals( other ) && RoleID == other.RoleID;
    }
}
