// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" ) ]
public abstract partial class Database : Randoms, IConnectableDbRoot, IHealthCheck, IUserTwoFactorTokenProvider<UserRecord>
{
    public const ClaimType DEFAULT_CLAIM_TYPES = ClaimType.UserID | ClaimType.UserName | ClaimType.GroupSid | ClaimType.Role;


    protected readonly ConcurrentBag<IDbTable> _tables = new();
    public             DbTable<AddressRecord>  Addresses { get; }
    public int? CommandTimeout
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => Options.CommandTimeout;
    }
    public             IConfiguration Configuration    { get; }
    protected internal SecuredString? ConnectionString { get; set; }
    public string CurrentSchema
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => Options.CurrentSchema;
    }
    public DbTable<GroupRecord> Groups { get; }
    public DbInstance Instance
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => Options.DbType;
    }
    public DbOptions Options { get; }
    protected internal PasswordValidator PasswordValidator
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => new(Options.PasswordRequirements);
    }
    public DbTable<RecoveryCodeRecord>     RecoveryCodes     { get; }
    public DbTable<RoleRecord>             Roles             { get; }
    public DbTable<UserGroupRecord>        UserGroups        { get; }
    public DbTable<UserLoginInfoRecord>    UserLogins        { get; }
    public DbTable<UserRecoveryCodeRecord> UserRecoveryCodes { get; }
    public DbTable<UserRoleRecord>         UserRoles         { get; }
    public DbTable<UserRecord>             Users             { get; }
    public AppVersion Version
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => Options.Version;
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


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected DbTable<TRecord> Create<TRecord>() where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        var table = new DbTable<TRecord>( this );
        return AddDisposable( table );
    }
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected TValue AddDisposable<TValue>( TValue value ) where TValue : IDbTable
    {
        _tables.Add( value );
        return value;
    }
    public void ResetSqlCaches()
    {
        foreach ( IDbTable table in _tables ) { table.ResetSqlCaches(); }
    }


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public CommandDefinition GetCommandDefinition( DbTransaction? transaction, in SqlCommand sql, CancellationToken token ) => sql.ToCommandDefinition( transaction, CommandTimeout, token );
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


    public virtual async IAsyncEnumerable<T> Where<T>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [ EnumeratorCancellation ] CancellationToken token = default ) where T : IDbReaderMapping<T>
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
    public virtual async IAsyncEnumerable<T> WhereValue<T>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [ EnumeratorCancellation ] CancellationToken token = default ) where T : struct
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
