// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/12/2024  11:06

using Jakar.Extensions.UserGuid;
using Microsoft.AspNetCore.Authentication.JwtBearer;



namespace Jakar.Extensions.Blazor;


public class LoginState( HttpContext context, IAuthenticationService authentication ) : ObservableClass
{
    public const       string                    KEY             = nameof(LoginState);
    protected readonly HttpContext               _context        = context;
    protected readonly IAuthenticationService    _authentication = authentication;
    private            UserModel?                _model;
    private            Guid?                     _userID;
    private            string?                   _userName;
    private            AuthenticationProperties? _properties;


    public static string                    AuthenticationScheme { get;                set; } = JwtBearerDefaults.AuthenticationScheme;
    public        UserModel?                Model                { get => _model;      set => SetProperty( ref _model,      value ); }
    public        Guid?                     UserID               { get => _userID;     set => SetProperty( ref _userID,     value ); }
    public        string?                   UserName             { get => _userName;   set => SetProperty( ref _userName,   value ); }
    public        AuthenticationProperties? Properties           { get => _properties; set => SetProperty( ref _properties, value ); }


    public static LoginState Get( IServiceProvider provider ) => provider.GetRequiredService<LoginState>();


    public Task Login( UserModel model, AuthenticationProperties? properties = null )
    {
        Model = model;
        return Login( new ClaimsIdentity( model.GetClaims(), AuthenticationScheme ), properties );
    }
    public Task Login( ClaimsIdentity  model, AuthenticationProperties? properties = null ) => Login( new ClaimsPrincipal( model ), properties );
    public Task Login( ClaimsPrincipal model, AuthenticationProperties? properties = null ) => _authentication.SignInAsync( _context, AuthenticationScheme, model, Properties = properties );
    public Task Logout() => _authentication.SignOutAsync( _context, AuthenticationScheme, Properties );
}



public interface ILoginState
{
    [CascadingParameter( Name = LoginState.KEY )] public LoginState User { get; set; }
}
