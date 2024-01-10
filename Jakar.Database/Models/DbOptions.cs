// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:46 PM

namespace Jakar.Database;


public sealed class DbOptions : IOptions<DbOptions>, IDbOptions
{
    public const           string                DEFAULT_SQL_CONNECTION_STRING_KEY = "DEFAULT";
    public const           string                JWT_KEY                           = "JWT";
    public const           string                USER_EXISTS                       = "User Exists";
    public const           string                JWT_ALGORITHM                     = SecurityAlgorithms.HmacSha512Signature;
    public const           string                AUTHENTICATION_TYPE               = JwtBearerDefaults.AuthenticationScheme;
    public const           int                   COMMAND_TIMEOUT                   = 300;
    public static readonly FrozenSet<DbInstance> Instances                         = Enum.GetValues<DbInstance>().ToFrozenSet();


    public static DbOptions               Default              => new();
    public        string                  AuthenticationType   { get; set; } = AUTHENTICATION_TYPE;
    public        TimeSpan                ClockSkew            { get; set; } = TimeSpan.FromSeconds( 60 );
    public        int?                    CommandTimeout       { get; set; } = COMMAND_TIMEOUT;
    public        ConnectionStringOptions ConnectionString     { get; set; }
    public        DbInstance              DbType               { get; set; } = DbInstance.Postgres;
    public        Uri                     Domain               { get; set; } = new("https://localhost:443");
    DbInstance IDbOptions.                Instance             => DbType;
    public string                         JWTAlgorithm         { get; set; } = JWT_ALGORITHM;
    public string                         JWTKey               { get; set; } = JWT_KEY;
    public string                         AppName              { get; set; } = string.Empty;
    public PasswordRequirements           PasswordRequirements { get; set; } = new();
    public string                         TokenAudience        { get; set; } = string.Empty;
    public string                         TokenIssuer          { get; set; } = string.Empty;
    public string                         UserExists           { get; set; } = USER_EXISTS;
    DbOptions IOptions<DbOptions>.        Value                => this;
    public AppVersion                     Version              { get; set; } = AppVersion.Default;


    public DbOptions()
    {
        Func<IConfiguration, SecuredString> func = GetConnectionString;
        ConnectionString = func;
    }


    public DbOptions WithAppName<T>()
        where T : IAppName
    {
        AppName = typeof(T).Name;
        return this;
    }

    public static void GetConnectionString( IMigrationRunnerBuilder provider ) => provider.WithGlobalConnectionString( GetConnectionString );
    public static string GetConnectionString( IServiceProvider provider )
    {
        ValueTask<SecuredString> task    = GetConnectionStringAsync( provider );
        SecuredString            secured = task.CallSynchronously();
        string                   value   = secured.ToString();
        Debug.WriteLine( value );
        return value;
    }
    public static async ValueTask<SecuredString> GetConnectionStringAsync( IServiceProvider provider )
    {
        using var source = new CancellationTokenSource( TimeSpan.FromMinutes( 5 ) );
        return await GetConnectionStringAsync( provider, source.Token );
    }
    public static async ValueTask<SecuredString> GetConnectionStringAsync( IServiceProvider provider, CancellationToken token )
    {
        IOptions<DbOptions> options       = provider.GetRequiredService<IOptions<DbOptions>>();
        IConfiguration      configuration = provider.GetRequiredService<IConfiguration>();
        SecuredString       secure        = await options.Value.GetConnectionStringAsync( configuration, token );
        return secure;
    }
    public async ValueTask<SecuredString> GetConnectionStringAsync( IConfiguration configuration, CancellationToken token )
    {
        ConnectionStringOptions result = ConnectionString;
        if ( result.IsT0 ) { return result.AsT0; }

        if ( result.IsT1 ) { return result.AsT1(); }

        if ( result.IsT2 ) { return await result.AsT2( token ); }

        if ( result.IsT3 ) { return await result.AsT3( token ); }

        if ( result.IsT4 ) { return result.AsT4( configuration ); }

        if ( result.IsT5 ) { return result.AsT5( configuration, token ); }

        if ( result.IsT6 ) { return await result.AsT6( configuration, token ); }

        if ( result.IsT7 ) { return await result.AsT7( configuration, token ); }

        return GetConnectionString( configuration );
    }
    internal static SecuredString GetConnectionString( IConfiguration configuration ) =>
        configuration.GetConnectionString( DEFAULT_SQL_CONNECTION_STRING_KEY ) ?? throw new KeyNotFoundException( DEFAULT_SQL_CONNECTION_STRING_KEY );
}



// public interface IConnectionStringProvider { }
