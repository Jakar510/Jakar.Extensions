// Jakar.Extensions :: Jakar.Extensions.Serilog
// 07/08/2024  09:07

namespace Jakar.Extensions.Serilog;


[SuppressMessage( "ReSharper", "MemberHidesStaticFromOuterClass" )]
public class TelemetryTags
{
    public string AddGroup               { get; set; } = nameof(AddGroup);
    public string AddGroupRights         { get; set; } = nameof(AddGroupRights);
    public string AddRole                { get; set; } = nameof(AddRole);
    public string AddRoleRights          { get; set; } = nameof(AddRoleRights);
    public string AddUser                { get; set; } = nameof(AddUser);
    public string AddUserAddress         { get; set; } = nameof(AddUserAddress);
    public string AddUserLoginInfo       { get; set; } = nameof(AddUserLoginInfo);
    public string AddUserRecoveryCode    { get; set; } = nameof(AddUserRecoveryCode);
    public string AddUserRights          { get; set; } = nameof(AddUserRights);
    public string AddUserSubscription    { get; set; } = nameof(AddUserSubscription);
    public string AddUserToGroup         { get; set; } = nameof(AddUserToGroup);
    public string AddUserToRole          { get; set; } = nameof(AddUserToRole);
    public string ConnectDatabase        { get; set; } = nameof(ConnectDatabase);
    public string GroupID                { get; set; } = nameof(GroupID);
    public string LoginUser              { get; set; } = nameof(LoginUser);
    public string RemoveGroup            { get; set; } = nameof(RemoveGroup);
    public string RemoveGroupRights      { get; set; } = nameof(RemoveGroupRights);
    public string RemoveRole             { get; set; } = nameof(RemoveRole);
    public string RemoveRoleRights       { get; set; } = nameof(RemoveRoleRights);
    public string RemoveUser             { get; set; } = nameof(RemoveUser);
    public string RemoveUserAddress      { get; set; } = nameof(RemoveUserAddress);
    public string RemoveUserFromGroup    { get; set; } = nameof(RemoveUserFromGroup);
    public string RemoveUserFromRole     { get; set; } = nameof(RemoveUserFromRole);
    public string RemoveUserLoginInfo    { get; set; } = nameof(RemoveUserLoginInfo);
    public string RemoveUserRecoveryCode { get; set; } = nameof(RemoveUserRecoveryCode);
    public string RemoveUserRights       { get; set; } = nameof(RemoveUserRights);
    public string RemoveUserSubscription { get; set; } = nameof(RemoveUserSubscription);
    public string RoleID                 { get; set; } = nameof(RoleID);
    public string SessionID              { get; set; } = nameof(SessionID);
    public string UpdateGroup            { get; set; } = nameof(UpdateGroup);
    public string UpdateRole             { get; set; } = nameof(UpdateRole);
    public string UpdateUser             { get; set; } = nameof(UpdateUser);
    public string UpdateUserAddress      { get; set; } = nameof(UpdateUserAddress);
    public string UpdateUserLoginInfo    { get; set; } = nameof(UpdateUserLoginInfo);
    public string UpdateUserSubscription { get; set; } = nameof(UpdateUserSubscription);
    public string VerifyLogin            { get; set; } = nameof(VerifyLogin);


    public void ClearPrefix()
    {
        AddGroup               = nameof(AddGroup);
        AddGroupRights         = nameof(AddGroupRights);
        AddRole                = nameof(AddRole);
        AddRoleRights          = nameof(AddRoleRights);
        AddUser                = nameof(AddUser);
        AddUserAddress         = nameof(AddUserAddress);
        AddUserLoginInfo       = nameof(AddUserLoginInfo);
        AddUserRecoveryCode    = nameof(AddUserRecoveryCode);
        AddUserRights          = nameof(AddUserRights);
        AddUserSubscription    = nameof(AddUserSubscription);
        AddUserToGroup         = nameof(AddUserToGroup);
        AddUserToRole          = nameof(AddUserToRole);
        ConnectDatabase        = nameof(ConnectDatabase);
        GroupID                = nameof(GroupID);
        LoginUser              = nameof(LoginUser);
        RemoveGroup            = nameof(RemoveGroup);
        RemoveGroupRights      = nameof(RemoveGroupRights);
        RemoveRole             = nameof(RemoveRole);
        RemoveRoleRights       = nameof(RemoveRoleRights);
        RemoveUser             = nameof(RemoveUser);
        RemoveUserAddress      = nameof(RemoveUserAddress);
        RemoveUserFromGroup    = nameof(RemoveUserFromGroup);
        RemoveUserFromRole     = nameof(RemoveUserFromRole);
        RemoveUserLoginInfo    = nameof(RemoveUserLoginInfo);
        RemoveUserRecoveryCode = nameof(RemoveUserRecoveryCode);
        RemoveUserRights       = nameof(RemoveUserRights);
        RemoveUserSubscription = nameof(RemoveUserSubscription);
        RoleID                 = nameof(RoleID);
        SessionID              = nameof(SessionID);
        UpdateGroup            = nameof(UpdateGroup);
        UpdateRole             = nameof(UpdateRole);
        UpdateUser             = nameof(UpdateUser);
        UpdateUserAddress      = nameof(UpdateUserAddress);
        UpdateUserLoginInfo    = nameof(UpdateUserLoginInfo);
        UpdateUserSubscription = nameof(UpdateUserSubscription);
        VerifyLogin            = nameof(VerifyLogin);
    }
    public void AddPrefix( scoped in ReadOnlySpan<char> prefix )
    {
        ConnectDatabase        = GetPrefix( prefix, ConnectDatabase,        nameof(ConnectDatabase) );
        AddUser                = GetPrefix( prefix, AddUser,                nameof(AddUser) );
        UpdateUser             = GetPrefix( prefix, UpdateUser,             nameof(UpdateUser) );
        RemoveUser             = GetPrefix( prefix, RemoveUser,             nameof(RemoveUser) );
        AddUserLoginInfo       = GetPrefix( prefix, AddUserLoginInfo,       nameof(AddUserLoginInfo) );
        UpdateUserLoginInfo    = GetPrefix( prefix, UpdateUserLoginInfo,    nameof(UpdateUserLoginInfo) );
        RemoveUserLoginInfo    = GetPrefix( prefix, RemoveUserLoginInfo,    nameof(RemoveUserLoginInfo) );
        AddUserAddress         = GetPrefix( prefix, AddUserAddress,         nameof(AddUserAddress) );
        UpdateUserAddress      = GetPrefix( prefix, UpdateUserAddress,      nameof(UpdateUserAddress) );
        RemoveUserAddress      = GetPrefix( prefix, RemoveUserAddress,      nameof(RemoveUserAddress) );
        AddUserSubscription    = GetPrefix( prefix, AddUserSubscription,    nameof(AddUserSubscription) );
        UpdateUserSubscription = GetPrefix( prefix, UpdateUserSubscription, nameof(UpdateUserSubscription) );
        RemoveUserSubscription = GetPrefix( prefix, RemoveUserSubscription, nameof(RemoveUserSubscription) );
        AddUserRecoveryCode    = GetPrefix( prefix, AddUserRecoveryCode,    nameof(AddUserRecoveryCode) );
        RemoveUserRecoveryCode = GetPrefix( prefix, RemoveUserRecoveryCode, nameof(RemoveUserRecoveryCode) );
        AddUserRights          = GetPrefix( prefix, AddUserRights,          nameof(AddUserRights) );
        RemoveUserRights       = GetPrefix( prefix, RemoveUserRights,       nameof(RemoveUserRights) );
        LoginUser              = GetPrefix( prefix, LoginUser,              nameof(LoginUser) );
        VerifyLogin            = GetPrefix( prefix, VerifyLogin,            nameof(VerifyLogin) );
        AddGroup               = GetPrefix( prefix, AddGroup,               nameof(AddGroup) );
        RemoveGroup            = GetPrefix( prefix, RemoveGroup,            nameof(RemoveGroup) );
        UpdateGroup            = GetPrefix( prefix, UpdateGroup,            nameof(UpdateGroup) );
        AddGroupRights         = GetPrefix( prefix, AddGroupRights,         nameof(AddGroupRights) );
        RemoveGroupRights      = GetPrefix( prefix, RemoveGroupRights,      nameof(RemoveGroupRights) );
        AddUserToGroup         = GetPrefix( prefix, AddUserToGroup,         nameof(AddUserToGroup) );
        RemoveUserFromGroup    = GetPrefix( prefix, RemoveUserFromGroup,    nameof(RemoveUserFromGroup) );
        AddRole                = GetPrefix( prefix, AddRole,                nameof(AddRole) );
        RemoveRole             = GetPrefix( prefix, RemoveRole,             nameof(RemoveRole) );
        UpdateRole             = GetPrefix( prefix, UpdateRole,             nameof(UpdateRole) );
        AddRoleRights          = GetPrefix( prefix, AddRoleRights,          nameof(AddRoleRights) );
        RemoveRoleRights       = GetPrefix( prefix, RemoveRoleRights,       nameof(RemoveRoleRights) );
        AddUserToRole          = GetPrefix( prefix, AddUserToRole,          nameof(AddUserToRole) );
        RemoveUserFromRole     = GetPrefix( prefix, RemoveUserFromRole,     nameof(RemoveUserFromRole) );
    }


    private string GetPrefix( scoped in ReadOnlySpan<char> prefix, in string tag, in string defaultTag ) => prefix.IsEmpty
                                                                                                                ? tag.Length == 0
                                                                                                                      ? defaultTag
                                                                                                                      : tag
                                                                                                                : tag.Length == 0
                                                                                                                    ? GetResult( prefix, tag )
                                                                                                                    : GetResult( prefix, defaultTag );
    private string GetResult( scoped in ReadOnlySpan<char> prefix, scoped in ReadOnlySpan<char> tag ) => $"{prefix}.{tag}";
}
