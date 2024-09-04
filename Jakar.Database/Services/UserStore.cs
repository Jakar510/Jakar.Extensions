namespace Jakar.Database;


public interface IUserStore : IUserLoginStore<UserRecord>, IUserClaimStore<UserRecord>, IUserSecurityStampStore<UserRecord>, IUserTwoFactorStore<UserRecord>, IUserPasswordStore<UserRecord>, IUserEmailStore<UserRecord>, IUserLockoutStore<UserRecord>, IUserAuthenticatorKeyStore<UserRecord>, IUserTwoFactorRecoveryCodeStore<UserRecord>, IUserPhoneNumberStore<UserRecord> { }



[SuppressMessage( "ReSharper", "AccessToStaticMemberViaDerivedType" )]
public sealed class UserStore( Database dbContext ) : IUserStore
{
    private readonly Database _dbContext = dbContext;


    public static void Register( IServiceCollection builder ) =>
        builder.AddSingleton<UserStore>().AddTransient<IUserStore>( Get ).AddTransient<IUserStore<UserRecord>>( Get ).AddTransient<IUserLoginStore<UserRecord>>( Get ).AddTransient<IUserClaimStore<UserRecord>>( Get ).AddTransient<IUserPasswordStore<UserRecord>>( Get ).AddTransient<IUserSecurityStampStore<UserRecord>>( Get ).AddTransient<IUserTwoFactorStore<UserRecord>>( Get ).AddTransient<IUserEmailStore<UserRecord>>( Get ).AddTransient<IUserLockoutStore<UserRecord>>( Get ).AddTransient<IUserAuthenticatorKeyStore<UserRecord>>( Get ).AddTransient<IUserTwoFactorRecoveryCodeStore<UserRecord>>( Get ).AddTransient<IUserPhoneNumberStore<UserRecord>>( Get );


    public static UserStore                 Get( IServiceProvider provider )                                                                                      => provider.GetRequiredService<UserStore>();
    void IDisposable.                       Dispose()                                                                                                             { }
    public async Task<string?>              GetAuthenticatorKeyAsync( UserRecord        user,               CancellationToken   token )                           => await _dbContext.GetAuthenticatorKeyAsync( user, token );
    public async Task                       SetAuthenticatorKeyAsync( UserRecord        user,               string              key,    CancellationToken token ) => await _dbContext.SetAuthenticatorKeyAsync( user, key, token );
    public async Task                       AddClaimsAsync( UserRecord                  user,               IEnumerable<Claim>  claims, CancellationToken token ) => await _dbContext.AddClaimsAsync( user, claims, token );
    public async Task<IList<Claim>>         GetClaimsAsync( UserRecord                  user,               CancellationToken   token )                                                       => await _dbContext.GetClaimsAsync( user, ClaimType.All, token );
    public async Task<IList<UserRecord>>    GetUsersForClaimAsync( Claim                claim,              CancellationToken   token )                                                       => await _dbContext.GetUsersForClaimAsync( claim, token ).ToList( token );
    public async Task                       RemoveClaimsAsync( UserRecord               user,               IEnumerable<Claim>  claims, CancellationToken token )                             => await _dbContext.RemoveClaimsAsync( user, claims, token );
    public async Task                       ReplaceClaimAsync( UserRecord               user,               Claim               claim,  Claim             newClaim, CancellationToken token ) => await _dbContext.ReplaceClaimAsync( user, claim, newClaim, token );
    public async Task<UserRecord?>          FindByEmailAsync( string                    email,              CancellationToken   token )                                    => await _dbContext.FindByEmailAsync( email, token );
    public async Task<string?>              GetEmailAsync( UserRecord                   user,               CancellationToken   token )                                    => await _dbContext.GetEmailAsync( user, token );
    public async Task<string?>              GetNormalizedEmailAsync( UserRecord         user,               CancellationToken   token )                                    => await _dbContext.GetNormalizedEmailAsync( user, token );
    public async Task<bool>                 GetEmailConfirmedAsync( UserRecord          user,               CancellationToken   token )                                    => await _dbContext.GetEmailConfirmedAsync( user, token );
    public async Task                       SetEmailAsync( UserRecord                   user,               string?             email,           CancellationToken token ) => await _dbContext.SetEmailAsync( user, email, token );
    public async Task                       SetEmailConfirmedAsync( UserRecord          user,               bool                confirmed,       CancellationToken token ) => await _dbContext.SetEmailConfirmedAsync( user, confirmed, token );
    public async Task                       SetNormalizedEmailAsync( UserRecord         user,               string?             normalizedEmail, CancellationToken token ) => await _dbContext.SetNormalizedEmailAsync( user, normalizedEmail, token );
    public async Task<int>                  GetAccessFailedCountAsync( UserRecord       user,               CancellationToken   token )                                => await _dbContext.GetAccessFailedCountAsync( user, token );
    public async Task<bool>                 GetLockoutEnabledAsync( UserRecord          user,               CancellationToken   token )                                => await _dbContext.GetLockoutEnabledAsync( user, token );
    public async Task<DateTimeOffset?>      GetLockoutEndDateAsync( UserRecord          user,               CancellationToken   token )                                => await _dbContext.GetLockoutEndDateAsync( user, token );
    public async Task<int>                  IncrementAccessFailedCountAsync( UserRecord user,               CancellationToken   token )                                => await _dbContext.IncrementAccessFailedCountAsync( user, token );
    public async Task                       ResetAccessFailedCountAsync( UserRecord     user,               CancellationToken   token )                                => await _dbContext.ResetAccessFailedCountAsync( user, token );
    public async Task                       SetLockoutEnabledAsync( UserRecord          user,               bool                enabled,     CancellationToken token ) => await _dbContext.SetLockoutEnabledAsync( user, enabled, token );
    public async Task                       SetLockoutEndDateAsync( UserRecord          user,               DateTimeOffset?     lockoutEnd,  CancellationToken token ) => await _dbContext.SetLockoutEndDateAsync( user, lockoutEnd, token );
    public async Task                       AddLoginAsync( UserRecord                   user,               UserLoginInfo       login,       CancellationToken token ) => await _dbContext.AddLoginAsync( user, login, token );
    public async Task<UserRecord?>          FindByLoginAsync( string                    loginProvider,      string              providerKey, CancellationToken token ) => await _dbContext.FindByLoginAsync( loginProvider, providerKey, token );
    public async Task<IList<UserLoginInfo>> GetLoginsAsync( UserRecord                  user,               CancellationToken   token )                                                      => await _dbContext.GetLoginsAsync( user, token ).Select( static x => x.ToUserLoginInfo() ).ToList( token );
    public async Task                       RemoveLoginAsync( UserRecord                user,               string              loginProvider, string providerKey, CancellationToken token ) => await _dbContext.RemoveLoginAsync( user, loginProvider, providerKey, token );
    public async Task<IdentityResult>       CreateAsync( UserRecord                     user,               CancellationToken   token )                             => await _dbContext.CreateAsync( user, token );
    public async Task<IdentityResult>       DeleteAsync( UserRecord                     user,               CancellationToken   token )                             => await _dbContext.DeleteAsync( user, token );
    public async Task<UserRecord?>          FindByIdAsync( string                       userId,             CancellationToken   token )                             => await _dbContext.Users.Get( nameof(UserRecord.UserName), userId,             token );
    public async Task<UserRecord?>          FindByNameAsync( string                     normalizedUserName, CancellationToken   token )                             => await _dbContext.Users.Get( nameof(UserRecord.FullName), normalizedUserName, token );
    public async Task<string?>              GetNormalizedUserNameAsync( UserRecord      user,               CancellationToken   token )                             => await _dbContext.GetNormalizedUserNameAsync( user, token );
    public async Task<string>               GetUserIdAsync( UserRecord                  user,               CancellationToken   token )                             => await _dbContext.GetUserIdAsync( user, token );
    public async Task<string?>              GetUserNameAsync( UserRecord                user,               CancellationToken   token )                             => await _dbContext.GetUserNameAsync( user, token );
    public async Task                       SetNormalizedUserNameAsync( UserRecord      user,               string?             fullName, CancellationToken token ) => await _dbContext.SetNormalizedUserNameAsync( user, fullName, token );
    public async Task                       SetUserNameAsync( UserRecord                user,               string?             userName, CancellationToken token ) => await _dbContext.SetUserNameAsync( user, userName, token );
    public async Task<IdentityResult>       UpdateAsync( UserRecord                     user,               CancellationToken   token )                                => await _dbContext.UpdateAsync( user, token );
    public async Task<string?>              GetPhoneNumberAsync( UserRecord             user,               CancellationToken   token )                                => await _dbContext.GetPhoneNumberAsync( user, token );
    public async Task<bool>                 GetPhoneNumberConfirmedAsync( UserRecord    user,               CancellationToken   token )                                => await _dbContext.GetPhoneNumberConfirmedAsync( user, token );
    public async Task                       SetPhoneNumberAsync( UserRecord             user,               string?             phoneNumber, CancellationToken token ) => await _dbContext.SetPhoneNumberAsync( user, phoneNumber, token );
    public async Task                       SetPhoneNumberConfirmedAsync( UserRecord    user,               bool                confirmed,   CancellationToken token ) => await _dbContext.SetPhoneNumberConfirmedAsync( user, confirmed, token );
    public async Task<string?>              GetSecurityStampAsync( UserRecord           user,               CancellationToken   token )                          => await _dbContext.GetSecurityStampAsync( user, token );
    public async Task                       SetSecurityStampAsync( UserRecord           user,               string              stamp, CancellationToken token ) => await _dbContext.SetSecurityStampAsync( user, stamp, token );
    public async Task<int>                  CountCodesAsync( UserRecord                 user,               CancellationToken   token )                                  => (await user.Codes( _dbContext, token ).ToList( token )).Count;
    public async Task<bool>                 RedeemCodeAsync( UserRecord                 user,               string              code,          CancellationToken token ) => await user.RedeemCode( _dbContext, code, token );
    public async Task                       ReplaceCodesAsync( UserRecord               user,               IEnumerable<string> recoveryCodes, CancellationToken token ) => await user.ReplaceCodes( _dbContext, recoveryCodes, token );
    public async Task<bool>                 GetTwoFactorEnabledAsync( UserRecord        user,               CancellationToken   token )                                 => await _dbContext.GetTwoFactorEnabledAsync( user, token );
    public async Task                       SetTwoFactorEnabledAsync( UserRecord        user,               bool                enabled,      CancellationToken token ) => await _dbContext.SetTwoFactorEnabledAsync( user, enabled, token );
    public async Task                       SetPasswordHashAsync( UserRecord            user,               string?             passwordHash, CancellationToken token ) => await _dbContext.SetPasswordHashAsync( user, passwordHash, token );
    public async Task<string?>              GetPasswordHashAsync( UserRecord            user,               CancellationToken   token ) => await _dbContext.GetPasswordHashAsync( user, token );
    public async Task<bool>                 HasPasswordAsync( UserRecord                user,               CancellationToken   token ) => await _dbContext.HasPasswordAsync( user, token );
}
