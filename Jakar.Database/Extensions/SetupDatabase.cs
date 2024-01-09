// Jakar.Extensions :: Jakar.Database
// 1/8/2024  20:28

namespace Jakar.Database;


[ Experimental( "SetupDatabase" ) ]
public abstract class SetupDatabase<T>
    where T : Database
{
    private readonly WebApplicationBuilder _builder;
    public           string                AuthenticationScheme            { get; init; } = JwtBearerDefaults.AuthenticationScheme;
    public           string                AuthenticationSchemeDisplayName { get; init; } = nameof(JwtBearerHandler);
    public           string                CookieDisplayName               { get; init; } = CookieAuthenticationDefaults.AuthenticationScheme;
    public           bool                  RequireMfa                      { get; init; } = false;
    public           bool                  UseAuthorizationAuthentication  { get; init; } = true;
    public           bool                  UseIdentity                     { get; init; } = true;
    public           bool                  UseMicrosoftAccount             { get; init; } = true;
    public           bool                  UseGoogle                       { get; init; } = true;
    public           bool                  UseOpenIdConnect                { get; init; } = true;
    public           string                MicrosoftIdentityWebApiSection  { get; init; } = "AzureAd";


    protected SetupDatabase( WebApplicationBuilder builder ) { _builder = builder; }


    public void Run()
    {
        _builder.AddPasswordValidator( Configure );
        if ( UseAuthorizationAuthentication ) { SetupAuth(); }

        if ( UseIdentity ) { SetupIdentity(); }
    }
    private void SetupIdentity()
    {
        _builder.Services.AddOptions<IdentityOptions>().Configure( Configure );

        RoleStore.Register( _builder );
        UserStore.Register( _builder );

        IdentityBuilder builder = _builder.Services.AddIdentity<UserRecord, RoleRecord>()
                                          .AddUserStore<UserStore>()
                                          .AddUserManager<UserRecordManager>()
                                          .AddRoleStore<RoleStore>()
                                          .AddRoleManager<RoleManager>()
                                          .AddSignInManager<SignInManager>()
                                          .AddUserValidator<UserValidator>()
                                          .AddTokenProvider<TokenProvider>( nameof(TokenProvider) )
                                          .AddRoleValidator<RoleValidator>()
                                          .AddPasswordValidator<UserPasswordValidator>()
                                          .AddTokenProvider( TokenOptions.DefaultProvider,              typeof(DataProtectorTokenProvider<UserRecord>) )
                                          .AddTokenProvider( TokenOptions.DefaultEmailProvider,         typeof(EmailTokenProvider<UserRecord>) )
                                          .AddTokenProvider( TokenOptions.DefaultPhoneProvider,         typeof(PhoneNumberTokenProvider<UserRecord>) )
                                          .AddTokenProvider( TokenOptions.DefaultAuthenticatorProvider, typeof(AuthenticatorTokenProvider<UserRecord>) );
    }
    private void SetupAuth()
    {
        AuthenticationBuilder authentication = _builder.Services.AddAuthentication( Configure );

        authentication.AddJwtBearer( AuthenticationScheme,
                                     AuthenticationSchemeDisplayName,
                                     bearer =>
                                     {
                                         bearer.TokenHandlers.Add( DbTokenHandler.Instance );
                                         Configure( bearer );
                                     } );

        authentication.AddCookie( AuthenticationScheme, CookieDisplayName, Configure );

        AuthorizationBuilder authorization = _builder.Services.AddAuthorizationBuilder();
        Configure( authentication, authorization );
    }
    protected void AddDefaultAuth( AuthenticationBuilder authentication, AuthorizationBuilder authorization )
    {
        authentication.AddCookie( IdentityConstants.ApplicationScheme, ConfigureApplication );
        authentication.AddCookie( IdentityConstants.ExternalScheme,    ConfigureExternal );

        if ( UseMicrosoftAccount ) { authentication.AddMicrosoftAccount( Configure ); }

        if ( UseGoogle ) { authentication.AddGoogle( Configure ); }

        if ( UseOpenIdConnect ) { authentication.AddOpenIdConnect( Configure ); }

        authentication.AddMicrosoftIdentityWebApi( _builder.Configuration.GetSection( MicrosoftIdentityWebApiSection ) ).EnableTokenAcquisitionToCallDownstreamApi().AddInMemoryTokenCaches();

        if ( RequireMfa ) { authorization.AddPolicy( nameof(Jakar.Database.RequireMfa), static policy => policy.Requirements.Add( new RequireMfa() ) ); }

        // .AddMicrosoftGraph( builder.Configuration.GetSection( "Graph" ) );
    }


    protected virtual void Configure( AuthenticationBuilder authentication, AuthorizationBuilder authorization ) => AddDefaultAuth( authentication, authorization );
    protected virtual void Configure( AuthenticationOptions options )
    {
        options.DefaultScheme             = AuthenticationScheme;
        options.DefaultAuthenticateScheme = AuthenticationScheme;
    }
    protected abstract void Configure( DbOptions                              options );
    protected abstract void Configure( JwtBearerOptions                       options );
    protected abstract void Configure( OpenIdConnectOptions                   options );
    protected abstract void Configure( MicrosoftAccountOptions                options );
    protected abstract void Configure( GoogleOptions                          options );
    protected abstract void Configure( IdentityOptions                        options );
    protected abstract void Configure( PasswordRequirements                   options );
    protected abstract void Configure( CookieAuthenticationOptions            options );
    protected abstract void ConfigureApplication( CookieAuthenticationOptions options );
    protected abstract void ConfigureExternal( CookieAuthenticationOptions    options );
}
