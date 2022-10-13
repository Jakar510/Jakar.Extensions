// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:35 PM

namespace Jakar.Database;


public static partial class DbExtensions
{
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


    public static WebApplicationBuilder AddUserStore( this WebApplicationBuilder builder )
    {
        builder.Services.AddIdentity<UserRecord, RoleRecord>()
               .AddRoleManager<RoleStore>()
               .AddUserStore<UserStore>()
               .AddPasswordValidator<PwdValidator>()
               .AddUserValidator<UserValidator>();

        builder.AddScoped<IUserStore, UserStore>();
        builder.AddScoped<IUserLoginStore<UserRecord>, UserStore>();
        builder.AddScoped<IUserClaimStore<UserRecord>, UserStore>();
        builder.AddScoped<IUserPasswordStore<UserRecord>, UserStore>();
        builder.AddScoped<IUserSecurityStampStore<UserRecord>, UserStore>();
        builder.AddScoped<IUserTwoFactorStore<UserRecord>, UserStore>();
        builder.AddScoped<IUserEmailStore<UserRecord>, UserStore>();
        builder.AddScoped<IUserLockoutStore<UserRecord>, UserStore>();
        builder.AddScoped<IUserAuthenticatorKeyStore<UserRecord>, UserStore>();
        builder.AddScoped<IUserTwoFactorRecoveryCodeStore<UserRecord>, UserStore>();
        builder.AddScoped<IUserPhoneNumberStore<UserRecord>, UserStore>();
        return builder;
    }
}
