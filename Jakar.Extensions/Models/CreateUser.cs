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
    [MinLength( PasswordRequirements.MIN_LENGTH )]
    [MaxLength( PasswordRequirements.MAX_LENGTH )]
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

            OnPropertyChanged( nameof(IsValidData) );
            OnPropertyChanged( nameof(UserName) );
        }
    }
    public bool IsValid         => !string.IsNullOrWhiteSpace( UserName )                               && IsValidData                                 && IsValidPassword;
    public bool IsValidData     => !string.IsNullOrWhiteSpace( Data.FirstName )                         && !string.IsNullOrWhiteSpace( Data.LastName ) && !string.IsNullOrWhiteSpace( Data.PhoneNumber );
    public bool IsValidPassword => string.Equals( Password, ConfirmPassword, StringComparison.Ordinal ) && PasswordValidator.Check( Password, PasswordRequirements );


    [Required]
    [MinLength( PasswordRequirements.MIN_LENGTH )]
    [MaxLength( PasswordRequirements.MAX_LENGTH )]
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
}
