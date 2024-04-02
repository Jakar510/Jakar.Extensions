// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:5


namespace Jakar.Extensions;


[Serializable]
public abstract class CreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel> : UserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, IChangePassword, IVerifyRequestProvider
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
    where TGroupModel : IGroupModel<TID>
    where TRoleModel : IRoleModel<TID>
    where TAddress : IAddress<TID>
    where TClass : CreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, new()
{
    private string _confirmPassword = string.Empty;
    private string _userPassword    = string.Empty;


    [Required, StringLength( UNICODE_STRING_CAPACITY )]
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


    [Required, StringLength( UNICODE_STRING_CAPACITY )]
    public virtual string Password
    {
        get => _userPassword;
        set
        {
            if ( SetProperty( ref _userPassword, value ) ) { OnPropertyChanged( nameof(IsValid) ); }
        }
    }


    [Required, StringLength( UNICODE_STRING_CAPACITY )]
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
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
    where TClass : CreateUserModel<TClass, TID>, new()
{
    protected CreateUserModel() : base() { }
    protected CreateUserModel( IUserData<TID> value ) : base( value ) { }
    protected CreateUserModel( string         firstName, string lastName ) : base( firstName, lastName ) { }
}



[Serializable]
public sealed class CreateUserModel<TID> : CreateUserModel<CreateUserModel<TID>, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
{
    public CreateUserModel() : base() { }
    public CreateUserModel( IUserData<TID> value ) : base( value ) { }
    public CreateUserModel( string         firstName, string lastName ) : base( firstName, lastName ) { }
}
