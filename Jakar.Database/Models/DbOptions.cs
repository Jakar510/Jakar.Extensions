// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:46 PM

using FluentMigrator.Runner.Initialization;



namespace Jakar.Database;


public sealed class DbOptions : IOptions<DbOptions>, IDbOptions
{
    public const           string                AUTHENTICATION_TYPE                       = JwtBearerDefaults.AuthenticationScheme;
    public const           int                   COMMAND_TIMEOUT                           = 300;
    public const           string                DEFAULT_SQL_CONNECTION_STRING_KEY         = "DEFAULT";
    public const           string                DEFAULT_SQL_CONNECTION_STRING_SECTION_KEY = "ConnectionStrings";
    public const           string                JWT_ALGORITHM                             = SecurityAlgorithms.HmacSha512Signature;
    public const           string                JWT_KEY                                   = "JWT";
    public const           string                USER_EXISTS                               = "User Exists";
    public static readonly FrozenSet<DbInstance> Instances                                 = Enum.GetValues<DbInstance>().ToFrozenSet();


    public static DbOptions                                        Default                  => new();
    public        string                                           AppName                  { get; set; } = string.Empty;
    public        string                                           AuthenticationType       { get; set; } = AUTHENTICATION_TYPE;
    public        TimeSpan                                         ClockSkew                { get; set; } = TimeSpan.FromMinutes( 1 );
    public        int?                                             CommandTimeout           { get; set; } = COMMAND_TIMEOUT;
    public        SecuredStringResolverOptions                     ConnectionStringResolver { get; set; } = (Func<IConfiguration, SecuredString>)GetConnectionString;
    public        DbInstance                                       DbType                   { get; set; } = DbInstance.Postgres;
    public        Uri                                              Domain                   { get; set; } = new("https://localhost:443");
    DbInstance IDbOptions.                                         Instance                 => DbType;
    public string                                                  JWTAlgorithm             { get; set; } = JWT_ALGORITHM;
    public string                                                  JWTKey                   { get; set; } = JWT_KEY;
    public PasswordRequirements                                    PasswordRequirements     { get; set; } = new();
    public (LocalFile Pem, SecuredStringResolverOptions Password)? DataProtectorKey         { get; set; }
    public string                                                  TokenAudience            { get; set; } = string.Empty;
    public string                                                  TokenIssuer              { get; set; } = string.Empty;
    public string                                                  UserExists               { get; set; } = USER_EXISTS;
    DbOptions IOptions<DbOptions>.                                 Value                    => this;
    public AppVersion                                              Version                  { get; set; } = AppVersion.Default;


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
        using CancellationTokenSource source = new CancellationTokenSource( TimeSpan.FromMinutes( 5 ) );
        return await GetConnectionStringAsync( provider, source.Token );
    }
    public static async ValueTask<SecuredString> GetConnectionStringAsync( IServiceProvider provider, CancellationToken token )
    {
        IOptions<DbOptions> options       = provider.GetRequiredService<IOptions<DbOptions>>();
        IConfiguration      configuration = provider.GetRequiredService<IConfiguration>();
        SecuredString       secure        = await options.Value.GetConnectionStringAsync( configuration, token );
        return secure;
    }
    public async  ValueTask<SecuredString> GetConnectionStringAsync( IConfiguration configuration, CancellationToken token, string key = "Default", string section = "ConnectionStrings" ) => await ConnectionStringResolver.GetSecuredStringAsync( configuration, token, key, section );
    public static SecuredString            GetConnectionString( IConfiguration      configuration ) => configuration.GetConnectionString( "Default" ) ?? throw new KeyNotFoundException( "Default" );
}



// public interface IConnectionStringProvider { }
