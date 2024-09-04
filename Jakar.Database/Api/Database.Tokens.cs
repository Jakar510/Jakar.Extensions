// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:56 PM

using Status = Jakar.Extensions.Status;



namespace Jakar.Database;


public abstract partial class Database
{
    private static readonly Synchronized<TimeSpan> _accessTokenExpirationTime  = new(TimeSpan.FromMinutes( 15 ));
    private static readonly Synchronized<TimeSpan> _refreshTokenExpirationTime = new(TimeSpan.FromDays( 90 ));


    public static TimeSpan AccessTokenExpirationTime  { get => _accessTokenExpirationTime;  set => _accessTokenExpirationTime.Value = value; }
    public static TimeSpan RefreshTokenExpirationTime { get => _refreshTokenExpirationTime; set => _refreshTokenExpirationTime.Value = value; }


    public virtual ValueTask<SigningCredentials>        GetSigningCredentials( CancellationToken        token ) => new(Configuration.GetSigningCredentials( Settings ));
    public virtual ValueTask<TokenValidationParameters> GetTokenValidationParameters( CancellationToken token ) => new(Configuration.GetTokenValidationParameters( Settings ));


    public virtual async ValueTask<JwtSecurityToken> GetJwtSecurityToken( IEnumerable<Claim> claims, CancellationToken token )
    {
        SigningCredentials signinCredentials = await GetSigningCredentials( token );
        JwtSecurityToken   security          = new JwtSecurityToken( Settings.TokenIssuer, Settings.TokenAudience, claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes( 15 ), signinCredentials );
        return security;
    }
    public async ValueTask<ClaimsPrincipal?> ValidateToken( string jsonToken, CancellationToken token )
    {
        TokenValidationParameters parameters = await GetTokenValidationParameters( token );
        return DbTokenHandler.Instance.ValidateToken( jsonToken, parameters, out _ );
    }


    /// <summary> Only to be used for <see cref="ITokenService"/> </summary>
    /// <exception cref="OutOfRangeException"> </exception>
    public ValueTask<ErrorOrResult<Tokens>> Authenticate( ILoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Authenticate, request, types, token );
    protected virtual async ValueTask<ErrorOrResult<Tokens>> Authenticate( DbConnection connection, DbTransaction transaction, ILoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, nameof(UserRecord.UserName), request.UserName, token );
        if ( record is null ) { return Error.Unauthorized( request.UserName ); }

        ErrorOrResult<SubscriptionStatus> status = await ValidateSubscription( connection, transaction, record, token );

        if ( status.HasErrors )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, record, token );

            return status.Errors;
        }

        if ( record.IsDisabled )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, record, token );

            return Error.Disabled();
        }

        if ( record.IsLocked )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, record, token );

            return Error.Locked();
        }

        if ( UserRecord.VerifyPassword( ref record, request ) ) { return await GetToken( connection, transaction, record, types, token ); }

        record = record.MarkBadLogin();
        await Users.Update( connection, transaction, record, token );
        return Error.Unauthorized( request.UserName );
    }


    public ValueTask<ErrorOrResult<UserModel>> TryGetUserModel( ILoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( TryGetUserModel, request, types, token );
    protected virtual async ValueTask<ErrorOrResult<UserModel>> TryGetUserModel( DbConnection connection, DbTransaction transaction, ILoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, nameof(UserRecord.UserName), request.UserName, token );
        if ( record is null ) { return Error.Unauthorized( request.UserName ); }

        ErrorOrResult<SubscriptionStatus> status = await ValidateSubscription( connection, transaction, record, token );

        if ( status.HasErrors )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, record, token );

            return status.Errors;
        }

        if ( record.IsDisabled )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, record, token );

            return Error.Disabled();
        }

        if ( record.IsLocked )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, record, token );

            return Error.Locked();
        }


        if ( UserRecord.VerifyPassword( ref record, request ) )
        {
            UserModel model = await record.ToUserModel( connection, transaction, this, token );
            return model;
        }

        record = record.MarkBadLogin();
        await Users.Update( connection, transaction, record, token );
        return Error.Unauthorized( request.UserName );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
    protected static DateTime GetExpiration( in DateTimeOffset? recordExpiration, in TimeSpan offset )
    {
        DateTime expires = DateTime.UtcNow + offset;
        if ( recordExpiration is null ) { return expires; }

        DateTime date = recordExpiration.Value.LocalDateTime;
        if ( expires > date ) { expires = date; }

        return expires;
    }


    public ValueTask<Tokens> GetToken( UserRecord user, ClaimType types = default, CancellationToken token = default ) => this.TryCall( GetToken, user, types, token );
    public virtual async ValueTask<Tokens> GetToken( DbConnection connection, DbTransaction transaction, UserRecord record, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        Claim[]            claims         = await record.GetUserClaims( connection, transaction, this, types, token );
        DateTimeOffset?    expires        = await GetSubscriptionExpiration( connection, transaction, record, token );
        DateTime           refreshExpires = GetExpiration( expires, RefreshTokenExpirationTime );
        SigningCredentials credentials    = await GetSigningCredentials( token );

        string accessToken = DbTokenHandler.Instance.CreateToken( new SecurityTokenDescriptor
                                                                  {
                                                                      Subject            = new ClaimsIdentity( claims ),
                                                                      Expires            = GetExpiration( expires, AccessTokenExpirationTime ),
                                                                      Issuer             = Settings.TokenIssuer,
                                                                      Audience           = Settings.TokenAudience,
                                                                      IssuedAt           = DateTime.UtcNow,
                                                                      SigningCredentials = credentials
                                                                  } );

        string refresh = DbTokenHandler.Instance.CreateToken( new SecurityTokenDescriptor
                                                              {
                                                                  Subject            = new ClaimsIdentity( claims ),
                                                                  Expires            = refreshExpires,
                                                                  Issuer             = Settings.TokenIssuer,
                                                                  Audience           = Settings.TokenAudience,
                                                                  IssuedAt           = DateTime.UtcNow,
                                                                  SigningCredentials = credentials
                                                              } );

        record = record.WithRefreshToken( refresh, refreshExpires );
        await Users.Update( connection, transaction, record, token );
        return new Tokens( record.ID.Value, record.FullName, Version, accessToken, refresh );
    }


    public ValueTask<ErrorOrResult<Tokens>> Refresh( string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Refresh, refreshToken, types, token );
    public virtual async ValueTask<ErrorOrResult<Tokens>> Refresh( DbConnection connection, DbTransaction transaction, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> result = await VerifyLogin( connection, transaction, refreshToken, types, token );
        if ( result.TryGetValue( out UserRecord? record, out Error[]? error ) is false ) { return error; }

        DateTimeOffset? expires = await GetSubscriptionExpiration( connection, transaction, record, token );

        if ( UserRecord.CheckRefreshToken( ref record, refreshToken ) is false )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, record, token );
            return Error.Unauthorized( record.UserName );
        }


        Claim[] claims = await record.GetUserClaims( connection, transaction, this, types | DEFAULT_CLAIM_TYPES, token );

        SecurityTokenDescriptor descriptor = new()
                                             {
                                                 Subject            = new ClaimsIdentity( claims ),
                                                 Expires            = GetExpiration( expires, TimeSpan.FromMinutes( 15 ) ),
                                                 Issuer             = Settings.TokenIssuer,
                                                 Audience           = Settings.TokenAudience,
                                                 IssuedAt           = DateTime.UtcNow,
                                                 SigningCredentials = await GetSigningCredentials( token )
                                             };


        string accessToken = DbTokenHandler.Instance.CreateToken( descriptor );
        return new Tokens( record.ID.Value, record.FullName, Version, accessToken, refreshToken );
    }


    public ValueTask<ErrorOrResult<Tokens>> Verify( string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Verify, jsonToken, types, token );
    public virtual async ValueTask<ErrorOrResult<Tokens>> Verify( DbConnection connection, DbTransaction transaction, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> result = await VerifyLogin( connection, transaction, jsonToken, types, token );

        return result.TryGetValue( out UserRecord? record, out Error[]? error )
                   ? await GetToken( connection, transaction, record, types, token )
                   : error;
    }


    public ValueTask<ErrorOrResult<UserRecord>> VerifyLogin( string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( VerifyLogin, jsonToken, types, token );
    protected virtual async ValueTask<ErrorOrResult<UserRecord>> VerifyLogin( DbConnection connection, DbTransaction transaction, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        JwtSecurityTokenHandler   handler              = new();
        TokenValidationParameters validationParameters = await GetTokenValidationParameters( token );
        TokenValidationResult     validationResult     = await handler.ValidateTokenAsync( jsonToken, validationParameters );

        if ( validationResult.Exception is not null )
        {
            Exception e = validationResult.Exception;
            return Error.Create( Status.InternalServerError, e.GetType().Name, e.Message, e.Source, e.MethodName() );
        }

        Claim[]     claims = validationResult.ClaimsIdentity.Claims.ToArray();
        UserRecord? record = await UserRecord.TryFromClaims( connection, transaction, this, claims, types | DEFAULT_CLAIM_TYPES, token );
        if ( record is null ) { return Error.NotFound(); }


        record.LastLogin = DateTimeOffset.UtcNow;
        await Users.Update( connection, transaction, record, token );
        return record;
    }


    public async ValueTask<UserLoginInfoRecord[]> GetUserLoginInfoRecords( string purpose, UserRecord user, CancellationToken token )
    {
        HashSet<UserLoginInfoRecord> list = await UserLogins.Where( true, UserLoginInfoRecord.GetDynamicParameters( user, purpose ), token ).ToHashSet( token );
        return [..list];
    }


    public Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) => GenerateAsync( purpose, manager, user, CancellationToken.None );
    public virtual async Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user, CancellationToken token )
    {
        Tokens tokens = await GetToken( user, token: token );
        return tokens.AccessToken;
    }

    public Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user ) => ValidateAsync( purpose, token, manager, user, CancellationToken.None );
    public virtual async Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user, CancellationToken cancellationToken )
    {
        // AuthenticatorTokenProvider<UserRecord> provider = new AuthenticatorTokenProvider<UserRecord>();
        // return await provider.ValidateAsync( purpose, token, manager, user );
        string? key = await manager.GetAuthenticatorKeyAsync( user );

        if ( string.IsNullOrWhiteSpace( key ) is false )
        {
            if ( long.TryParse( token, out _ ) ) { return OneTimePassword.Create( key, Settings.TokenIssuer ).ValidateToken( token ); }

            JwtSecurityTokenHandler   handler    = new();
            TokenValidationParameters parameters = await GetTokenValidationParameters( cancellationToken );
            TokenValidationResult     result     = await handler.ValidateTokenAsync( token, parameters );
            return result.IsValid;
        }
        else
        {
            ErrorOrResult<Tokens> result = await Verify( token, token: cancellationToken );
            return result.HasValue;
        }
    }


    public Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user ) => CanGenerateTwoFactorTokenAsync( manager, user, CancellationToken.None );
    public virtual async Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user, CancellationToken token )
    {
        if ( user.IsValidID() is false || string.IsNullOrWhiteSpace( user.UserName ) ) { return false; }

        return await UserLogins.Where( true, UserLoginInfoRecord.GetDynamicParameters( user ), token ).Any( token );
    }
}
