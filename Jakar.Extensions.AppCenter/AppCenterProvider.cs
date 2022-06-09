namespace Jakar.Extensions.AppCenter;


public class AppCenterProvider
{
    public const string DEFAULT_OUTPUT_TEMPLATE       = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const long   DEFAULT_FILE_SIZE_LIMIT_BYTES = 1L * 1024 * 1024 * 1024; // 1GB
}
