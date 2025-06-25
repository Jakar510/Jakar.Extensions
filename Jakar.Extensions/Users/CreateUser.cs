// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:5


namespace Jakar.Extensions;


[Serializable]
public abstract class CreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel> : UserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, IChangePassword, ILoginRequestProvider
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
    where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
    where TAddress : IAddress<TID>, IEquatable<TAddress>
    where TClass : CreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, IEqualComparable<TClass>, new()
{
    private string _confirmPassword = string.Empty;
    private string _userPassword    = string.Empty;


    [Required, StringLength( UNICODE_CAPACITY )]
    public virtual string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            if ( SetProperty( ref _confirmPassword, value ) ) { OnPropertyChanged( nameof(IsValid) ); }
        }
    }


    [JsonIgnore]                                                                       public override bool IsValid         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => base.IsValid                                   && IsValidPassword; }
    [JsonIgnore, MemberNotNullWhen( true, nameof(Password), nameof(ConfirmPassword) )] public virtual  bool IsValidPassword { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => string.IsNullOrWhiteSpace( Password ) is false && string.Equals( Password, ConfirmPassword, StringComparison.Ordinal ) && PasswordValidator.Check( Password ); }


    [Required, StringLength( UNICODE_CAPACITY )]
    public virtual string Password
    {
        get => _userPassword;
        set
        {
            if ( SetProperty( ref _userPassword, value ) ) { OnPropertyChanged( nameof(IsValid) ); }
        }
    }


    [Required, StringLength( UNICODE_CAPACITY )]
    public override string UserName
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )] get => base.UserName;
        set
        {
            base.UserName = value;
            OnPropertyChanged( nameof(IsValid) );
        }
    }


    protected CreateUserModel() : base() { }
    protected CreateUserModel( IUserData<TID> value ) : base( value ) { }
    protected CreateUserModel( string         firstName, string lastName ) : base( firstName, lastName ) { }


    public static TClass Register( string email, string password )
    {
        TClass user = new()
                      {
                          Password        = password,
                          ConfirmPassword = password,
                          UserName        = email,
                          Email           = email
                      };

        Debug.Assert( user.IsValid );
        return user;
    }
    public static TClass Register( string userName, string password, string email )
    {
        TClass user = new()
                      {
                          Password        = password,
                          ConfirmPassword = password,
                          UserName        = userName,
                          Email           = email
                      };

        Debug.Assert( user.IsValid );
        return user;
    }


    public LoginRequest         GetLoginRequest()                       => new(UserName, Password);
    public LoginRequest<TValue> GetLoginRequest<TValue>( TValue value ) => new(UserName, Password, value);


    public virtual bool Validate( ICollection<string> errors )
    {
        if ( string.IsNullOrWhiteSpace( UserName ) || !UserName.IsEmailAddress() ) { errors.Add( "Must provide a valid email address" ); }

        if ( string.IsNullOrWhiteSpace( Password ) || string.IsNullOrWhiteSpace( ConfirmPassword ) ) { errors.Add( "Password must not be empty" ); }

        if ( !string.Equals( Password, ConfirmPassword, StringComparison.Ordinal ) ) { errors.Add( "Passwords be equal" ); }

        if ( !IsValidEmail ) { errors.Add( "Invalid Email" ); }

        return errors.Count == 0;
    }
}



[Serializable]
public abstract class CreateUserModel<TClass, TID> : CreateUserModel<TClass, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : CreateUserModel<TClass, TID>, ICreateUserModel<TClass, TID>, ICreateUserModel<TClass, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>, IEqualComparable<TClass>, new()
{
    protected CreateUserModel() : base() { }
    protected CreateUserModel( IUserData<TID> value ) : base( value ) { }
    protected CreateUserModel( string         firstName, string lastName ) : base( firstName, lastName ) { }
}



[Serializable]
public sealed class CreateUserModel<TID> : CreateUserModel<CreateUserModel<TID>, TID>, ICreateUserModel<CreateUserModel<TID>, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>, IEqualComparable<CreateUserModel<TID>>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public CreateUserModel() : base() { }
    public CreateUserModel( IUserData<TID> value ) : base( value ) { }
    public CreateUserModel( string         firstName, string lastName ) : base( firstName, lastName ) { }


    public static CreateUserModel<TID> Create( IUserData<TID> model )                                                                                                                                                   => new(model);
    public static CreateUserModel<TID> Create( IUserData<TID> model, IEnumerable<UserAddress<TID>>            addresses, IEnumerable<GroupModel<TID>>            groups, IEnumerable<RoleModel<TID>>            roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static CreateUserModel<TID> Create( IUserData<TID> model, scoped in ReadOnlySpan<UserAddress<TID>> addresses, scoped in ReadOnlySpan<GroupModel<TID>> groups, scoped in ReadOnlySpan<RoleModel<TID>> roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static async ValueTask<CreateUserModel<TID>> CreateAsync( IUserData<TID> model, IAsyncEnumerable<UserAddress<TID>> addresses, IAsyncEnumerable<GroupModel<TID>> groups, IAsyncEnumerable<RoleModel<TID>> roles, CancellationToken token = default )
    {
        CreateUserModel<TID> user = Create( model );
        await user.Addresses.Add( addresses, token );
        await user.Groups.Add( groups, token );
        await user.Roles.Add( roles, token );
        return user;
    }


    public override bool Equals( object? other )                                                => other is CreateUserModel<TID> x && Equals( x );
    public override int  GetHashCode()                                                          => base.GetHashCode();
    public static   bool operator ==( CreateUserModel<TID>? left, CreateUserModel<TID>? right ) =>  Sorter.Equals( left, right );
    public static   bool operator !=( CreateUserModel<TID>? left, CreateUserModel<TID>? right ) =>  Sorter.DoesNotEqual( left, right );
    public static   bool operator >( CreateUserModel<TID>   left, CreateUserModel<TID>  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( CreateUserModel<TID>  left, CreateUserModel<TID>  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( CreateUserModel<TID>   left, CreateUserModel<TID>  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( CreateUserModel<TID>  left, CreateUserModel<TID>  right ) => Sorter.LessThanOrEqualTo( left, right );
}
