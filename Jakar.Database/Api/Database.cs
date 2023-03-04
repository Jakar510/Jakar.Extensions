// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using System.Security.Claims;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public abstract partial class Database : Randoms, IConnectableDb, IAsyncDisposable, IHealthCheck
{
    public const       ClaimType                       DEFAULT_CLAIM_TYPES = ClaimType.UserID | ClaimType.UserName;
    protected readonly ConcurrentBag<IAsyncDisposable> _disposables        = new();
    private            Uri                             _domain             = new("https://localhost:443");
    private            string                          _currentSchema      = string.Empty;


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


    public async ValueTask<Tokens> GetJwtToken( DbConnection connection, DbTransaction transaction, UserRecord user, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        List<Claim> claims = await user.GetUserClaims( connection, transaction, this, types | DEFAULT_CLAIM_TYPES, token );


        DateTime expires = DateTime.UtcNow + TimeSpan.FromMinutes( 30 );

        if ( user.SubscriptionExpires.HasValue )
        {
            DateTime date = user.SubscriptionExpires.Value.LocalDateTime;
            if ( expires > date ) { expires = date; }
        }

        var descriptor = new SecurityTokenDescriptor
                         {
                             Subject            = new ClaimsIdentity( claims ),
                             Expires            = expires,
                             Issuer             = Options.TokenIssuer,
                             Audience           = Options.TokenAudience,
                             IssuedAt           = DateTime.UtcNow,
                             SigningCredentials = Configuration.GetSigningCredentials(),
                         };


        DateTime refreshExpires = DateTime.UtcNow + TimeSpan.FromDays( 90 );

        if ( user.SubscriptionExpires.HasValue )
        {
            DateTime date = user.SubscriptionExpires.Value.LocalDateTime;
            if ( refreshExpires > date ) { refreshExpires = date; }
        }

        var refreshDescriptor = new SecurityTokenDescriptor
                                {
                                    Subject            = new ClaimsIdentity( claims ),
                                    Expires            = refreshExpires,
                                    Issuer             = Options.TokenIssuer,
                                    Audience           = Options.TokenAudience,
                                    IssuedAt           = DateTime.UtcNow,
                                    SigningCredentials = Configuration.GetSigningCredentials(),
                                };

        return await GetJwtToken( connection, transaction, descriptor, refreshDescriptor, refreshExpires, user, token );
    }
    public async ValueTask<Tokens> GetJwtToken( DbConnection            connection,
                                                DbTransaction           transaction,
                                                SecurityTokenDescriptor descriptor,
                                                SecurityTokenDescriptor refreshDescriptor,
                                                DateTimeOffset          refreshExpires,
                                                UserRecord              user,
                                                CancellationToken       token = default
    )
    {
        var    handler     = new JwtSecurityTokenHandler();
        string accessToken = handler.WriteToken( handler.CreateToken( descriptor ) );
        string refresh     = handler.WriteToken( handler.CreateToken( refreshDescriptor ) );

        user.SetRefreshToken( refresh, refreshExpires );
        await Users.Update( connection, transaction, user, token );
        return new Tokens( accessToken, refresh, Version, user.UserID, user.FullName );
    }
    public async ValueTask<UserRecord?> ValidateJwtToken( DbConnection connection, DbTransaction transaction, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        var                       handler              = new JwtSecurityTokenHandler();
        TokenValidationParameters validationParameters = Configuration.GetTokenValidationParameters( Options );
        TokenValidationResult     validationResult     = await handler.ValidateTokenAsync( refreshToken, validationParameters );
        if ( validationResult.Exception is not null ) { throw validationResult.Exception; }

        Claim[] claims = validationResult.ClaimsIdentity.Claims.ToArray();
        return await UserRecord.TryFromClaims( connection, transaction, this, claims, types, token );
    }


    public ValueTask<Tokens> GetToken( UserRecord user, ClaimType types, CancellationToken token ) => this.TryCall( GetToken, user, types, token );
    public virtual ValueTask<Tokens> GetToken( DbConnection connection, DbTransaction transaction, UserRecord user, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) =>
        GetJwtToken( connection, transaction, user, types, token );


    public ValueTask<ActionResult<Tokens>> Refresh( string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Refresh, refreshToken, types, token );
    public async ValueTask<ActionResult<Tokens>> Refresh( DbConnection connection, DbTransaction transaction, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, refreshToken, types, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? user )
                   ? actionResult
                   : await GetToken( connection, transaction, user, types, token );
    }


    protected virtual async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction transaction, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        var user = await ValidateJwtToken( connection, transaction, refreshToken, types, token );
        if ( user is null ) { return LoginResult.State.BadCredentials; }


        user.LastLogin = DateTimeOffset.UtcNow;
        await Users.Update( connection, transaction, user, token );
        return user;
    }
    public ValueTask<ActionResult<Tokens>> Register( VerifyRequest<UserData> request, string rights, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) =>
        this.TryCall( Register, request, rights, types, token );
    public virtual async ValueTask<ActionResult<Tokens>> Register( DbConnection connection, DbTransaction transaction, VerifyRequest<UserData> request, string rights, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return new ConflictObjectResult( request ); }


        if ( !PasswordValidator.Validate( request.UserPassword, out bool lengthPassed, out bool specialPassed, out bool numericPassed, out bool lowerPassed, out bool upperPassed, out bool blockedPassed ) )
        {
            var state = new ModelStateDictionary();
            state.AddModelError( "Error", "Password Validation Failed" );

            if ( lengthPassed ) { state.AddModelError( "Details", "Password not long enough" ); }

            if ( specialPassed ) { state.AddModelError( "Details", "Password must contain a special character" ); }

            if ( numericPassed ) { state.AddModelError( "Details", "Password must contain a numeric character" ); }

            if ( lowerPassed ) { state.AddModelError( "Details", "Password must contain a lower case character" ); }

            if ( upperPassed ) { state.AddModelError( "Details", "Password must contain a upper case character" ); }

            if ( blockedPassed ) { state.AddModelError( "Details", "Password cannot be a blocked password" ); }

            return new BadRequestObjectResult( state );
        }


        record = UserRecord.Create( request, rights );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, types, token );
    }


    public ValueTask<ActionResult<T>> Verify<T>( VerifyRequest request, Func<UserRecord, ActionResult<T>> func, CancellationToken token = default ) => this.TryCall( Verify, request, func, token );
    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ActionResult<T>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : func( caller );
    }
    public ValueTask<ActionResult<T>> Verify<T>( VerifyRequest request, Func<UserRecord, ValueTask<ActionResult<T>>> func, CancellationToken token = default ) => this.TryCall( Verify, request, func, token );
    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ValueTask<ActionResult<T>>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }
    public ValueTask<ActionResult<T>> Verify<T>( VerifyRequest request, Func<UserRecord, Task<ActionResult<T>>> func, CancellationToken token = default ) => this.TryCall( Verify, request, func, token );
    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, Task<ActionResult<T>>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }


    public ValueTask<ActionResult<Tokens>> Verify( string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Verify, refreshToken, types, token );
    public async ValueTask<ActionResult<Tokens>> Verify( DbConnection connection, DbTransaction transaction, string refreshToken, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, refreshToken, types, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? user )
                   ? actionResult
                   : await GetToken( connection, transaction, user, types, token );
    }


    public ValueTask<ActionResult<Tokens>> Verify( VerifyRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default ) => this.TryCall( Verify, request, types, token );
    public virtual async ValueTask<ActionResult<Tokens>> Verify( DbConnection connection, DbTransaction transaction, VerifyRequest request, ClaimType types = DEFAULT_CLAIM_TYPES, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? user )
                   ? actionResult
                   : await GetToken( connection, transaction, user, types, token );
    }


    public ValueTask<ActionResult> Verify( VerifyRequest request, Func<UserRecord, ActionResult> func, CancellationToken token = default ) => this.TryCall( Verify, request, func, token );
    public virtual async ValueTask<ActionResult> Verify( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ActionResult> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : func( caller );
    }


    public ValueTask<ActionResult> Verify( VerifyRequest request, Func<UserRecord, ValueTask<ActionResult>> func, CancellationToken token = default ) => this.TryCall( Verify, request, func, token );
    public virtual async ValueTask<ActionResult> Verify( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ValueTask<ActionResult>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }


    public ValueTask<ActionResult> Verify( VerifyRequest request, Func<UserRecord, Task<ActionResult>> func, CancellationToken token = default ) => this.TryCall( Verify, request, func, token );
    public virtual async ValueTask<ActionResult> Verify( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, Task<ActionResult>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }


    protected virtual async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction? transaction, VerifyRequest request, CancellationToken token = default )
    {
        UserRecord? user = await Users.Get( connection, transaction, nameof(UserRecord.UserName), request.UserLogin, token );
        if ( user is null || !user.VerifyPassword( request.UserPassword ) ) { return LoginResult.State.BadCredentials; }

        try
        {
            user.SetActive();
            if ( !user.IsActive ) { return LoginResult.State.Inactive; }

            if ( !user.IsDisabled ) { return LoginResult.State.Disabled; }

            if ( !user.IsLocked ) { return LoginResult.State.Locked; }

            // if ( user.SubscriptionExpires > DateTimeOffset.UtcNow ) { return LoginResult.State.ExpiredSubscription; }

            // if ( user.SubscriptionID.IsValidID() ) { return LoginResult.State.NoSubscription; }

            return user;
        }
        finally { await Users.Update( connection, transaction, user, token ); }
    }
}
