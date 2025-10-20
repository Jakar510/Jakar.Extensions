// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using Microsoft.Data.SqlClient;
using ZXing.Aztec.Internal;
using IsolationLevel = System.Data.IsolationLevel;



namespace Jakar.Database;


[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract partial class Database : Randoms, IConnectableDbRoot, IHealthCheck, IUserTwoFactorTokenProvider<UserRecord>
{
    public const                ClaimType                        DEFAULT_CLAIM_TYPES = ClaimType.UserID | ClaimType.UserName | ClaimType.Group | ClaimType.Role;
    protected readonly          ConcurrentBag<IDbTable>          _tables             = [];
    public readonly             DbOptions                        Options;
    public readonly             DbTable<AddressRecord>           Addresses;
    public readonly             DbTable<FileRecord>              Files;
    public readonly             DbTable<GroupRecord>             Groups;
    public readonly             DbTable<RecoveryCodeRecord>      RecoveryCodes;
    public readonly             DbTable<RoleRecord>              Roles;
    public readonly             DbTable<UserAddressRecord>       UserAddresses;
    public readonly             DbTable<UserGroupRecord>         UserGroups;
    public readonly             DbTable<UserLoginProviderRecord> UserLogins;
    public readonly             DbTable<UserRecord>              Users;
    public readonly             DbTable<UserRecoveryCodeRecord>  UserRecoveryCodes;
    public readonly             DbTable<UserRoleRecord>          UserRoles;
    protected internal readonly DbTable<MigrationRecord>         Migrations;
    protected readonly          FusionCache                      _cache;
    public readonly             IConfiguration                   Configuration;
    protected                   ActivitySource?                  _activitySource;
    protected                   Meter?                           _meter;
    protected                   string?                          _className;


    public static Database?     Current       { get; set; }
    public static DataProtector DataProtector { get; set; } = new(RSAEncryptionPadding.OaepSHA1);
    public string ClassName
    {
        get => _className ??= GetType()
                  .GetFullName();
    }
    protected internal SecuredString?               ConnectionString          { get; set; }
    ref readonly       DbOptions IConnectableDbRoot.Options                   => ref Options;
    public virtual     PasswordValidator            PasswordValidator         { get => DbOptions.PasswordRequirements.GetValidator(); }
    public             IsolationLevel               TransactionIsolationLevel { get; set; } = IsolationLevel.RepeatableRead;
    public             AppVersion                   Version                   { get => Options.AppInformation.Version; }
    public readonly    ThreadLocal<UserRecord?>     LoggedInUser = new();


    static Database()
    {
        EnumSqlHandler<SupportedLanguage>.Register();
        EnumSqlHandler<MimeType>.Register();
        EnumSqlHandler<Status>.Register();
        EnumSqlHandler<AppVersionFormat>.Register();
        DateTimeOffsetHandler.Register();
        DateTimeHandler.Register();
        DateOnlyHandler.Register();
        TimeOnlyHandler.Register();
        AppVersionHandler.Register();
        RecordID<AddressRecord>.RegisterDapperTypeHandlers();
        RecordID<RecoveryCodeRecord>.RegisterDapperTypeHandlers();
        RecordID<GroupRecord>.RegisterDapperTypeHandlers();
        RecordID<RoleRecord>.RegisterDapperTypeHandlers();
        RecordID<UserRecord>.RegisterDapperTypeHandlers();
        RecordID<UserLoginProviderRecord>.RegisterDapperTypeHandlers();
        RecordID<UserRecoveryCodeRecord>.RegisterDapperTypeHandlers();
        RecordID<UserGroupRecord>.RegisterDapperTypeHandlers();
        RecordID<UserRoleRecord>.RegisterDapperTypeHandlers();
        RecordID<UserAddressRecord>.RegisterDapperTypeHandlers();
    }
    protected Database( IConfiguration configuration, IOptions<DbOptions> options, FusionCache cache ) : base()
    {
        _cache            = cache;
        Configuration     = configuration;
        Options           = options.Value;
        Users             = Create<UserRecord>();
        Roles             = Create<RoleRecord>();
        UserRoles         = Create<UserRoleRecord>();
        UserGroups        = Create<UserGroupRecord>();
        Groups            = Create<GroupRecord>();
        RecoveryCodes     = Create<RecoveryCodeRecord>();
        UserLogins        = Create<UserLoginProviderRecord>();
        UserRecoveryCodes = Create<UserRecoveryCodeRecord>();
        Addresses         = Create<AddressRecord>();
        UserAddresses     = Create<UserAddressRecord>();
        Files             = Create<FileRecord>();
        Migrations        = Create<MigrationRecord>();
        Current           = this;
        Task.Run(InitDataProtector);
    }
    public virtual async ValueTask DisposeAsync()
    {
        foreach ( IDbTable disposable in _tables ) { await disposable.DisposeAsync(); }

        _tables.Clear();
        ConnectionString?.Dispose();
        ConnectionString = null;
        NpgsqlConnection.ClearAllPools();
        GC.SuppressFinalize(this);
    }


    // Task IHostedService.StartAsync( CancellationToken cancellationToken ) => _tableCache.StartAsync( cancellationToken );
    // Task IHostedService.StopAsync( CancellationToken  cancellationToken ) => _tableCache.StopAsync( cancellationToken );


    public async ValueTask<bool> HasAccess<TRight>( CancellationToken token, params TRight[] rights )
        where TRight : unmanaged, Enum
    {
        await using NpgsqlConnection connection   = await ConnectAsync(token);
        UserRecord?                  loggedInUser = LoggedInUser.Value;
        if ( loggedInUser is null ) { return false; }

        bool result = await HasPermission(connection, null, loggedInUser, token, rights);
        return result;
    }
    public async ValueTask<bool> HasPermission<TRight>( NpgsqlConnection connection, NpgsqlTransaction? transaction, UserRecord user, CancellationToken token, params TRight[] rights )
        where TRight : unmanaged, Enum
    {
        HashSet<TRight> permissions = await CurrentPermissions<TRight>(connection, transaction, user, token);

        foreach ( TRight right in rights.AsSpan() )
        {
            if ( permissions.Contains(right) ) { return false; }
        }

        return true;
    }
    public async ValueTask<HashSet<TRight>> CurrentPermissions<TRight>( NpgsqlConnection connection, NpgsqlTransaction? transaction, UserRecord user, CancellationToken token )
        where TRight : unmanaged, Enum
    {
        HashSet<IUserRights> models = new(DEFAULT_CAPACITY) { user };
        HashSet<TRight>      rights = new(Permissions<TRight>.EnumValues.Length);

        await foreach ( GroupRecord record in user.GetGroups(connection, transaction, this, token) ) { models.Add(record); }

        await foreach ( RoleRecord record in user.GetRoles(connection, transaction, this, token) ) { models.Add(record); }

        using Permissions<TRight> results = Permissions<TRight>.Create(models);

        foreach ( ( TRight permission, bool value ) in results.Rights )
        {
            if ( value ) { rights.Add(permission); }
        }

        return rights;
    }


    protected async Task InitDataProtector()
    {
        if ( Options.DataProtectorKey.HasValue )
        {
            ( LocalFile pem, SecuredStringResolverOptions password ) = Options.DataProtectorKey.Value;
            await InitDataProtector(pem, password);
        }
    }
    protected async ValueTask InitDataProtector( LocalFile pem, SecuredStringResolverOptions password, CancellationToken token = default ) => DataProtector = await DataProtector.WithKeyAsync(pem, await password.GetSecuredStringAsync(Configuration, token), token);


    protected virtual NpgsqlConnection CreateConnection( in SecuredString secure ) => new(secure.ToString());
    public async ValueTask<NpgsqlConnection> ConnectAsync( CancellationToken token )
    {
        ConnectionString ??= await Options.GetConnectionStringAsync(Configuration, token);
        NpgsqlConnection connection = CreateConnection(ConnectionString);
        await connection.OpenAsync(token);
        return connection;
    }


    protected virtual DbTable<TSelf> Create<TSelf>()
        where TSelf : class, ITableRecord<TSelf>
    {
        DbTable<TSelf> table = new(this, _cache);
        return AddDisposable(table);
    }


    protected TValue AddDisposable<TValue>( TValue value )
        where TValue : IDbTable
    {
        _tables.Add(value);
        return value;
    }


    public virtual async Task<HealthCheckResult> CheckHealthAsync( HealthCheckContext context, CancellationToken token = default )
    {
        try
        {
            await using NpgsqlConnection connection = await ConnectAsync(token);

            return connection.State switch
                   {
                       ConnectionState.Broken     => HealthCheckResult.Unhealthy(),
                       ConnectionState.Closed     => HealthCheckResult.Degraded(),
                       ConnectionState.Open       => HealthCheckResult.Healthy(),
                       ConnectionState.Connecting => HealthCheckResult.Healthy(),
                       ConnectionState.Executing  => HealthCheckResult.Healthy(),
                       ConnectionState.Fetching   => HealthCheckResult.Healthy(),
                       _                          => throw new OutOfRangeException(connection.State)
                   };
        }
        catch ( Exception e ) { return HealthCheckResult.Unhealthy(e.Message, e); }
    }


    public ValueTask<ErrorOrResult<SessionToken>> Register<TRequest>( TRequest request, string rights, ClaimType types = default, CancellationToken token = default )
        where TRequest : ILoginRequest<UserModel> => this.TryCall(Register, request, rights, types, token);
    public virtual async ValueTask<ErrorOrResult<SessionToken>> Register<TRequest>( NpgsqlConnection connection, NpgsqlTransaction transaction, TRequest request, string rights, ClaimType types = default, CancellationToken token = default )
        where TRequest : ILoginRequest<UserModel>
    {
        UserRecord? record = await Users.Get(connection, transaction, true, UserRecord.GetDynamicParameters(request), token);
        if ( record is not null ) { return Error.NotFound(request.UserName); }

        if ( !PasswordValidator.Validate(request.Password, out PasswordValidator.Results results) ) { return Error.Unauthorized(in results); }

        record = UserRecord.Create(request, rights);
        record = await Users.Insert(connection, transaction, record, token);
        return await GetToken(connection, transaction, record, types, token);
    }


    public virtual async IAsyncEnumerable<TSelf> Where<TSelf>( NpgsqlConnection connection, NpgsqlTransaction? transaction, string sql, PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>, IDateCreated
    {
        SqlCommand<TSelf>            command = new(sql, parameters);
        await using NpgsqlCommand    cmd     = command.ToCommand(connection, transaction);
        await using NpgsqlDataReader reader  = await cmd.ExecuteReaderAsync(token);
        await foreach ( TSelf record in reader.CreateAsync<TSelf>(token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TValue> Where<TSelf, TValue>( NpgsqlConnection connection, NpgsqlTransaction? transaction, string sql, PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
        where TValue : struct
        where TSelf : ITableRecord<TSelf>
    {
        SqlCommand<TSelf>            command = SqlCommand<TSelf>.Create(sql, parameters);
        await using NpgsqlCommand    cmd     = command.ToCommand(connection, transaction);
        await using NpgsqlDataReader reader  = await cmd.ExecuteReaderAsync(token);
        while ( await reader.ReadAsync(token) ) { yield return reader.GetFieldValue<TValue>(0); }
    }
}
