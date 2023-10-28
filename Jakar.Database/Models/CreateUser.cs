// Jakar.Extensions :: Jakar.Database
// 04/30/2023  9:43 PM

namespace Jakar.Database;


public class CreateUser : ObservableClass, ILoginRequest, IVerifyRequestProvider
{
    private string    _confirmPassword = string.Empty;
    private string    _userLogin       = string.Empty;
    private string    _userPassword    = string.Empty;
    private UserData? _user;

    public virtual string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            if ( SetProperty( ref _confirmPassword, value ) ) { OnPropertyChanged( nameof(IsValid) ); }
        }
    }
    public virtual UserData Data
    {
        get
        {
            if ( _user is not null ) { return _user; }

            _user                 =  new UserData();
            _user.PropertyChanged -= User_OnPropertyChanged;
            return _user;
        }
        set
        {
            if ( _user is not null ) { _user.PropertyChanged -= User_OnPropertyChanged; }

            if ( !SetProperty( ref _user, value ) ) { return; }

            if ( _user is not null ) { _user.PropertyChanged += User_OnPropertyChanged; }
        }
    }


    public virtual bool IsValid => !string.IsNullOrWhiteSpace( UserLogin )       &&
                                   UserLogin.IsEmailAddress()                    &&
                                   !string.IsNullOrWhiteSpace( UserPassword )    &&
                                   !string.IsNullOrWhiteSpace( ConfirmPassword ) &&
                                   string.Equals( UserPassword, ConfirmPassword, StringComparison.Ordinal );

    public virtual string UserLogin
    {
        get => _userLogin;
        set
        {
            if ( !SetProperty( ref _userLogin, value ) ) { return; }

            OnPropertyChanged( nameof(IsValid) );
        }
    }
    public virtual string UserPassword
    {
        get => _userPassword;
        set
        {
            if ( SetProperty( ref _userPassword, value ) ) { OnPropertyChanged( nameof(IsValid) ); }
        }
    }


    public CreateUser() { }
    public static CreateUser Register( string email, string password )
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var user = new CreateUser
                   {
                       UserPassword    = password,
                       ConfirmPassword = password,
                       UserLogin       = email
                   };

        user.Data.Email = email;
        Debug.Assert( user.IsValid );
        return user;
    }
    public static CreateUser Register( string userName, string password, string email )
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var user = new CreateUser
                   {
                       UserPassword    = password,
                       ConfirmPassword = password,
                       UserLogin       = userName
                   };

        user.Data.Email = email;
        Debug.Assert( user.IsValid );
        return user;
    }


    protected void                       User_OnPropertyChanged( object? sender, PropertyChangedEventArgs e ) => OnPropertyChanged( nameof(IsValid) );
    public    VerifyRequest<CreateUser>  GetVerifyRequest() => new(UserLogin, UserPassword, this);
    VerifyRequest IVerifyRequestProvider.GetVerifyRequest() => GetVerifyRequest();


    public virtual bool Validate( ICollection<string> errors )
    {
        if ( string.IsNullOrWhiteSpace( UserLogin ) || !UserLogin.IsEmailAddress() ) { errors.Add( "Must provide a valid email address" ); }

        if ( string.IsNullOrWhiteSpace( UserPassword ) || string.IsNullOrWhiteSpace( ConfirmPassword ) ) { errors.Add( "Password must not be empty" ); }

        if ( !string.Equals( UserPassword, ConfirmPassword, StringComparison.Ordinal ) ) { errors.Add( "Passwords be equal" ); }

        if ( !Data.IsValidEmail ) { errors.Add( "Invalid Email" ); }

        return errors.Count == 0;
    }
}
