// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:5


namespace Jakar.Extensions;


[Serializable]
public abstract class CreateUserModel<TSelf, TID, TAddress, TGroupModel, TRoleModel> : UserModel<TSelf, TID, TAddress, TGroupModel, TRoleModel>, IChangePassword, ILoginRequestProvider
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
    where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
    where TAddress : IAddress<TID>, IEquatable<TAddress>
    where TSelf : CreateUserModel<TSelf, TID, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TSelf, TID, TAddress, TGroupModel, TRoleModel>, IEqualComparable<TSelf>, IJsonModel<TSelf>, new()
{
    private string __confirmPassword = string.Empty;
    private string __userPassword    = string.Empty;


    [Required] [StringLength(UNICODE_CAPACITY)] public virtual string ConfirmPassword
    {
        get => __confirmPassword;
        set
        {
            if ( SetProperty(ref __confirmPassword, value) ) { OnPropertyChanged(nameof(IsValid)); }
        }
    }


    [JsonIgnore]                                                                      public override bool IsValid         { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => base.IsValid                         && IsValidPassword; }
    [JsonIgnore] [MemberNotNullWhen(true, nameof(Password), nameof(ConfirmPassword))] public virtual  bool IsValidPassword { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => !string.IsNullOrWhiteSpace(Password) && string.Equals(Password, ConfirmPassword, StringComparison.Ordinal) && PasswordValidator.Check(Password); }


    [Required] [StringLength(UNICODE_CAPACITY)] public virtual string Password
    {
        get => __userPassword;
        set
        {
            if ( SetProperty(ref __userPassword, value) ) { OnPropertyChanged(nameof(IsValid)); }
        }
    }


    [Required] [StringLength(UNICODE_CAPACITY)] public override string UserName
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => base.UserName;
        set
        {
            base.UserName = value;
            OnPropertyChanged(nameof(IsValid));
        }
    }


    public AppVersion Version { get; set; } = AppVersion.Default;


    protected CreateUserModel() : base() { }
    protected CreateUserModel( IUserData<TID> value ) : base(value) { }
    protected CreateUserModel( string         firstName, string lastName ) : base(firstName, lastName) { }


    public static TSelf Register( string email, string password )
    {
        TSelf user = new()
                      {
                          Password        = password,
                          ConfirmPassword = password,
                          UserName        = email,
                          Email           = email
                      };

        Debug.Assert(user.IsValid);
        return user;
    }
    public static TSelf Register( string userName, string password, string email )
    {
        TSelf user = new()
                      {
                          Password        = password,
                          ConfirmPassword = password,
                          UserName        = userName,
                          Email           = email
                      };

        Debug.Assert(user.IsValid);
        return user;
    }


    public LoginRequest      GetLoginRequest()                         => new(UserName, Password);
    public NetworkCredential GetCredential( Uri uri, string authType ) => new(UserName, Password, uri.ToString());


    public virtual bool Validate( ICollection<string> errors )
    {
        if ( string.IsNullOrWhiteSpace(UserName) || !UserName.IsEmailAddress() ) { errors.Add("Must provide a valid email address"); }

        if ( string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword) ) { errors.Add("Password must not be empty"); }

        if ( !string.Equals(Password, ConfirmPassword, StringComparison.Ordinal) ) { errors.Add("Passwords be equal"); }

        if ( !IsValidEmail ) { errors.Add("Invalid Email"); }

        return errors.Count == 0;
    }
}
