// Jakar.Extensions :: Jakar.Extensions
// 4/4/2024  13:50


// ReSharper disable CheckNamespace

namespace Jakar.Extensions.UserLong;
// ReSharper restore CheckNamespace



[Serializable]
public sealed class UserAddress : UserAddress<UserAddress, long>, IAddress<UserAddress, long>
{
    public UserAddress() : base() { }
    public UserAddress( Match                        match ) : base( match ) { }
    public UserAddress( IAddress<long>               address ) : base( address ) { }
    public UserAddress( string                       line1, string line2, string city, string stateOrProvince, string postalCode, string country, long id = 0 ) : base( line1, line2, city, stateOrProvince, postalCode, country, id ) { }
    public static UserAddress Create( Match          match )                                                                                                          => new(match);
    public static UserAddress Create( IAddress<long> address )                                                                                                        => new(address);
    public static UserAddress Create( string         line1, string line2, string city, string stateOrProvince, string postalCode, string country, long id = 0 ) => new(line1, line2, city, stateOrProvince, postalCode, country, id);
    public new static UserAddress Parse( string value, IFormatProvider? provider )
    {
        Match match = Validate.Re.Address.Match( value );
        return new UserAddress( match );
    }
    public new static bool TryParse( string? value, IFormatProvider? provider, [NotNullWhen( true )] out UserAddress? result )
    {
        try
        {
            result = string.IsNullOrWhiteSpace( value ) is false
                         ? Parse( value, provider )
                         : null;

            return result is not null;
        }
        catch ( Exception )
        {
            result = null;
            return false;
        }
    }
    public override bool Equals( object?           other )                  => other is UserAddress x && Equals( x );
    public override int  GetHashCode()                                      => base.GetHashCode();
    public static   bool operator ==( UserAddress? left, UserAddress? right ) => Equalizer.Equals( left, right );
    public static   bool operator !=( UserAddress? left, UserAddress? right ) => Equalizer.Equals( left, right ) is false;
    public static   bool operator >( UserAddress   left, UserAddress  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( UserAddress  left, UserAddress  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( UserAddress   left, UserAddress  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( UserAddress  left, UserAddress  right ) => Sorter.LessThanOrEqualTo( left, right );
}



[Serializable]
public sealed class GroupModel : GroupModel<GroupModel, long>, IGroupModel<GroupModel, long>
{
    public GroupModel( string                            nameOfGroup, long? ownerID, long? createdBy, long id, string rights ) : base( nameOfGroup, ownerID, createdBy, id, rights ) { }
    public GroupModel( IGroupModel<long>                 model ) : base( model ) { }
    public static   GroupModel Create( IGroupModel<long> model )            => new(model);
    public override bool       Equals( object?           other )            => other is GroupModel x && Equals( x );
    public override int        GetHashCode()                                => base.GetHashCode();
    public static   bool operator ==( GroupModel? left, GroupModel? right ) => Equalizer.Equals( left, right );
    public static   bool operator !=( GroupModel? left, GroupModel? right ) => Equalizer.Equals( left, right ) is false;
    public static   bool operator >( GroupModel   left, GroupModel  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( GroupModel  left, GroupModel  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( GroupModel   left, GroupModel  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( GroupModel  left, GroupModel  right ) => Sorter.LessThanOrEqualTo( left, right );
}



[Serializable]
public sealed class RoleModel : RoleModel<RoleModel, long>, IRoleModel<RoleModel, long>
{
    public RoleModel( string                           nameOfRole, string rights, long id ) : base( nameOfRole, rights, id ) { }
    public RoleModel( IRoleModel<long>                 model ) : base( model ) { }
    public static   RoleModel Create( IRoleModel<long> model )            => new(model);
    public override bool      Equals( object?          other )            => other is UserModel x && Equals( x );
    public override int       GetHashCode()                               => base.GetHashCode();
    public static   bool operator ==( RoleModel? left, RoleModel? right ) => Equalizer.Equals( left, right );
    public static   bool operator !=( RoleModel? left, RoleModel? right ) => Equalizer.Equals( left, right ) is false;
    public static   bool operator >( RoleModel   left, RoleModel  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( RoleModel  left, RoleModel  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( RoleModel   left, RoleModel  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( RoleModel  left, RoleModel  right ) => Sorter.LessThanOrEqualTo( left, right );
}



[Serializable]
public sealed class UserModel : UserModel<UserModel, long, UserAddress, GroupModel, RoleModel>, ICreateUserModel<UserModel, long, UserAddress, GroupModel, RoleModel>, IEqualComparable<UserModel>
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
    public override bool Equals( object? other )                          => other is UserModel x && Equals( x );
    public override int  GetHashCode()                                    => base.GetHashCode();
    public static   bool operator ==( UserModel? left, UserModel? right ) => Equalizer.Equals( left, right );
    public static   bool operator !=( UserModel? left, UserModel? right ) => Equalizer.Equals( left, right ) is false;
    public static   bool operator >( UserModel   left, UserModel  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( UserModel  left, UserModel  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( UserModel   left, UserModel  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( UserModel  left, UserModel  right ) => Sorter.LessThanOrEqualTo( left, right );
}



[Serializable]
[method: SetsRequiredMembers]
public sealed class FileData( long fileSize, string hash, string payload, FileMetaData metaData, long id = 0 ) : FileData<FileData, long, FileMetaData>( fileSize, hash, payload, id, metaData ), IFileData<FileData, long, FileMetaData>
{
    [SetsRequiredMembers] public FileData( IFileData<long, FileMetaData> file ) : this( file, file.MetaData ) { }
    [SetsRequiredMembers] public FileData( IFileData<long>               file,     FileMetaData              metaData ) : this( file.FileSize, file.Hash, file.Payload, metaData ) { }
    [SetsRequiredMembers] public FileData( FileMetaData                  metaData, params ReadOnlySpan<byte> content ) : this( content.Length, content.Hash_SHA512(), Convert.ToBase64String( content ), metaData ) { }


    public static FileData Create( long fileSize, string hash, string payload, long id, FileMetaData metaData ) => new(fileSize, hash, payload, metaData, id);
}
