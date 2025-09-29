// Jakar.Extensions :: Jakar.Extensions.Serilog
// 03/10/2025  17:03

namespace Jakar.Extensions.Serilog;


public class SeriloggerConstants : ObservableClass
{
    public const           string     DEFAULT_CONSOLE_OUTPUT_TEMPLATE   = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const           string     DEFAULT_DEBUG_OUTPUT_TEMPLATE     = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const           long       DEFAULT_FILE_SIZE_LIMIT_BYTES     = 1L * 1024 * 1024 * 1024; // 1GB
    public const           int        DEFAULT_RETAINED_FILE_COUNT_LIMIT = 31;                      // A long month of logs
    public const           string     SEQ_API_KEY_NAME                  = "X-Seq-ApiKey";
    public const           string     SEQ_BUFFER_DIRECTORY              = "SeqBuffer";
    public const           string     SHARED_NAME                       = "Serilogger";
    public static readonly FileData[] Empty                             = [];
    public static readonly object[]   NoPropertyValues                  = [];
}
