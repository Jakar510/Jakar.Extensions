// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:56 PM

namespace Jakar.Database;


public abstract partial class Database
{
    private TimeSpan _accessTokenExpirationTime  = TimeSpan.FromMinutes( 15 );
    private TimeSpan _refreshTokenExpirationTime = TimeSpan.FromDays( 90 );


    public TimeSpan AccessTokenExpirationTime
    {
        get => _accessTokenExpirationTime;
        set => SetProperty( ref _accessTokenExpirationTime, value );
    }
    public TimeSpan RefreshTokenExpirationTime
    {
        get => _refreshTokenExpirationTime;
        set => SetProperty( ref _refreshTokenExpirationTime, value );
    }


    public virtual ValueTask<SigningCredentials> GetSigningCredentials( CancellationToken               token ) => new(Configuration.GetSigningCredentials( Options ));
    public virtual ValueTask<TokenValidationParameters> GetTokenValidationParameters( CancellationToken token ) => new(Configuration.GetTokenValidationParameters( Options ));


    public virtual async ValueTask<JwtSecurityToken> GetEmailJwtSecurityToken( IEnumerable<Claim> claims, CancellationToken token )
    {
        SigningCredentials signinCredentials = await GetSigningCredentials( token );
        var                security          = new JwtSecurityToken( Options.TokenIssuer, Options.TokenAudience, claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes( 15 ), signinCredentials );
        return security;
    }


    /// <summary> Only to be used for <see cref="ITokenService"/> </summary>
    /// <exception cref="ArgumentOutOfRangeException"> </exception>
    public ValueTask<Tokens?> Authenticate( VerifyRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Authenticate, request, types, token );
    protected virtual async ValueTask<Tokens?> Authenticate( DbConnection connection, DbTransaction transaction, VerifyRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        UserRecord? user = await Users.Get( nameof(UserRecord.UserName), request.UserLogin, token );
        if ( user is null ) { return default; }

        if ( user.SubscriptionExpires.HasValue )
        {
            if ( user.SubscriptionExpires.Value < DateTimeOffset.UtcNow )
            {
                user.LastBadAttempt = DateTimeOffset.UtcNow;
                await Users.Update( connection, transaction, user, token );

                return default;
            }
        }

        if ( user.IsDisabled )
        {
            user.LastBadAttempt = DateTimeOffset.UtcNow;
            await Users.Update( connection, transaction, user, token );

            return default;
        }

        if ( user.IsLocked )
        {
            user.LastBadAttempt = DateTimeOffset.UtcNow;
            await Users.Update( connection, transaction, user, token );

            return default;
        }


        if ( user.VerifyPassword( request.UserPassword ) ) { return await GetToken( connection, transaction, user, types, token ); }

        await Users.Update( connection, transaction, user, token );
        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
    protected static DateTime GetExpiration( UserRecord record, TimeSpan offset )
    {
        DateTime expires = DateTime.UtcNow + offset;

        if ( record.SubscriptionExpires is null ) { return expires; }

        DateTime date = record.SubscriptionExpires.Value.LocalDateTime;
        if ( expires > date ) { expires = date; }

        return expires;
    }


    public async Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user )
    {
        // IUserTwoFactorTokenProvider<UserRecord> provider = new AuthenticatorTokenProvider<UserRecord>()
        Tokens token = await GetToken( user, DEFAULT_CLAIM_TYPES, CancellationToken.None );
        return token.AccessToken;
    }
    public async Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user )
    {
        // IUserTwoFactorTokenProvider<UserRecord> provider = new AuthenticatorTokenProvider<UserRecord>()
        OneOf<Tokens, Error> result = await Verify( token, DEFAULT_CLAIM_TYPES, CancellationToken.None );
        return result.IsT0;
    }
    public async Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user )
    {
        if ( !user.IsValidID() || string.IsNullOrWhiteSpace( user.UserName ) ) { return false; }

        IEnumerable<UserLoginInfoRecord> logins = await UserLogins.Where( true, UserLoginInfoRecord.GetDynamicParameters( user ) );
        return logins.Any();
    }
    public ValueTask<Tokens> GetToken( UserRecord user, ClaimType types = default, CancellationToken token = default ) => this.TryCall( GetToken, user, types, token );
    public virtual async ValueTask<Tokens> GetToken( DbConnection connection, DbTransaction transaction, UserRecord user, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        Claim[] claims = await user.GetUserClaims( connection, transaction, this, types, token );

        var accessDescriptor = new SecurityTokenDescriptor
                               {
                                   Subject            = new ClaimsIdentity( claims ),
                                   Expires            = GetExpiration( user, AccessTokenExpirationTime ),
                                   Issuer             = Options.TokenIssuer,
                                   Audience           = Options.TokenAudience,
                                   IssuedAt           = DateTime.UtcNow,
                                   SigningCredentials = await GetSigningCredentials( token )
                               };


        DateTime refreshExpires = GetExpiration( user, RefreshTokenExpirationTime );

        var refreshDescriptor = new SecurityTokenDescriptor
                                {
                                    Subject            = new ClaimsIdentity( claims ),
                                    Expires            = refreshExpires,
                                    Issuer             = Options.TokenIssuer,
                                    Audience           = Options.TokenAudience,
                                    IssuedAt           = DateTime.UtcNow,
                                    SigningCredentials = await GetSigningCredentials( token )
                                };


        var    handler     = new JwtSecurityTokenHandler();
        string accessToken = handler.WriteToken( handler.CreateToken( accessDescriptor ) );
        string refresh     = handler.WriteToken( handler.CreateToken( refreshDescriptor ) );


        user.SetRefreshToken( refresh, refreshExpires );
        await Users.Update( connection, transaction, user, token );
        return new Tokens( user.UserID, user.FullName, Version, accessToken, refresh );
    }


    public ValueTask<OneOf<Tokens, Error>> Refresh( string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Refresh, refreshToken, types, token );
    public virtual async ValueTask<OneOf<Tokens, Error>> Refresh( DbConnection connection, DbTransaction transaction, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, refreshToken, types, token );
        if ( loginResult.GetResult( out Error? error, out UserRecord? record ) ) { return error.Value; }

        if ( !record.IsHashedRefreshToken( refreshToken ) )
        {
            record.MarkBadLogin();
            await Users.Update( connection, transaction, record, token );
            return new Error( Status.Unauthorized );
        }


        Claim[] claims = await record.GetUserClaims( connection, transaction, this, types | DEFAULT_CLAIM_TYPES, token );

        var descriptor = new SecurityTokenDescriptor
                         {
                             Subject            = new ClaimsIdentity( claims ),
                             Expires            = GetExpiration( record, TimeSpan.FromMinutes( 15 ) ),
                             Issuer             = Options.TokenIssuer,
                             Audience           = Options.TokenAudience,
                             IssuedAt           = DateTime.UtcNow,
                             SigningCredentials = await GetSigningCredentials( token )
                         };

        var    handler     = new JwtSecurityTokenHandler();
        string accessToken = handler.WriteToken( handler.CreateToken( descriptor ) );
        return new Tokens( record.UserID, record.FullName, Version, accessToken, refreshToken );
    }


    public ValueTask<OneOf<Tokens, Error>> Verify( string jwt, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Verify, jwt, types, token );
    public virtual async ValueTask<OneOf<Tokens, Error>> Verify( DbConnection connection, DbTransaction transaction, string jwt, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, jwt, types, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await GetToken( connection, transaction, record, types, token );
    }


    public ValueTask<LoginResult> VerifyLogin( string jwt, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( VerifyLogin, jwt, types, token );
    protected virtual async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction transaction, string jwt, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        var                       handler              = new JwtSecurityTokenHandler();
        TokenValidationParameters validationParameters = await GetTokenValidationParameters( token );
        TokenValidationResult     validationResult     = await handler.ValidateTokenAsync( jwt, validationParameters );
        if ( validationResult.Exception is not null ) { return new LoginResult( validationResult.Exception ); }


        Claim[]     claims = validationResult.ClaimsIdentity.Claims.GetArray();
        UserRecord? record = await UserRecord.TryFromClaims( connection, transaction, this, claims, types | DEFAULT_CLAIM_TYPES, token );
        if ( record is null ) { return new LoginResult( LoginResult.State.NotFound ); }


        record.LastLogin = DateTimeOffset.UtcNow;
        await Users.Update( connection, transaction, record, token );
        return record;
    }
}
