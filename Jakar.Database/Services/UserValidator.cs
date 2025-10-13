// Jakar.Extensions :: Jakar.Database
// 10/11/2022  12:07 PM

namespace Jakar.Database;


public class UserValidator : IUserValidator<UserRecord>
{
    protected virtual IdentityError[] Check( UserRecord user )
    {
        List<IdentityError> errors = new(5);
        Check(errors, user);
        return [.. errors];
    }
    protected virtual void Check<TValue>( in TValue errors, UserRecord user )
        where TValue : ICollection<IdentityError>
    {
        /*
        if ( user.CreatedBy.IsValidID() )
        {
            errors.Add( new IdentityError
                        {
                            Description = $"{nameof(UserRecord.CreatedBy)} is invalid"
                        } );
        }
        */

        if ( !string.IsNullOrWhiteSpace(user.UserName) ) { errors.Add(new IdentityError { Description = $"{nameof(UserRecord.UserName)} is invalid" }); }
    }


    public virtual Task<IdentityResult> ValidateAsync( UserManager<UserRecord> manager, UserRecord user )
    {
        IdentityError[] errors = Check(user);

        return Task.FromResult(errors.Length > 0
                                   ? IdentityResult.Failed(errors)
                                   : IdentityResult.Success);
    }
}
