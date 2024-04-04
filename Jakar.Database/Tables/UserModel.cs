// Jakar.Extensions :: Jakar.Database
// 4/4/2024  10:1

namespace Jakar.Database;


// [Serializable] public sealed record UserAddress : UserAddress<Guid> { }



[Serializable]
public sealed record GroupModel : GroupModel<GroupModel, Guid>, IGroupModel<GroupModel, Guid>
{
    public GroupModel( string                          NameOfGroup, Guid? OwnerID, Guid? CreatedBy, Guid ID, string Rights ) : base( NameOfGroup, OwnerID, CreatedBy, ID, Rights ) { }
    public GroupModel( IGroupModel<Guid>               model ) : base( model ) { }
    public static GroupModel Create( IGroupModel<Guid> model ) => new(model);
}



[Serializable]
public sealed record RoleModel : RoleModel<RoleModel, Guid>, IRoleModel<RoleModel, Guid>
{
    public RoleModel( string                         NameOfRole, string Rights, Guid ID ) : base( NameOfRole, Rights, ID ) { }
    public RoleModel( IRoleModel<Guid>               model ) : base( model ) { }
    public static RoleModel Create( IRoleModel<Guid> model ) => new(model);
}



[Serializable]
public sealed class UserModel : UserModel<UserModel, Guid, UserAddress<Guid>, GroupModel, RoleModel>, ICreateUserModel<UserModel, Guid, UserAddress<Guid>, GroupModel, RoleModel>
{
    public UserModel() : base() { }
    public UserModel( IUserData<Guid>               value ) : base( value ) { }
    public UserModel( string                        firstName, string lastName ) : base( firstName, lastName ) { }
    public static UserModel Create( IUserData<Guid> model ) => new(model);
    public static UserModel Create( IUserData<Guid> model, IEnumerable<UserAddress<Guid>> addresses, IEnumerable<GroupModel> groups, IEnumerable<RoleModel> roles )
    {
        UserModel user = Create( model );
        user.WithAddresses( addresses );
        user.Groups.Add( groups );
        user.Roles.Add( roles );
        return user;
    }
    public static async ValueTask<UserModel> CreateAsync( IUserData<Guid> model, IAsyncEnumerable<UserAddress<Guid>> addresses, IAsyncEnumerable<GroupModel> groups, IAsyncEnumerable<RoleModel> roles, CancellationToken token = default )
    {
        UserModel user = Create( model );
        await user.Addresses.Add( addresses, token );
        await user.Groups.Add( groups, token );
        await user.Roles.Add( roles, token );
        return user;
    }
}
