// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:56 PM

namespace Jakar.Database;


public abstract partial class Database
{
    private static readonly SynchronizedValue<TimeSpan> __accessTokenExpirationTime  = new(TimeSpan.FromMinutes(15));
    private static readonly SynchronizedValue<TimeSpan> __refreshTokenExpirationTime = new(TimeSpan.FromDays(90));


    public static TimeSpan AccessTokenExpirationTime  { get => __accessTokenExpirationTime;  set => __accessTokenExpirationTime.Value = value; }
    public static TimeSpan RefreshTokenExpirationTime { get => __refreshTokenExpirationTime; set => __refreshTokenExpirationTime.Value = value; }


    public virtual ValueTask<SigningCredentials>        GetSigningCredentials( CancellationToken        token ) => new(Configuration.GetSigningCredentials(Options));
    public virtual ValueTask<TokenValidationParameters> GetTokenValidationParameters( CancellationToken token ) => new(Configuration.GetTokenValidationParameters(Options));


    public virtual async ValueTask<JwtSecurityToken> GetJwtSecurityToken( IEnumerable<Claim> claims, CancellationToken token )
    {
        SigningCredentials signinCredentials = await GetSigningCredentials(token);
        JwtSecurityToken   security          = new(Options.TokenIssuer, Options.TokenAudience, claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(15), signinCredentials);
        return security;
    }
    public async ValueTask<ClaimsPrincipal?> ValidateToken( string jsonToken, CancellationToken token )
    {
        TokenValidationParameters parameters = await GetTokenValidationParameters(token);
        return DbTokenHandler.Instance.ValidateToken(jsonToken, parameters, out _);
    }


    /// <summary> Only to be used for <see cref="IEmailTokenService"/> </summary>
    /// <exception cref="OutOfRangeException"> </exception>
    public ValueTask<ErrorOrResult<Tokens>> Authenticate( ILoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall(Authenticate, request, types, token);
    protected virtual async ValueTask<ErrorOrResult<Tokens>> Authenticate( NpgsqlConnection connection, DbTransaction transaction, ILoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get(connection, transaction, nameof(UserRecord.UserName), request.UserName, token);
        if ( record is null ) { return Error.Unauthorized(request.UserName); }

        ErrorOrResult<SubscriptionStatus> status = await ValidateSubscription(connection, transaction, record, token);

        if ( status.HasErrors )
        {
            record = record.MarkBadLogin();
            await Users.Update(connection, transaction, record, token);

            return status.Error;
        }

        if ( record.IsDisabled )
        {
            record = record.MarkBadLogin();
            await Users.Update(connection, transaction, record, token);

            return Error.Disabled();
        }

        if ( record.IsLocked )
        {
            record = record.MarkBadLogin();
            await Users.Update(connection, transaction, record, token);

            return Error.Locked();
        }

        if ( UserRecord.VerifyPassword(ref record, request) ) { return await GetToken(connection, transaction, record, types, token); }

        record = record.MarkBadLogin();
        await Users.Update(connection, transaction, record, token);
        return Error.Unauthorized(request.UserName);
    }


    public ValueTask<ErrorOrResult<UserModel>> TryGetUserModel( ILoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall(TryGetUserModel, request, types, token);
    protected virtual async ValueTask<ErrorOrResult<UserModel>> TryGetUserModel( NpgsqlConnection connection, DbTransaction transaction, ILoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get(connection, transaction, nameof(UserRecord.UserName), request.UserName, token);
        if ( record is null ) { return Error.Unauthorized(request.UserName); }

        ErrorOrResult<SubscriptionStatus> status = await ValidateSubscription(connection, transaction, record, token);

        if ( status.TryGetValue(out Errors? errors) )
        {
            record = record.MarkBadLogin();
            await Users.Update(connection, transaction, record, token);

            return errors;
        }

        if ( record.IsDisabled )
        {
            record = record.MarkBadLogin();
            await Users.Update(connection, transaction, record, token);

            return Error.Disabled();
        }

        if ( record.IsLocked )
        {
            record = record.MarkBadLogin();
            await Users.Update(connection, transaction, record, token);

            return Error.Locked();
        }


        if ( UserRecord.VerifyPassword(ref record, request) )
        {
            UserModel model = await record.ToUserModel(connection, transaction, this, token);
            return model;
        }

        record = record.MarkBadLogin();
        await Users.Update(connection, transaction, record, token);
        return Error.Unauthorized(request.UserName);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected static DateTime GetExpiration( in DateTimeOffset? recordExpiration, in TimeSpan offset )
    {
        DateTime expires = DateTime.UtcNow + offset;
        if ( recordExpiration is null ) { return expires; }

        DateTime date = recordExpiration.Value.LocalDateTime;
        if ( expires > date ) { expires = date; }

        return expires;
    }


    public ValueTask<Tokens> GetToken( UserRecord user, ClaimType types = default, CancellationToken token = default ) => this.TryCall(GetToken, user, types, token);
    public virtual async ValueTask<Tokens> GetToken( NpgsqlConnection connection, DbTransaction transaction, UserRecord record, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        Claim[]            claims         = await record.GetUserClaims(connection, transaction, this, types, token);
        DateTimeOffset?    expires        = await GetSubscriptionExpiration(connection, transaction, record, token);
        DateTime           refreshExpires = GetExpiration(expires, RefreshTokenExpirationTime);
        SigningCredentials credentials    = await GetSigningCredentials(token);

        string accessToken = DbTokenHandler.Instance.CreateToken(new SecurityTokenDescriptor
                                                                 {
                                                                     Subject            = new ClaimsIdentity(claims),
                                                                     Expires            = GetExpiration(expires, AccessTokenExpirationTime),
                                                                     Issuer             = Options.TokenIssuer,
                                                                     Audience           = Options.TokenAudience,
                                                                     IssuedAt           = DateTime.UtcNow,
                                                                     SigningCredentials = credentials
                                                                 });

        string refresh = DbTokenHandler.Instance.CreateToken(new SecurityTokenDescriptor
                                                             {
                                                                 Subject            = new ClaimsIdentity(claims),
                                                                 Expires            = refreshExpires,
                                                                 Issuer             = Options.TokenIssuer,
                                                                 Audience           = Options.TokenAudience,
                                                                 IssuedAt           = DateTime.UtcNow,
                                                                 SigningCredentials = credentials
                                                             });

        record = record.WithRefreshToken(refresh, refreshExpires);
        await Users.Update(connection, transaction, record, token);
        return new Tokens(record.ID.Value, record.FullName, Version, accessToken, refresh);
    }


    public ValueTask<ErrorOrResult<Tokens>> Refresh( string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall(Refresh, refreshToken, types, token);
    public virtual async ValueTask<ErrorOrResult<Tokens>> Refresh( NpgsqlConnection connection, DbTransaction transaction, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> result = await VerifyLogin(connection, transaction, refreshToken, types, token);
        if ( !result.TryGetValue(out UserRecord? record, out Errors? error) ) { return error; }

        DateTimeOffset? expires = await GetSubscriptionExpiration(connection, transaction, record, token);

        if ( !UserRecord.CheckRefreshToken(ref record, refreshToken) )
        {
            record = record.MarkBadLogin();
            await Users.Update(connection, transaction, record, token);
            return Error.Unauthorized(record.UserName);
        }


        Claim[] claims = await record.GetUserClaims(connection, transaction, this, types | DEFAULT_CLAIM_TYPES, token);

        SecurityTokenDescriptor descriptor = new()
                                             {
                                                 Subject            = new ClaimsIdentity(claims),
                                                 Expires            = GetExpiration(expires, TimeSpan.FromMinutes(15)),
                                                 Issuer             = Options.TokenIssuer,
                                                 Audience           = Options.TokenAudience,
                                                 IssuedAt           = DateTime.UtcNow,
                                                 SigningCredentials = await GetSigningCredentials(token)
                                             };


        string accessToken = DbTokenHandler.Instance.CreateToken(descriptor);
        return new Tokens(record.ID.Value, record.FullName, Version, accessToken, refreshToken);
    }


    public ValueTask<ErrorOrResult<Tokens>> Verify( string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall(Verify, jsonToken, types, token);
    public virtual async ValueTask<ErrorOrResult<Tokens>> Verify( NpgsqlConnection connection, DbTransaction transaction, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> result = await VerifyLogin(connection, transaction, jsonToken, types, token);

        return result.TryGetValue(out UserRecord? record, out Errors? error)
                   ? await GetToken(connection, transaction, record, types, token)
                   : error;
    }


    public ValueTask<ErrorOrResult<UserRecord>> VerifyLogin( string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall(VerifyLogin, jsonToken, types, token);
    protected virtual async ValueTask<ErrorOrResult<UserRecord>> VerifyLogin( NpgsqlConnection connection, DbTransaction transaction, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        JwtSecurityTokenHandler   handler              = new();
        TokenValidationParameters validationParameters = await GetTokenValidationParameters(token);
        TokenValidationResult     validationResult     = await handler.ValidateTokenAsync(jsonToken, validationParameters);

        if ( validationResult.Exception is not null )
        {
            Exception e = validationResult.Exception;
            return Error.Create(Status.InternalServerError, e.GetType().Name, e.Message, e.Source, e.MethodName());
        }

        Claim[]                   claims = validationResult.ClaimsIdentity.Claims.ToArray();
        ErrorOrResult<UserRecord> result = await UserRecord.TryFromClaims(connection, transaction, this, claims, types | DEFAULT_CLAIM_TYPES, token);
        if ( !result.TryGetValue(out UserRecord? record, out Errors? errors) ) { return errors; }

        record.LastLogin = DateTimeOffset.UtcNow;
        await Users.Update(connection, transaction, record, token);
        return record;
    }


    public async ValueTask<UserLoginProviderRecord[]> GetUserLoginInfoRecords( string purpose, UserRecord user, CancellationToken token )
    {
        HashSet<UserLoginProviderRecord> list = await UserLogins.Where(true, UserLoginProviderRecord.GetDynamicParameters(user, purpose), token).ToHashSet(token);
        return [..list];
    }


    public Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) => GenerateAsync(purpose, manager, user, CancellationToken.None);
    public virtual async Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user, CancellationToken token )
    {
        Tokens tokens = await GetToken(user, token: token);
        return tokens.AccessToken;
    }

    public Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user ) => ValidateAsync(purpose, token, manager, user, CancellationToken.None);
    public virtual async Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user, CancellationToken cancellationToken )
    {
        // AuthenticatorTokenProvider<UserRecord> provider = new AuthenticatorTokenProvider<UserRecord>();
        // return await provider.ValidateAsync( purpose, token, manager, user );
        string? key = await manager.GetAuthenticatorKeyAsync(user);

        if ( !string.IsNullOrWhiteSpace(key) )
        {
            if ( long.TryParse(token, out _) ) { return OneTimePassword.Create(key, Options.TokenIssuer).ValidateToken(token); }

            JwtSecurityTokenHandler   handler    = new();
            TokenValidationParameters parameters = await GetTokenValidationParameters(cancellationToken);
            TokenValidationResult     result     = await handler.ValidateTokenAsync(token, parameters);
            return result.IsValid;
        }
        else
        {
            ErrorOrResult<Tokens> result = await Verify(token, token: cancellationToken);
            return result.HasValue;
        }
    }


    public Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user ) => CanGenerateTwoFactorTokenAsync(manager, user, CancellationToken.None);
    public virtual async Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user, CancellationToken token )
    {
        if ( !user.IsValidID() || string.IsNullOrWhiteSpace(user.UserName) ) { return false; }

        return await UserLogins.Where(true, UserLoginProviderRecord.GetDynamicParameters(user), token).Any(token);
    }
}
