#pragma warning disable IDE0060
#pragma warning disable CA1822



namespace Jakar.Database;


[SuppressMessage("ReSharper", "UnusedParameter.Global")][SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
public partial class Database
{
    public virtual ValueTask<string?> GetSecurityStampAsync( UserRecord user, CancellationToken token = default )                => new(user.SecurityStamp);
    public         ValueTask          SetSecurityStampAsync( UserRecord user, string            stamp, CancellationToken token ) => this.TryCall(SetSecurityStampAsync, user, stamp, token);
    public async ValueTask SetSecurityStampAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, string stamp, CancellationToken token )
    {
        user.SecurityStamp = stamp;
        await Users.Update(connection, transaction, user, token);
    }


    public       Task<string?> GetPasswordHashAsync( UserRecord? user, CancellationToken token )                                 => Task.FromResult(user?.PasswordHash);
    public       Task<bool>    HasPasswordAsync( UserRecord      user, CancellationToken token )                                 => Task.FromResult(user.HasPassword());
    public async Task          SetPasswordHashAsync( UserRecord  user, string?           passwordHash, CancellationToken token ) => await this.TryCall(SetPasswordHashAsync, user, passwordHash, token);
    public async ValueTask SetPasswordHashAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, string? passwordHash, CancellationToken token )
    {
        user.PasswordHash = passwordHash ?? string.Empty;
        await Users.Update(user, token);
    }



    #region User Auth Providers

    public IAsyncEnumerable<UserLoginProviderRecord> GetLoginsAsync<TSelf>( TSelf record, [EnumeratorCancellation] CancellationToken token )
        where TSelf : OwnedTableRecord<TSelf>, ITableRecord<TSelf> => this.TryCall(GetLoginsAsync, record, token);
    public virtual IAsyncEnumerable<UserLoginProviderRecord> GetLoginsAsync<TSelf>( NpgsqlConnection connection, DbTransaction transaction, TSelf record, [EnumeratorCancellation] CancellationToken token )
        where TSelf : OwnedTableRecord<TSelf>, ITableRecord<TSelf> => UserLogins.Where(connection, transaction, nameof(record.CreatedBy), record.CreatedBy, token);


    public ValueTask<ErrorOrResult<UserLoginProviderRecord>> AddLoginAsync( UserRecord user, UserLoginInfo login, CancellationToken token ) => this.TryCall(AddLoginAsync, user, login, token);
    public virtual async ValueTask<ErrorOrResult<UserLoginProviderRecord>> AddLoginAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, UserLoginInfo login, CancellationToken token )
    {
        UserLoginProviderRecord? record = await UserLogins.Get(connection, transaction, true, UserLoginProviderRecord.GetDynamicParameters(user, login), token);

        if ( record is not null )
        {
            Error  provider = Error.NotFound(nameof(UserLoginProviderRecord.LoginProvider), login.LoginProvider);
            Error  key      = Error.NotFound(nameof(UserLoginProviderRecord.ProviderKey),   login.ProviderKey);
            Error  userID   = Error.NotFound(nameof(UserLoginProviderRecord.CreatedBy),     user.ID.Value.ToString());
            Errors errors   = Errors.Create(provider, key, userID);
            return ErrorOrResult<UserLoginProviderRecord>.Create(errors);
        }

        record = new UserLoginProviderRecord(user, login);
        record = await UserLogins.Insert(connection, transaction, record, token);
        await SetAuthenticatorKeyAsync(connection, transaction, user, record.ProviderKey, token);
        return record;
    }


    public ValueTask<UserRecord?> FindByLoginAsync( string loginProvider, string providerKey, CancellationToken token ) => this.TryCall(FindByLoginAsync, loginProvider, providerKey, token);
    public virtual async ValueTask<UserRecord?> FindByLoginAsync( NpgsqlConnection connection, DbTransaction transaction, string loginProvider, string providerKey, CancellationToken token )
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(UserLoginProviderRecord.LoginProvider), loginProvider);
        parameters.Add(nameof(UserLoginProviderRecord.ProviderKey),   providerKey);
        return await Users.Get(connection, transaction, true, parameters, token);
    }


    public ValueTask RemoveLoginAsync( UserRecord user, string loginProvider, string providerKey, CancellationToken token ) => this.TryCall(RemoveLoginAsync, user, loginProvider, providerKey, token);
    public virtual async ValueTask RemoveLoginAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, string loginProvider, string providerKey, CancellationToken token )
    {
        PostgresParameters                        parameters = UserLoginProviderRecord.GetDynamicParameters(user, loginProvider, providerKey);
        IAsyncEnumerable<UserLoginProviderRecord> records    = UserLogins.Where(connection, transaction, true, parameters, token);
        await foreach ( UserLoginProviderRecord record in records ) { await UserLogins.Delete(connection, transaction, record, token); }
    }


    public         ValueTask<string?> GetAuthenticatorKeyAsync( UserRecord       user,       CancellationToken token )                                                 => this.TryCall(GetAuthenticatorKeyAsync, user, token);
    public virtual ValueTask<string?> GetAuthenticatorKeyAsync( NpgsqlConnection connection, DbTransaction     transaction, UserRecord user, CancellationToken token ) => new(user.AuthenticatorKey);


    public ValueTask SetAuthenticatorKeyAsync( UserRecord user, string key, CancellationToken token ) => this.TryCall(SetAuthenticatorKeyAsync, user, key, token);
    public virtual async ValueTask SetAuthenticatorKeyAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, string key, CancellationToken token )
    {
        user.AuthenticatorKey = key;
        await Users.Update(connection, transaction, user, token);
    }


    public virtual ValueTask<bool> GetTwoFactorEnabledAsync( UserRecord user, CancellationToken token )                            => new(user.IsTwoFactorEnabled);
    public         ValueTask       SetTwoFactorEnabledAsync( UserRecord user, bool              enabled, CancellationToken token ) => this.TryCall(SetTwoFactorEnabledAsync, user, enabled, token);
    public virtual async ValueTask SetTwoFactorEnabledAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, bool enabled, CancellationToken token )
    {
        user.IsTwoFactorEnabled = enabled;
        await Users.Update(connection, transaction, user, token);
    }

    #endregion



    #region User Email

    public ValueTask<string?> GetEmailAsync( UserRecord           user, CancellationToken token = default ) => new(user.Email);
    public ValueTask<string?> GetNormalizedEmailAsync( UserRecord user, CancellationToken token = default ) => new(user.Email);
    public ValueTask<bool>    GetEmailConfirmedAsync( UserRecord  user, CancellationToken token = default ) => new(user.IsEmailConfirmed);


    public ValueTask SetEmailAsync( UserRecord user, string? email, CancellationToken token ) => this.TryCall(SetEmailAsync, user, email, token);
    public async ValueTask SetEmailAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, string? email, CancellationToken token )
    {
        user.Email = email ?? string.Empty;
        await Users.Update(connection, transaction, user, token);
    }


    public ValueTask SetEmailConfirmedAsync( UserRecord user, bool confirmed, CancellationToken token ) => this.TryCall(SetEmailConfirmedAsync, user, confirmed, token);
    public async ValueTask SetEmailConfirmedAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, bool confirmed, CancellationToken token )
    {
        user.IsEmailConfirmed = confirmed;
        await Users.Update(connection, transaction, user, token);
    }


    public ValueTask SetNormalizedEmailAsync( UserRecord user, string? normalizedEmail, CancellationToken token ) => this.TryCall(SetNormalizedEmailAsync, user, normalizedEmail, token);
    public async ValueTask SetNormalizedEmailAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, string? normalizedEmail, CancellationToken token )
    {
        user.Email = normalizedEmail ?? string.Empty;
        await Users.Update(connection, transaction, user, token);
    }


    public ValueTask<int>             GetAccessFailedCountAsync( UserRecord user, CancellationToken token = default ) => new(user.BadLogins ?? 0);
    public ValueTask<bool>            GetLockoutEnabledAsync( UserRecord    user, CancellationToken token = default ) => new(user.IsLocked);
    public ValueTask<DateTimeOffset?> GetLockoutEndDateAsync( UserRecord    user, CancellationToken token = default ) => new(user.LockoutEnd);

    #endregion



    #region User Phone Number

    public virtual ValueTask<string?> GetPhoneNumberAsync( UserRecord          user, CancellationToken token = default ) => new(user.PhoneNumber);
    public virtual ValueTask<bool>    GetPhoneNumberConfirmedAsync( UserRecord user, CancellationToken token = default ) => new(user.IsPhoneNumberConfirmed);


    public ValueTask SetPhoneNumberAsync( UserRecord user, string? phoneNumber, CancellationToken token ) => this.TryCall(SetPhoneNumberAsync, user, phoneNumber, token);
    public virtual async ValueTask SetPhoneNumberAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, string? phoneNumber, CancellationToken token )
    {
        user.PhoneNumber = phoneNumber ?? string.Empty;
        await Users.Update(connection, transaction, user, token);
    }


    public ValueTask SetPhoneNumberConfirmedAsync( UserRecord user, bool confirmed, CancellationToken token ) => this.TryCall(SetPhoneNumberConfirmedAsync, user, confirmed, token);
    public virtual async ValueTask SetPhoneNumberConfirmedAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, bool confirmed, CancellationToken token )
    {
        user.IsPhoneNumberConfirmed = confirmed;
        await Users.Update(connection, transaction, user, token);
    }

    #endregion



    #region User Lock/Unlock

    public ValueTask<int> IncrementAccessFailedCountAsync( UserRecord user, CancellationToken token ) => this.TryCall(IncrementAccessFailedCountAsync, user, token);
    public virtual async ValueTask<int> IncrementAccessFailedCountAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        user = user.MarkBadLogin();
        await Users.Update(connection, transaction, user, token);
        return user.BadLogins ?? 0;
    }


    public ValueTask ResetAccessFailedCountAsync( UserRecord user, CancellationToken token ) => this.TryCall(ResetAccessFailedCountAsync, user, token);
    public virtual async ValueTask ResetAccessFailedCountAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        user = user.Unlock();
        await Users.Update(connection, transaction, user, token);
    }


    public ValueTask SetLockoutEnabledAsync( UserRecord user, bool enabled, CancellationToken token ) => this.TryCall(SetLockoutEnabledAsync, user, enabled, token);
    public virtual async ValueTask SetLockoutEnabledAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, bool enabled, CancellationToken token )
    {
        user = enabled
                   ? user.Disable()
                   : user.Enable();

        await Users.Update(connection, transaction, user, token);
    }


    public ValueTask SetLockoutEndDateAsync( UserRecord user, DateTimeOffset? lockoutEnd, CancellationToken token ) => this.TryCall(SetLockoutEndDateAsync, user, lockoutEnd, token);
    public virtual async ValueTask SetLockoutEndDateAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, DateTimeOffset? lockoutEnd, CancellationToken token )
    {
        user.LockoutEnd = lockoutEnd;
        await Users.Update(connection, transaction, user, token);
    }

    #endregion



    #region Find User

    public ValueTask<UserRecord?> FindByIdAsync( string userID, CancellationToken token ) => this.TryCall(FindByIdAsync, userID, token);
    public virtual async ValueTask<UserRecord?> FindByIdAsync( NpgsqlConnection connection, DbTransaction transaction, string userID, CancellationToken token ) =>
        Guid.TryParse(userID, out Guid guid)
            ? await Users.Get(connection, transaction, nameof(UserRecord.ID),       guid,   token)
            : await Users.Get(connection, transaction, nameof(UserRecord.UserName), userID, token);


    public static readonly StringTags                           UserFullName = new([nameof(UserRecord.UserName), nameof(UserRecord.FullName)]);
    public                 ValueTask<ErrorOrResult<UserRecord>> FindByNameAsync( string normalizedUserName, CancellationToken token ) => this.TryCall(FindByNameAsync, normalizedUserName, token);
    public virtual async ValueTask<ErrorOrResult<UserRecord>> FindByNameAsync( NpgsqlConnection connection, DbTransaction transaction, string normalizedUserName, CancellationToken token )
    {
        ErrorOrResult<UserRecord> user = await Users.Get(connection, transaction, nameof(UserRecord.UserName), normalizedUserName, token);
        if ( user.HasValue ) { return user; }

        user = await Users.Get(connection, transaction, nameof(UserRecord.FullName), normalizedUserName, token);
        if ( user.HasValue ) { return user; }

        return Error.NotFound(UserFullName, normalizedUserName);
    }


    public       ValueTask<UserRecord?> FindByEmailAsync( string           email,      CancellationToken token )                                              => this.TryCall(FindByEmailAsync, email, token);
    public async ValueTask<UserRecord?> FindByEmailAsync( NpgsqlConnection connection, DbTransaction     transaction, string email, CancellationToken token ) => await Users.Get(connection, transaction, nameof(UserRecord.Email), email, token);

    #endregion



    #region User CRUD

    public ValueTask<IdentityResult> CreateAsync( UserRecord user, CancellationToken token ) => this.TryCall(CreateAsync, user, token);
    public virtual async ValueTask<IdentityResult> CreateAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord record, CancellationToken token )
    {
        ErrorOrResult<UserRecord> user = await Users.Get(connection, transaction, true, UserRecord.GetDynamicParameters(record.UserName), token);
        if ( user.HasValue ) { return IdentityResult.Failed(new IdentityError { Description = Options.UserExists }); }

        await Users.Insert(connection, transaction, record, token);
        return IdentityResult.Success;
    }


    public ValueTask<IdentityResult> DeleteAsync( UserRecord user, CancellationToken token ) => this.TryCall(DeleteAsync, user, token);
    public virtual async ValueTask<IdentityResult> DeleteAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        try
        {
            await Users.Delete(connection, transaction, user.ID, token);
            return IdentityResult.Success;
        }
        catch ( Exception e ) { return IdentityResult.Failed(new IdentityError { Description = e.Message }); }
    }


    public ValueTask<IdentityResult> UpdateAsync( UserRecord user, CancellationToken token ) => this.TryCall(UpdateAsync, user, token);
    public async ValueTask<IdentityResult> UpdateAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        try
        {
            await Users.Update(connection, transaction, user, token);
            return IdentityResult.Success;
        }
        catch ( Exception e ) { return IdentityResult.Failed(new IdentityError { Description = e.Message }); }
    }

    #endregion



    #region User UserName

    public virtual ValueTask<string> GetNormalizedUserNameAsync( UserRecord user, CancellationToken token = default ) => new(user.UserName);
    public virtual ValueTask<string> GetUserIdAsync( UserRecord             user, CancellationToken token = default ) => new(user.ID.ToString());
    public virtual ValueTask<string> GetUserNameAsync( UserRecord           user, CancellationToken token = default ) => new(user.UserName);


    public ValueTask SetNormalizedUserNameAsync( UserRecord user, string? fullName, CancellationToken token ) => this.TryCall(SetNormalizedUserNameAsync, user, fullName, token);
    public virtual async ValueTask SetNormalizedUserNameAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, string? fullName, CancellationToken token )
    {
        user.FullName = fullName ?? string.Empty;
        await Users.Update(connection, transaction, user, token);
    }


    public ValueTask SetUserNameAsync( UserRecord user, string? userName, CancellationToken token ) => this.TryCall(SetUserNameAsync, user, userName, token);
    public virtual async ValueTask SetUserNameAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user, string? userName, CancellationToken token )
    {
        user = user with { UserName = userName ?? string.Empty };
        await Users.Update(connection, transaction, user, token);
    }

    #endregion



    #region Claims

    public         ValueTask AddClaimsAsync( UserRecord       user,       IEnumerable<Claim> claims,      CancellationToken token )                                                    => this.TryCall(AddClaimsAsync, user, claims, token);
    public virtual ValueTask AddClaimsAsync( NpgsqlConnection connection, DbTransaction      transaction, UserRecord        user, IEnumerable<Claim> claims, CancellationToken token ) => ValueTask.CompletedTask;


    public         ValueTask<Claim[]> GetClaimsAsync( UserRecord       user,       ClaimType      types,       CancellationToken token )                                          => this.Call(GetClaimsAsync, user, types, token);
    public virtual ValueTask<Claim[]> GetClaimsAsync( NpgsqlConnection connection, DbTransaction? transaction, UserRecord        user, ClaimType types, CancellationToken token ) => user.GetUserClaims(connection, transaction, this, types, token);


    public IAsyncEnumerable<UserRecord> GetUsersForClaimAsync( Claim claim, [EnumeratorCancellation] CancellationToken token ) => this.TryCall(GetUsersForClaimAsync, claim, token);
    public virtual async IAsyncEnumerable<UserRecord> GetUsersForClaimAsync( NpgsqlConnection connection, DbTransaction transaction, Claim claim, [EnumeratorCancellation] CancellationToken token )
    {
        await foreach ( UserRecord record in UserRecord.TryFromClaims(connection, transaction, this, claim, token) ) { yield return record; }
    }


    public         ValueTask RemoveClaimsAsync( UserRecord       user,       IEnumerable<Claim> claims,      CancellationToken token )                                                    => this.TryCall(RemoveClaimsAsync, user, claims, token);
    public virtual ValueTask RemoveClaimsAsync( NpgsqlConnection connection, DbTransaction      transaction, UserRecord        user, IEnumerable<Claim> claims, CancellationToken token ) => ValueTask.CompletedTask;


    public         ValueTask ReplaceClaimAsync( UserRecord       user,       Claim         claim,       Claim      newClaim, CancellationToken token )                                          => this.TryCall(ReplaceClaimAsync, user, claim, newClaim, token);
    public virtual ValueTask ReplaceClaimAsync( NpgsqlConnection connection, DbTransaction transaction, UserRecord user,     Claim             claim, Claim newClaim, CancellationToken token ) => ValueTask.CompletedTask;

    #endregion
}
