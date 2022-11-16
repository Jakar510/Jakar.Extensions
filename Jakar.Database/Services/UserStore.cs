namespace Jakar.Database;


public sealed class UserStore : IUserStore
{
    private readonly Database _dbContext;


    public UserStore( Database dbContext ) => _dbContext = dbContext;


    public void Dispose() { }


    public Task<string> GetAuthenticatorKeyAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.ProviderKey ?? string.Empty );
    public async Task SetAuthenticatorKeyAsync( UserRecord user, string key, CancellationToken token )
    {
        user.ProviderKey = key;
        await _dbContext.Users.Update( user, token );
    }


    public async Task AddClaimsAsync( UserRecord user, IEnumerable<Claim> claims, CancellationToken token ) =>

        // TODO: 
        await Task.CompletedTask;
    public async Task<IList<Claim>> GetClaimsAsync( UserRecord user, CancellationToken token )
    {
        await using DbConnection connection = await _dbContext.ConnectAsync( token );
        return await user.GetUserClaims( connection, default, _dbContext, token );
    }
    public async Task<IList<UserRecord>> GetUsersForClaimAsync( Claim claim, CancellationToken token ) =>

        // TODO: 
        await Task.FromResult( new List<UserRecord>() );
    public async Task RemoveClaimsAsync( UserRecord user, IEnumerable<Claim> claims, CancellationToken token ) =>

        // TODO: 
        await Task.CompletedTask;
    public async Task ReplaceClaimAsync( UserRecord user, Claim claim, Claim newClaim, CancellationToken token ) =>

        // TODO: 
        await Task.CompletedTask;


    public async Task<UserRecord> FindByEmailAsync( string email, CancellationToken token ) => await _dbContext.Users.Get( nameof(UserRecord.Email), email, token ) ?? throw new RecordNotFoundException( email );

    public Task<string?> GetEmailAsync( UserRecord           user, CancellationToken token ) => Task.FromResult( user.Email );
    public Task<string?> GetNormalizedEmailAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.Email );
    public Task<bool> GetEmailConfirmedAsync( UserRecord     user, CancellationToken token ) => Task.FromResult( user.IsEmailConfirmed );
    public async Task SetEmailAsync( UserRecord user, string? email, CancellationToken token )
    {
        user.Email = email;
        await _dbContext.Users.Update( user, token );
    }

    public async Task SetEmailConfirmedAsync( UserRecord user, bool confirmed, CancellationToken token )
    {
        user.IsEmailConfirmed = confirmed;
        token.ThrowIfCancellationRequested();
        await ValueTask.CompletedTask;
    }

    public async Task SetNormalizedEmailAsync( UserRecord user, string? normalizedEmail, CancellationToken token )
    {
        user.Email = normalizedEmail;
        token.ThrowIfCancellationRequested();
        await ValueTask.CompletedTask;
    }
    public Task<int> GetAccessFailedCountAsync( UserRecord          user, CancellationToken token ) => Task.FromResult( user.BadLogins );
    public Task<bool> GetLockoutEnabledAsync( UserRecord            user, CancellationToken token ) => Task.FromResult( user.IsLocked );
    public Task<DateTimeOffset?> GetLockoutEndDateAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.LockoutEnd );


    public async Task<int> IncrementAccessFailedCountAsync( UserRecord user, CancellationToken token )
    {
        user.MarkBadLogin();
        await _dbContext.Users.Update( user, token );
        return user.BadLogins;
    }
    public async Task ResetAccessFailedCountAsync( UserRecord user, CancellationToken token )
    {
        user.Unlock();
        await _dbContext.Users.Update( user, token );
    }
    public async Task SetLockoutEnabledAsync( UserRecord user, bool enabled, CancellationToken token )
    {
        if (enabled) { user.Disable(); }
        else { user.Enable(); }

        await _dbContext.Users.Update( user, token );
    }
    public async Task SetLockoutEndDateAsync( UserRecord user, DateTimeOffset? lockoutEnd, CancellationToken token )
    {
        user.LockoutEnd = lockoutEnd;
        await _dbContext.Users.Update( user, token );
    }


    public async Task AddLoginAsync( UserRecord user, UserLoginInfo login, CancellationToken token )
    {
        user.AddUserLoginInfo( login );
        await _dbContext.Users.Update( user, token );
    }
    public async Task<UserRecord> FindByLoginAsync( string loginProvider, string providerKey, CancellationToken token )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserRecord.LoginProvider), loginProvider );
        parameters.Add( nameof(UserRecord.ProviderKey),   providerKey );

        return await _dbContext.Users.Get( true, parameters, token ) ?? new UserRecord();
    }
    public Task<IList<UserLoginInfo>> GetLoginsAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.GetUserLoginInfo() );
    public async Task RemoveLoginAsync( UserRecord user, string loginProvider, string providerKey, CancellationToken token )
    {
        user.RemoveUserLoginInfo();
        await _dbContext.Users.Update( user, token );
    }


    public Task<string> GetPasswordHashAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.PasswordHash );
    public Task<bool> HasPasswordAsync( UserRecord       user, CancellationToken token ) => Task.FromResult( user.HasPassword() );
    public async Task SetPasswordHashAsync( UserRecord user, string passwordHash, CancellationToken token )
    {
        // user.UpdatePassword(passwordHash);
        user.PasswordHash  = passwordHash;
        user.SecurityStamp = Randoms.GenerateToken();
        await _dbContext.Users.Update( user, token );
    }


    public Task<string?> GetPhoneNumberAsync( UserRecord       user, CancellationToken token ) => Task.FromResult( user.PhoneNumber );
    public Task<bool> GetPhoneNumberConfirmedAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.IsPhoneNumberConfirmed );
    public async Task SetPhoneNumberAsync( UserRecord user, string? phoneNumber, CancellationToken token )
    {
        user.PhoneNumber = phoneNumber;
        await _dbContext.Users.Update( user, token );
    }
    public async Task SetPhoneNumberConfirmedAsync( UserRecord user, bool confirmed, CancellationToken token )
    {
        user.IsPhoneNumberConfirmed = confirmed;
        await _dbContext.Users.Update( user, token );
    }


    public Task<string> GetSecurityStampAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.SecurityStamp ?? string.Empty );
    public async Task SetSecurityStampAsync( UserRecord user, string stamp, CancellationToken token )
    {
        user.SecurityStamp = stamp;
        await _dbContext.Users.Update( user, token );
    }


    public async Task<IdentityResult> CreateAsync( UserRecord user, CancellationToken token )
    {
        token.ThrowIfCancellationRequested();


        if (await _dbContext.Users.Get( nameof(UserRecord.UserName), user.UserName, token ) is not null)
        {
            return IdentityResult.Failed( new IdentityError
                                          {
                                              Description = "User Exists"
                                          } );
        }


        await _dbContext.Users.Insert( user, token );

        return IdentityResult.Success;
    }
    public async Task<IdentityResult> DeleteAsync( UserRecord user, CancellationToken token )
    {
        token.ThrowIfCancellationRequested();

        try
        {
            await _dbContext.Users.Delete( user.ID, token );
            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            return IdentityResult.Failed( new IdentityError
                                          {
                                              Description = e.Message
                                          } );
        }
    }


    public async Task<UserRecord> FindByIdAsync( string userId, CancellationToken token ) => await _dbContext.Users.Get( long.Parse( userId ), token ) ?? throw new RecordNotFoundException( userId );
    public async Task<UserRecord> FindByNameAsync( string normalizedUserName, CancellationToken token ) =>
        await _dbContext.Users.Get( nameof(UserRecord.FullName), normalizedUserName, token ) ?? throw new RecordNotFoundException( normalizedUserName );


    public Task<string> GetNormalizedUserNameAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.FullName ?? string.Empty );
    public Task<string> GetUserIdAsync( UserRecord             user, CancellationToken token ) => Task.FromResult( user.ID.ToString() );
    public Task<string> GetUserNameAsync( UserRecord           user, CancellationToken token ) => Task.FromResult( user.UserName );

    public async Task SetNormalizedUserNameAsync( UserRecord user, string fullName, CancellationToken token )
    {
        user.FullName = fullName;
        await _dbContext.Users.Update( user, token );
    }
    public async Task SetUserNameAsync( UserRecord user, string userName, CancellationToken token )
    {
        user.UserName = userName;
        await _dbContext.Users.Update( user, token );
    }


    public async Task<IdentityResult> UpdateAsync( UserRecord user, CancellationToken token )
    {
        await _dbContext.Users.Update( user, token );
        return IdentityResult.Success;
    }
    public Task<int> CountCodesAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.CountCodes() );
    public async Task<bool> RedeemCodeAsync( UserRecord user, string code, CancellationToken token )
    {
        await ValueTask.CompletedTask;
        if (token.IsCancellationRequested) { return false; }

        return user.RedeemCode( code );
    }
    public async Task ReplaceCodesAsync( UserRecord user, IEnumerable<string> recoveryCodes, CancellationToken token )
    {
        await ValueTask.CompletedTask;
        if (token.IsCancellationRequested) { return; }

        user.ReplaceCode( recoveryCodes );
    }


    public Task<bool> GetTwoFactorEnabledAsync( UserRecord user, CancellationToken token ) => Task.FromResult( user.IsTwoFactorEnabled );
    public async Task SetTwoFactorEnabledAsync( UserRecord user, bool enabled, CancellationToken token )
    {
        user.IsTwoFactorEnabled = enabled;
        await _dbContext.Users.Update( user, token );
    }
}
