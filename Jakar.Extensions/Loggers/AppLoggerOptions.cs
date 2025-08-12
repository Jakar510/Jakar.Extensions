// Jakar.Extensions :: Jakar.Extensions
// 08/12/2025  16:43

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Async;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;



namespace Jakar.Extensions;


public sealed class ScreenShot( ReadOnlyMemory<byte> data ) : ILogEventEnricher
{
    public const    int                  MAX_SIZE    = 1024 * 1024 * 5;
    public readonly ReadOnlyMemory<byte> Data        = data;
    public readonly string               ReferenceID = CreateID(data.Span);
    public readonly string               Value       = Convert.ToBase64String(data.Span);
    private         LogEventProperty?    __property;


    public static bool      EnableLogging { get; set; }
    public        bool      IsEmpty       => Data.IsEmpty;
    public static LocalFile File          => AppLoggerOptions.Current.Paths.Screenshot;


    public static implicit operator ScreenShot( ReadOnlyMemory<byte> data ) => new(data);
    public static implicit operator ScreenShot( byte[]               data ) => new(data);


    public LogEventProperty ToProperty() => __property ??= new LogEventProperty(nameof(ReferenceID), new ScalarValue(ReferenceID));
    public void Enrich( LogEvent log, ILogEventPropertyFactory propertyFactory )
    {
        if ( EnableLogging ) { log.AddPropertyIfAbsent(ToProperty()); }
    }


    public static string CreateID( params ReadOnlySpan<byte> data ) => data.Hash().ToString();
    public static string CreateID()
    {
        Span<byte> span = stackalloc byte[16];
        RandomNumberGenerator.Fill(span);
        return Convert.ToHexString(span);
    }
}



public sealed class AppLoggerOptions : IOptions<AppLoggerOptions>, IDisposable, IAsyncDisposable, ITelemetryActivityEnricher
{
    public const           string             CONSOLE_DEFAULT_OUTPUT_TEMPLATE     = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const           string             DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const           string             SEQ_API_KEY_NAME                    = "X-Seq-ApiKey";
    public const           string             SEQ_BUFFER_DIRECTORY                = "SeqBuffer";
    public static readonly LoggingLevelSwitch LoggingLevel                        = new(LogEventLevel.Verbose);
    private                FilePaths?         __paths;
    private                LocalDirectory?    __appDataDirectory;
    private                LocalDirectory?    __cacheDirectory;
    private                LocalDirectory?    __seqBuffer;


    public static AppLoggerOptions             Current       { get; set; } = new();
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
    public DeviceInfo DeviceInfo { get; set; } = new();
    public ref readonly AppInfo Info
    {
        get
        {
            TelemetrySource? current = TelemetrySource.Current;

            return ref current is null
                           ? ref AppInfo.Invalid
                           : ref current.Info;
        }
    }
    public ILogEventEnricher[]        Enrichers  { get;               set; } = [];
    public ITextFormatter             Formatter  { get;               set; } = new CompactJsonFormatter();
    public FileLifecycleHooks?        Hooks      { get;               set; }
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
    public Func<CancellationToken, ValueTask<ScreenShot?>> TakeScreenShot { get; set; } = TakeEmptyScreenShot;
    public ConsoleTheme?                                   Theme          { get; set; }
    AppLoggerOptions IOptions<AppLoggerOptions>.           Value          => this;


    public void Dispose()
    {
        __appDataDirectory?.Dispose();
        __cacheDirectory?.Dispose();
        __seqBuffer?.Dispose();
        __paths?.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        if ( __appDataDirectory is not null ) { await __appDataDirectory.DisposeAsync(); }

        if ( __cacheDirectory is not null ) { await __cacheDirectory.DisposeAsync(); }

        if ( __seqBuffer is not null ) { await __seqBuffer.DisposeAsync(); }

        if ( __paths is not null ) { await Disposables.CastAndDisposeAsync(ref __paths); }
    }


    private LocalDirectory GetAppDataDirectory() => __appDataDirectory ??= Path.Join(Environment.CurrentDirectory, FilePaths.APP_DATA_DIRECTORY);
    private LocalDirectory GetCacheDirectory()   => __cacheDirectory ??= Path.Join(Environment.CurrentDirectory,   FilePaths.CACHE_DIRECTORY);
    private FilePaths      GetPaths()            => __paths ??= new FilePaths(AppDataDirectory, CacheDirectory);
    private LocalDirectory GetSeqBuffer()        => __seqBuffer ??= Path.Join(AppDataDirectory, SEQ_BUFFER_DIRECTORY);


    private static ValueTask<ScreenShot?> TakeEmptyScreenShot( CancellationToken token ) => new(default(ScreenShot));


    public Logger CreateLogger( TelemetrySource source )
    {
        // const int EVENT_BODY_LIMIT_BYTES = 262144 + ScreenShot.MAX_SIZE;
        // EVENT_BODY_LIMIT_BYTES.WriteToDebug();
        // , eventBodyLimitBytes: EVENT_BODY_LIMIT_BYTES

        LoggerConfiguration builder = new();

        builder.MinimumLevel.Verbose();
        builder.MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Warning);
        builder.MinimumLevel.Override(nameof(System),    LogEventLevel.Warning);

        builder.Destructure.ToMaximumDepth(1000);
        builder.Destructure.ToMaximumStringLength(int.MaxValue);
        builder.Destructure.ToMaximumCollectionCount(99999);

        builder.Enrich.FromLogContext();

        builder.Enrich.With(new TelemetryActivityEnricher(this, source));

        builder.WriteTo.Debug(LogEventLevel.Verbose, DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE, CultureInfo.InvariantCulture);

        builder.WriteTo.File(Formatter, Paths.LogsFile.FullPath, flushToDiskInterval: TimeSpan.FromSeconds(2), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, encoding: Encoding.Default, hooks: Hooks, retainedFileTimeLimit: TimeSpan.FromDays(15));

        AddNativeLogs?.Invoke(builder);
        return builder.CreateLogger();
    }



    public sealed class DeviceMetaData : DeviceMetaData<LogEventProperty>, ILogEventEnricher
    {
        public LogEventProperty ToProperty() =>
            _property ??= new LogEventProperty(nameof(DeviceInfo),
                                               new StructureValue([
                                                                      Enricher.GetProperty(DeviceAppVersion,   nameof(DeviceAppVersion)),
                                                                      Enricher.GetProperty(DeviceID,           nameof(DeviceID)),
                                                                      Enricher.GetProperty(DeviceManufacturer, nameof(DeviceManufacturer)),
                                                                      Enricher.GetProperty(DeviceModel,        nameof(DeviceModel)),
                                                                      Enricher.GetProperty(DevicePlatform,     nameof(DevicePlatform)),
                                                                      Enricher.GetProperty(DeviceVersion,      nameof(DeviceVersion)),
                                                                      Enricher.GetProperty(PackageName,        nameof(PackageName))
                                                                  ]));
        public void Enrich( LogEvent log, ILogEventPropertyFactory propertyFactory ) => log.AddPropertyIfAbsent(ToProperty());
    }
}
