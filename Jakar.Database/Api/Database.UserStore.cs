﻿#pragma warning disable IDE0060
#pragma warning disable CA1822


using System.Security.Claims;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "UnusedParameter.Global" )]
public partial class Database
{
    public virtual ValueTask<string?> GetSecurityStampAsync( UserRecord user, CancellationToken token = default ) => new(user.SecurityStamp);
    public ValueTask SetSecurityStampAsync( UserRecord                  user, string            stamp, CancellationToken token ) => this.TryCall( SetSecurityStampAsync, user, stamp, token );
    public async ValueTask SetSecurityStampAsync( DbConnection connection, DbTransaction transaction, UserRecord user, string stamp, CancellationToken token )
    {
        user.SecurityStamp = stamp;
        await Users.Update( user, token );
    }



    #region User Auth Providers

    public ValueTask<UserLoginInfo[]> GetLoginsAsync( UserRecord user, CancellationToken token ) => this.TryCall( GetLoginsAsync, user, token );
    public virtual async ValueTask<UserLoginInfo[]> GetLoginsAsync( DbConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        UserLoginInfoRecord[] records = await UserLogins.Where( connection, transaction, nameof(UserRecord.UserID), user.UserID, token );

        return records.Select( x => x.ToUserLoginInfo() )
                      .ToArray();
    }


    public ValueTask<UserLoginInfoRecord> AddLoginAsync( UserRecord user, UserLoginInfo login, CancellationToken token ) => this.TryCall( AddLoginAsync, user, login, token );
    public virtual async ValueTask<UserLoginInfoRecord> AddLoginAsync( DbConnection connection, DbTransaction transaction, UserRecord user, UserLoginInfo login, CancellationToken token )
    {
        UserLoginInfoRecord? record = await UserLogins.Get( connection, transaction, true, UserLoginInfoRecord.GetDynamicParameters( user, login ), token );

        if ( record is not null )
        {
            string message = $"[ {nameof(login.LoginProvider)}: {login.LoginProvider}  |  {nameof(login.ProviderKey)}: {login.ProviderKey} ] already exists for {nameof(UserRecord.UserID)}: {user.UserID}";
            throw new DuplicateRecordException( message );
        }

        record = new UserLoginInfoRecord( user, login );
        record = await UserLogins.Insert( connection, transaction, record, token );
        await SetAuthenticatorKeyAsync( connection, transaction, user, record.ProviderKey, token );
        return record;
    }


    public ValueTask<UserRecord?> FindByLoginAsync( string loginProvider, string providerKey, CancellationToken token ) => this.TryCall( FindByLoginAsync, loginProvider, providerKey, token );
    public virtual async ValueTask<UserRecord?> FindByLoginAsync( DbConnection connection, DbTransaction transaction, string loginProvider, string providerKey, CancellationToken token )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserLoginInfoRecord.LoginProvider), loginProvider );
        parameters.Add( nameof(UserLoginInfoRecord.ProviderKey),   providerKey );
        return await Users.Get( true, parameters, token );
    }


    public ValueTask RemoveLoginAsync( UserRecord user, string loginProvider, string providerKey, CancellationToken token ) => this.TryCall( RemoveLoginAsync, user, loginProvider, providerKey, token );
    public virtual async ValueTask RemoveLoginAsync( DbConnection connection, DbTransaction transaction, UserRecord user, string loginProvider, string providerKey, CancellationToken token )
    {
        UserLoginInfoRecord[] records = await UserLogins.Where( connection, transaction, true, UserLoginInfoRecord.GetDynamicParameters( user, loginProvider, providerKey ), token );
        foreach ( UserLoginInfoRecord record in records ) { await UserLogins.Delete( connection, transaction, record, token ); }
    }


    public ValueTask<string?> GetAuthenticatorKeyAsync( UserRecord           user,       CancellationToken token ) => this.TryCall( GetAuthenticatorKeyAsync, user, token );
    public virtual ValueTask<string?> GetAuthenticatorKeyAsync( DbConnection connection, DbTransaction     transaction, UserRecord user, CancellationToken token ) => new(user.SecurityStamp);


    public ValueTask SetAuthenticatorKeyAsync( UserRecord user, string key, CancellationToken token ) => this.TryCall( SetAuthenticatorKeyAsync, user, key, token );
    public virtual async ValueTask SetAuthenticatorKeyAsync( DbConnection connection, DbTransaction transaction, UserRecord user, string key, CancellationToken token )
    {
        user.SecurityStamp = key;
        await Users.Update( connection, transaction, user, token );
    }


    public virtual ValueTask<bool> GetTwoFactorEnabledAsync( UserRecord user, CancellationToken token ) => new(user.IsTwoFactorEnabled);
    public ValueTask SetTwoFactorEnabledAsync( UserRecord               user, bool              enabled, CancellationToken token ) => this.TryCall( SetTwoFactorEnabledAsync, user, enabled, token );
    public virtual async ValueTask SetTwoFactorEnabledAsync( DbConnection connection, DbTransaction transaction, UserRecord user, bool enabled, CancellationToken token )
    {
        user.IsTwoFactorEnabled = enabled;
        await Users.Update( connection, transaction, user, token );
    }

    #endregion



    #region User Email

    public ValueTask<string?> GetEmailAsync( UserRecord           user, CancellationToken token = default ) => new(user.Email);
    public ValueTask<string?> GetNormalizedEmailAsync( UserRecord user, CancellationToken token = default ) => new(user.Email);
    public ValueTask<bool> GetEmailConfirmedAsync( UserRecord     user, CancellationToken token = default ) => new(user.IsEmailConfirmed);


    public ValueTask SetEmailAsync( UserRecord user, string? email, CancellationToken token ) => this.TryCall( SetEmailAsync, user, email, token );
    public async ValueTask SetEmailAsync( DbConnection connection, DbTransaction transaction, UserRecord user, string? email, CancellationToken token )
    {
        user.Email = email;
        await Users.Update( connection, transaction, user, token );
    }


    public ValueTask SetEmailConfirmedAsync( UserRecord user, bool confirmed, CancellationToken token ) => this.TryCall( SetEmailConfirmedAsync, user, confirmed, token );
    public async ValueTask SetEmailConfirmedAsync( DbConnection connection, DbTransaction transaction, UserRecord user, bool confirmed, CancellationToken token )
    {
        user.IsEmailConfirmed = confirmed;
        await Users.Update( connection, transaction, user, token );
    }


    public ValueTask SetNormalizedEmailAsync( UserRecord user, string? normalizedEmail, CancellationToken token ) => this.TryCall( SetNormalizedEmailAsync, user, normalizedEmail, token );
    public async ValueTask SetNormalizedEmailAsync( DbConnection connection, DbTransaction transaction, UserRecord user, string? normalizedEmail, CancellationToken token )
    {
        user.Email = normalizedEmail;
        await Users.Update( connection, transaction, user, token );
    }


    public ValueTask<int> GetAccessFailedCountAsync( UserRecord          user, CancellationToken token = default ) => new(user.BadLogins);
    public ValueTask<bool> GetLockoutEnabledAsync( UserRecord            user, CancellationToken token = default ) => new(user.IsLocked);
    public ValueTask<DateTimeOffset?> GetLockoutEndDateAsync( UserRecord user, CancellationToken token = default ) => new(user.LockoutEnd);

    #endregion



    #region User Phone Number

    public virtual ValueTask<string?> GetPhoneNumberAsync( UserRecord       user, CancellationToken token = default ) => new(user.PhoneNumber);
    public virtual ValueTask<bool> GetPhoneNumberConfirmedAsync( UserRecord user, CancellationToken token = default ) => new(user.IsPhoneNumberConfirmed);


    public ValueTask SetPhoneNumberAsync( UserRecord user, string? phoneNumber, CancellationToken token ) => this.TryCall( SetPhoneNumberAsync, user, phoneNumber, token );
    public virtual async ValueTask SetPhoneNumberAsync( DbConnection connection, DbTransaction transaction, UserRecord user, string? phoneNumber, CancellationToken token )
    {
        user.PhoneNumber = phoneNumber;
        await Users.Update( user, token );
    }


    public ValueTask SetPhoneNumberConfirmedAsync( UserRecord user, bool confirmed, CancellationToken token ) => this.TryCall( SetPhoneNumberConfirmedAsync, user, confirmed, token );
    public virtual async ValueTask SetPhoneNumberConfirmedAsync( DbConnection connection, DbTransaction transaction, UserRecord user, bool confirmed, CancellationToken token )
    {
        user.IsPhoneNumberConfirmed = confirmed;
        await Users.Update( user, token );
    }

    #endregion



    #region User Lock/Unlock

    public ValueTask<int> IncrementAccessFailedCountAsync( UserRecord user, CancellationToken token ) => this.TryCall( IncrementAccessFailedCountAsync, user, token );
    public virtual async ValueTask<int> IncrementAccessFailedCountAsync( DbConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        user.MarkBadLogin();
        await Users.Update( connection, transaction, user, token );
        return user.BadLogins;
    }


    public ValueTask ResetAccessFailedCountAsync( UserRecord user, CancellationToken token ) => this.TryCall( ResetAccessFailedCountAsync, user, token );
    public virtual async ValueTask ResetAccessFailedCountAsync( DbConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        user.Unlock();
        await Users.Update( connection, transaction, user, token );
    }


    public ValueTask SetLockoutEnabledAsync( UserRecord user, bool enabled, CancellationToken token ) => this.TryCall( SetLockoutEnabledAsync, user, enabled, token );
    public virtual async ValueTask SetLockoutEnabledAsync( DbConnection connection, DbTransaction transaction, UserRecord user, bool enabled, CancellationToken token )
    {
        if ( enabled ) { user.Disable(); }
        else { user.Enable(); }

        await Users.Update( connection, transaction, user, token );
    }


    public ValueTask SetLockoutEndDateAsync( UserRecord user, DateTimeOffset? lockoutEnd, CancellationToken token ) => this.TryCall( SetLockoutEndDateAsync, user, lockoutEnd, token );
    public virtual async ValueTask SetLockoutEndDateAsync( DbConnection connection, DbTransaction transaction, UserRecord user, DateTimeOffset? lockoutEnd, CancellationToken token )
    {
        user.LockoutEnd = lockoutEnd;
        await Users.Update( connection, transaction, user, token );
    }

    #endregion



    #region Find User

    public ValueTask<UserRecord?> FindByIdAsync( string userID, CancellationToken token ) => this.TryCall( FindByIdAsync, userID, token );
    public virtual async ValueTask<UserRecord?> FindByIdAsync( DbConnection connection, DbTransaction transaction, string userID, CancellationToken token ) =>
        Guid.TryParse( userID, out Guid guid )
            ? await Users.Get( connection, transaction, nameof(UserRecord.UserID), guid, token )
            : long.TryParse( userID, out long id )
                ? await Users.Get( connection, transaction, id,                          token )
                : await Users.Get( connection, transaction, nameof(UserRecord.UserName), userID, token );


    public ValueTask<UserRecord?> FindByNameAsync( string normalizedUserName, CancellationToken token ) => this.TryCall( FindByNameAsync, normalizedUserName, token );
    public virtual async ValueTask<UserRecord?> FindByNameAsync( DbConnection connection, DbTransaction transaction, string normalizedUserName, CancellationToken token ) =>
        await Users.Get( connection, transaction, nameof(UserRecord.UserName), normalizedUserName, token ) ?? await Users.Get( connection, transaction, nameof(UserRecord.FullName), normalizedUserName, token );


    public ValueTask<UserRecord?> FindByEmailAsync( string email, CancellationToken token ) => this.TryCall( FindByEmailAsync, email, token );
    public async ValueTask<UserRecord?> FindByEmailAsync( DbConnection connection, DbTransaction transaction, string email, CancellationToken token ) =>
        await Users.Get( connection, transaction, nameof(UserRecord.Email), email, token );

    #endregion



    #region User CRUD

    public ValueTask<IdentityResult> CreateAsync( UserRecord user, CancellationToken token ) => this.TryCall( CreateAsync, user, token );
    public virtual async ValueTask<IdentityResult> CreateAsync( DbConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        if ( await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( user.UserName ), token ) is not null )
        {
            return IdentityResult.Failed( new IdentityError
                                          {
                                              Description = Options.UserExists
                                          } );
        }


        await Users.Insert( connection, transaction, user, token );
        return IdentityResult.Success;
    }


    public ValueTask<IdentityResult> DeleteAsync( UserRecord user, CancellationToken token ) => this.TryCall( DeleteAsync, user, token );
    public virtual async ValueTask<IdentityResult> DeleteAsync( DbConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        try
        {
            await Users.Delete( connection, transaction, user.ID, token );
            return IdentityResult.Success;
        }
        catch ( Exception e )
        {
            return IdentityResult.Failed( new IdentityError
                                          {
                                              Description = e.Message
                                          } );
        }
    }


    public ValueTask<IdentityResult> UpdateAsync( UserRecord user, CancellationToken token ) => this.TryCall( UpdateAsync, user, token );
    public async ValueTask<IdentityResult> UpdateAsync( DbConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token )
    {
        try
        {
            await Users.Update( connection, transaction, user, token );
            return IdentityResult.Success;
        }
        catch ( Exception e )
        {
            return IdentityResult.Failed( new IdentityError
                                          {
                                              Description = e.Message
                                          } );
        }
    }

    #endregion



    #region User UserName

    public virtual ValueTask<string> GetNormalizedUserNameAsync( UserRecord user, CancellationToken token = default ) => new(user.UserName);
    public virtual ValueTask<string> GetUserIdAsync( UserRecord             user, CancellationToken token = default ) => new(user.UserID.ToString());
    public virtual ValueTask<string> GetUserNameAsync( UserRecord           user, CancellationToken token = default ) => new(user.UserName);


    public ValueTask SetNormalizedUserNameAsync( UserRecord user, string? fullName, CancellationToken token ) => this.TryCall( SetNormalizedUserNameAsync, user, fullName, token );
    public virtual async ValueTask SetNormalizedUserNameAsync( DbConnection connection, DbTransaction transaction, UserRecord user, string? fullName, CancellationToken token )
    {
        user.FullName = fullName;
        await Users.Update( connection, transaction, user, token );
    }


    public ValueTask SetUserNameAsync( UserRecord user, string? userName, CancellationToken token ) => this.TryCall( SetUserNameAsync, user, userName, token );
    public virtual async ValueTask SetUserNameAsync( DbConnection connection, DbTransaction transaction, UserRecord user, string? userName, CancellationToken token )
    {
        user.UserName = userName ?? string.Empty;
        await Users.Update( connection, transaction, user, token );
    }

    #endregion



    #region Claims

    public ValueTask AddClaimsAsync( UserRecord           user,       IEnumerable<Claim> claims,      CancellationToken token ) => this.TryCall( AddClaimsAsync, user, claims, token );
    public virtual ValueTask AddClaimsAsync( DbConnection connection, DbTransaction      transaction, UserRecord        user, IEnumerable<Claim> claims, CancellationToken token ) => ValueTask.CompletedTask;


    public ValueTask<IList<Claim>> GetClaimsAsync( UserRecord                 user,       CancellationToken token ) => this.Call( GetClaimsAsync, user, token );
    public virtual async ValueTask<IList<Claim>> GetClaimsAsync( DbConnection connection, DbTransaction?    transaction, UserRecord user, CancellationToken token ) => await user.GetUserClaims( connection, transaction, this, token );


    public ValueTask<IList<UserRecord>> GetUsersForClaimAsync( Claim                claim,      CancellationToken token ) => this.TryCall( GetUsersForClaimAsync, claim, token );
    public virtual ValueTask<IList<UserRecord>> GetUsersForClaimAsync( DbConnection connection, DbTransaction     transaction, Claim claim, CancellationToken token ) => new(new List<UserRecord>());


    public ValueTask RemoveClaimsAsync( UserRecord           user,       IEnumerable<Claim> claims,      CancellationToken token ) => this.TryCall( AddClaimsAsync, user, claims, token );
    public virtual ValueTask RemoveClaimsAsync( DbConnection connection, DbTransaction      transaction, UserRecord        user, IEnumerable<Claim> claims, CancellationToken token ) => ValueTask.CompletedTask;


    public ValueTask ReplaceClaimAsync( UserRecord           user,       Claim         claim,       Claim      newClaim, CancellationToken token ) => this.TryCall( ReplaceClaimAsync, user, claim, newClaim, token );
    public virtual ValueTask ReplaceClaimAsync( DbConnection connection, DbTransaction transaction, UserRecord user,     Claim             claim, Claim newClaim, CancellationToken token ) => ValueTask.CompletedTask;

    #endregion
}