// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/12/2024  11:06

using System.Collections.Specialized;
using System.ComponentModel;
using Jakar.Extensions.UserGuid;
using Microsoft.AspNetCore.Authentication.JwtBearer;



namespace Jakar.Extensions.Blazor;


public interface ILoginUserState : ICascadingValueName, INotifyPropertyChanged
{
    UserModel?                Model      { get; set; }
    Guid?                     UserID     { get; set; }
    string?                   UserName   { get; set; }
    AuthenticationProperties? Properties { get; set; }
    Task                      Login( UserModel       model, AuthenticationProperties? properties = null );
    Task                      Login( ClaimsIdentity  model, AuthenticationProperties? properties = null );
    Task                      Login( ClaimsPrincipal model, AuthenticationProperties? properties = null );
    Task                      Logout();
}



public class LoginUserState( HttpContext context, IAuthenticationService authentication ) : ObservableClass, ILoginUserState
{
    public const       string                    KEY             = nameof(LoginUserState);
    protected readonly HttpContext               _context        = context;
    protected readonly IAuthenticationService    _authentication = authentication;
    private            UserModel?                _model;
    private            Guid?                     _userID;
    private            string?                   _userName;
    private            AuthenticationProperties? _properties;


    public static  string                    CascadingName        => KEY;
    public static  string                    AuthenticationScheme { get;                set; } = JwtBearerDefaults.AuthenticationScheme;
    public virtual UserModel?                Model                { get => _model;      set => SetProperty( ref _model,      value ); }
    public virtual Guid?                     UserID               { get => _userID;     set => SetProperty( ref _userID,     value ); }
    public virtual string?                   UserName             { get => _userName;   set => SetProperty( ref _userName,   value ); }
    public virtual AuthenticationProperties? Properties           { get => _properties; set => SetProperty( ref _properties, value ); }


    public static LoginUserState Get( IServiceProvider provider ) => provider.GetRequiredService<LoginUserState>();


    public virtual Task Login( UserModel model, AuthenticationProperties? properties = null )
    {
        Model = model;
        return Login( new ClaimsIdentity( model.GetClaims(), AuthenticationScheme ), properties );
    }
    public virtual Task Login( ClaimsIdentity  model, AuthenticationProperties? properties = null ) => Login( new ClaimsPrincipal( model ), properties );
    public virtual Task Login( ClaimsPrincipal model, AuthenticationProperties? properties = null ) => _authentication.SignInAsync( _context, AuthenticationScheme, model, Properties = properties );
    public virtual Task Logout() => _authentication.SignOutAsync( _context, AuthenticationScheme, Properties );
}



public interface ILoginState
{
    [CascadingParameter( Name = LoginUserState.KEY )] public LoginUserState User { get; set; }
}
