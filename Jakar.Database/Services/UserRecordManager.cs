// Jakar.Database ::  Jakar.Database 
// 04/11/2023  7:50 PM

namespace Jakar.Database;


public class UserManager( Database                                    database,
                          UserStore                                   store,
                          IOptions<IdentityOptions>                   optionsAccessor,
                          IPasswordHasher<UserRecord>                 passwordHasher,
                          IEnumerable<IUserValidator<UserRecord>>     userValidators,
                          IEnumerable<IPasswordValidator<UserRecord>> passwordValidators,
                          ILookupNormalizer                           keyNormalizer,
                          IdentityErrorDescriber                      errors,
                          IServiceProvider                            services,
                          ILogger<UserManager>                        logger ) : UserManager<UserRecord>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
{
    protected readonly Database _database = database;


    public override async Task<IList<Claim>>      GetClaimsAsync( UserRecord       user )                                                                            => await _database.TryCall(GetClaimsAsync, user, CancellationToken.None);
    public async          ValueTask<IList<Claim>> GetClaimsAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token ) => await user.GetUserClaims(connection, transaction, _database, Claims.DEFAULTS, token);
}
