// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public abstract class Database : Randoms, IConnectableDb, IAsyncDisposable, IHealthCheck
{
    protected readonly ConcurrentBag<IAsyncDisposable> _disposables = new();
    protected readonly IConfiguration                  _configuration;
    public abstract    AppVersion                      Version { get; }
    public             DbTable<RoleRecord>             Roles   { get; }


    public DbTable<UserRecord>     Users     { get; }
    public DbTable<UserRoleRecord> UserRoles { get; }
    public string                  Domain    { get; set; } = "https://localhost:443";


    static Database()
    {
        EnumSqlHandler<SupportedLanguage>.Register();
        EnumSqlHandler<MimeType>.Register();
        EnumSqlHandler<Status>.Register();
        EnumSqlHandler<AppVersion.Format>.Register();
        DateTimeOffsetHandler.Register();
        DateTimeHandler.Register();
        DateOnlyHandler.Register();
        TimeOnlyHandler.Register();
        AppVersionHandler.Register();
    }
    protected Database( IConfiguration configuration ) : base()
    {
        _configuration = configuration;
        Users          = Create<UserRecord>();
        Roles          = Create<RoleRecord>();
        UserRoles      = Create<UserRoleRecord>();
    }

    public static WebApplicationBuilder Register<T>( WebApplicationBuilder builder ) where T : Database
    {
        builder.AddSingleton<T>();
        return builder;
    }
    private static async ValueTask<LoginResult> VerifyLogin( UserRecord user )
    {
        await ValueTask.CompletedTask;

        if (!user.IsActive) { return LoginResult.State.Inactive; }

        if (!user.IsDisabled) { return LoginResult.State.Disabled; }

        if (!user.IsLocked) { return LoginResult.State.Locked; }

        // if ( user.SubscriptionExpires > DateTimeOffset.UtcNow ) { return LoginResult.State.ExpiredSubscription; }
        // if ( user.SubscriptionID.IsValidID() ) { return LoginResult.State.NoSubscription; }

        return user;
    }

    protected DbTable<TRecord> Create<TRecord>() where TRecord : TableRecord<TRecord> => AddDisposable( new DbTable<TRecord>( this ) );
    protected TValue AddDisposable<TValue>( TValue value ) where TValue : IAsyncDisposable
    {
        _disposables.Add( value );
        return value;
    }


    protected abstract DbConnection CreateConnection();


    private async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction? transaction, VerifyRequest request, CancellationToken token = default )
    {
        UserRecord? user = await Users.Get( connection, transaction, nameof(UserRecord.UserName), request.UserLogin, token );
        if (user is null) { return LoginResult.State.BadCredentials; }

        PasswordVerificationResult passResult = user.VerifyPassword( request.UserPassword );

        switch (passResult)
        {
            case PasswordVerificationResult.Failed: { return LoginResult.State.BadCredentials; }

            case PasswordVerificationResult.Success: { return await VerifyLogin( user ); }

            case PasswordVerificationResult.SuccessRehashNeeded:
            {
                user.UpdatePassword( request.UserPassword );
                return await VerifyLogin( user );
            }

            default: throw new OutOfRangeException( nameof(passResult), passResult );
        }
    }
    public async ValueTask<ActionResult<UserRecord>> Verify( DbConnection connection, DbTransaction? transaction, ControllerBase controller, VerifyRequest request, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );
        if (loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? caller )) { return actionResult; }

        return caller;
    }


    public ValueTask<Tokens> GetJwtToken( UserRecord user, CancellationToken token ) => this.Call( GetJwtToken, user, token );
    public async ValueTask<Tokens> GetJwtToken( DbConnection connection, DbTransaction? transaction, UserRecord user, CancellationToken token )
    {
        var            handler = new JwtSecurityTokenHandler();
        List<Claim>    claims  = await user.GetUserClaims( connection, transaction, this, token );
        DateTimeOffset date    = _configuration.TokenExpiration();

        if (user.SubscriptionExpires.HasValue)
        {
            DateTimeOffset expires = user.SubscriptionExpires.Value;
            if (date > expires) { date = expires; }
        }

        var descriptor = new SecurityTokenDescriptor
                         {
                             Subject            = new ClaimsIdentity( claims ),
                             Expires            = date.LocalDateTime.ToUniversalTime(),
                             SigningCredentials = _configuration.GetSigningCredentials()
                         };

        SecurityToken? security     = handler.CreateToken( descriptor );
        string         refreshToken = GenerateToken();
        user.SetRefreshToken( refreshToken, date );

        return Tokens.Create( handler.WriteToken( security ), refreshToken, Version, user );
    }


    /// <summary>
    ///     Only to be used for
    ///     <see cref = "ITokenService" />
    /// </summary>
    /// <exception cref = "ArgumentOutOfRangeException" > </exception>
    public async Task<Tokens> Authenticate( VerifyRequest request, CancellationToken token )
    {
        await using DbConnection  connection  = await ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( token );


        try
        {
            Tokens result = await Authenticate( connection, transaction, request, token );
            await transaction.CommitAsync( token );
            return result;
        }
        catch (Exception e)
        {
            e.WriteToDebug();
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    protected virtual async Task<Tokens> Authenticate( DbConnection connection, DbTransaction transaction, VerifyRequest request, CancellationToken token )
    {
        UserRecord? user = await Users.Get( nameof(UserRecord.UserName), request.UserLogin, token );
        if (user is null) { return default; }

        if (user.SubscriptionExpires.HasValue)
        {
            if (user.SubscriptionExpires.Value < DateTimeOffset.UtcNow)
            {
                user.LastBadAttempt = DateTimeOffset.UtcNow;
                await Users.Update( connection, transaction, user, token );

                return default;
            }
        }

        if (user.IsDisabled)
        {
            user.LastBadAttempt = DateTimeOffset.UtcNow;
            await Users.Update( connection, transaction, user, token );

            return default;
        }

        if (user.IsLocked)
        {
            user.LastBadAttempt = DateTimeOffset.UtcNow;
            await Users.Update( connection, transaction, user, token );

            return default;
        }


        PasswordVerificationResult passwordVerificationResult = user.VerifyPassword( request.UserPassword );

        switch (passwordVerificationResult)
        {
            case PasswordVerificationResult.Failed:
                user.MarkBadLogin();
                await Users.Update( connection, transaction, user, token );

                return default;

            case PasswordVerificationResult.Success:
                user.SetActive();
                await Users.Update( connection, transaction, user, token );
                return await GetToken( connection, transaction, user, token );

            case PasswordVerificationResult.SuccessRehashNeeded:
                user.SetActive();
                user.UpdatePassword( request.UserPassword );
                await Users.Update( connection, transaction, user, token );
                return await GetToken( connection, transaction, user, token );

            default: throw new ArgumentOutOfRangeException( nameof(passwordVerificationResult), passwordVerificationResult, "out of range" );
        }
    }
    protected virtual ValueTask<Tokens> GetToken( DbConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token ) => GetJwtToken( connection, transaction, user, token );


    public virtual async ValueTask DisposeAsync()
    {
        foreach (IAsyncDisposable disposable in _disposables) { await disposable.DisposeAsync(); }

        _disposables.Clear();
    }
    public DbConnection Connect()
    {
        DbConnection connection = CreateConnection();
        connection.Open();
        return connection;
    }
    public async ValueTask<DbConnection> ConnectAsync( CancellationToken token )
    {
        DbConnection connection = CreateConnection();
        await connection.OpenAsync( token );
        return connection;
    }


    public virtual async Task<HealthCheckResult> CheckHealthAsync( HealthCheckContext context, CancellationToken token = default )
    {
        try
        {
            await using DbConnection connection = await ConnectAsync( token );

            return connection.State switch
                   {
                       ConnectionState.Broken     => HealthCheckResult.Unhealthy(),
                       ConnectionState.Closed     => HealthCheckResult.Degraded(),
                       ConnectionState.Open       => HealthCheckResult.Healthy(),
                       ConnectionState.Connecting => HealthCheckResult.Healthy(),
                       ConnectionState.Executing  => HealthCheckResult.Healthy(),
                       ConnectionState.Fetching   => HealthCheckResult.Healthy(),
                       _                          => throw new ArgumentOutOfRangeException()
                   };
        }
        catch (Exception e) { return HealthCheckResult.Unhealthy( e.Message ); }
    }
}
