// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using System.Diagnostics.Metrics;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public abstract partial class Database : Randoms, IConnectableDbRoot, IHealthCheck, IUserTwoFactorTokenProvider<UserRecord>, IOpenTelemetry
{
    public const       ClaimType               DEFAULT_CLAIM_TYPES = ClaimType.UserID | ClaimType.UserName | ClaimType.Group | ClaimType.Role;
    protected readonly ConcurrentBag<IDbTable> _tables             = [];
    protected readonly ISqlCacheFactory        _sqlCacheFactory;
    protected readonly ITableCache             _tableCache;
    protected          ActivitySource?         _activitySource;
    protected          Meter?                  _meter;
    protected          string?                 _className;


    public static      Database?                       Current           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static      DataProtector                   DataProtector     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = new(RSAEncryptionPadding.OaepSHA1);
    public             ActivitySource                  ActivitySource    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _activitySource ??= new ActivitySource( ClassName, Version.ToString() ); }
    public             DbTable<AddressRecord>          Addresses         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    protected internal string                          ClassName         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _className ??= GetType().GetFullName(); }
    public             int?                            CommandTimeout    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.CommandTimeout; }
    public             IConfiguration                  Configuration     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    protected internal SecuredString?                  ConnectionString  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public             DbTypeInstance                  DbTypeInstance    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.DbTypeInstance; }
    public             DbTable<FileRecord>             Files             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             DbTable<GroupRecord>            Groups            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             Meter                           Meter             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _meter ??= new Meter( ClassName, Version.ToString() ); }
    protected internal PasswordValidator               PasswordValidator { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.PasswordRequirements.GetValidator(); }
    public             DbTable<RecoveryCodeRecord>     RecoveryCodes     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             DbTable<RoleRecord>             Roles             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             DbOptions                       Settings          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             DbTable<UserAddressRecord>      UserAddresses     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             DbTable<UserGroupRecord>        UserGroups        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             DbTable<UserLoginInfoRecord>    UserLogins        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             DbTable<UserRecoveryCodeRecord> UserRecoveryCodes { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             DbTable<UserRoleRecord>         UserRoles         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             DbTable<UserRecord>             Users             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public             AppVersion                      Version           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.Version; }


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
        RecordID<AddressRecord>.RegisterDapperTypeHandlers();
        RecordID<RecoveryCodeRecord>.RegisterDapperTypeHandlers();
        RecordID<GroupRecord>.RegisterDapperTypeHandlers();
        RecordID<RoleRecord>.RegisterDapperTypeHandlers();
        RecordID<UserRecord>.RegisterDapperTypeHandlers();
        RecordID<UserLoginInfoRecord>.RegisterDapperTypeHandlers();
        RecordID<UserRecoveryCodeRecord>.RegisterDapperTypeHandlers();
        RecordID<UserGroupRecord>.RegisterDapperTypeHandlers();
        RecordID<UserRoleRecord>.RegisterDapperTypeHandlers();
        RecordID<UserAddressRecord>.RegisterDapperTypeHandlers();
    }
    protected Database( IConfiguration configuration, ISqlCacheFactory sqlCacheFactory, ITableCache tableCache, IOptions<DbOptions> options ) : base()
    {
        _sqlCacheFactory  = sqlCacheFactory;
        _tableCache       = tableCache;
        Configuration     = configuration;
        Settings          = options.Value;
        Users             = Create<UserRecord>();
        Roles             = Create<RoleRecord>();
        UserRoles         = Create<UserRoleRecord>();
        UserGroups        = Create<UserGroupRecord>();
        Groups            = Create<GroupRecord>();
        RecoveryCodes     = Create<RecoveryCodeRecord>();
        UserLogins        = Create<UserLoginInfoRecord>();
        UserRecoveryCodes = Create<UserRecoveryCodeRecord>();
        Addresses         = Create<AddressRecord>();
        UserAddresses     = Create<UserAddressRecord>();
        Files             = Create<FileRecord>();
        Current           = this;
        Task.Run( InitDataProtector );
    }
    public virtual async ValueTask DisposeAsync()
    {
        foreach ( IDbTable disposable in _tables ) { await disposable.DisposeAsync(); }

        _tables.Clear();
        ConnectionString?.Dispose();
        ConnectionString = null;
        GC.SuppressFinalize( this );
    }


    public Activity? StartActivity( ActivityKind kind = ActivityKind.Internal, [CallerMemberName] string name = EMPTY ) => ActivitySource.StartActivity( name );
    public bool TryStartActivity( [NotNullWhen( true )] out Activity? activity, ActivityKind kind = ActivityKind.Internal, [CallerMemberName] string name = EMPTY )
    {
        activity = StartActivity( kind, name );
        return activity is not null;
    }


    Task IHostedService.StartAsync( CancellationToken cancellationToken ) => _tableCache.StartAsync( cancellationToken );
    Task IHostedService.StopAsync( CancellationToken  cancellationToken ) => _tableCache.StopAsync( cancellationToken );


    protected async Task InitDataProtector()
    {
        if ( Settings.DataProtectorKey.HasValue )
        {
            (LocalFile pem, SecuredStringResolverOptions password) = Settings.DataProtectorKey.Value;
            await InitDataProtector( pem, password );
        }
    }
    protected async ValueTask InitDataProtector( LocalFile pem, SecuredStringResolverOptions password, CancellationToken token = default ) => DataProtector = await DataProtector.WithKeyAsync( pem, await password.GetSecuredStringAsync( Configuration, token ), token );


    protected abstract DbConnection CreateConnection( in SecuredString secure );
    public async ValueTask<DbConnection> ConnectAsync( CancellationToken token )
    {
        ConnectionString ??= await Settings.GetConnectionStringAsync( Configuration, token );
        DbConnection connection = CreateConnection( ConnectionString );
        await connection.OpenAsync( token );
        return connection;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected virtual DbTable<TRecord> Create<TRecord>()
        where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        var table = new DbTable<TRecord>( this, _sqlCacheFactory );
        return AddDisposable( table );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected TValue AddDisposable<TValue>( TValue value )
        where TValue : IDbTable
    {
        _tables.Add( value );
        return value;
    }


    public void ResetCaches()
    {
        foreach ( IDbTable table in _tables ) { table.ResetCaches(); }
    }


    public ITableCache<TRecord> GetCache<TRecord>( DbTable<TRecord> table )
        where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord> => _tableCache.GetCache( table );


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
    public CommandDefinition GetCommand( Activity? activity, in SqlCommand sql, DbTransaction? transaction, CancellationToken token )
    {
        activity?.AddEvent( new ActivityEvent( nameof(GetCommand) ) );
        return sql.ToCommandDefinition( transaction, token, CommandTimeout );
    }
    public async ValueTask<DbDataReader> ExecuteReaderAsync( DbConnection connection, DbTransaction? transaction, Activity? activity, SqlCommand sql, CancellationToken token )
    {
        try
        {
            /*
            DbCommand dbCommand = connection.CreateCommand();
            dbCommand.CommandTimeout = CommandTimeout;
            if ( commandType.HasValue ) { dbCommand.CommandType = commandType.Value; }

            // BindByName - bool
            // InitialLONGFetchSize - int
            // FetchSize - long
            if ( dbCommand is SqlCommand msSql ) { }

            else if ( dbCommand is not NpgsqlCommand postgres ) { }

            DbDataReader temp = await dbCommand.ExecuteReaderAsync( CommandBehavior.SequentialAccess, token );
            */

            CommandDefinition command = GetCommand( activity, sql, transaction, token );
            DbDataReader      reader  = await connection.ExecuteReaderAsync( command );
            return reader;
        }
        catch ( Exception e ) { throw new SqlException( sql.SQL, e ); }
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
        catch ( Exception e ) { return HealthCheckResult.Unhealthy( e.Message, e ); }
    }


    public ValueTask<ErrorOrResult<Tokens>> Register( Activity? activity, LoginRequest<UserModel> request, string rights, ClaimType types = default, CancellationToken token = default ) => this.TryCall( Register, activity, request, rights, types, token );
    public virtual async ValueTask<ErrorOrResult<Tokens>> Register( DbConnection connection, DbTransaction transaction, Activity? activity, LoginRequest<UserModel> request, string rights, ClaimType types = default, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, activity, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return Error.NotFound( request.UserName ); }

        if ( PasswordValidator.Validate( request.Password, out PasswordValidator.Results results ) is false ) { return Error.Unauthorized( results ); }

        record = UserRecord.Create( request, rights );
        record = await Users.Insert( connection, transaction, activity, record, token );
        return await GetToken( connection, transaction, activity, record, types, token );
    }


    public static DynamicParameters GetParameters( object? value, object? template = default, [CallerArgumentExpression( nameof(value) )] string? variableName = default )
    {
        ArgumentNullException.ThrowIfNull( variableName );
        var parameters = new DynamicParameters( template );
        parameters.Add( variableName, value );
        return parameters;
    }


    public virtual async IAsyncEnumerable<T> Where<T>( DbConnection connection, DbTransaction? transaction, Activity? activity, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : IDbReaderMapping<T>
    {
        DbDataReader reader;

        try
        {
            CommandDefinition command = GetCommand( activity, new SqlCommand( sql, parameters ), transaction, token );
            reader = await connection.ExecuteReaderAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }

        await foreach ( T record in T.CreateAsync( reader, token ) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<T> WhereValue<T>( DbConnection connection, DbTransaction? transaction, Activity? activity, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : struct
    {
        DbDataReader reader;

        try
        {
            CommandDefinition command = GetCommand( activity, new SqlCommand( sql, parameters ), transaction, token );
            await connection.QueryAsync<T>( command );

            reader = await connection.ExecuteReaderAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }

        while ( await reader.ReadAsync( token ) ) { yield return reader.GetFieldValue<T>( 0 ); }
    }
}
