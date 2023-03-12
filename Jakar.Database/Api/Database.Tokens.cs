// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:56 PM

namespace Jakar.Database;


public abstract partial class Database
{
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
    public ValueTask<Tokens?> Authenticate( VerifyRequest request, ClaimType types = default, CancellationToken token = default ) => this.TryCall( Authenticate, request, types, token );
    protected virtual async ValueTask<Tokens?> Authenticate( DbConnection connection, DbTransaction transaction, VerifyRequest request, ClaimType types = default, CancellationToken token = default )
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


    public ValueTask<Tokens> GetToken( UserRecord user, ClaimType types = default, CancellationToken token = default ) => this.TryCall( GetToken, user, types, token );

    public virtual async ValueTask<Tokens> GetToken( DbConnection connection, DbTransaction transaction, UserRecord user, ClaimType types = default, CancellationToken token = default )
    {
        Claim[] claims = await user.GetUserClaims( connection, transaction, this, types | DEFAULT_CLAIM_TYPES, token );

        var descriptor = new SecurityTokenDescriptor
                         {
                             Subject            = new ClaimsIdentity( claims ),
                             Expires            = GetExpiration( user, TimeSpan.FromMinutes( 15 ) ),
                             Issuer             = Options.TokenIssuer,
                             Audience           = Options.TokenAudience,
                             IssuedAt           = DateTime.UtcNow,
                             SigningCredentials = await GetSigningCredentials( token )
                         };


        DateTime refreshExpires = GetExpiration( user, TimeSpan.FromDays( 90 ) );

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
        string accessToken = handler.WriteToken( handler.CreateToken( descriptor ) );
        string refresh     = handler.WriteToken( handler.CreateToken( refreshDescriptor ) );


        user.SetHashedRefreshToken( refresh, refreshExpires );
        await Users.Update( connection, transaction, user, token );
        return new Tokens( accessToken, refresh, Version, user.UserID, user.FullName );
    }

    public ValueTask<OneOf<Tokens, Error>> Refresh( string refreshToken, ClaimType types = default, CancellationToken token = default ) => this.TryCall( Refresh, refreshToken, types, token );

    public async ValueTask<OneOf<Tokens, Error>> Refresh( DbConnection connection, DbTransaction transaction, string refreshToken, ClaimType types = default, CancellationToken token = default )
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
        return new Tokens( accessToken, refreshToken, Version, record.UserID, record.FullName );
    }

    public async ValueTask<OneOf<Tokens, Error>> Verify( DbConnection connection, DbTransaction transaction, string jwt, ClaimType types = default, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, jwt, types, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await GetToken( connection, transaction, record, types, token );
    }

    protected async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction transaction, string jwt, ClaimType types = default, CancellationToken token = default )
    {
        var                       handler              = new JwtSecurityTokenHandler();
        TokenValidationParameters validationParameters = await GetTokenValidationParameters( token );
        TokenValidationResult     validationResult     = await handler.ValidateTokenAsync( jwt, validationParameters );
        if ( validationResult.Exception is not null ) { return new LoginResult( validationResult.Exception ); }


        Claim[]     claims = validationResult.ClaimsIdentity.Claims.ToArray();
        UserRecord? record = await UserRecord.TryFromClaims( connection, transaction, this, claims, types | DEFAULT_CLAIM_TYPES, token );
        if ( record is null ) { return new LoginResult( LoginResult.State.NotFound ); }


        record.LastLogin = DateTimeOffset.UtcNow;
        await Users.Update( connection, transaction, record, token );
        return record;
    }
}
