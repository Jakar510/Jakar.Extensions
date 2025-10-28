// Jakar.Extensions :: Jakar.Extensions
// 08/12/2025  16:43

using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Async;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;



namespace Jakar.Extensions;


public class AppLoggerOptions : BaseClass, IOptions<AppLoggerOptions>, IDisposable, IAsyncDisposable, IOpenTelemetryActivityEnricher
{
    public static readonly LoggingLevelSwitch LoggingLevel = new(LogEventLevel.Verbose);
    private                FilePaths?         __paths;
    private                LocalDirectory?    __appDataDirectory;
    private                LocalDirectory?    __cacheDirectory;
    private                LocalDirectory?    __seqBuffer;


    public static AppLoggerOptions             Current       { get; private set; } = new();
    public        Action<LoggerConfiguration>? AddNativeLogs { get; set; }
    public        string?                      AppBuild      { get; set; }
    public LocalDirectory AppDataDirectory
    {
        get => GetAppDataDirectory();
        set
        {
            __appDataDirectory = value;
            __paths            = null;
            __seqBuffer        = null;
            Directory.CreateDirectory(value);
        }
    }
    public LocalDirectory CacheDirectory
    {
        get => GetCacheDirectory();
        set
        {
            __cacheDirectory = value;
            __paths          = null;
            Directory.CreateDirectory(value);
        }
    }
    public DeviceInformation   DeviceInfo { get; set; } = new();
    public ILogEventEnricher[] Enrichers  { get; set; } = [];
    public ITextFormatter      Formatter  { get; set; } = new CompactJsonFormatter();
    public FileLifecycleHooks? Hooks      { get; set; }
    public ref readonly AppInformation Info
    {
        get
        {
            TelemetrySource? current = TelemetrySource.Current;

            return ref current is null
                           ? ref AppInformation.Invalid
                           : ref current.Info;
        }
    }
    public IAsyncLogEventSinkMonitor? Monitor    { get;               set; }
    public FilePaths                  Paths      { get => GetPaths(); set => __paths = value; }
    public ScreenShot?                ScreenShot { get;               set; }
    public LocalDirectory SeqBuffer
    {
        get => GetSeqBuffer();
        set
        {
            __seqBuffer = value;
            Directory.CreateDirectory(value);
        }
    }
    public         TakeScreenShotAsync          TakeScreenShot        { get; set; } = ScreenShot.Empty;
    public virtual bool                         TakeScreenshotOnError { get; set; }
    public         ConsoleTheme?                Theme                 { get; set; }
    AppLoggerOptions IOptions<AppLoggerOptions>.Value                 => this;


    public AppLoggerOptions() => Current = this;
    public virtual void Dispose()
    {
        Disposables.ClearAndDispose(ref __appDataDirectory);
        Disposables.ClearAndDispose(ref __cacheDirectory);
        Disposables.ClearAndDispose(ref __seqBuffer);
        Disposables.ClearAndDispose(ref __paths);
        GC.SuppressFinalize(this);
    }
    public virtual async ValueTask DisposeAsync()
    {
        await Disposables.ClearAndDisposeAsync(ref __appDataDirectory);
        await Disposables.ClearAndDisposeAsync(ref __cacheDirectory);
        await Disposables.ClearAndDisposeAsync(ref __seqBuffer);
        await Disposables.ClearAndDisposeAsync(ref __paths);
        GC.SuppressFinalize(this);
    }


    private LocalDirectory GetAppDataDirectory() => __appDataDirectory ??= Path.Join(Environment.CurrentDirectory, APP_DATA_DIRECTORY);
    private LocalDirectory GetCacheDirectory()   => __cacheDirectory ??= Path.Join(Environment.CurrentDirectory,   CACHE_DIRECTORY);
    private FilePaths      GetPaths()            => __paths ??= new FilePaths(AppDataDirectory, CacheDirectory);
    private LocalDirectory GetSeqBuffer()        => __seqBuffer ??= Path.Join(AppDataDirectory, SEQ_BUFFER_DIRECTORY);


    public Logger CreateLogger( TelemetrySource source )
    {
        LoggerConfiguration builder = new();

        ConfigureLogger(builder, builder.MinimumLevel, builder.WriteTo, builder.AuditTo, builder.Enrich, builder.Filter, builder.Destructure, in source);

        AddNativeLogs?.Invoke(builder);

        return builder.CreateLogger();
    }


    public virtual void ConfigureLogger( in LoggerConfiguration configuration, in LoggerMinimumLevelConfiguration minimumLevel, in LoggerSinkConfiguration sinkTo, in LoggerAuditSinkConfiguration auditTo, in LoggerEnrichmentConfiguration enrichment, in LoggerFilterConfiguration filter, in LoggerDestructuringConfiguration destructure, in TelemetrySource source )
    {
        minimumLevel.Verbose();
        minimumLevel.Override(nameof(Microsoft), LogEventLevel.Warning);
        minimumLevel.Override(nameof(System),    LogEventLevel.Warning);

        destructure.ToMaximumDepth(1000);
        destructure.ToMaximumStringLength(int.MaxValue);
        destructure.ToMaximumCollectionCount(99999);

        enrichment.FromLogContext();
        OpenTelemetryActivityEnricher.Create(enrichment, this, source);

        SinkToDebug(in sinkTo);
        SinkToFile(in sinkTo);
    }
    public static void SinkToDebug( in LoggerSinkConfiguration sinkTo ) => sinkTo.Debug(LogEventLevel.Verbose, DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE, CultureInfo.InvariantCulture);
    public        void SinkToFile( in  LoggerSinkConfiguration sinkTo ) => sinkTo.File(Formatter, Paths.LogsFile.FullPath, flushToDiskInterval: TimeSpan.FromSeconds(2), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, encoding: Encoding.Default, hooks: Hooks, retainedFileTimeLimit: TimeSpan.FromDays(15));
}



public delegate void ConfigureLogger( in AppLoggerOptions options, in LoggerMinimumLevelConfiguration minimumLevel, in LoggerSinkConfiguration sinkTo, in LoggerAuditSinkConfiguration auditTo, in LoggerEnrichmentConfiguration enrichment, in LoggerFilterConfiguration filter, in LoggerDestructuringConfiguration destructure, in TelemetrySource source );
