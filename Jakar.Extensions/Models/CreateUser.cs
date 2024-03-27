// Jakar.Extensions :: Jakar.Extensions
// 3/5/2024  21:5


namespace Jakar.Extensions;


[Serializable]
public class CreateUser : ObservableClass, IValidator, IVerifyRequestProvider
{
    private string   _confirmPassword = string.Empty;
    private string   _password        = string.Empty;
    private string?  _userName;
    private UserData _data = new();

    public static PasswordRequirements PasswordRequirements { get => PasswordRequirements.Current; set => PasswordRequirements.Current = value; }


    [Required]
    [StringLength( PasswordRequirements.MAX_LENGTH, MinimumLength = PasswordRequirements.MIN_LENGTH )]
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            if ( SetProperty( ref _confirmPassword, value ) is false ) { return; }

            OnPropertyChanged( nameof(IsValid) );
            OnPropertyChanged( nameof(IsValidPassword) );
        }
    }

    public UserData Data
    {
        get => _data;
        set
        {
            if ( SetProperty( ref _data, value ) is false ) { return; }

            OnPropertyChanged( nameof(IsValid) );
            OnPropertyChanged( nameof(UserName) );
        }
    }
    public bool IsValid         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => !string.IsNullOrWhiteSpace( UserName )                               && Data.IsValid && IsValidPassword; }
    public bool IsValidPassword { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => string.Equals( Password, ConfirmPassword, StringComparison.Ordinal ) && PasswordValidator.Check( Password, PasswordRequirements ); }

#pragma warning disable IL2026
    [Required]
    [StringLength( PasswordRequirements.MAX_LENGTH, MinimumLength = PasswordRequirements.MIN_LENGTH )]
    [Compare( nameof(ConfirmPassword) )]
    public string Password
    {
        get => _password;
        set
        {
            if ( SetProperty( ref _password, value ) is false ) { return; }

            OnPropertyChanged( nameof(IsValid) );
            OnPropertyChanged( nameof(IsValidPassword) );
        }
    }
#pragma warning restore IL2026

    [Required]
    public string UserName
    {
        get => _userName ??= Data.Email;
        set
        {
            if ( SetProperty( ref _userName, value ) ) { Data.Email = value; }
        }
    }


    public CreateUser() : base() { }
    public VerifyRequest GetVerifyRequest() => new(UserName, Password);
    public CreateUser Update( IUserData value )
    {
        Data.WithUserData( value );
        return this;
    }

    public static bool Validate<T>( T user )
        where T : CreateUser
    {
        // ValidationContext context = new(this);
        return user.IsValid;
    }
}
