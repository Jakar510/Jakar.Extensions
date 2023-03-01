// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:35 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static partial class DbExtensions
{
    public static IHealthChecksBuilder AddHealthCheck<T>( this WebApplicationBuilder builder ) where T : IHealthCheck => builder.AddHealthCheck( WebBuilder.CreateHealthCheck<T>() );
    public static IHealthChecksBuilder AddHealthCheck( this WebApplicationBuilder builder, HealthCheckRegistration registration ) =>
        builder.Services.AddHealthChecks()
               .Add( registration );


    public static ILoggingBuilder AddFluentMigratorLogger( this ILoggingBuilder builder, bool showSql = true, bool showElapsedTime = true ) =>
        builder.AddProvider( new FluentMigratorConsoleLoggerProvider( new OptionsWrapper<FluentMigratorLoggerOptions>( new FluentMigratorLoggerOptions
                                                                                                                       {
                                                                                                                           ShowElapsedTime = showElapsedTime,
                                                                                                                           ShowSql         = showSql,
                                                                                                                       } ) ) );


    public static WebApplicationBuilder AddDatabase<T>( this WebApplicationBuilder builder, DbInstance instance, AppVersion version ) where T : Database => builder.AddDatabase<T>( configure =>
                                                                                                                                                                                    {
                                                                                                                                                                                        configure.DbType  = instance;
                                                                                                                                                                                        configure.Version = version;
                                                                                                                                                                                    } );
    public static WebApplicationBuilder AddDatabase<T>( this WebApplicationBuilder builder, Action<DbOptions> configure ) where T : Database
    {
        builder.AddOptions( configure );
        builder.AddSingleton<T>();
        builder.AddSingleton<Database>( provider => provider.GetRequiredService<T>() );
        builder.AddHealthCheck<T>();
        return builder;
    }


    public static WebApplicationBuilder AddEmailer( this WebApplicationBuilder builder )
    {
        builder.AddOptions<Emailer.Options>();
        return builder.AddScoped<Emailer>();
    }
    public static WebApplicationBuilder AddEmailer( this WebApplicationBuilder builder, Action<Emailer.Options> configure )
    {
        builder.AddOptions<Emailer.Options>()
               .Configure( configure );

        return builder.AddScoped<Emailer>();
    }


    public static WebApplicationBuilder AddPwdValidator( this WebApplicationBuilder builder )
    {
        builder.AddOptions<PasswordRequirements>();
        return builder.AddScoped<IPasswordValidator<UserRecord>, PwdValidator>();
    }
    public static WebApplicationBuilder AddPwdValidator( this WebApplicationBuilder builder, Action<PasswordRequirements> configure )
    {
        builder.AddOptions<PasswordRequirements>()
               .Configure( configure );

        return builder.AddScoped<IPasswordValidator<UserRecord>, PwdValidator>();
    }


    public static WebApplicationBuilder AddRoleStore( this WebApplicationBuilder builder ) => builder.AddScoped<IRoleStore<RoleRecord>, RoleStore>();


    public static WebApplicationBuilder AddTokenizer<TName>( this WebApplicationBuilder builder ) where TName : IAppName => builder.AddTokenizer<TName, Tokenizer<TName>>();
    public static WebApplicationBuilder AddTokenizer<TName, TTokenizer>( this WebApplicationBuilder builder ) where TName : IAppName
                                                                                                              where TTokenizer : Tokenizer<TName> => builder.AddScoped<ITokenService, TTokenizer>();


    public static WebApplicationBuilder AddUserStore( this WebApplicationBuilder builder, Action<PasswordRequirements>? configurePasswordRequirements = default ) =>
        builder.AddUserStore<UserValidator>( configurePasswordRequirements );
    public static WebApplicationBuilder AddUserStore<TUserValidator>( this WebApplicationBuilder builder, Action<PasswordRequirements>? configurePasswordRequirements = default ) where TUserValidator : UserValidator
    {
        OptionsBuilder<PasswordRequirements> req = builder.Services.AddOptions<PasswordRequirements>();
        if ( configurePasswordRequirements is not null ) { req.Configure( configurePasswordRequirements ); }


        builder.Services.AddIdentity<UserRecord, RoleRecord>()
               .AddRoleManager<RoleStore>()
               .AddUserStore<UserStore>()
               .AddPasswordValidator<PwdValidator>()
               .AddUserValidator<TUserValidator>();


        // builder.AddTransient<IUserPasswordStore<UserRecord>, UserStore>();
        builder.AddTransient<IUserLoginStore<UserRecord>, UserStore>();
        builder.AddTransient<IUserClaimStore<UserRecord>, UserStore>();
        builder.AddTransient<IUserSecurityStampStore<UserRecord>, UserStore>();
        builder.AddTransient<IUserTwoFactorStore<UserRecord>, UserStore>();
        builder.AddTransient<IUserEmailStore<UserRecord>, UserStore>();
        builder.AddTransient<IUserLockoutStore<UserRecord>, UserStore>();
        builder.AddTransient<IUserAuthenticatorKeyStore<UserRecord>, UserStore>();
        builder.AddTransient<IUserTwoFactorRecoveryCodeStore<UserRecord>, UserStore>();
        builder.AddTransient<IUserPhoneNumberStore<UserRecord>, UserStore>();
        return builder;
    }
}
