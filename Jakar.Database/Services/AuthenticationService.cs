using System.Net.Http.Headers;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ConvertToAutoPropertyWithPrivateSetter" )]
public class AuthenticatorServiceOptions() : IOptions<AuthenticatorServiceOptions>
{
    public const string                                               DEFAULT_LOGIN_PATH         = "Endpoints:Login";
    public const string                                               DEFAULT_REFRESH_TOKEN_PATH = "Endpoints:RefreshTokens";
    public       string?                                              LoginPath         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public       AppVersion                                           MinVersion        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = AppVersion.Default;
    public       string?                                              RefreshTokensPath { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    AuthenticatorServiceOptions IOptions<AuthenticatorServiceOptions>.Value             => this;
}



public interface IAuthenticatorService : IAuthenticationService
{
    Tokens?            Tokens { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    ValueTask<Tokens?> SetAccessToken( [NotNullIfNotNull( nameof(token) )] Tokens? token );
    Task<Tokens?>      Login( VerifyRequest                                        request, CancellationToken token, string configPath = AuthenticatorServiceOptions.DEFAULT_LOGIN_PATH );
    Task<Tokens?>      RefreshTokens( VerifyRequest                                request, CancellationToken token, string configPath = AuthenticatorServiceOptions.DEFAULT_REFRESH_TOKEN_PATH );
    Task               Logout();
}



[SuppressMessage( "ReSharper", "ConvertToAutoPropertyWithPrivateSetter" )]
public class AuthenticatorService<TSettings>( IHttpClientFactory factory, AuthStateProvider authStateProvider, TSettings settings, IConfiguration config, IAuthenticationService service, IOptions<AuthenticatorServiceOptions> options, ILogger<AuthenticatorService<TSettings>> logger ) : IAuthenticatorService
    where TSettings : class, IHostInfo
{
    protected readonly AuthStateProvider           _authStateProvider = authStateProvider;
    protected readonly IAuthenticationService      _service           = service;
    protected readonly IConfiguration              _config            = config;
    protected readonly AuthenticatorServiceOptions _options           = options.Value;
    protected readonly WebRequester                _client            = new(factory.CreateClient( nameof(AuthenticatorService<TSettings>) ), settings, logger);
    protected          Tokens?                     _tokens;
    protected          string?                     _minVersion;


    public Tokens? Tokens { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _tokens; }


    public virtual ValueTask<Tokens?> SetAccessToken( [NotNullIfNotNull( nameof(token) )] Tokens? token )
    {
        _tokens = token;
        if ( token is null ) { return ValueTask.FromResult<Tokens?>( null ); }

        _authStateProvider.NotifyUserAuthentication( token.AccessToken );
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( JwtBearerDefaults.AuthenticationScheme, token.AccessToken );
        return ValueTask.FromResult( _tokens );
    }
    public async Task<Tokens?> Login( VerifyRequest request, CancellationToken token, string configPath = AuthenticatorServiceOptions.DEFAULT_LOGIN_PATH )
    {
        _minVersion ??= _options.MinVersion.ToString();
        VerifyRequest<string> action = new(request, _minVersion);
        string?               path   = _options.LoginPath ?? _config[configPath];
        Guard.IsNotNullOrWhiteSpace( path );

        WebResponse<Tokens> result = await _client.Post( path, action, token ).AsJson<Tokens>();
        return await SetAccessToken( result.GetPayload() );
    }
    public async Task<Tokens?> RefreshTokens( VerifyRequest request, CancellationToken token, string configPath = AuthenticatorServiceOptions.DEFAULT_REFRESH_TOKEN_PATH )
    {
        _minVersion ??= _options.MinVersion.ToString();
        VerifyRequest<string> action = new(request, _minVersion);
        string?               path   = _options.RefreshTokensPath ?? _config[configPath];
        Guard.IsNotNullOrWhiteSpace( path );

        WebResponse<Tokens> result = await _client.Post( path, action, token ).AsJson<Tokens>();
        return await SetAccessToken( result.GetPayload() );
    }


    public async Task Logout()
    {
        await _authStateProvider.Logout();
        _client.DefaultRequestHeaders.Authorization = default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public       Task<AuthenticateResult> AuthenticateAsync( HttpContext context, string? scheme )                                                                            => _service.AuthenticateAsync( context, scheme );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public       Task                     ChallengeAsync( HttpContext    context, string? scheme, AuthenticationProperties? properties )                                      => _service.ChallengeAsync( context, scheme, properties );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public       Task                     ForbidAsync( HttpContext       context, string? scheme, AuthenticationProperties? properties )                                      => _service.ForbidAsync( context, scheme, properties );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public async Task                     SignOutAsync( HttpContext      context, string? scheme, AuthenticationProperties? properties )                                      => await _service.SignOutAsync( context, scheme, properties );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public async Task                     SignInAsync( HttpContext       context, string? scheme, ClaimsPrincipal           principal, AuthenticationProperties? properties ) => await _service.SignInAsync( context, scheme, principal, properties );
}
