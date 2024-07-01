// Jakar.Extensions :: Jakar.Extensions.Serilog
// 07/01/2024  15:07

using System.Text;
using Microsoft.Extensions.Options;
using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Sinks.Async;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;



namespace Jakar.Extensions.Serilog;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class TelemetryLogOptions : IOptions<TelemetryLogOptions>
{
    public const  string CONSOLE_DEFAULT_OUTPUT_TEMPLATE     = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    private const string DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const  long   DEFAULT_FILE_SIZE_LIMIT_BYTES       = 1L * 1024 * 1024 * 1024; // 1GB
    public const  int    DEFAULT_RETAINED_FILE_COUNT_LIMIT   = 31;                      // A long month of logs
    public const  string LOGS_DIRECTORY                      = "Logs";
    public const  string LOGS_FILE                           = "App.Logs";


    public bool                                       ApplyThemeToRedirectedOutput    { get; init; }
    public bool                                       BlockWhenFull                   { get; init; } = false;
    public bool                                       Buffered                        { get; init; }
    public int                                        BufferSize                      { get; init; } = 10000;
    public string                                     ConsoleOutputTemplate           { get; init; } = CONSOLE_DEFAULT_OUTPUT_TEMPLATE;
    public LogEventLevel                              ConsoleRestrictedToMinimumLevel { get; init; } = LevelAlias.Minimum;
    public CultureInfo                                CultureInfo                     { get; init; } = CultureInfo.InvariantCulture;
    public string                                     DebugOutputTemplate             { get; init; } = DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE;
    public LogEventLevel                              DebugRestrictedToMinimumLevel   { get; init; } = LevelAlias.Minimum;
    public Encoding?                                  Encoding                        { get; init; } = Encoding.Default;
    public long?                                      FileSizeLimitBytes              { get; init; } = DEFAULT_FILE_SIZE_LIMIT_BYTES;
    public TimeSpan?                                  FlushToDiskInterval             { get; init; }
    public ITextFormatter                             Formatter                       { get; init; } = new JsonFormatter();
    public FileLifecycleHooks?                        Hooks                           { get; init; }
    public LoggingLevelSwitch?                        LevelSwitch                     { get; init; }
    public LocalDirectory                             LogsDirectory                   { get; init; }
    public LocalFile                                  LogsFile                        { get; init; }
    public IAsyncLogEventSinkMonitor?                 Monitor                         { get; init; }
    public LogEventLevel                              RestrictedToMinimumLevel        { get; init; } = LevelAlias.Minimum;
    public int?                                       RetainedFileCountLimit          { get; init; } = DEFAULT_RETAINED_FILE_COUNT_LIMIT;
    public TimeSpan?                                  RetainedFileTimeLimit           { get; init; }
    public RollingInterval                            RollingInterval                 { get; init; } = RollingInterval.Day;
    public bool                                       RollOnFileSizeLimit             { get; init; } = true;
    public bool                                       Shared                          { get; init; }
    public LogEventLevel?                             StandardErrorFromLevel          { get; init; }
    public object?                                    SyncRoot                        { get; init; }
    public ConsoleTheme?                              Theme                           { get; init; }
    TelemetryLogOptions IOptions<TelemetryLogOptions>.Value                           => this;


    public TelemetryLogOptions() : this( LOGS_DIRECTORY ) { }
    public TelemetryLogOptions( LocalDirectory directory )
    {
        LogsDirectory = directory.Combine( LOGS_DIRECTORY );
        LogsFile      = LogsDirectory.Join( LOGS_FILE );
    }


    protected void Configure( LoggerSinkConfiguration sink ) => sink.File( Formatter,
                                                                           LogsFile.FullPath,
                                                                           RestrictedToMinimumLevel,
                                                                           FileSizeLimitBytes,
                                                                           LevelSwitch,
                                                                           Buffered,
                                                                           Shared,
                                                                           FlushToDiskInterval,
                                                                           RollingInterval,
                                                                           RollOnFileSizeLimit,
                                                                           RetainedFileCountLimit,
                                                                           Encoding,
                                                                           Hooks,
                                                                           RetainedFileTimeLimit );
    public virtual ILogger CreateSerilog<TApp>()
        where TApp : IAppID
    {
        LoggerConfiguration builder = new();
        builder.MinimumLevel.Verbose();
        builder.MinimumLevel.Override( "Microsoft", LogEventLevel.Warning );
        builder.MinimumLevel.Override( "System",    LogEventLevel.Warning );
        builder.Enrich.FromLogContext();
        builder.Enrich.With<AppContextEnricher<TApp>>();
        builder.WriteTo.Console( ConsoleRestrictedToMinimumLevel, ConsoleOutputTemplate, CultureInfo, LevelSwitch, StandardErrorFromLevel, Theme, ApplyThemeToRedirectedOutput, SyncRoot );
        builder.WriteTo.Debug( DebugRestrictedToMinimumLevel, DebugOutputTemplate, CultureInfo, LevelSwitch );
        builder.WriteTo.Async( Configure, BufferSize, BlockWhenFull, Monitor );

        return Log.Logger = builder.CreateLogger();
    }
}
