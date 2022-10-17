// Jakar.Extensions :: Jakar.Database
// 10/10/2022  4:57 PM

using Jakar.Database.Implementations;



namespace Jakar.Database;


[Serializable]
[Table( "UserRoles" )]
public sealed record UserRoleRecord : TableRecord<UserRoleRecord>
{
    public long RoleID { get; init; }


    public UserRoleRecord( UserRecord user, RoleRecord role )
    {
        UserID      = user.UserID;
        RoleID      = role.ID;
        DateCreated = DateTimeOffset.UtcNow;
    }


    public override int CompareTo( UserRoleRecord? other )
    {
        if (ReferenceEquals( this, other )) { return 0; }

        if (other is null) { return 1; }

        int userIDComparison = UserID.CompareTo( other.UserID );
        if (userIDComparison != 0) { return userIDComparison; }

        return RoleID.CompareTo( other.RoleID );
    }


    public static DynamicParameters GetDynamicParameters( UserRecord user )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserID), user.UserID );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( RoleRecord role )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(RoleID), role.ID );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( UserRecord user, RoleRecord role )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserID), user.UserID );
        parameters.Add( nameof(RoleID), role.ID );
        return parameters;
    }
    public override int GetHashCode() => HashCode.Combine( base.GetHashCode(), UserID, CreatedBy, RoleID );


    public async ValueTask<RoleRecord?> GetRole( DbConnection connection, DbTransaction? transaction, MsSqlDbTable<RoleRecord> table, CancellationToken token ) => await table.Get( connection, transaction, RoleID, token );
    public override bool Equals( UserRoleRecord? other )
    {
        if (other is null) { return false; }

        if (ReferenceEquals( this, other )) { return true; }

        return base.Equals( other ) && RoleID == other.RoleID;
    }
}
