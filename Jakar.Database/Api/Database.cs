// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

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
        DbTable<TRecord> table = new DbTable<TRecord>( this );
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
                       _                          => throw new OutOfRangeException( nameof(connection.State), connection.State )
                   };
        }
        catch ( Exception e ) { return HealthCheckResult.Unhealthy( e.Message ); }
    }


    public virtual async ValueTask<JwtSecurityToken> GetEmailJwtSecurityToken( IEnumerable<Claim> claims, CancellationToken token )
    {
        var signinCredentials = await GetSigningCredentials( token );
        var security          = new JwtSecurityToken( Options.TokenIssuer, Options.TokenAudience, claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes( 15 ), signinCredentials );
        return security;
    }
    public virtual ValueTask<SigningCredentials> GetSigningCredentials( CancellationToken               token ) => new(Configuration.GetSigningCredentials( Options ));
    public virtual ValueTask<TokenValidationParameters> GetTokenValidationParameters( CancellationToken token ) => new(Configuration.GetTokenValidationParameters( Options ));


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
        DateTime expires = DateTime.UtcNow + TimeSpan.FromMinutes( 30 );

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


        JwtSecurityTokenHandler handler     = new JwtSecurityTokenHandler();
        string                  accessToken = handler.WriteToken( handler.CreateToken( descriptor ) );
        string                  refresh     = handler.WriteToken( handler.CreateToken( refreshDescriptor ) );


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

        JwtSecurityTokenHandler handler     = new JwtSecurityTokenHandler();
        string                  accessToken = handler.WriteToken( handler.CreateToken( descriptor ) );
        return new Tokens( accessToken, refreshToken, Version, record.UserID, record.FullName );
    }


    [SuppressMessage( "ReSharper", "UnusedParameter.Global" )]
    protected virtual ValueTask<bool> ValidateSubscription( DbConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default )
    {
        // if ( user.SubscriptionID is null || user.SubscriptionID.Value != Guid.Empty ) { return LoginResult.State.NoSubscription; }

        return new ValueTask<bool>( false );
    }
    protected async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction? transaction, VerifyRequest request, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, nameof(UserRecord.UserName), request.UserLogin, token );
        if ( record is null ) { return LoginResult.State.NotFound; }

        try
        {
            if ( !record.VerifyPassword( request.UserPassword ) ) { return LoginResult.State.BadCredentials; }
        }
        finally { await Users.Update( connection, transaction, record, token ); }


        if ( !record.IsActive ) { return LoginResult.State.Inactive; }

        if ( !record.IsDisabled ) { return LoginResult.State.Disabled; }

        if ( !record.IsLocked ) { return LoginResult.State.Locked; }

        if ( await ValidateSubscription( connection, transaction, record, token ) ) { return LoginResult.State.ExpiredSubscription; }


        return record;
    }
    protected async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction transaction, string jwt, ClaimType types = default, CancellationToken token = default )
    {
        JwtSecurityTokenHandler   handler              = new JwtSecurityTokenHandler();
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


    public ValueTask<OneOf<Tokens, Error>> Register( VerifyRequest<UserData> request, string rights, ClaimType types = default, CancellationToken token = default ) =>
        this.TryCall( Register, request, rights, types, token );
    public virtual async ValueTask<OneOf<Tokens, Error>> Register( DbConnection connection, DbTransaction transaction, VerifyRequest<UserData> request, string rights, ClaimType types = default, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return new Error( Status.BadRequest, request ); }


        if ( !PasswordValidator.Validate( request.UserPassword, out bool lengthPassed, out bool specialPassed, out bool numericPassed, out bool lowerPassed, out bool upperPassed, out bool blockedPassed ) )
        {
            ModelStateDictionary state = new ModelStateDictionary();
            state.AddModelError( "Error", "Password Validation Failed" );

            if ( lengthPassed ) { state.AddModelError( "Details", "Password not long enough" ); }

            if ( specialPassed ) { state.AddModelError( "Details", "Password must contain a special character" ); }

            if ( numericPassed ) { state.AddModelError( "Details", "Password must contain a numeric character" ); }

            if ( lowerPassed ) { state.AddModelError( "Details", "Password must contain a lower case character" ); }

            if ( upperPassed ) { state.AddModelError( "Details", "Password must contain a upper case character" ); }

            if ( blockedPassed ) { state.AddModelError( "Details", "Password cannot be a blocked password" ); }

            return new Error( Status.BadRequest, state );
        }


        record = UserRecord.Create( request, rights );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, types, token );
    }


    public virtual async ValueTask<OneOf<T, Error>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, T> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : func( record );
    }
    public virtual async ValueTask<OneOf<T, Error>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ValueTask<T>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await func( record );
    }
    public virtual async ValueTask<OneOf<T, Error>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, Task<T>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await func( record );
    }
    public async ValueTask<OneOf<Tokens, Error>> Verify( DbConnection connection, DbTransaction transaction, string jwt, ClaimType types = default, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, jwt, types, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await GetToken( connection, transaction, record, types, token );
    }
    public virtual async ValueTask<OneOf<Tokens, Error>> Verify( DbConnection connection, DbTransaction transaction, VerifyRequest request, ClaimType types = default, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await GetToken( connection, transaction, record, types, token );
    }
    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ActionResult<T>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : func( caller );
    }
    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ValueTask<ActionResult<T>>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }
    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, Task<ActionResult<T>>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }


    public ValueTask<OneOf<T, Error>> Verify<T>( VerifyRequest   request,      Func<UserRecord, T>                          func,            CancellationToken token = default ) => this.TryCall( Verify, request,      func,  token );
    public ValueTask<OneOf<T, Error>> Verify<T>( VerifyRequest   request,      Func<UserRecord, ValueTask<T>>               func,            CancellationToken token = default ) => this.TryCall( Verify, request,      func,  token );
    public ValueTask<OneOf<T, Error>> Verify<T>( VerifyRequest   request,      Func<UserRecord, Task<T>>                    func,            CancellationToken token = default ) => this.TryCall( Verify, request,      func,  token );
    public ValueTask<OneOf<Tokens, Error>> Verify( string        refreshToken, ClaimType                                    types = default, CancellationToken token = default ) => this.TryCall( Verify, refreshToken, types, token );
    public ValueTask<OneOf<Tokens, Error>> Verify( VerifyRequest request,      ClaimType                                    types = default, CancellationToken token = default ) => this.TryCall( Verify, request,      types, token );
    public ValueTask<ActionResult<T>> Verify<T>( VerifyRequest   request,      Func<UserRecord, ActionResult<T>>            func,            CancellationToken token = default ) => this.TryCall( Verify, request,      func,  token );
    public ValueTask<ActionResult<T>> Verify<T>( VerifyRequest   request,      Func<UserRecord, ValueTask<ActionResult<T>>> func,            CancellationToken token = default ) => this.TryCall( Verify, request,      func,  token );
    public ValueTask<ActionResult<T>> Verify<T>( VerifyRequest   request,      Func<UserRecord, Task<ActionResult<T>>>      func,            CancellationToken token = default ) => this.TryCall( Verify, request,      func,  token );
}
