// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:56 PM

namespace Jakar.Database;


public abstract partial class Database
{
    private static readonly Synchronized<TimeSpan> _accessTokenExpirationTime  = new(TimeSpan.FromMinutes( 15 ));
    private static readonly Synchronized<TimeSpan> _refreshTokenExpirationTime = new(TimeSpan.FromDays( 90 ));


    public static TimeSpan AccessTokenExpirationTime  { get => _accessTokenExpirationTime;  set => _accessTokenExpirationTime.Value = value; }
    public static TimeSpan RefreshTokenExpirationTime { get => _refreshTokenExpirationTime; set => _refreshTokenExpirationTime.Value = value; }


    public virtual ValueTask<SigningCredentials>        GetSigningCredentials( CancellationToken        token ) => new(Configuration.GetSigningCredentials( Options ));
    public virtual ValueTask<TokenValidationParameters> GetTokenValidationParameters( CancellationToken token ) => new(Configuration.GetTokenValidationParameters( Options ));


    public virtual async ValueTask<JwtSecurityToken> GetJwtSecurityToken( IEnumerable<Claim> claims, CancellationToken token )
    {
        SigningCredentials signinCredentials = await GetSigningCredentials( token );
        JwtSecurityToken                security          = new JwtSecurityToken( Options.TokenIssuer, Options.TokenAudience, claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes( 15 ), signinCredentials );
        return security;
    }
    public async ValueTask<ClaimsPrincipal?> ValidateToken( string jsonToken, CancellationToken token )
    {
        TokenValidationParameters parameters = await GetTokenValidationParameters( token );
        return DbTokenHandler.Instance.ValidateToken( jsonToken, parameters, out _ );
    }


    /// <summary> Only to be used for <see cref="ITokenService"/> </summary>
    /// <exception cref="OutOfRangeException"> </exception>
    public ValueTask<Tokens?> Authenticate( VerifyRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Authenticate, request, types, token );
    protected virtual async ValueTask<Tokens?> Authenticate( DbConnection connection, DbTransaction transaction, VerifyRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        UserRecord? user = await Users.Get( nameof(UserRecord.UserName), request.UserName, token );
        if ( user is null ) { return default; }

        if ( !await ValidateSubscription( connection, transaction, user, token ) ) { return default; }

        if ( user.IsDisabled )
        {
            user = user.MarkBadLogin();
            await Users.Update( connection, transaction, user, token );

            return default;
        }

        if ( user.IsLocked )
        {
            user = user.MarkBadLogin();
            await Users.Update( connection, transaction, user, token );

            return default;
        }


        if ( UserRecord.VerifyPassword( ref user, request ) ) { return await GetToken( connection, transaction, user, types, token ); }

        await Users.Update( connection, transaction, user, token );
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


    public ValueTask<Tokens> GetToken( UserRecord user, ClaimType types = default, CancellationToken token = default ) => this.TryCall( GetToken, user, types, token );
    public virtual async ValueTask<Tokens> GetToken( DbConnection connection, DbTransaction transaction, UserRecord record, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        Claim[]         claims  = await record.GetUserClaims( connection, transaction, this, types, token );
        DateTimeOffset? expires = await GetSubscriptionExpiration( connection, transaction, record, token );


        DateTime           refreshExpires = GetExpiration( expires, RefreshTokenExpirationTime );
        SigningCredentials credentials    = await GetSigningCredentials( token );

        string accessToken = DbTokenHandler.Instance.CreateToken( new SecurityTokenDescriptor
                                                                  {
                                                                      Subject            = new ClaimsIdentity( claims ),
                                                                      Expires            = GetExpiration( expires, AccessTokenExpirationTime ),
                                                                      Issuer             = Options.TokenIssuer,
                                                                      Audience           = Options.TokenAudience,
                                                                      IssuedAt           = DateTime.UtcNow,
                                                                      SigningCredentials = credentials
                                                                  } );

        string refresh = DbTokenHandler.Instance.CreateToken( new SecurityTokenDescriptor
                                                              {
                                                                  Subject            = new ClaimsIdentity( claims ),
                                                                  Expires            = refreshExpires,
                                                                  Issuer             = Options.TokenIssuer,
                                                                  Audience           = Options.TokenAudience,
                                                                  IssuedAt           = DateTime.UtcNow,
                                                                  SigningCredentials = credentials
                                                              } );

        record = record.WithRefreshToken( refresh, refreshExpires );
        await Users.Update( connection, transaction, record, token );
        return new Tokens( record.UserID, record.FullName, Version, accessToken, refresh );
    }


    public ValueTask<OneOf<Tokens, Error>> Refresh( string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Refresh, refreshToken, types, token );
    public virtual async ValueTask<OneOf<Tokens, Error>> Refresh( DbConnection connection, DbTransaction transaction, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, refreshToken, types, token );
        if ( loginResult.GetResult( out Error? error, out UserRecord? record ) ) { return error.Value; }

        DateTimeOffset? expires = await GetSubscriptionExpiration( connection, transaction, record, token );

        if ( UserRecord.CheckRefreshToken( ref record, refreshToken ) is false )
        {
            record = record.MarkBadLogin();
            await Users.Update( connection, transaction, record, token );
            return new Error( Status.Unauthorized );
        }


        Claim[] claims = await record.GetUserClaims( connection, transaction, this, types | DEFAULT_CLAIM_TYPES, token );

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
                                             {
                                                 Subject            = new ClaimsIdentity( claims ),
                                                 Expires            = GetExpiration( expires, TimeSpan.FromMinutes( 15 ) ),
                                                 Issuer             = Options.TokenIssuer,
                                                 Audience           = Options.TokenAudience,
                                                 IssuedAt           = DateTime.UtcNow,
                                                 SigningCredentials = await GetSigningCredentials( token )
                                             };


        string accessToken = DbTokenHandler.Instance.CreateToken( descriptor );
        return new Tokens( record.UserID, record.FullName, Version, accessToken, refreshToken );
    }


    public ValueTask<OneOf<Tokens, Error>> Verify( string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Verify, jsonToken, types, token );
    public virtual async ValueTask<OneOf<Tokens, Error>> Verify( DbConnection connection, DbTransaction transaction, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, jsonToken, types, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await GetToken( connection, transaction, record, types, token );
    }


    public ValueTask<LoginResult> VerifyLogin( string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( VerifyLogin, jsonToken, types, token );
    protected virtual async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction transaction, string jsonToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        JwtSecurityTokenHandler                       handler              = new JwtSecurityTokenHandler();
        TokenValidationParameters validationParameters = await GetTokenValidationParameters( token );
        TokenValidationResult     validationResult     = await handler.ValidateTokenAsync( jsonToken, validationParameters );
        if ( validationResult.Exception is not null ) { return new LoginResult( validationResult.Exception ); }


        Claim[]     claims = validationResult.ClaimsIdentity.Claims.ToArray();
        UserRecord? record = await UserRecord.TryFromClaims( connection, transaction, this, claims, types | DEFAULT_CLAIM_TYPES, token );
        if ( record is null ) { return new LoginResult( LoginResult.State.NotFound ); }


        record.LastLogin = DateTimeOffset.UtcNow;
        await Users.Update( connection, transaction, record, token );
        return record;
    }


    public async ValueTask<UserLoginInfoRecord[]> GetUserLoginInfoRecords( string purpose, UserRecord user, CancellationToken token = default )
    {
        HashSet<UserLoginInfoRecord> list = await UserLogins.Where( true, UserLoginInfoRecord.GetDynamicParameters( user, purpose ), token ).ToHashSet( token );
        return [..list];
    }
    async Task<string> IUserTwoFactorTokenProvider<UserRecord>.GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) => await GenerateAsync( purpose, manager, user );
    public virtual async ValueTask<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user, CancellationToken token = default )
    {
        Tokens tokens = await GetToken( user, token: token );
        return tokens.AccessToken;
    }
    async Task<bool> IUserTwoFactorTokenProvider<UserRecord>.ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user ) => await ValidateAsync( purpose, token, manager, user );
    public virtual async ValueTask<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user, CancellationToken cancellationToken = default )
    {
        string? key = await manager.GetAuthenticatorKeyAsync( user );

        if ( string.IsNullOrWhiteSpace( key ) is false )
        {
            if ( int.TryParse( token, out _ ) )
            {
                AuthenticatorTokenProvider<UserRecord> provider = new AuthenticatorTokenProvider<UserRecord>();
                return await provider.ValidateAsync( purpose, token, manager, user );
            }

            IOneTimePassword otp = OneTimePassword.Create( key, Options.TokenIssuer );
            return otp.ValidateToken( token );
        }

        OneOf<Tokens, Error> result = await Verify( token, token: cancellationToken );
        return result.IsT0;
    }
    async Task<bool> IUserTwoFactorTokenProvider<UserRecord>.CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user ) => await CanGenerateTwoFactorTokenAsync( manager, user );
    public virtual async ValueTask<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user, CancellationToken token = default )
    {
        if ( user.IsValidID() is false || string.IsNullOrWhiteSpace( user.UserName ) ) { return false; }

        return await UserLogins.Where( true, UserLoginInfoRecord.GetDynamicParameters( user ), token ).Any( token );
    }
}
