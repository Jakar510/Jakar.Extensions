// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:5


namespace Jakar.Extensions;


[Serializable]
public abstract class CreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel> : UserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, IChangePassword, IVerifyRequestProvider
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
    where TGroupModel : IGroupModel<TID>
    where TRoleModel : IRoleModel<TID>
    where TAddress : IAddress<TID>
#if NET8_0_OR_GREATER
    where TClass : CreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TClass, TID>, ICreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, new()
#else
    where TClass : CreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, new()
#endif
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


    [JsonIgnore] public override bool IsValid         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => base.IsValid                                   && IsValidPassword; }
    [JsonIgnore] public virtual  bool IsValidPassword { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => string.IsNullOrWhiteSpace( Password ) is false && string.Equals( Password, ConfirmPassword, StringComparison.Ordinal ); }


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


    public VerifyRequest                 GetVerifyRequest() => new(UserName, Password);
    VerifyRequest IVerifyRequestProvider.GetVerifyRequest() => GetVerifyRequest();


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
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
#if NET8_0_OR_GREATER
    where TClass : CreateUserModel<TClass, TID>, ICreateUserModel<TClass, TID>, ICreateUserModel<TClass, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>, new()
#else
    where TClass : CreateUserModel<TClass, TID>, new()
#endif
{
    protected CreateUserModel() : base() { }
    protected CreateUserModel( IUserData<TID> value ) : base( value ) { }
    protected CreateUserModel( string         firstName, string lastName ) : base( firstName, lastName ) { }
}



[Serializable]
public sealed class CreateUserModel<TID> :
#if NET8_0_OR_GREATER
    CreateUserModel<CreateUserModel<TID>, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>,
    ICreateUserModel<CreateUserModel<TID>, TID>,
    ICreateUserModel<CreateUserModel<TID>, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>
#else
    CreateUserModel<CreateUserModel<TID>, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>
#endif

#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    public CreateUserModel() : base() { }
    public CreateUserModel( IUserData<TID>                    value ) : base( value ) { }
    public CreateUserModel( string                            firstName, string lastName ) : base( firstName, lastName ) { }
    public static CreateUserModel<TID> Create( IUserData<TID> model ) => new(model);
    public static CreateUserModel<TID> Create( IUserData<TID> model, IEnumerable<UserAddress<TID>> addresses, IEnumerable<GroupModel<TID>> groups, IEnumerable<RoleModel<TID>> roles )
    {
        CreateUserModel<TID> user = new();
        user.With( model );
        user.WithAddresses( addresses );
        user.Groups.Add( groups );
        user.Roles.Add( roles );
        return user;
    }
}
