// Jakar.Extensions :: Jakar.Database
// 4/4/2024  10:1

// ReSharper disable once CheckNamespace

namespace Jakar.Extensions.UserGuid;


[Serializable]
public sealed record UserAddress :
#if NET8_0_OR_GREATER
    UserAddress<UserAddress, Guid>,
    IAddress<UserAddress, Guid>
#else
    UserAddress<UserAddress, Guid>
#endif
{
    public UserAddress() { }
    public UserAddress( IAddress<Guid>               address ) : base( address ) { }
    public static UserAddress Create( IAddress<Guid> address ) => new(address);
}



[Serializable]
public sealed record GroupModel :
#if NET8_0_OR_GREATER
    GroupModel<GroupModel, Guid>,
    IGroupModel<GroupModel, Guid>
#else
    GroupModel<GroupModel, Guid>
#endif
{
    public GroupModel( string                          NameOfGroup, Guid? OwnerID, Guid? CreatedBy, Guid ID, string Rights ) : base( NameOfGroup, OwnerID, CreatedBy, ID, Rights ) { }
    public GroupModel( IGroupModel<Guid>               model ) : base( model ) { }
    public static GroupModel Create( IGroupModel<Guid> model ) => new(model);
}



[Serializable]
public sealed record RoleModel :
#if NET8_0_OR_GREATER
    RoleModel<RoleModel, Guid>,
    IRoleModel<RoleModel, Guid>
#else
    RoleModel<RoleModel, Guid>
#endif
{
    public RoleModel( string                         NameOfRole, string Rights, Guid ID ) : base( NameOfRole, Rights, ID ) { }
    public RoleModel( IRoleModel<Guid>               model ) : base( model ) { }
    public static RoleModel Create( IRoleModel<Guid> model ) => new(model);
}



[Serializable]
public sealed class UserModel :
#if NET8_0_OR_GREATER
    UserModel<UserModel, Guid, UserAddress, GroupModel, RoleModel>,
    ICreateUserModel<UserModel, Guid, UserAddress, GroupModel, RoleModel>
#else
    UserModel<UserModel, Guid, UserAddress, GroupModel, RoleModel>
#endif
{
    public UserModel() : base() { }
    public UserModel( IUserData<Guid> value ) : base( value ) { }
    public UserModel( string          firstName, string lastName ) : base( firstName, lastName ) { }


    public static UserModel Create( IUserData<Guid> model )                                                                                                                                    => new(model);
    public static UserModel Create( IUserData<Guid> model, IEnumerable<UserAddress>            addresses, IEnumerable<GroupModel>            groups, IEnumerable<RoleModel>            roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static UserModel Create( IUserData<Guid> model, scoped in ReadOnlySpan<UserAddress> addresses, scoped in ReadOnlySpan<GroupModel> groups, scoped in ReadOnlySpan<RoleModel> roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static async ValueTask<UserModel> CreateAsync( IUserData<Guid> model, IAsyncEnumerable<UserAddress> addresses, IAsyncEnumerable<GroupModel> groups, IAsyncEnumerable<RoleModel> roles, CancellationToken token = default )
    {
        UserModel user = Create( model );
        await user.Addresses.Add( addresses, token );
        await user.Groups.Add( groups, token );
        await user.Roles.Add( roles, token );
        return user;
    }
}
