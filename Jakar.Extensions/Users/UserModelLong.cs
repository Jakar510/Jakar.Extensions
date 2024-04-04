// Jakar.Extensions :: Jakar.Extensions
// 4/4/2024  13:50


// ReSharper disable once CheckNamespace

namespace Jakar.Extensions.UserLong;


[Serializable]
public sealed record UserAddress :
#if NET8_0_OR_GREATER
    UserAddress<UserAddress, long>,
    IAddress<UserAddress, long>
#else
    UserAddress<UserAddress, long>
#endif
{
    public UserAddress() { }
    public UserAddress( IAddress<long>               address ) : base( address ) { }
    public static UserAddress Create( IAddress<long> address ) => new(address);
}



[Serializable]
public sealed record GroupModel :
#if NET8_0_OR_GREATER
    GroupModel<GroupModel, long>,
    IGroupModel<GroupModel, long>
#else
    GroupModel<GroupModel, long>
#endif
{
    public GroupModel( string                          NameOfGroup, long? OwnerID, long? CreatedBy, long ID, string Rights ) : base( NameOfGroup, OwnerID, CreatedBy, ID, Rights ) { }
    public GroupModel( IGroupModel<long>               model ) : base( model ) { }
    public static GroupModel Create( IGroupModel<long> model ) => new(model);
}



[Serializable]
public sealed record RoleModel :
#if NET8_0_OR_GREATER
    RoleModel<RoleModel, long>,
    IRoleModel<RoleModel, long>
#else
    RoleModel<RoleModel, long>
#endif
{
    public RoleModel( string                         NameOfRole, string Rights, long ID ) : base( NameOfRole, Rights, ID ) { }
    public RoleModel( IRoleModel<long>               model ) : base( model ) { }
    public static RoleModel Create( IRoleModel<long> model ) => new(model);
}



[Serializable]
public sealed class UserModel :
#if NET8_0_OR_GREATER
    UserModel<UserModel, long, UserAddress, GroupModel, RoleModel>,
    ICreateUserModel<UserModel, long, UserAddress, GroupModel, RoleModel>
#else
    UserModel<UserModel, long, UserAddress, GroupModel, RoleModel>
#endif
{
    public UserModel() : base() { }
    public UserModel( IUserData<long> value ) : base( value ) { }
    public UserModel( string          firstName, string lastName ) : base( firstName, lastName ) { }


    public static UserModel Create( IUserData<long> model )                                                                                                                                    => new(model);
    public static UserModel Create( IUserData<long> model, IEnumerable<UserAddress>            addresses, IEnumerable<GroupModel>            groups, IEnumerable<RoleModel>            roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static UserModel Create( IUserData<long> model, scoped in ReadOnlySpan<UserAddress> addresses, scoped in ReadOnlySpan<GroupModel> groups, scoped in ReadOnlySpan<RoleModel> roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static async ValueTask<UserModel> CreateAsync( IUserData<long> model, IAsyncEnumerable<UserAddress> addresses, IAsyncEnumerable<GroupModel> groups, IAsyncEnumerable<RoleModel> roles, CancellationToken token = default )
    {
        UserModel user = Create( model );
        await user.Addresses.Add( addresses, token );
        await user.Groups.Add( groups, token );
        await user.Roles.Add( roles, token );
        return user;
    }
}
