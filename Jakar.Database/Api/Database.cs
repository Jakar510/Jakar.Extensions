// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;



namespace Jakar.Database;


public static class DatabaseDefaults
{
    public static string Schema
    {
        get
        {
            lock (_schemaLock) { return _defaultSchema; }
        }
        set
        {
            lock (_schemaLock) { _defaultSchema = value; }
        }
    }


    public const            string DEFAULT_SCHEMA = "dbo";
    private static readonly object _schemaLock    = new();
    private static          string _defaultSchema = DEFAULT_SCHEMA;
}



[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public abstract class Database : Randoms, IConnectableDb, IAsyncDisposable, IHealthCheck
{
    protected readonly ConcurrentBag<IAsyncDisposable> _disposables   = new();
    private            string                          _currentSchema = DatabaseDefaults.Schema;
    private            Uri                             _domain        = new("https://localhost:443");


    public virtual     AppVersion                  Version           => Options.Version ?? throw new NullReferenceException( nameof(DbOptions.Version) );
    public virtual     DbInstance                  Instance          => Options.DbType;
    public             DbOptions                   Options           { get; }
    public             DbTable<GroupRecord>        Groups            { get; }
    public             DbTable<RecoveryCodeRecord> RecoveryCodes     { get; }
    public             DbTable<RoleRecord>         Roles             { get; }
    public             DbTable<UserGroupRecord>    UserGroups        { get; }
    public             DbTable<UserRecord>         Users             { get; }
    public             DbTable<UserRoleRecord>     UserRoles         { get; }
    public             IConfiguration              Configuration     { get; }
    protected abstract PasswordRequirements        _Requirements     { get; }
    protected internal PasswordValidator           PasswordValidator => new(_Requirements);
    public virtual     string                      ConnectionString  => Configuration.ConnectionString();
    public string CurrentSchema
    {
        get => _currentSchema;
        set
        {
            if ( SetProperty( ref _currentSchema, value ) ) { DatabaseDefaults.Schema = value; }
        }
    }
    public Uri Domain
    {
        get => _domain;
        set => SetProperty( ref _domain, value );
    }


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
    protected Database( IConfiguration configuration, IOptions<DbOptions> options ) : base()
    {
        Configuration = configuration;
        Options       = options.Value;
        Users         = Create<UserRecord>();
        Roles         = Create<RoleRecord>();
        UserRoles     = Create<UserRoleRecord>();
        UserGroups    = Create<UserGroupRecord>();
        Groups        = Create<GroupRecord>();
        RecoveryCodes = Create<RecoveryCodeRecord>();
    }


    /// <summary> Only to be used for <see cref="ITokenService"/> </summary>
    /// <exception cref="ArgumentOutOfRangeException"> </exception>
    public ValueTask<Tokens?> Authenticate( VerifyRequest request, CancellationToken token ) => this.TryCall( Authenticate, request, token );
    protected virtual async ValueTask<Tokens?> Authenticate( DbConnection connection, DbTransaction transaction, VerifyRequest request, CancellationToken token )
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


        PasswordVerificationResult passwordVerificationResult = user.VerifyPassword( request.UserPassword );

        switch ( passwordVerificationResult )
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


    protected internal ValueTask<IReadOnlyCollection<RecoveryCodeRecord>> Codes( UserRecord user, CancellationToken token ) => this.TryCall( Codes, user, token );
    protected internal async ValueTask<IReadOnlyCollection<RecoveryCodeRecord>> Codes( DbConnection connection, DbTransaction transaction, UserRecord user, CancellationToken token ) =>
        await RecoveryCodes.Where( connection, transaction, nameof(RecoveryCodeRecord.CreatedBy), user.UserID, token );


    protected abstract DbConnection CreateConnection();


    public ValueTask<Tokens> GetJwtToken( UserRecord user, CancellationToken token ) => this.Call( GetJwtToken, user, token );
    public async ValueTask<Tokens> GetJwtToken( DbConnection connection, DbTransaction? transaction, UserRecord user, CancellationToken token )
    {
        var            handler = new JwtSecurityTokenHandler();
        List<Claim>    claims  = await user.GetUserClaims( connection, transaction, this, token );
        DateTimeOffset date    = Configuration.TokenExpiration();

        if ( user.SubscriptionExpires.HasValue )
        {
            DateTimeOffset expires = user.SubscriptionExpires.Value;
            if ( date > expires ) { date = expires; }
        }

        var descriptor = new SecurityTokenDescriptor
                         {
                             Subject            = new ClaimsIdentity( claims ),
                             Expires            = date.LocalDateTime.ToUniversalTime(),
                             SigningCredentials = Configuration.GetSigningCredentials()
                         };

        SecurityToken? security     = handler.CreateToken( descriptor );
        string         refreshToken = GenerateToken();
        user.SetRefreshToken( refreshToken, date );

        await Users.Update( connection, transaction, user, token );
        return Tokens.Create( handler.WriteToken( security ), refreshToken, Version, user );
    }


    public ValueTask<Tokens> GetToken( UserRecord           user,       CancellationToken token ) => this.Call( GetToken, user, token );
    public virtual ValueTask<Tokens> GetToken( DbConnection connection, DbTransaction?    transaction, UserRecord user, CancellationToken token ) => GetJwtToken( connection, transaction, user, token );


    public ValueTask<bool> RedeemCode( UserRecord user, string code, CancellationToken token ) => this.TryCall( RedeemCode, user, code, token );
    public async ValueTask<bool> RedeemCode( DbConnection connection, DbTransaction transaction, UserRecord user, string code, CancellationToken token )
    {
        IReadOnlyCollection<RecoveryCodeRecord> records = await Codes( connection, transaction, user, token );

        foreach ( RecoveryCodeRecord record in records )
        {
            if ( !record.IsValid( code ) ) { continue; }

            await RecoveryCodes.Delete( connection, transaction, record, token );
            return true;
        }

        return false;
    }


    public ValueTask<ActionResult<Tokens>> Refresh( ControllerBase controller, string refreshToken, CancellationToken token ) => this.TryCall( Refresh, controller, refreshToken, token );
    public async ValueTask<ActionResult<Tokens>> Refresh( DbConnection connection, DbTransaction? transaction, ControllerBase controller, string refreshToken, CancellationToken token )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, refreshToken, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? user )
                   ? actionResult
                   : await GetToken( connection, transaction, user, token );
    }


    public ValueTask<ActionResult<Tokens>> Register( ControllerBase controller, VerifyRequest<UserData> request, CancellationToken token = default ) => this.TryCall( Register, controller, request, token );
    public virtual async ValueTask<ActionResult<Tokens>> Register( DbConnection connection, DbTransaction transaction, ControllerBase controller, VerifyRequest<UserData> request, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return controller.Duplicate(); }


        if ( !PasswordValidator.Validate( request.Request.UserPassword ) )
        {
            controller.AddError( "Password Validation Failed" );
            return controller.BadRequest( controller.ModelState );
        }


        record = UserRecord.Create( request );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, token );
    }
    public ValueTask<string[]> ReplaceCodes( UserRecord user, int count = 10, CancellationToken token = default ) => this.TryCall( ReplaceCodes, user, count, token );
    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, UserRecord user, int count = 10, CancellationToken token = default )
    {
        await RecoveryCodes.Delete( connection, transaction, await Codes( connection, transaction, user, token ), token );


        string[] codes = new string[count];

        for ( int i = 0; i < count; i++ )
        {
            (RecoveryCodeRecord record, string code) = RecoveryCodeRecord.Create( user );
            codes[i]                                 = code;
            await RecoveryCodes.Insert( connection, transaction, record, token );
        }

        return codes;
    }
    public ValueTask ReplaceCodes( UserRecord user, IEnumerable<string> recoveryCodes, CancellationToken token = default ) => this.TryCall( ReplaceCodes, user, recoveryCodes, token );
    public async ValueTask ReplaceCodes( DbConnection connection, DbTransaction transaction, UserRecord user, IEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        await RecoveryCodes.Delete( connection, transaction, await Codes( connection, transaction, user, token ), token );

        foreach ( string recoveryCode in recoveryCodes )
        {
            var record = RecoveryCodeRecord.Create( user, recoveryCode );
            await RecoveryCodes.Insert( connection, transaction, record, token );
        }
    }
    
    
    public ValueTask<ActionResult<T>> Verify<T>( ControllerBase controller, VerifyRequest request, Func<UserRecord, ActionResult<T>> func, CancellationToken token = default ) => this.TryCall( Verify, controller, request, func, token );
    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, ControllerBase controller, VerifyRequest request, Func<UserRecord, ActionResult<T>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : func( caller );
    }
    public ValueTask<ActionResult<T>> Verify<T>( ControllerBase controller, VerifyRequest request, Func<UserRecord, ValueTask<ActionResult<T>>> func, CancellationToken token = default ) => this.TryCall( Verify, controller, request, func, token );
    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection                                 connection,
                                                               DbTransaction?                               transaction,
                                                               ControllerBase                               controller,
                                                               VerifyRequest                                request,
                                                               Func<UserRecord, ValueTask<ActionResult<T>>> func,
                                                               CancellationToken                            token = default
    )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }
    public ValueTask<ActionResult<T>> Verify<T>( ControllerBase controller, VerifyRequest request, Func<UserRecord, Task<ActionResult<T>>> func, CancellationToken token = default ) => this.TryCall( Verify, controller, request, func, token );
    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, ControllerBase controller, VerifyRequest request, Func<UserRecord, Task<ActionResult<T>>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }


    public ValueTask<ActionResult<Tokens>> Verify( ControllerBase controller, string refreshToken, CancellationToken token ) => this.TryCall( Verify, controller, refreshToken, token );
    public async ValueTask<ActionResult<Tokens>> Verify( DbConnection connection, DbTransaction? transaction, ControllerBase controller, string refreshToken, CancellationToken token )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, refreshToken, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? user )
                   ? actionResult
                   : await GetToken( connection, transaction, user, token );
    }


    public ValueTask<ActionResult<Tokens>> Verify( ControllerBase controller, VerifyRequest request, CancellationToken token = default ) => this.TryCall( Verify, controller, request, token );
    public virtual async ValueTask<ActionResult<Tokens>> Verify( DbConnection connection, DbTransaction? transaction, ControllerBase controller, VerifyRequest request, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? user )
                   ? actionResult
                   : await GetToken( connection, transaction, user, token );
    }


    public ValueTask<ActionResult> Verify( ControllerBase controller, VerifyRequest request, Func<UserRecord, ActionResult> func, CancellationToken token = default ) => this.TryCall( Verify, controller, request, func, token );
    public virtual async ValueTask<ActionResult> Verify( DbConnection connection, DbTransaction? transaction, ControllerBase controller, VerifyRequest request, Func<UserRecord, ActionResult> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : func( caller );
    }


    public ValueTask<ActionResult> Verify( ControllerBase controller, VerifyRequest request, Func<UserRecord, ValueTask<ActionResult>> func, CancellationToken token = default ) => this.TryCall( Verify, controller, request, func, token );
    public virtual async ValueTask<ActionResult> Verify( DbConnection connection, DbTransaction? transaction, ControllerBase controller, VerifyRequest request, Func<UserRecord, ValueTask<ActionResult>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }


    public ValueTask<ActionResult> Verify( ControllerBase controller, VerifyRequest request, Func<UserRecord, Task<ActionResult>> func, CancellationToken token = default ) => this.TryCall( Verify, controller, request, func, token );
    public virtual async ValueTask<ActionResult> Verify( DbConnection connection, DbTransaction? transaction, ControllerBase controller, VerifyRequest request, Func<UserRecord, Task<ActionResult>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }


    protected virtual async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction? transaction, VerifyRequest request, CancellationToken token = default )
    {
        UserRecord? user = await Users.Get( connection, transaction, nameof(UserRecord.UserName), request.UserLogin, token );
        if ( user is null ) { return LoginResult.State.BadCredentials; }

        PasswordVerificationResult passResult = user.VerifyPassword( request.UserPassword );

        switch ( passResult )
        {
            case PasswordVerificationResult.Failed:
            {
                user.MarkBadLogin();
                await Users.Update( connection, transaction, user, token );
                return LoginResult.State.BadCredentials;
            }

            case PasswordVerificationResult.Success:
            {
                user.SetActive();
                return await VerifyLogin( user );
            }

            case PasswordVerificationResult.SuccessRehashNeeded:
            {
                user.SetActive();
                user.UpdatePassword( request.UserPassword );
                await Users.Update( connection, transaction, user, token );
                return await VerifyLogin( user );
            }

            default: throw new OutOfRangeException( nameof(passResult), passResult );
        }
    }
    protected virtual async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction? transaction, string refreshToken, CancellationToken cancellationToken = default )
    {
        UserRecord? user = await Users.Get( connection, transaction, nameof(UserRecord.RefreshToken), refreshToken, cancellationToken );
        if ( user is null ) { return LoginResult.State.BadCredentials; }

        user.SetActive();
        await Users.Update( connection, transaction, user, cancellationToken );
        return await VerifyLogin( user );
    }
    protected virtual async ValueTask<LoginResult> VerifyLogin( UserRecord user )
    {
        await ValueTask.CompletedTask;

        if ( !user.IsActive ) { return LoginResult.State.Inactive; }

        if ( !user.IsDisabled ) { return LoginResult.State.Disabled; }

        if ( !user.IsLocked ) { return LoginResult.State.Locked; }

        // if ( user.SubscriptionExpires > DateTimeOffset.UtcNow ) { return LoginResult.State.ExpiredSubscription; }

        // if ( user.SubscriptionID.IsValidID() ) { return LoginResult.State.NoSubscription; }

        return user;
    }



    #region Core

    protected TValue AddDisposable<TValue>( TValue value ) where TValue : IAsyncDisposable
    {
        _disposables.Add( value );
        return value;
    }
    protected virtual DbTable<TRecord> Create<TRecord>() where TRecord : TableRecord<TRecord>
    {
        var table = new DbTable<TRecord>( this );
        return AddDisposable( table );
    }

    public virtual async ValueTask DisposeAsync()
    {
        foreach ( IAsyncDisposable disposable in _disposables ) { await disposable.DisposeAsync(); }

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
        catch ( Exception e ) { return HealthCheckResult.Unhealthy( e.Message ); }
    }

    #endregion
}
