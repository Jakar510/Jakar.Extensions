// Jakar.Extensions :: Experiments
// 10/07/2026  16:03

using Jakar.Json;



namespace Experiments;


[Serializable]
[JsonModel]
public sealed partial class SharedUserModel : UserModel<SharedUserModel, Guid, UserAddress, GroupModel, RoleModel>, ICreateUserModel<SharedUserModel, Guid, UserAddress, GroupModel, RoleModel>, IUserModel
{
    public SharedUserModel() : base() { }
    public SharedUserModel( IUserData<Guid> value ) : base(value) { }
    public SharedUserModel( string          firstName, string lastName ) : base(firstName, lastName) { }


    public static SharedUserModel Create( IUserData<Guid> model )                                                                                                                                    => new(model);
    public static SharedUserModel Create( IUserData<Guid> model, IEnumerable<UserAddress>            addresses, IEnumerable<GroupModel>            groups, IEnumerable<RoleModel>            roles ) => Create(model).With(addresses).With(groups).With(roles);
    public static SharedUserModel Create( IUserData<Guid> model, scoped in ReadOnlySpan<UserAddress> addresses, scoped in ReadOnlySpan<GroupModel> groups, scoped in ReadOnlySpan<RoleModel> roles ) => Create(model).With(addresses).With(groups).With(roles);
    public static async ValueTask<SharedUserModel> CreateAsync( IUserData<Guid> model, IAsyncEnumerable<UserAddress> addresses, IAsyncEnumerable<GroupModel> groups, IAsyncEnumerable<RoleModel> roles, CancellationToken token = default )
    {
        SharedUserModel user = Create(model);

        await user.Addresses.Add(addresses, token).ConfigureAwait(false);

        await user.Groups.Add(groups, token).ConfigureAwait(false);

        await user.Roles.Add(roles, token).ConfigureAwait(false);

        return user;
    }
    public override bool Equals( object? other )                                      => other is SharedUserModel x && Equals(x);
    public override int  GetHashCode()                                                => base.GetHashCode();
    public static   bool operator ==( SharedUserModel? left, SharedUserModel? right ) => EqualityComparer<SharedUserModel>.Default.Equals(left, right);
    public static   bool operator !=( SharedUserModel? left, SharedUserModel? right ) => !EqualityComparer<SharedUserModel>.Default.Equals(left, right);
    public static   bool operator >( SharedUserModel   left, SharedUserModel  right ) => left.CompareTo(right) > 0;
    public static   bool operator >=( SharedUserModel  left, SharedUserModel  right ) => left.CompareTo(right) >= 0;
    public static   bool operator <( SharedUserModel   left, SharedUserModel  right ) => left.CompareTo(right) < 0;
    public static   bool operator <=( SharedUserModel  left, SharedUserModel  right ) => left.CompareTo(right) <= 0;
}
