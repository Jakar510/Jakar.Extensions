// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public abstract partial class Database : Randoms, IConnectableDbRoot, IHealthCheck, IUserTwoFactorTokenProvider<UserRecord>
{
    public const       ClaimType               DEFAULT_CLAIM_TYPES = ClaimType.UserID | ClaimType.UserName | ClaimType.GroupSid | ClaimType.Role;
    protected readonly ConcurrentBag<IDbTable> _tables             = [];
    protected readonly IDistributedCache       _distributedCache;
    protected readonly ISqlCacheFactory        _sqlCacheFactory;
    protected readonly ITableCacheFactory      _tableCacheFactory;
    protected readonly string                  _className;


    public static      Database?                       Current           { get; set; }
    public static      DataProtector                   DataProtector     { get; set; } = new(RSAEncryptionPadding.OaepSHA1);
    public             DbTable<AddressRecord>          Addresses         { get; }
    public             int?                            CommandTimeout    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Options.CommandTimeout; }
    public             IConfiguration                  Configuration     { get; }
    protected internal SecuredString?                  ConnectionString  { get; set; }
    public             DbTable<GroupRecord>            Groups            { get; }
    public             DbInstance                      Instance          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Options.DbType; }
    public             DbOptions                       Options           { get; }
    protected internal PasswordValidator               PasswordValidator { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Options.PasswordRequirements.GetValidator(); }
    public             DbTable<RecoveryCodeRecord>     RecoveryCodes     { get; }
    public             DbTable<RoleRecord>             Roles             { get; }
    public             DbTable<UserGroupRecord>        UserGroups        { get; }
    public             DbTable<UserLoginInfoRecord>    UserLogins        { get; }
    public             DbTable<UserRecoveryCodeRecord> UserRecoveryCodes { get; }
    public             DbTable<UserRoleRecord>         UserRoles         { get; }
    public             DbTable<UserAddressRecord>      UserAddresses     { get; }
    public             DbTable<UserRecord>             Users             { get; }
    public             AppVersion                      Version           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Options.Version; }


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

    protected Database( IConfiguration configuration, ISqlCacheFactory sqlCacheFactory, IOptions<DbOptions> options, IDistributedCache distributedCache, ITableCacheFactory tableCacheFactory ) : base()
    {
        _className         = GetType().Name;
        _sqlCacheFactory   = sqlCacheFactory;
        _tableCacheFactory = tableCacheFactory;
        _distributedCache  = distributedCache;
        Configuration      = configuration;
        Options            = options.Value;
        Users              = Create<UserRecord>();
        Roles              = Create<RoleRecord>();
        UserRoles          = Create<UserRoleRecord>();
        UserGroups         = Create<UserGroupRecord>();
        Groups             = Create<GroupRecord>();
        RecoveryCodes      = Create<RecoveryCodeRecord>();
        UserLogins         = Create<UserLoginInfoRecord>();
        UserRecoveryCodes  = Create<UserRecoveryCodeRecord>();
        Addresses          = Create<AddressRecord>();
        UserAddresses      = Create<UserAddressRecord>();
        Current            = this;
    }
    public virtual async ValueTask DisposeAsync()
    {
        foreach ( IDbTable disposable in _tables ) { await disposable.DisposeAsync(); }

        _tables.Clear();
        ConnectionString?.Dispose();
        ConnectionString = null;
        GC.SuppressFinalize( this );
    }


    protected abstract DbConnection CreateConnection( in SecuredString secure );
    public async ValueTask<DbConnection> ConnectAsync( CancellationToken token )
    {
        ConnectionString ??= await Options.GetConnectionStringAsync( Configuration, token );
        DbConnection connection = CreateConnection( ConnectionString );
        await connection.OpenAsync( token );
        return connection;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected virtual DbTable<TRecord> Create<TRecord>()
        where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        DbTable<TRecord> table = new DbTable<TRecord>( this, _sqlCacheFactory );
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
        where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord> => _tableCacheFactory.GetCache( table );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public CommandDefinition GetCommandDefinition( DbTransaction? transaction, in SqlCommand sql, CancellationToken token ) =>
        sql.ToCommandDefinition( transaction, CommandTimeout, token );
    public async ValueTask<DbDataReader> ExecuteReaderAsync( DbConnection connection, DbTransaction? transaction, SqlCommand sql, CancellationToken token )
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

            CommandDefinition command = GetCommandDefinition( transaction, sql, token );
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


    public ValueTask<OneOf<Tokens, Error>> Register( VerifyRequest<UserModel<Guid>> request, string rights, ClaimType types = default, CancellationToken token = default ) =>
        this.TryCall( Register, request, rights, types, token );
    public virtual async ValueTask<OneOf<Tokens, Error>> Register( DbConnection connection, DbTransaction transaction, VerifyRequest<UserModel<Guid>> request, string rights, ClaimType types = default, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return new Error( Status.BadRequest, request ); }


        if ( PasswordValidator.Validate( request.Password, out PasswordValidator.Results results ) is false )
        {
            ModelStateDictionary state = new ModelStateDictionary();
            state.AddModelError( "Error", "Password Validation Failed" );

            if ( results.LengthPassed ) { state.AddModelError( "Details", "Password not long enough" ); }

            if ( results.SpecialPassed ) { state.AddModelError( "Details", "Password must contain a special character" ); }

            if ( results.NumericPassed ) { state.AddModelError( "Details", "Password must contain a numeric character" ); }

            if ( results.LowerPassed ) { state.AddModelError( "Details", "Password must contain a lower case character" ); }

            if ( results.UpperPassed ) { state.AddModelError( "Details", "Password must contain a upper case character" ); }

            if ( results.BlockedPassed ) { state.AddModelError( "Details", "Password cannot be a blocked password" ); }

            return new Error( Status.BadRequest, state );
        }


        record = UserRecord.Create( request, rights );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, types, token );
    }


    public static DynamicParameters GetParameters( object? value, object? template = default, [CallerArgumentExpression( nameof(value) )] string? variableName = default )
    {
        ArgumentNullException.ThrowIfNull( variableName );
        DynamicParameters parameters = new DynamicParameters( template );
        parameters.Add( variableName, value );
        return parameters;
    }


    public virtual async IAsyncEnumerable<T> Where<T>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : IDbReaderMapping<T>
    {
        DbDataReader reader;

        try
        {
            CommandDefinition command = GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            reader = await connection.ExecuteReaderAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }

        await foreach ( T record in T.CreateAsync( reader, token ) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<T> WhereValue<T>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : struct
    {
        DbDataReader reader;

        try
        {
            CommandDefinition command = GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            await connection.QueryAsync<T>( command );

            reader = await connection.ExecuteReaderAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }

        while ( await reader.ReadAsync( token ) ) { yield return reader.GetFieldValue<T>( 0 ); }
    }
}
