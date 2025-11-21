// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/12/2024  11:06

using Microsoft.AspNetCore.Authorization;



namespace Jakar.Extensions.Blazor;


public interface ILoginUserState : ICascadingValueName, INotifyPropertyChanged, IAuthorizationHandler
{
    UserModel?                Model      { get; set; }
    AuthenticationProperties? Properties { get; set; }
    Guid?                     UserID     { get; set; }
    string?                   UserName   { get; set; }
    Task                      Login( UserModel       model, AuthenticationProperties? properties = null );
    Task                      Login( ClaimsIdentity  model, AuthenticationProperties? properties = null );
    Task                      Login( ClaimsPrincipal model, AuthenticationProperties? properties = null );
    Task                      Logout();
}



public class LoginUserState( HttpContext context, IAuthenticationService authentication ) : BaseClass, ILoginUserState
{
    public const       string                 KEY             = nameof(LoginUserState);
    protected readonly HttpContext            _context        = context;
    protected readonly IAuthenticationService _authentication = authentication;
    public static      string                 AuthenticationScheme { get; set; } = JwtBearerDefaults.AuthenticationScheme;


    public static  string                    CascadingName => KEY;
    public virtual UserModel?                Model         { get; set => SetProperty(ref field, value); }
    public virtual AuthenticationProperties? Properties    { get; set => SetProperty(ref field, value); }
    public virtual Guid?                     UserID        { get; set => SetProperty(ref field, value); }
    public virtual string?                   UserName      { get; set => SetProperty(ref field, value); }


    public static LoginUserState Get( IServiceProvider provider ) => provider.GetRequiredService<LoginUserState>();


    public async Task HandleAsync( AuthorizationHandlerContext context ) { await Login(context.User, Properties); }


    public virtual Task Login( UserModel model, AuthenticationProperties? properties = null )
    {
        Model  = model;
        UserID = model.ID;
        return Login(new ClaimsIdentity(model.GetClaims(), AuthenticationScheme), properties);
    }
    public virtual       Task Login( ClaimsIdentity  model, AuthenticationProperties? properties = null ) => Login(new ClaimsPrincipal(model), properties);
    public virtual async Task Login( ClaimsPrincipal model, AuthenticationProperties? properties = null ) => await _authentication.SignInAsync(_context, AuthenticationScheme, model, Properties = properties);
    public virtual async Task Logout() => await _authentication.SignOutAsync(_context, AuthenticationScheme, Properties);
}



public interface ILoginState<TValue>
    where TValue : ILoginUserState
{
    [CascadingParameter(Name = LoginUserState.KEY)] public TValue User { get; set; }
}



public interface ILoginState : ILoginState<LoginUserState>;
