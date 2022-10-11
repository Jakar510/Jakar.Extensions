// Jakar.Extensions :: Jakar.Database
// 10/11/2022  11:26 AM

namespace Jakar.Database;


public sealed class PwdValidator : IPasswordValidator<UserRecord>
{
    private readonly PasswordRequirements _options;

    public PwdValidator( IOptions<PasswordRequirements> options ) { PasswordValidator.Current = _options = options.Value; }


    public static WebApplicationBuilder Register( WebApplicationBuilder builder )
    {
        builder.AddOptions<PasswordRequirements>();
        builder.AddScoped<IPasswordValidator<UserRecord>, PwdValidator>();
        return builder;
    }
    public static WebApplicationBuilder Register( WebApplicationBuilder builder, Action<PasswordRequirements> configure )
    {
        builder.AddOptions<PasswordRequirements>()
               .Configure( configure );

        builder.AddScoped<IPasswordValidator<UserRecord>, PwdValidator>();
        return builder;
    }


    public IdentityResult Validate( in ReadOnlySpan<char> password )
    {
        return PasswordValidator.Check( password, _options )
                   ? IdentityResult.Success
                   : IdentityResult.Failed();
    }


    public Task<IdentityResult> ValidateAsync( UserManager<UserRecord> manager, UserRecord user, string password ) => Task.FromResult( Validate( password ) );
}
