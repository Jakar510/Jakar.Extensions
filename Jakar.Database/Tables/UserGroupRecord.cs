// TrueLogic :: TrueLogic.Common.Hosting
// 10/25/2022  9:19 AM

namespace Jakar.Database;


[Serializable]
[Table( "UserGroups" )]
public record UserGroupRecord : TableRecord<UserGroupRecord>
{
    public long GroupID { get; init; }


    public UserGroupRecord() { }
    public UserGroupRecord( UserRecord user, GroupRecord group )
    {
        UserID  = user.UserID;
        GroupID = group.ID;
    }


    public static DynamicParameters GetDynamicParameters( UserRecord user )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserID), user.UserID );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( GroupRecord record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(GroupID), record.ID );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( UserRecord user, GroupRecord record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserID),  user.UserID );
        parameters.Add( nameof(GroupID), record.ID );
        return parameters;
    }
}
