// Jakar.Extensions :: Jakar.Database
// 10/11/2022  12:07 PM

namespace Jakar.Database;


public class UserValidator : IUserValidator<UserRecord>
{
    public static WebApplicationBuilder Register( WebApplicationBuilder builder )
    {
        builder.AddScoped<IUserValidator<UserRecord>, UserValidator>();
        return builder;
    }
    public static WebApplicationBuilder Register<T>( WebApplicationBuilder builder ) where T : UserValidator
    {
        builder.AddScoped<IUserValidator<UserRecord>, T>();
        return builder;
    }


    protected IdentityError[] Check( UserRecord user )
    {
        var errors = new List<IdentityError>( 5 );
        Check( errors, user );
        return errors.ToArray();
    }
    protected virtual void Check( in ICollection<IdentityError> errors, UserRecord user )
    {
        if ( !user.UserID.IsValidID() )
        {
            errors.Add( new IdentityError
                        {
                            Description = $"{nameof(UserRecord.UserID)} is invalid",
                        } );
        }

        if ( !string.IsNullOrWhiteSpace( user.UserName ) )
        {
            errors.Add( new IdentityError
                        {
                            Description = $"{nameof(UserRecord.UserName)} is invalid",
                        } );
        }
    }


    public Task<IdentityResult> ValidateAsync( UserManager<UserRecord> manager, UserRecord user )
    {
        IdentityError[] errors = Check( user );

        return Task.FromResult( errors.Any()
                                    ? IdentityResult.Failed( errors )
                                    : IdentityResult.Success );
    }
}
