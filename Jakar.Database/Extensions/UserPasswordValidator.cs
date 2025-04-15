// Jakar.Extensions :: Jakar.Database
// 04/12/2023  12:25 PM

namespace Jakar.Database;


public sealed class UserPasswordValidator( IOptions<PasswordRequirements> options ) : IPasswordValidator<UserRecord>
{
    private readonly PasswordRequirements _options = options.Value;


    public IdentityResult Validate( in ReadOnlySpan<char> password )
    {
        return PasswordValidator.Check( password, _options )
                   ? IdentityResult.Success
                   : IdentityResult.Failed();
    }


    public Task<IdentityResult> ValidateAsync( UserManager<UserRecord> manager, UserRecord user, string? password ) { return Task.FromResult( Validate( password ) ); }
}
