// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:46 PM

namespace Jakar.Database;


public sealed class DbOptions : IOptions<DbOptions>, IDbOptions
{
    public static readonly Uri                       Local_433           = new("https://localhost:443");
    public static readonly Uri                       Local_80            = new("http://localhost:80");
    public const           string                    AUTHENTICATION_TYPE = JwtBearerDefaults.AuthenticationScheme;
    public const           int                       COMMAND_TIMEOUT     = 300;
    public const           string                    JWT_ALGORITHM       = SecurityAlgorithms.HmacSha512Signature;
    public const           string                    JWT_KEY             = "JWT";
    public const           string                    USER_EXISTS         = "User Exists";
    public static readonly FrozenSet<DbTypeInstance> Instances           = Enum.GetValues<DbTypeInstance>().ToFrozenSet();


    public static DbOptions                                               Default                  => new();
    public        string                                                  AppName                  { get;                                 set; } = string.Empty;
    public        string                                                  AuthenticationType       { get;                                 set; } = AUTHENTICATION_TYPE;
    public        TimeSpan                                                ClockSkew                { get;                                 set; } = TimeSpan.FromMinutes( 1 );
    public        int?                                                    CommandTimeout           { get;                                 set; } = COMMAND_TIMEOUT;
    public        SecuredStringResolverOptions                            ConnectionStringResolver { get;                                 set; } = (Func<IConfiguration, SecuredString>)GetConnectionString;
    public        DbTypeInstance                                          DbTypeInstance           { get;                                 set; } = DbTypeInstance.Postgres;
    public        Uri                                                     Domain                   { get;                                 set; } = Local_433;
    public        string                                                  JWTAlgorithm             { get;                                 set; } = JWT_ALGORITHM;
    public        string                                                  JWTKey                   { get;                                 set; } = JWT_KEY;
    public        PasswordRequirements                                    PasswordRequirements     { get => PasswordRequirements.Current; set => PasswordRequirements.Current = value; }
    public        (LocalFile Pem, SecuredStringResolverOptions Password)? DataProtectorKey         { get;                                 set; }
    public        string                                                  TokenAudience            { get;                                 set; } = string.Empty;
    public        string                                                  TokenIssuer              { get;                                 set; } = string.Empty;
    public        string                                                  UserExists               { get;                                 set; } = USER_EXISTS;
    DbOptions IOptions<DbOptions>.                                        Value                    => this;
    public AppVersion                                                     Version                  { get; set; } = AppVersion.Default;


    public static IOptions<DbOptions> Get( IServiceProvider provider ) => provider.GetRequiredService<DbOptions>();


    public DbOptions WithAppName<TApp>()
        where TApp : IAppName
    {
        AppName = TApp.AppName;
        Version = TApp.AppVersion;
        return this;
    }
    public static SecuredString GetConnectionString( IConfiguration          configuration ) => SecuredStringResolverOptions.GetSecuredString( configuration );
    public static void          GetConnectionString( IMigrationRunnerBuilder provider )      => provider.WithGlobalConnectionString( GetConnectionString );
    public static string GetConnectionString( IServiceProvider provider )
    {
        ValueTask<SecuredString> task    = GetConnectionStringAsync( provider );
        SecuredString            secured = task.CallSynchronously();
        string                   value   = secured.ToString();
        return value;
    }
    public static async ValueTask<SecuredString> GetConnectionStringAsync( IServiceProvider provider )
    {
        using CancellationTokenSource source = new( TimeSpan.FromMinutes( 5 ) );
        return await GetConnectionStringAsync( provider, source.Token );
    }
    public static async ValueTask<SecuredString> GetConnectionStringAsync( IServiceProvider provider, CancellationToken token )
    {
        DbOptions      options       = provider.GetRequiredService<IOptions<DbOptions>>().Value;
        IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
        SecuredString  secure        = await options.GetConnectionStringAsync( configuration, token );
        return secure;
    }
    public async ValueTask<SecuredString> GetConnectionStringAsync( IConfiguration configuration, CancellationToken token ) => await ConnectionStringResolver.GetSecuredStringAsync( configuration, token );
}



// public interface IConnectionStringProvider { }
