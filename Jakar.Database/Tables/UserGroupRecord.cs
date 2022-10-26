// TrueLogic :: TrueLogic.Common.Hosting
// 10/25/2022  9:19 AM

namespace Jakar.Database;


[Serializable]
[Table( "UserGroups" )]
public record UserGroupRecord : TableRecord<UserGroupRecord>
{
    public long GroupID { get; init; }


    public UserGroupRecord() { }
    public UserGroupRecord( UserRecord user, GroupRecord group ) : base( user ) => GroupID = group.ID;


    public static DynamicParameters GetDynamicParameters( GroupRecord record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(GroupID), record.ID );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( UserRecord user, GroupRecord record )
    {
        DynamicParameters parameters = GetDynamicParameters(user);
        parameters.Add( nameof(GroupID), record.ID );
        return parameters;
    }
}
