// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:56 PM

using System.Diagnostics;



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
    public ValueTask<ErrorOrResult<Tokens>> Authenticate( Activity? activity, LoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Authenticate, activity, request, types, token );
    protected virtual async ValueTask<ErrorOrResult<Tokens>> Authenticate( DbConnection connection, DbTransaction transaction, Activity? activity, LoginRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, activity, nameof(UserRecord.UserName), request.UserName, token );
        if ( record is null ) { return default; }

        ErrorOrResult<SubscriptionStatus> status = await ValidateSubscription( connection, transaction, record, token );

        if ( status.HasErrors )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, activity, record, token );

            return status.Errors;
        }

        if ( record.IsDisabled )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, activity, record, token );

            return Error.Disabled();
        }

        if ( record.IsLocked )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, activity, record, token );

            return Error.Locked();
        }


        if ( UserRecord.VerifyPassword( ref record, request ) ) { return await GetToken( connection, transaction, activity, record, types, token ); }

        await Users.Update( connection, transaction, activity, record, token );
        return default;
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


    public ValueTask<Tokens> GetToken( Activity? activity, UserRecord user, ClaimType types = default, CancellationToken token = default ) => this.TryCall( GetToken, activity, user, types, token );
    public virtual async ValueTask<Tokens> GetToken( DbConnection connection, DbTransaction transaction, Activity? activity, UserRecord record, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        Claim[]            claims         = await record.GetUserClaims( connection, transaction, activity, this, types, token );
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
        await Users.Update( connection, transaction, activity, record, token );
        return new Tokens( record.ID.Value, record.FullName, Version, accessToken, refresh );
    }


    public ValueTask<ErrorOrResult<Tokens>> Refresh( Activity? activity, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Refresh, activity, refreshToken, types, token );
    public virtual async ValueTask<ErrorOrResult<Tokens>> Refresh( DbConnection connection, DbTransaction transaction, Activity? activity, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> result = await VerifyLogin( connection, transaction, activity, refreshToken, types, token );
        if ( result.TryGetValue( out UserRecord? record, out Error[]? error ) is false ) { return error; }

        DateTimeOffset? expires = await GetSubscriptionExpiration( connection, transaction, record, token );

        if ( UserRecord.CheckRefreshToken( ref record, refreshToken ) is false )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, activity, record, token );
            return Error.Unauthorized( record.UserName );
        }


        Claim[] claims = await record.GetUserClaims( connection, transaction, activity, this, types | DEFAULT_CLAIM_TYPES, token );

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


    public ValueTask<ErrorOrResult<Tokens>> Verify( Activity? activity, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Verify, activity, jsonToken, types, token );
    public virtual async ValueTask<ErrorOrResult<Tokens>> Verify( DbConnection connection, DbTransaction transaction, Activity? activity, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> result = await VerifyLogin( connection, transaction, activity, jsonToken, types, token );

        return result.TryGetValue( out UserRecord? record, out Error[]? error )
                   ? await GetToken( connection, transaction, activity, record, types, token )
                   : error;
    }


    public ValueTask<ErrorOrResult<UserRecord>> VerifyLogin( Activity? activity, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( VerifyLogin, activity, jsonToken, types, token );
    protected virtual async ValueTask<ErrorOrResult<UserRecord>> VerifyLogin( DbConnection connection, DbTransaction transaction, Activity? activity, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
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
        UserRecord? record = await UserRecord.TryFromClaims( connection, transaction, activity, this, claims, types | DEFAULT_CLAIM_TYPES, token );
        if ( record is null ) { return Error.NotFound(); }


        record.LastLogin = DateTimeOffset.UtcNow;
        await Users.Update( connection, transaction, activity, record, token );
        return record;
    }


    public async ValueTask<UserLoginInfoRecord[]> GetUserLoginInfoRecords( Activity? activity, string purpose, UserRecord user, CancellationToken token )
    {
        HashSet<UserLoginInfoRecord> list = await UserLogins.Where( activity, true, UserLoginInfoRecord.GetDynamicParameters( user, purpose ), token ).ToHashSet( token );
        return [..list];
    }


    public Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) => GenerateAsync( Activity.Current, purpose, manager, user, CancellationToken.None );
    public virtual async Task<string> GenerateAsync( Activity? activity, string purpose, UserManager<UserRecord> manager, UserRecord user, CancellationToken token )
    {
        Tokens tokens = await GetToken( activity, user, token: token );
        return tokens.AccessToken;
    }


    public Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user ) => ValidateAsync( Activity.Current, purpose, token, manager, user, CancellationToken.None );
    public virtual async Task<bool> ValidateAsync( Activity? activity, string purpose, string token, UserManager<UserRecord> manager, UserRecord user, CancellationToken cancellationToken )
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
            ErrorOrResult<Tokens> result = await Verify( activity, token, token: cancellationToken );
            return result.HasValue;
        }
    }


    public Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user )                          => CanGenerateTwoFactorTokenAsync( manager,          user,    CancellationToken.None );
    public Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user, CancellationToken token ) => CanGenerateTwoFactorTokenAsync( Activity.Current, manager, user, token );
    public virtual async Task<bool> CanGenerateTwoFactorTokenAsync( Activity? activity, UserManager<UserRecord> manager, UserRecord user, CancellationToken token )
    {
        if ( user.IsValidID() is false || string.IsNullOrWhiteSpace( user.UserName ) ) { return false; }

        return await UserLogins.Where( activity, true, UserLoginInfoRecord.GetDynamicParameters( user ), token ).Any( token );
    }
}
