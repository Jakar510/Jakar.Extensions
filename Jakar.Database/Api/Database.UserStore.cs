#pragma warning disable IDE0060
#pragma warning disable CA1822



    namespace Jakar.Database;


    [SuppressMessage( "ReSharper", "UnusedParameter.Global" ), SuppressMessage( "ReSharper", "MemberCanBeMadeStatic.Global" )]
    public partial class Database
    {
        public virtual ValueTask<string?> GetSecurityStampAsync( UserRecord user,     CancellationToken token = default )                                         => new(user.SecurityStamp);
        public         ValueTask          SetSecurityStampAsync( UserRecord user,     string            stamp, CancellationToken token )                          => SetSecurityStampAsync( Activity.Current, user, stamp, token );
        public         ValueTask          SetSecurityStampAsync( Activity?  activity, UserRecord        user,  string            stamp, CancellationToken token ) => this.TryCall( SetSecurityStampAsync, activity, user, stamp, token );
        public async ValueTask SetSecurityStampAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, string stamp, CancellationToken token )
        {
            user.SecurityStamp = stamp;
            await Users.Update( connection, transaction, activity, user, token );
        }


        public       Task<string?> GetPasswordHashAsync( UserRecord user,     CancellationToken token )                                                                 => Task.FromResult( user?.PasswordHash );
        public       Task<bool>    HasPasswordAsync( UserRecord     user,     CancellationToken token )                                                                 => Task.FromResult( user.HasPassword() );
        public       Task          SetPasswordHashAsync( UserRecord user,     string?           passwordHash, CancellationToken token )                                 => SetPasswordHashAsync( Activity.Current, user, passwordHash, token );
        public async Task          SetPasswordHashAsync( Activity?  activity, UserRecord        user,         string?           passwordHash, CancellationToken token ) => await this.TryCall( SetPasswordHashAsync, activity, user, passwordHash, token );
        public async ValueTask SetPasswordHashAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, string? passwordHash, CancellationToken token )
        {
            user.PasswordHash = passwordHash ?? string.Empty;
            await Users.Update( activity, user, token );
        }



        #region User Auth Providers

        public IAsyncEnumerable<UserLoginInfoRecord> GetLoginsAsync<TRecord>( TRecord record, [EnumeratorCancellation] CancellationToken token )
            where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord> => GetLoginsAsync( Activity.Current, record, token );
        public IAsyncEnumerable<UserLoginInfoRecord> GetLoginsAsync<TRecord>( Activity? activity, TRecord record, [EnumeratorCancellation] CancellationToken token )
            where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord> => this.TryCall( GetLoginsAsync, activity, record, token );
        public virtual IAsyncEnumerable<UserLoginInfoRecord> GetLoginsAsync<TRecord>( DbConnection connection, DbTransaction transaction, Activity? activity, TRecord record, [EnumeratorCancellation] CancellationToken token )
            where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord> => UserLogins.Where( connection, transaction, activity, nameof(record.OwnerUserID), record.OwnerUserID, token );


        public ValueTask<ErrorOrResult<UserLoginInfoRecord>> AddLoginAsync( Activity? activity, UserRecord user, UserLoginInfo login, CancellationToken token ) => this.TryCall( AddLoginAsync, activity, user, login, token );
        public virtual async ValueTask<ErrorOrResult<UserLoginInfoRecord>> AddLoginAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, UserLoginInfo login, CancellationToken token )
        {
            UserLoginInfoRecord? record = await UserLogins.Get( connection, transaction, activity, true, UserLoginInfoRecord.GetDynamicParameters( user, login ), token );

            if ( record is not null )
            {
                Error provider = Error.NotFound( nameof(UserLoginInfoRecord.LoginProvider), login.LoginProvider );
                Error key      = Error.NotFound( nameof(UserLoginInfoRecord.ProviderKey),   login.ProviderKey );
                Error userID   = Error.NotFound( nameof(UserLoginInfoRecord.OwnerUserID),   user.ID.Value.ToString() );
                return ErrorOrResult<UserLoginInfoRecord>.Create( provider, key, userID );
            }

            record = new UserLoginInfoRecord( user, login );
            record = await UserLogins.Insert( connection, transaction, activity, record, token );
            await SetAuthenticatorKeyAsync( connection, transaction, activity, user, record.ProviderKey, token );
            return record;
        }


        public ValueTask<UserRecord?> FindByLoginAsync( Activity? activity, string loginProvider, string providerKey, CancellationToken token ) => this.TryCall( FindByLoginAsync, activity, loginProvider, providerKey, token );
        public virtual async ValueTask<UserRecord?> FindByLoginAsync( DbConnection connection, DbTransaction transaction, Activity? activity, string loginProvider, string providerKey, CancellationToken token )
        {
            DynamicParameters parameters = new();
            parameters.Add( nameof(UserLoginInfoRecord.LoginProvider), loginProvider );
            parameters.Add( nameof(UserLoginInfoRecord.ProviderKey),   providerKey );
            return await Users.Get( connection, transaction, activity, true, parameters, token );
        }


        public ValueTask RemoveLoginAsync( Activity? activity, UserRecord user, string loginProvider, string providerKey, CancellationToken token ) => this.TryCall( RemoveLoginAsync, activity, user, loginProvider, providerKey, token );
        public virtual async ValueTask RemoveLoginAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, string loginProvider, string providerKey, CancellationToken token )
        {
            DynamicParameters                     parameters = UserLoginInfoRecord.GetDynamicParameters( user, loginProvider, providerKey );
            IAsyncEnumerable<UserLoginInfoRecord> records    = UserLogins.Where( connection, transaction, activity, true, parameters, token );
            await foreach ( UserLoginInfoRecord record in records ) { await UserLogins.Delete( connection, transaction, activity, record, token ); }
        }


        public         ValueTask<string?> GetAuthenticatorKeyAsync( Activity?    activity,   UserRecord    user,        CancellationToken token )                                              => this.TryCall( GetAuthenticatorKeyAsync, activity, user, token );
        public virtual ValueTask<string?> GetAuthenticatorKeyAsync( DbConnection connection, DbTransaction transaction, Activity?         activity, UserRecord user, CancellationToken token ) => new(user.AuthenticatorKey);


        public ValueTask SetAuthenticatorKeyAsync( UserRecord user,     string     key,  CancellationToken token )                        => SetAuthenticatorKeyAsync( Activity.Current, user, key, token );
        public ValueTask SetAuthenticatorKeyAsync( Activity?  activity, UserRecord user, string            key, CancellationToken token ) => this.TryCall( SetAuthenticatorKeyAsync, activity, user, key, token );
        public virtual async ValueTask SetAuthenticatorKeyAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, string key, CancellationToken token )
        {
            user.AuthenticatorKey = key;
            await Users.Update( connection, transaction, activity, user, token );
        }


        public virtual ValueTask<bool> GetTwoFactorEnabledAsync( UserRecord user,     CancellationToken token )                                                       => new(user.IsTwoFactorEnabled);
        public         ValueTask       SetTwoFactorEnabledAsync( UserRecord user,     bool              enabled, CancellationToken token )                            => SetTwoFactorEnabledAsync( Activity.Current, user, enabled, token );
        public         ValueTask       SetTwoFactorEnabledAsync( Activity?  activity, UserRecord        user,    bool              enabled, CancellationToken token ) => this.TryCall( SetTwoFactorEnabledAsync, activity, user, enabled, token );
        public virtual async ValueTask SetTwoFactorEnabledAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, bool enabled, CancellationToken token )
        {
            user.IsTwoFactorEnabled = enabled;
            await Users.Update( connection, transaction, activity, user, token );
        }

        #endregion



        #region User Email

        public ValueTask<string?> GetEmailAsync( UserRecord           user, CancellationToken token = default ) => new(user.Email);
        public ValueTask<string?> GetNormalizedEmailAsync( UserRecord user, CancellationToken token = default ) => new(user.Email);
        public ValueTask<bool>    GetEmailConfirmedAsync( UserRecord  user, CancellationToken token = default ) => new(user.IsEmailConfirmed);


        public ValueTask SetEmailAsync( Activity? activity, UserRecord user, string? email, CancellationToken token ) => this.TryCall( SetEmailAsync, activity, user, email, token );
        public async ValueTask SetEmailAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, string? email, CancellationToken token )
        {
            user.Email = email ?? string.Empty;
            await Users.Update( connection, transaction, activity, user, token );
        }


        public ValueTask SetEmailConfirmedAsync( Activity? activity, UserRecord user, bool confirmed, CancellationToken token ) => this.TryCall( SetEmailConfirmedAsync, activity, user, confirmed, token );
        public async ValueTask SetEmailConfirmedAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, bool confirmed, CancellationToken token )
        {
            user.IsEmailConfirmed = confirmed;
            await Users.Update( connection, transaction, activity, user, token );
        }


        public ValueTask SetNormalizedEmailAsync( Activity? activity, UserRecord user, string? normalizedEmail, CancellationToken token ) => this.TryCall( SetNormalizedEmailAsync, activity, user, normalizedEmail, token );
        public async ValueTask SetNormalizedEmailAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, string? normalizedEmail, CancellationToken token )
        {
            user.Email = normalizedEmail ?? string.Empty;
            await Users.Update( connection, transaction, activity, user, token );
        }


        public ValueTask<int>             GetAccessFailedCountAsync( UserRecord user, CancellationToken token = default ) => new(user.BadLogins ?? 0);
        public ValueTask<bool>            GetLockoutEnabledAsync( UserRecord    user, CancellationToken token = default ) => new(user.IsLocked);
        public ValueTask<DateTimeOffset?> GetLockoutEndDateAsync( UserRecord    user, CancellationToken token = default ) => new(user.LockoutEnd);

        #endregion



        #region User Phone Number

        public virtual ValueTask<string?> GetPhoneNumberAsync( UserRecord          user, CancellationToken token = default ) => new(user.PhoneNumber);
        public virtual ValueTask<bool>    GetPhoneNumberConfirmedAsync( UserRecord user, CancellationToken token = default ) => new(user.IsPhoneNumberConfirmed);


        public ValueTask SetPhoneNumberAsync( UserRecord user,     string?    phoneNumber, CancellationToken token )                                => SetPhoneNumberAsync( Activity.Current, user, phoneNumber, token );
        public ValueTask SetPhoneNumberAsync( Activity?  activity, UserRecord user,        string?           phoneNumber, CancellationToken token ) => this.TryCall( SetPhoneNumberAsync, activity, user, phoneNumber, token );
        public virtual async ValueTask SetPhoneNumberAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, string? phoneNumber, CancellationToken token )
        {
            user.PhoneNumber = phoneNumber ?? string.Empty;
            await Users.Update( connection, transaction, activity, user, token );
        }


        public ValueTask SetPhoneNumberConfirmedAsync( UserRecord user,     bool       confirmed, CancellationToken token )                              => SetPhoneNumberConfirmedAsync( Activity.Current, user, confirmed, token );
        public ValueTask SetPhoneNumberConfirmedAsync( Activity?  activity, UserRecord user,      bool              confirmed, CancellationToken token ) => this.TryCall( SetPhoneNumberConfirmedAsync, activity, user, confirmed, token );
        public virtual async ValueTask SetPhoneNumberConfirmedAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, bool confirmed, CancellationToken token )
        {
            user.IsPhoneNumberConfirmed = confirmed;
            await Users.Update( connection, transaction, activity, user, token );
        }

        #endregion



        #region User Lock/Unlock

        public ValueTask<int> IncrementAccessFailedCountAsync( UserRecord user,     CancellationToken token )                         => IncrementAccessFailedCountAsync( Activity.Current, user, token );
        public ValueTask<int> IncrementAccessFailedCountAsync( Activity?  activity, UserRecord        user, CancellationToken token ) => this.TryCall( IncrementAccessFailedCountAsync, activity, user, token );
        public virtual async ValueTask<int> IncrementAccessFailedCountAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, CancellationToken token )
        {
            user = user.MarkBadLogin();
            await Users.Update( connection, transaction, activity, user, token );
            return user.BadLogins ?? 0;
        }


        public ValueTask ResetAccessFailedCountAsync( UserRecord user,     CancellationToken token )                         => ResetAccessFailedCountAsync( Activity.Current, user, token );
        public ValueTask ResetAccessFailedCountAsync( Activity?  activity, UserRecord        user, CancellationToken token ) => this.TryCall( ResetAccessFailedCountAsync, activity, user, token );
        public virtual async ValueTask ResetAccessFailedCountAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, CancellationToken token )
        {
            user = user.Unlock();
            await Users.Update( connection, transaction, activity, user, token );
        }


        public ValueTask SetLockoutEnabledAsync( UserRecord user,     bool       enabled, CancellationToken token )                            => SetLockoutEnabledAsync( Activity.Current, user, enabled, token );
        public ValueTask SetLockoutEnabledAsync( Activity?  activity, UserRecord user,    bool              enabled, CancellationToken token ) => this.TryCall( SetLockoutEnabledAsync, activity, user, enabled, token );
        public virtual async ValueTask SetLockoutEnabledAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, bool enabled, CancellationToken token )
        {
            user = enabled
                       ? user.Disable()
                       : user.Enable();

            await Users.Update( connection, transaction, activity, user, token );
        }


        public ValueTask SetLockoutEndDateAsync( UserRecord user,     DateTimeOffset? lockoutEnd, CancellationToken token )                               => SetLockoutEndDateAsync( Activity.Current, user, lockoutEnd, token );
        public ValueTask SetLockoutEndDateAsync( Activity?  activity, UserRecord      user,       DateTimeOffset?   lockoutEnd, CancellationToken token ) => this.TryCall( SetLockoutEndDateAsync, activity, user, lockoutEnd, token );
        public virtual async ValueTask SetLockoutEndDateAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, DateTimeOffset? lockoutEnd, CancellationToken token )
        {
            user.LockoutEnd = lockoutEnd;
            await Users.Update( connection, transaction, activity, user, token );
        }

        #endregion



        #region Find User

        public ValueTask<UserRecord?> FindByIdAsync( string    userID,   CancellationToken token )                           => FindByIdAsync( Activity.Current, userID, token );
        public ValueTask<UserRecord?> FindByIdAsync( Activity? activity, string            userID, CancellationToken token ) => this.TryCall( FindByIdAsync, activity, userID, token );
        public virtual async ValueTask<UserRecord?> FindByIdAsync( DbConnection connection, DbTransaction transaction, Activity? activity, string userID, CancellationToken token ) =>
            Guid.TryParse( userID, out Guid guid )
                ? await Users.Get( connection, transaction, activity, nameof(UserRecord.ID),       guid,   token )
                : await Users.Get( connection, transaction, activity, nameof(UserRecord.UserName), userID, token );


        public ValueTask<UserRecord?> FindByNameAsync( string    normalizedUserName, CancellationToken token )                                       => FindByNameAsync( Activity.Current, normalizedUserName, token );
        public ValueTask<UserRecord?> FindByNameAsync( Activity? activity,           string            normalizedUserName, CancellationToken token ) => this.TryCall( FindByNameAsync, activity, normalizedUserName, token );
        public virtual async ValueTask<UserRecord?> FindByNameAsync( DbConnection connection, DbTransaction transaction, Activity? activity, string normalizedUserName, CancellationToken token ) =>
            await Users.Get( connection, transaction, activity, nameof(UserRecord.UserName), normalizedUserName, token ) ?? await Users.Get( connection, transaction, activity, nameof(UserRecord.FullName), normalizedUserName, token );


        public       ValueTask<UserRecord?> FindByEmailAsync( string       email,      CancellationToken token )                                                                          => FindByEmailAsync( Activity.Current, email, token );
        public       ValueTask<UserRecord?> FindByEmailAsync( Activity?    activity,   string            email,       CancellationToken token )                                           => this.TryCall( FindByEmailAsync, activity, email, token );
        public async ValueTask<UserRecord?> FindByEmailAsync( DbConnection connection, DbTransaction     transaction, Activity?         activity, string email, CancellationToken token ) => await Users.Get( connection, transaction, activity, nameof(UserRecord.Email), email, token );

        #endregion



        #region User CRUD

        public ValueTask<IdentityResult> CreateAsync( UserRecord user,     CancellationToken token )                         => CreateAsync( Activity.Current, user, token );
        public ValueTask<IdentityResult> CreateAsync( Activity?  activity, UserRecord        user, CancellationToken token ) => this.TryCall( CreateAsync, activity, user, token );
        public virtual async ValueTask<IdentityResult> CreateAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, CancellationToken token )
        {
            if ( await Users.Get( connection, transaction, activity, true, UserRecord.GetDynamicParameters( user.UserName ), token ) is not null ) { return IdentityResult.Failed( new IdentityError { Description = Settings.UserExists } ); }


            await Users.Insert( connection, transaction, activity, user, token );
            return IdentityResult.Success;
        }


        public ValueTask<IdentityResult> DeleteAsync( UserRecord user,     CancellationToken token )                         => DeleteAsync( Activity.Current, user, token );
        public ValueTask<IdentityResult> DeleteAsync( Activity?  activity, UserRecord        user, CancellationToken token ) => this.TryCall( DeleteAsync, activity, user, token );
        public virtual async ValueTask<IdentityResult> DeleteAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, CancellationToken token )
        {
            try
            {
                await Users.Delete( connection, transaction, activity, user.ID, token );
                return IdentityResult.Success;
            }
            catch ( Exception e ) { return IdentityResult.Failed( new IdentityError { Description = e.Message } ); }
        }


        public ValueTask<IdentityResult> UpdateAsync( UserRecord user,     CancellationToken token )                         => UpdateAsync( Activity.Current, user, token );
        public ValueTask<IdentityResult> UpdateAsync( Activity?  activity, UserRecord        user, CancellationToken token ) => this.TryCall( UpdateAsync, activity, user, token );
        public async ValueTask<IdentityResult> UpdateAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, CancellationToken token )
        {
            try
            {
                await Users.Update( connection, transaction, activity, user, token );
                return IdentityResult.Success;
            }
            catch ( Exception e ) { return IdentityResult.Failed( new IdentityError { Description = e.Message } ); }
        }

        #endregion



        #region User UserName

        public virtual ValueTask<string> GetNormalizedUserNameAsync( UserRecord user, CancellationToken token = default ) => new(user.UserName);
        public virtual ValueTask<string> GetUserIdAsync( UserRecord             user, CancellationToken token = default ) => new(user.ID.ToString());
        public virtual ValueTask<string> GetUserNameAsync( UserRecord           user, CancellationToken token = default ) => new(user.UserName);


        public ValueTask SetNormalizedUserNameAsync( UserRecord user,     string?    fullName, CancellationToken token )                             => SetNormalizedUserNameAsync( Activity.Current, user, fullName, token );
        public ValueTask SetNormalizedUserNameAsync( Activity?  activity, UserRecord user,     string?           fullName, CancellationToken token ) => this.TryCall( SetNormalizedUserNameAsync, activity, user, fullName, token );
        public virtual async ValueTask SetNormalizedUserNameAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, string? fullName, CancellationToken token )
        {
            user.FullName = fullName ?? string.Empty;
            await Users.Update( connection, transaction, activity, user, token );
        }


        public ValueTask SetUserNameAsync( UserRecord user,     string?    userName, CancellationToken token )                             => SetUserNameAsync( Activity.Current, user, userName, token );
        public ValueTask SetUserNameAsync( Activity?  activity, UserRecord user,     string?           userName, CancellationToken token ) => this.TryCall( SetUserNameAsync, activity, user, userName, token );
        public virtual async ValueTask SetUserNameAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord user, string? userName, CancellationToken token )
        {
            user = user with { UserName = userName ?? string.Empty };
            await Users.Update( connection, transaction, activity, user, token );
        }

        #endregion



        #region Claims

        public         ValueTask AddClaimsAsync( Activity?    activity,   UserRecord    user,        IEnumerable<Claim> claims,   CancellationToken token )                                                    => this.TryCall( AddClaimsAsync, activity, user, claims, token );
        public virtual ValueTask AddClaimsAsync( DbConnection connection, DbTransaction transaction, Activity?          activity, UserRecord        user, IEnumerable<Claim> claims, CancellationToken token ) => ValueTask.CompletedTask;


        public         ValueTask<Claim[]> GetClaimsAsync( UserRecord   user,       ClaimType      types,       CancellationToken token )                                                                      => GetClaimsAsync( Activity.Current, user, types, token );
        public         ValueTask<Claim[]> GetClaimsAsync( Activity?    activity,   UserRecord     user,        ClaimType         types,    CancellationToken token )                                          => this.Call( GetClaimsAsync, activity, user, types, token );
        public virtual ValueTask<Claim[]> GetClaimsAsync( DbConnection connection, DbTransaction? transaction, Activity?         activity, UserRecord        user, ClaimType types, CancellationToken token ) => user.GetUserClaims( connection, transaction, activity, this, types, token );


        public IAsyncEnumerable<UserRecord> GetUsersForClaimAsync( Claim     claim,    [EnumeratorCancellation] CancellationToken token )                                                   => GetUsersForClaimAsync( Activity.Current, claim, token );
        public IAsyncEnumerable<UserRecord> GetUsersForClaimAsync( Activity? activity, Claim                                      claim, [EnumeratorCancellation] CancellationToken token ) => this.TryCall( GetUsersForClaimAsync, activity, claim, token );
        public virtual async IAsyncEnumerable<UserRecord> GetUsersForClaimAsync( DbConnection connection, DbTransaction transaction, Activity? activity, Claim claim, [EnumeratorCancellation] CancellationToken token )
        {
            await foreach ( UserRecord record in UserRecord.TryFromClaims( connection, transaction, activity, this, claim, token ) ) { yield return record; }
        }


        public         ValueTask RemoveClaimsAsync( UserRecord   user,       IEnumerable<Claim> claims,      CancellationToken  token )                                                                                => RemoveClaimsAsync( Activity.Current, user, claims, token );
        public         ValueTask RemoveClaimsAsync( Activity?    activity,   UserRecord         user,        IEnumerable<Claim> claims,   CancellationToken token )                                                    => this.TryCall( RemoveClaimsAsync, activity, user, claims, token );
        public virtual ValueTask RemoveClaimsAsync( DbConnection connection, DbTransaction      transaction, Activity?          activity, UserRecord        user, IEnumerable<Claim> claims, CancellationToken token ) => ValueTask.CompletedTask;


        public         ValueTask ReplaceClaimAsync( UserRecord   user,       Claim         claim,       Claim     newClaim, CancellationToken token )                                                                      => ReplaceClaimAsync( Activity.Current, user, claim, newClaim, token );
        public         ValueTask ReplaceClaimAsync( Activity?    activity,   UserRecord    user,        Claim     claim,    Claim             newClaim, CancellationToken token )                                          => this.TryCall( ReplaceClaimAsync, activity, user, claim, newClaim, token );
        public virtual ValueTask ReplaceClaimAsync( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord        user,     Claim             claim, Claim newClaim, CancellationToken token ) => ValueTask.CompletedTask;

        #endregion
    }
