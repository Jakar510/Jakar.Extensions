// Jakar.Extensions :: Jakar.Database
// 04/12/2023  12:25 PM

namespace Jakar.Database;


public class UserPasswordValidator( IOptions<PasswordRequirements> options ) : IPasswordValidator<UserRecord>
{
    protected readonly PasswordRequirements __options = options.Value;


    public virtual IdentityResult Validate( in ReadOnlySpan<char> password ) =>
        PasswordValidator.Check(password, __options)
            ? IdentityResult.Success
            : IdentityResult.Failed();


    public virtual Task<IdentityResult> ValidateAsync( UserManager<UserRecord> manager, UserRecord user, string? password ) => Task.FromResult(Validate(password));
}
