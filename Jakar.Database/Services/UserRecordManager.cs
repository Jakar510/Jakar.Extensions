// Jakar.Database ::  Jakar.Database 
// 04/11/2023  7:50 PM

namespace Jakar.Database;


public class UserRecordManager : UserManager<UserRecord>
{
    private readonly Database _database;
    public UserRecordManager( Database                                    database,
                              IUserStore<UserRecord>                      store,
                              IOptions<IdentityOptions>                   optionsAccessor,
                              IPasswordHasher<UserRecord>                 passwordHasher,
                              IEnumerable<IUserValidator<UserRecord>>     userValidators,
                              IEnumerable<IPasswordValidator<UserRecord>> passwordValidators,
                              ILookupNormalizer                           keyNormalizer,
                              IdentityErrorDescriber                      errors,
                              IServiceProvider                            services,
                              ILogger<UserManager<UserRecord>>            logger
    ) : base( store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger ) => _database = database;


    public override async Task<IList<Claim>> GetClaimsAsync( UserRecord user ) => await _database.TryCall( GetClaimsAsync, user, CancellationToken.None );
    public async ValueTask<IList<Claim>> GetClaimsAsync( DbConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        // return await base.GetClaimsAsync( user );
        return await user.GetUserClaims( connection, transaction, _database, ClaimType.UserID | ClaimType.UserName | ClaimType.Role | ClaimType.GroupSid, token );
    }
}
