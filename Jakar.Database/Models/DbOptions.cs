// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:46 PM

using System.Configuration;
using System.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;



namespace Jakar.Database;


public sealed class DbOptions : IOptions<DbOptions>, IDbOptions
{
    public const string  DEFAULT_SQL_CONNECTION_STRING_KEY = "DEFAULT";
    private      string? _currentSchema;


    public string   AuthenticationType { get; set; } = JwtBearerDefaults.AuthenticationScheme;
    public TimeSpan ClockSkew          { get; set; } = TimeSpan.FromSeconds( 60 );
    public int?     CommandTimeout     { get; set; } = 300;
    public string CurrentSchema
    {
        get => _currentSchema ??= DbType switch
                                  {
                                      DbInstance.MsSql    => "dbo",
                                      DbInstance.Postgres => "public",
                                      _                   => throw new OutOfRangeException( nameof(DbType), DbType )
                                  };
        set => _currentSchema = value;
    }
    public DbInstance              DbType               { get; set; } = DbInstance.Postgres;
    public Uri                     Domain               { get; set; } = new("https://localhost");
    DbInstance IDbOptions.         Instance             => DbType;
    public string                  JWTAlgorithm         { get; set; } = SecurityAlgorithms.HmacSha512Signature;
    public string                  JWTKey               { get; set; } = "JWT";
    public PasswordRequirements    PasswordRequirements { get; set; } = new();
    public string                  TokenAudience        { get; set; } = string.Empty;
    public string                  TokenIssuer          { get; set; } = string.Empty;
    public string                  UserExists           { get; set; } = "User Exists";
    DbOptions IOptions<DbOptions>. Value                => this;
    public AppVersion              Version              { get; set; } = AppVersion.Default;
    public ConnectionStringOptions ConnectionString     { get; set; }


    public DbOptions()
    {
        Func<IConfiguration, SecuredString> func = GetConnectionString;
        ConnectionString = func;
    }


    public static void GetConnectionString( IMigrationRunnerBuilder provider ) => provider.WithGlobalConnectionString( GetConnectionString );
    public static string GetConnectionString( IServiceProvider provider )
    {
        Task<SecuredString> task    = GetConnectionStringAsync( provider );
        SecuredString       secured = task.CallSynchronously();
        string              value   = secured.ToString();
        Debug.WriteLine( value );
        return value;
    }
    public static async Task<SecuredString> GetConnectionStringAsync( IServiceProvider provider )
    {
        using var source = new CancellationTokenSource( TimeSpan.FromMinutes( 2 ) );
        return await GetConnectionStringAsync( provider, source.Token );
    }
    public static async Task<SecuredString> GetConnectionStringAsync( IServiceProvider provider, CancellationToken token )
    {
        IOptions<DbOptions> options       = provider.GetRequiredService<IOptions<DbOptions>>();
        IConfiguration      configuration = provider.GetRequiredService<IConfiguration>();
        SecuredString       secure        = await options.Value.GetConnectionStringAsync( configuration, token );
        return secure;
    }
    public async Task<SecuredString> GetConnectionStringAsync( IConfiguration configuration, CancellationToken token )
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
