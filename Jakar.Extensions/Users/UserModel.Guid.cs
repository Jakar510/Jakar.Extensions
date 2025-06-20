// Jakar.Extensions :: Jakar.Database
// 4/4/2024  10:1


// ReSharper disable CheckNamespace

namespace Jakar.Extensions.UserGuid;
// ReSharper restore CheckNamespace



[Serializable]
public sealed class UserAddress : UserAddress<UserAddress, Guid>, IAddress<UserAddress, Guid>, IEqualComparable<UserAddress>
{
    public UserAddress() : base() { }
    public UserAddress( Match                        match ) : base( match ) { }
    public UserAddress( IAddress<Guid>               address ) : base( address ) { }
    public UserAddress( string                       line1, string line2, string city, string stateOrProvince, string postalCode, string country, Guid id = default ) : base( line1, line2, city, stateOrProvince, postalCode, country, id ) { }
    public static UserAddress Create( Match          match )                                                                                                          => new(match);
    public static UserAddress Create( IAddress<Guid> address )                                                                                                        => new(address);
    public static UserAddress Create( string         line1, string line2, string city, string stateOrProvince, string postalCode, string country, Guid id = default ) => new(line1, line2, city, stateOrProvince, postalCode, country, id);
    public new static UserAddress Parse( string value, IFormatProvider? provider )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        Match               match         = Validate.Re.Address.Match( value );
        return new UserAddress( match );
    }
    public new static bool TryParse( string? value, IFormatProvider? provider, [NotNullWhen( true )] out UserAddress? result )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try
        {
            result = string.IsNullOrWhiteSpace( value ) is false
                         ? Parse( value, provider )
                         : null;

            return result is not null;
        }
        catch ( Exception e )
        {
            telemetrySpan.AddException( e );
            result = null;
            return false;
        }
    }
    public override bool Equals( object? other )                              => other is UserAddress x && Equals( x );
    public override int  GetHashCode()                                        => HashCode.Combine( Line1, Line2, City, PostalCode, Country, ID, Address );
    public static   bool operator ==( UserAddress? left, UserAddress? right ) => Equalizer.Equals( left, right );
    public static   bool operator !=( UserAddress? left, UserAddress? right ) => Equalizer.Equals( left, right ) is false;
    public static   bool operator >( UserAddress   left, UserAddress  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( UserAddress  left, UserAddress  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( UserAddress   left, UserAddress  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( UserAddress  left, UserAddress  right ) => Sorter.LessThanOrEqualTo( left, right );
}



[Serializable]
public sealed class GroupModel : GroupModel<GroupModel, Guid>, IGroupModel<GroupModel, Guid>, IEqualComparable<GroupModel>
{
    public GroupModel( string                            nameOfGroup, Guid? ownerID, Guid? createdBy, Guid id, string rights ) : base( nameOfGroup, ownerID, createdBy, id, rights ) { }
    public GroupModel( IGroupModel<Guid>                 model ) : base( model ) { }
    public static   GroupModel Create( IGroupModel<Guid> model )            => new(model);
    public override bool       Equals( object?           other )            => other is GroupModel x && Equals( x );
    public override int        GetHashCode()                                => HashCode.Combine( NameOfGroup, ID, Rights );
    public static   bool operator ==( GroupModel? left, GroupModel? right ) => Equalizer.Equals( left, right );
    public static   bool operator !=( GroupModel? left, GroupModel? right ) => Equalizer.Equals( left, right ) is false;
    public static   bool operator >( GroupModel   left, GroupModel  right ) => Sorter.Compare( left, right ) > 0;
    public static   bool operator >=( GroupModel  left, GroupModel  right ) => Sorter.Compare( left, right ) >= 0;
    public static   bool operator <( GroupModel   left, GroupModel  right ) => Sorter.Compare( left, right ) < 0;
    public static   bool operator <=( GroupModel  left, GroupModel  right ) => Sorter.Compare( left, right ) <= 0;
}



[Serializable]
public sealed class RoleModel : RoleModel<RoleModel, Guid>, IRoleModel<RoleModel, Guid>, IEqualComparable<RoleModel>
{
    public RoleModel( string                           nameOfRole, string rights, Guid id ) : base( nameOfRole, rights, id ) { }
    public RoleModel( IRoleModel<Guid>                 model ) : base( model ) { }
    public static   RoleModel Create( IRoleModel<Guid> model )            => new(model);
    public override bool      Equals( object?          other )            => other is RoleModel x && Equals( x );
    public override int       GetHashCode()                               => HashCode.Combine( NameOfRole, ID, Rights );
    public static   bool operator ==( RoleModel? left, RoleModel? right ) => Equalizer.Equals( left, right );
    public static   bool operator !=( RoleModel? left, RoleModel? right ) => Equalizer.Equals( left, right ) is false;
    public static   bool operator >( RoleModel   left, RoleModel  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( RoleModel  left, RoleModel  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( RoleModel   left, RoleModel  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( RoleModel  left, RoleModel  right ) => Sorter.LessThanOrEqualTo( left, right );
}



[Serializable]
public sealed class UserModel : UserModel<UserModel, Guid, UserAddress, GroupModel, RoleModel>, ICreateUserModel<UserModel, Guid, UserAddress, GroupModel, RoleModel>, IEqualComparable<UserModel>
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
public sealed class FileData( long fileSize, string hash, string payload, FileMetaData metaData, Guid id = default ) : FileData<FileData, Guid, FileMetaData>( fileSize, hash, payload, id, metaData ), IFileData<FileData, Guid, FileMetaData>
{
    [SetsRequiredMembers] public FileData( IFileData<Guid, FileMetaData> file ) : this( file, file.MetaData ) { }
    [SetsRequiredMembers] public FileData( IFileData<Guid>               file,     FileMetaData              metaData ) : this( file.FileSize, file.Hash, file.Payload, metaData ) { }
    [SetsRequiredMembers] public FileData( FileMetaData                  metaData, params ReadOnlySpan<byte> content ) : this( content.Length, content.Hash_SHA512(), Convert.ToBase64String( content ), metaData ) { }


    public static FileData Create( long fileSize, string hash, string payload, Guid id, FileMetaData metaData ) => new(fileSize, hash, payload, metaData, id);
}
