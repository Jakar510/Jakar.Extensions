// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using System.Transactions;
using Microsoft.Extensions.Caching.Hybrid;
using ZXing.Aztec.Internal;
using IsolationLevel = System.Data.IsolationLevel;
using Status = Jakar.Extensions.Status;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" ), SuppressMessage( "ReSharper", "InconsistentNaming" )]
public abstract partial class Database : Randoms, IConnectableDbRoot, IHealthCheck, IUserTwoFactorTokenProvider<UserRecord>
{
    public const       ClaimType                        DEFAULT_CLAIM_TYPES = ClaimType.UserID | ClaimType.UserName | ClaimType.Group | ClaimType.Role;
    protected readonly ConcurrentBag<IDbTable>          _tables             = [];
    public readonly    DbOptions                        Settings;
    public readonly    DbTable<AddressRecord>           Addresses;
    public readonly    DbTable<FileRecord>              Files;
    public readonly    DbTable<GroupRecord>             Groups;
    public readonly    DbTable<RecoveryCodeRecord>      RecoveryCodes;
    public readonly    DbTable<RoleRecord>              Roles;
    public readonly    DbTable<UserAddressRecord>       UserAddresses;
    public readonly    DbTable<UserGroupRecord>         UserGroups;
    public readonly    DbTable<UserLoginProviderRecord> UserLogins;
    public readonly    DbTable<UserRecord>              Users;
    public readonly    DbTable<UserRecoveryCodeRecord>  UserRecoveryCodes;
    public readonly    DbTable<UserRoleRecord>          UserRoles;
    public readonly    IConfiguration                   Configuration;
    protected readonly HybridCache                      _cache;
    protected          ActivitySource?                  _activitySource;
    protected          Meter?                           _meter;
    protected          string?                          _className;


    public static      Database?         Current                   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static      DataProtector     DataProtector             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = new(RSAEncryptionPadding.OaepSHA1);
    public             string            ClassName                 { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _className ??= GetType().GetFullName(); }
    public             int?              CommandTimeout            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.CommandTimeout; }
    protected internal SecuredString?    ConnectionString          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public             PasswordValidator PasswordValidator         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.PasswordRequirements.GetValidator(); }
    public             IsolationLevel    TransactionIsolationLevel { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = IsolationLevel.RepeatableRead;
    public             AppVersion        Version                   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.Version; }


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
        RecordID<UserLoginProviderRecord>.RegisterDapperTypeHandlers();
        RecordID<UserRecoveryCodeRecord>.RegisterDapperTypeHandlers();
        RecordID<UserGroupRecord>.RegisterDapperTypeHandlers();
        RecordID<UserRoleRecord>.RegisterDapperTypeHandlers();
        RecordID<UserAddressRecord>.RegisterDapperTypeHandlers();
    }
    protected Database( IConfiguration configuration, IOptions<DbOptions> options, HybridCache cache ) : base()
    {
        _cache            = cache;
        Configuration     = configuration;
        Settings          = options.Value;
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


    // Task IHostedService.StartAsync( CancellationToken cancellationToken ) => _tableCache.StartAsync( cancellationToken );
    // Task IHostedService.StopAsync( CancellationToken  cancellationToken ) => _tableCache.StopAsync( cancellationToken );


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
        DbTable<TRecord> table = new(this, _cache);
        return AddDisposable( table );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected TValue AddDisposable<TValue>( TValue value )
        where TValue : IDbTable
    {
        _tables.Add( value );
        return value;
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
    public CommandDefinition GetCommand<T>( T command, DbTransaction? transaction, CancellationToken token, CommandType? commandType = null )
        where T : IDapperSqlCommand
    {
        Activity.Current?.AddEvent( new ActivityEvent( nameof(GetCommand) ) );
        return new CommandDefinition( command.Sql, ParametersDictionary.LoadFrom( command ), transaction, CommandTimeout, commandType, CommandFlags.Buffered, token );
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
    public CommandDefinition GetCommand( SqlCommand sql, DbTransaction? transaction, CancellationToken token )
    {
        Activity.Current?.AddEvent( new ActivityEvent( nameof(GetCommand) ) );
        return sql.ToCommandDefinition( transaction, token, CommandTimeout );
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
    public SqlCommand.Definition GetCommand( SqlCommand sql, DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        Activity.Current?.AddEvent( new ActivityEvent( nameof(GetCommand) ) );
        return sql.ToCommandDefinition( connection, transaction, token, CommandTimeout );
    }


    public async ValueTask<DbDataReader> ExecuteReaderAsync<T>( DbConnection connection, DbTransaction? transaction, T command, CancellationToken token )
        where T : IDapperSqlCommand
    {
        try
        {
            CommandDefinition definition = GetCommand( command, transaction, token );
            return await connection.ExecuteReaderAsync( definition );
        }
        catch ( Exception e ) { throw new SqlException( command.Sql, e ); }
    }
    public async ValueTask<DbDataReader> ExecuteReaderAsync( DbConnection connection, DbTransaction? transaction, SqlCommand sql, CancellationToken token )
    {
        try
        {
            CommandDefinition command = GetCommand( sql, transaction, token );
            return await connection.ExecuteReaderAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql.sql, e ); }
    }
    public async ValueTask<DbDataReader> ExecuteReaderAsync( SqlCommand.Definition definition )
    {
        try { return await definition.connection.ExecuteReaderAsync( definition ); }
        catch ( Exception e ) { throw new SqlException( definition.command.CommandText, definition.command.Parameters as DynamicParameters, e ); }
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
                       _                          => throw new OutOfRangeException( connection.State )
                   };
        }
        catch ( Exception e ) { return HealthCheckResult.Unhealthy( e.Message, e ); }
    }


    public ValueTask<ErrorOrResult<Tokens>> Register( LoginRequest<UserModel> request, string rights, ClaimType types = default, CancellationToken token = default ) => this.TryCall( Register, request, rights, types, token );
    public virtual async ValueTask<ErrorOrResult<Tokens>> Register( DbConnection connection, DbTransaction transaction, LoginRequest<UserModel> request, string rights, ClaimType types = default, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return Error.NotFound( request.UserName ); }

        if ( PasswordValidator.Validate( request.Password, out PasswordValidator.Results results ) is false ) { return Error.Unauthorized( results ); }

        record = UserRecord.Create( request, rights );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, types, token );
    }


    public static DynamicParameters GetParameters( object? value, object? template = null, [CallerArgumentExpression( nameof(value) )] string? variableName = null )
    {
        ArgumentNullException.ThrowIfNull( variableName );
        var parameters = new DynamicParameters( template );
        parameters.Add( variableName, value );
        return parameters;
    }


    public virtual async IAsyncEnumerable<T> Where<T>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : IDbReaderMapping<T>, IRecordPair
    {
        DbDataReader reader;

        try
        {
            CommandDefinition command = GetCommand( new SqlCommand( sql, parameters ), transaction, token );
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
            CommandDefinition command = GetCommand( new SqlCommand( sql, parameters ), transaction, token );
            await connection.QueryAsync<T>( command );

            reader = await connection.ExecuteReaderAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }

        while ( await reader.ReadAsync( token ) ) { yield return reader.GetFieldValue<T>( 0 ); }
    }
}
