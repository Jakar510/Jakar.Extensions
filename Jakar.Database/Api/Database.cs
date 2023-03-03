// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using Microsoft.AspNetCore.Mvc;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public abstract partial class Database : Randoms, IConnectableDb, IAsyncDisposable, IHealthCheck
{
    protected readonly ConcurrentBag<IAsyncDisposable> _disposables   = new();
    private            Uri                             _domain        = new("https://localhost:443");
    private            string                          _currentSchema = string.Empty;


    public string CurrentSchema
    {
        get => _currentSchema;
        protected set => SetProperty( ref _currentSchema, value );
    }
    public         IConfiguration Configuration    { get; }
    public virtual string         ConnectionString => Configuration.ConnectionString();
    public Uri Domain
    {
        get => _domain;
        set => SetProperty( ref _domain, value );
    }
    public             DbTable<GroupRecord>            Groups            { get; }
    public             DbInstance                      Instance          => Options.DbType;
    public             DbOptions                       Options           { get; }
    protected internal PasswordValidator               PasswordValidator => new(Options.PasswordRequirements);
    public             DbTable<RecoveryCodeRecord>     RecoveryCodes     { get; }
    public             DbTable<RoleRecord>             Roles             { get; }
    public             DbTable<UserGroupRecord>        UserGroups        { get; }
    public             DbTable<UserLoginInfoRecord>    UserLogins        { get; }
    public             DbTable<UserRecoveryCodeRecord> UserRecoveryCodes { get; }
    public             DbTable<UserRoleRecord>         UserRoles         { get; }
    public             DbTable<UserRecord>             Users             { get; }
    public             AppVersion                      Version           => Options.Version;


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
        Configuration     = configuration;
        Options           = options.Value;
        Users             = Create<UserRecord>();
        Roles             = Create<RoleRecord>();
        UserRoles         = Create<UserRoleRecord>();
        UserGroups        = Create<UserGroupRecord>();
        Groups            = Create<GroupRecord>();
        RecoveryCodes     = Create<RecoveryCodeRecord>();
        UserLogins        = Create<UserLoginInfoRecord>();
        UserRecoveryCodes = Create<UserRecoveryCodeRecord>();

        CurrentSchema = Instance switch
                        {
                            DbInstance.MsSql    => "dbo",
                            DbInstance.Postgres => "public",
                            _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                        };
    }


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
    

    [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
    public static T[] GetArray<T>( IEnumerable<T> enumerable ) => enumerable switch
                                                                  {
                                                                      List<T> list       => list.GetInternalArray(),
                                                                      Collection<T> list => list.GetInternalArray(),
                                                                      T[] array          => array,
                                                                      _                  => enumerable.ToArray()
                                                                  };


    protected abstract DbConnection CreateConnection();
    public virtual async ValueTask DisposeAsync()
    {
        foreach ( IAsyncDisposable disposable in _disposables ) { await disposable.DisposeAsync(); }

        _disposables.Clear();
        GC.SuppressFinalize( this );
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



    #region Tokens

    /// <summary> Only to be used for <see cref="ITokenService"/> </summary>
    /// <exception cref="ArgumentOutOfRangeException"> </exception>
    public ValueTask<Tokens?> Authenticate( VerifyRequest request, ClaimType types, CancellationToken token ) => this.TryCall( Authenticate, request, types, token );
    protected virtual async ValueTask<Tokens?> Authenticate( DbConnection connection, DbTransaction transaction, VerifyRequest request, ClaimType types, CancellationToken token )
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
                return await GetToken( connection, transaction, user, types, token );

            case PasswordVerificationResult.SuccessRehashNeeded:
                user.SetActive();
                user.UpdatePassword( request.UserPassword );
                await Users.Update( connection, transaction, user, token );
                return await GetToken( connection, transaction, user, types, token );

            default: throw new OutOfRangeException( nameof(passwordVerificationResult), passwordVerificationResult );
        }
    }


    public ValueTask<Tokens> GetJwtToken( UserRecord user, ClaimType types, CancellationToken token ) => this.TryCall( GetJwtToken, user, types, token );
    public async ValueTask<Tokens> GetJwtToken( DbConnection connection, DbTransaction transaction, UserRecord user, ClaimType types, CancellationToken token )
    {
        List<Claim> claims = await user.GetUserClaims( connection, transaction, this, types, token );

        DateTimeOffset expires = Configuration.TokenExpiration();

        // ReSharper disable once InvertIf
        if ( user.SubscriptionExpires.HasValue )
        {
            DateTimeOffset date = user.SubscriptionExpires.Value;
            if ( expires > date ) { expires = date; }
        }

        if ( user.SubscriptionExpires < expires ) { expires = user.SubscriptionExpires.Value; }

        var handler = new JwtSecurityTokenHandler();

        var descriptor = new SecurityTokenDescriptor
                         {
                             Subject            = new ClaimsIdentity( claims ),
                             Expires            = expires.LocalDateTime,
                             SigningCredentials = Configuration.GetSigningCredentials(),
                         };

        string accessToken = handler.WriteToken( handler.CreateToken( descriptor ) );
        string refresh     = handler.WriteToken( handler.CreateToken( descriptor ) );

        user.SetRefreshToken( refresh, expires );
        await Users.Update( connection, transaction, user, token );
        var result = new Tokens( accessToken, refresh, Version, user.UserID, user.FullName );
        return result;
    }


    public ValueTask<Tokens> GetToken( UserRecord           user,       ClaimType     types,       CancellationToken token ) => this.TryCall( GetToken, user, types, token );
    public virtual ValueTask<Tokens> GetToken( DbConnection connection, DbTransaction transaction, UserRecord        user, ClaimType types, CancellationToken token ) => GetJwtToken( connection, transaction, user, types, token );


    public ValueTask<ActionResult<Tokens>> Refresh( ControllerBase controller, string refreshToken, ClaimType types, CancellationToken token ) => this.TryCall( Refresh, controller, refreshToken, types, token );
    public async ValueTask<ActionResult<Tokens>> Refresh( DbConnection connection, DbTransaction transaction, ControllerBase controller, string refreshToken, ClaimType types, CancellationToken token )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, refreshToken, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? user )
                   ? actionResult
                   : await GetToken( connection, transaction, user, types, token );
    }

    #endregion



    #region Logins

    public ValueTask<ActionResult<Tokens>> Register( ControllerBase controller, VerifyRequest<UserData> request, string rights, ClaimType types, CancellationToken token = default ) =>
        this.TryCall( ( connection, transaction, controller1, request1, rights1, token1 ) => Register( connection, transaction, controller1, request1, rights1, types, token1 ), controller, request, rights, token );
    public virtual async ValueTask<ActionResult<Tokens>> Register( DbConnection connection, DbTransaction transaction, ControllerBase controller, VerifyRequest<UserData> request, string rights, ClaimType types, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return controller.Duplicate(); }


        if ( !PasswordValidator.Validate( request.UserPassword ) )
        {
            controller.AddError( "Password Validation Failed" );
            return controller.BadRequest( controller.ModelState );
        }


        record = UserRecord.Create( request, rights );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, types, token );
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


    public ValueTask<ActionResult<Tokens>> Verify( ControllerBase controller, string refreshToken, ClaimType types, CancellationToken token ) => this.TryCall( Verify, controller, refreshToken, types, token );
    public async ValueTask<ActionResult<Tokens>> Verify( DbConnection connection, DbTransaction transaction, ControllerBase controller, string refreshToken, ClaimType types, CancellationToken token )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, refreshToken, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? user )
                   ? actionResult
                   : await GetToken( connection, transaction, user, types, token );
    }


    public ValueTask<ActionResult<Tokens>> Verify( ControllerBase controller, VerifyRequest request, ClaimType types, CancellationToken token = default ) => this.TryCall( Verify, controller, request, types, token );
    public virtual async ValueTask<ActionResult<Tokens>> Verify( DbConnection connection, DbTransaction transaction, ControllerBase controller, VerifyRequest request, ClaimType types, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( controller, out ActionResult? actionResult, out UserRecord? user )
                   ? actionResult
                   : await GetToken( connection, transaction, user, types, token );
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

    #endregion
}
