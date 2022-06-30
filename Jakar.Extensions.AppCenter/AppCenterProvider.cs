// using Jakar.Extensions.AppCenter.Api;

namespace Jakar.Extensions.AppCenter;


public class AppCenterProvider : IDisposable
{
    public const    string DEFAULT_OUTPUT_TEMPLATE       = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const    long   DEFAULT_FILE_SIZE_LIMIT_BYTES = 1L * 1024 * 1024 * 1024; // 1GB
    protected const string INSTALL_ID                    = nameof(INSTALL_ID);

    private readonly AppCenterApi _api;


    public AppCenterProvider( SecureString apiToken, LocalDirectory? directory )
    {
        directory ??= LocalDirectory.CurrentDirectory;
        _api      =   new AppCenterApi(apiToken, directory);
    }
    public void Dispose()
    {
        _api.Dispose();
        GC.SuppressFinalize(this);
    }


    public static AppCenterProvider Create( in string apiToken, LocalDirectory? directory = default ) => new(AppCenterApi.CreateSecureString(apiToken), directory);
    public static void Test()
    {
        var id = MsAppCenter.GetInstallIdAsync();
    }
}
