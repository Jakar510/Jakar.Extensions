// Jakar.Extensions :: Jakar.Extensions.Serilog
// 07/01/2024  15:07

using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Configuration;
using Serilog.Extensions.Logging;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Sinks.Async;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Serilog.ILogger;



namespace Jakar.Extensions.Serilog;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class Serilogger<TApp> : IOptions<Serilogger<TApp>>, ILoggerProvider, ILogger, ILogEventSink
    where TApp : IAppID
{
    public const     int                   BUFFER_SIZE                         = 10000;
    public const     string                CONSOLE_DEFAULT_OUTPUT_TEMPLATE     = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const     string                DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const     long                  DEFAULT_FILE_SIZE_LIMIT_BYTES       = 1L * 1024 * 1024 * 1024; // 1GB
    public const     int                   DEFAULT_RETAINED_FILE_COUNT_LIMIT   = 31;                      // A long month of logs
    public const     string                LOGS_DIRECTORY                      = "Logs";
    public const     string                LOGS_FILE                           = "App.Logs";
    private readonly Logger                _logger;
    private readonly SerilogLoggerProvider _provider;


    public bool                                 ApplyThemeToRedirectedOutput    { get; init; }
    public bool                                 BlockWhenFull                   { get; init; } = false;
    public bool                                 Buffered                        { get; init; }
    public int                                  BufferSize                      { get; init; } = BUFFER_SIZE;
    public string                               ConsoleOutputTemplate           { get; init; } = CONSOLE_DEFAULT_OUTPUT_TEMPLATE;
    public LogEventLevel                        ConsoleRestrictedToMinimumLevel { get; init; } = LevelAlias.Minimum;
    public CultureInfo                          CultureInfo                     { get; init; } = CultureInfo.InvariantCulture;
    public string                               DebugOutputTemplate             { get; init; } = DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE;
    public LogEventLevel                        DebugRestrictedToMinimumLevel   { get; init; } = LevelAlias.Minimum;
    public Encoding?                            Encoding                        { get; init; } = Encoding.Default;
    public long?                                FileSizeLimitBytes              { get; init; } = DEFAULT_FILE_SIZE_LIMIT_BYTES;
    public TimeSpan?                            FlushToDiskInterval             { get; init; }
    public ITextFormatter                       Formatter                       { get; init; } = new JsonFormatter();
    public FileLifecycleHooks?                  Hooks                           { get; init; }
    public LoggingLevelSwitch?                  LevelSwitch                     { get; init; }
    public LocalDirectory                       LogsDirectory                   { get; init; }
    public LocalFile                            LogsFile                        { get; init; }
    public IAsyncLogEventSinkMonitor?           Monitor                         { get; init; }
    public LogEventLevel                        RestrictedToMinimumLevel        { get; init; } = LevelAlias.Minimum;
    public int?                                 RetainedFileCountLimit          { get; init; } = DEFAULT_RETAINED_FILE_COUNT_LIMIT;
    public TimeSpan?                            RetainedFileTimeLimit           { get; init; }
    public RollingInterval                      RollingInterval                 { get; init; } = RollingInterval.Day;
    public bool                                 RollOnFileSizeLimit             { get; init; } = true;
    public bool                                 Shared                          { get; init; }
    public LogEventLevel?                       StandardErrorFromLevel          { get; init; }
    public object?                              SyncRoot                        { get; init; }
    public ConsoleTheme?                        Theme                           { get; init; }
    Serilogger<TApp> IOptions<Serilogger<TApp>>.Value                           => this;


    public Serilogger() : this( LocalDirectory.CurrentDirectory ) { }
    public Serilogger( LocalDirectory directory )
    {
        LogsDirectory = directory.Combine( LOGS_DIRECTORY );
        LogsFile      = LogsDirectory.Join( LOGS_FILE );
        Log.Logger    = _logger = GetLogger();
        _provider     = new SerilogLoggerProvider( this, true );
    }
    public void Dispose()
    {
        _provider.Dispose();
        _logger.Dispose();
        GC.SuppressFinalize( this );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Microsoft.Extensions.Logging.ILogger CreateLogger( string categoryName ) => _provider.CreateLogger( categoryName );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Microsoft.Extensions.Logging.ILogger CreateLogger<T>()                   => CreateLogger( typeof(T).Name );
    [MethodImpl( MethodImplOptions.AggressiveInlining )]        void ILogEventSink.                  Emit( LogEvent  logEvent )          => ((ILogEventSink)_logger).Emit( logEvent );
    [MethodImpl( MethodImplOptions.AggressiveInlining )]        void ILogger.                        Write( LogEvent logEvent )          => _logger.Write( logEvent );


    public ILoggingBuilder Configure( ILoggingBuilder builder, string? category = null, LogLevel level = LogLevel.Trace )
    {
        builder.ClearProviders();
        builder.AddProvider( _provider );
        builder.AddFilter<SerilogLoggerProvider>( category, level );
        return builder;
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
    protected virtual Logger GetLogger()
    {
        LoggerConfiguration builder = new();
        builder.MinimumLevel.Verbose();
        builder.MinimumLevel.Override( nameof(Microsoft), LogEventLevel.Warning );
        builder.MinimumLevel.Override( nameof(System),    LogEventLevel.Warning );
        builder.Enrich.FromLogContext();
        builder.Enrich.With<AppContextEnricher<TApp>>();
        builder.WriteTo.Console( ConsoleRestrictedToMinimumLevel, ConsoleOutputTemplate, CultureInfo, LevelSwitch, StandardErrorFromLevel, Theme, ApplyThemeToRedirectedOutput, SyncRoot );
        builder.WriteTo.Debug( DebugRestrictedToMinimumLevel, DebugOutputTemplate, CultureInfo, LevelSwitch );
        builder.WriteTo.Async( Configure, BufferSize, BlockWhenFull, Monitor );
        return builder.CreateLogger();
    }
}
