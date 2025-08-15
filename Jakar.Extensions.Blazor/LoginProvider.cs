// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/12/2024  11:06

using Microsoft.AspNetCore.Authorization;



namespace Jakar.Extensions.Blazor;


public interface ILoginUserState : ICascadingValueName, INotifyPropertyChanged, IAuthorizationHandler
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
    private            UserModel?                __model;
    private            Guid?                     __userID;
    private            string?                   __userName;
    private            AuthenticationProperties? __properties;


    public static  string                    CascadingName        => KEY;
    public static  string                    AuthenticationScheme { get;                set; } = JwtBearerDefaults.AuthenticationScheme;
    public virtual UserModel?                Model                { get => __model;      set => SetProperty( ref __model,      value ); }
    public virtual Guid?                     UserID               { get => __userID;     set => SetProperty( ref __userID,     value ); }
    public virtual string?                   UserName             { get => __userName;   set => SetProperty( ref __userName,   value ); }
    public virtual AuthenticationProperties? Properties           { get => __properties; set => SetProperty( ref __properties, value ); }


    public static LoginUserState Get( IServiceProvider provider ) => provider.GetRequiredService<LoginUserState>();


    public async Task HandleAsync( AuthorizationHandlerContext context ) { await Login( context.User, Properties ); }


    public virtual Task Login( UserModel model, AuthenticationProperties? properties = null )
    {
        Model  = model;
        UserID = model.ID;
        return Login( new ClaimsIdentity( model.GetClaims(), AuthenticationScheme ), properties );
    }
    public virtual       Task Login( ClaimsIdentity  model, AuthenticationProperties? properties = null ) => Login( new ClaimsPrincipal( model ), properties );
    public virtual async Task Login( ClaimsPrincipal model, AuthenticationProperties? properties = null ) => await _authentication.SignInAsync( _context, AuthenticationScheme, model, Properties = properties );
    public virtual async Task Logout() => await _authentication.SignOutAsync( _context, AuthenticationScheme, Properties );
}



public interface ILoginState<TValue>
    where TValue : ILoginUserState
{
    [CascadingParameter( Name = LoginUserState.KEY )] public TValue User { get; set; }
}



public interface ILoginState : ILoginState<LoginUserState>;
