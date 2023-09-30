// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" ) ]
public abstract partial class Database : Randoms, IConnectableDb, IAsyncDisposable, IHealthCheck, IUserTwoFactorTokenProvider<UserRecord>
{
    public const       ClaimType                       DEFAULT_CLAIM_TYPES = ClaimType.UserID | ClaimType.UserName | ClaimType.GroupSid | ClaimType.Role;
    protected readonly ConcurrentBag<IAsyncDisposable> _disposables        = new();


    public             int                             CommandTimeout    => Options.CommandTimeout;
    public             IConfiguration                  Configuration     { get; }
    public virtual     string                          ConnectionString  => Configuration.GetConnectionString( "DEFAULT" ) ?? throw new KeyNotFoundException( "DEFAULT" );
    public             string                          CurrentSchema     => Options.CurrentSchema;
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
    public             DbTable<AddressRecord>          Addresses         { get; }
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
        RecordID<GroupRecord>.DapperTypeHandler.Register();
        RecordID<RecoveryCodeRecord>.DapperTypeHandler.Register();
        RecordID<RoleRecord>.DapperTypeHandler.Register();
        RecordID<UserGroupRecord>.DapperTypeHandler.Register();
        RecordID<UserLoginInfoRecord>.DapperTypeHandler.Register();
        RecordID<UserRecoveryCodeRecord>.DapperTypeHandler.Register();
        RecordID<UserRoleRecord>.DapperTypeHandler.Register();
        RecordID<UserRecord>.DapperTypeHandler.Register();
        UserRights.DapperTypeHandler.Register();
        UserRights.DapperTypeHandlerNullable.Register();
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
        Addresses         = Create<AddressRecord>();
    }


    protected TValue AddDisposable<TValue>( TValue value ) where TValue : IAsyncDisposable
    {
        _disposables.Add( value );
        return value;
    }


    protected virtual DbTable<TRecord> Create<TRecord>() where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        var table = new DbTable<TRecord>( this );
        return AddDisposable( table );
    }


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


    public ValueTask<OneOf<Tokens, Error>> Register( VerifyRequest<UserData> request, string rights, ClaimType types = default, CancellationToken token = default ) =>
        this.TryCall( Register, request, rights, types, token );
    public virtual async ValueTask<OneOf<Tokens, Error>> Register( DbConnection connection, DbTransaction transaction, VerifyRequest<UserData> request, string rights, ClaimType types = default, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return new Error( Status.BadRequest, request ); }


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

            return new Error( Status.BadRequest, state );
        }


        record = UserRecord.Create( request, rights );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, types, token );
    }


    public static DynamicParameters GetParameters( object? value, object? template = default, [ CallerArgumentExpression( nameof(value) ) ] string? variableName = default )
    {
        ArgumentNullException.ThrowIfNull( variableName );
        var parameters = new DynamicParameters( template );
        parameters.Add( variableName, value );
        return parameters;
    }
}
