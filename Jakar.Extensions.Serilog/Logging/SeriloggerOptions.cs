// Jakar.Extensions :: Jakar.Extensions.Serilog
// 02/11/2025  16:02

using System.Net.Http;
using Microsoft.Extensions.Options;
using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Sinks.Async;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;



namespace Jakar.Extensions.Serilog;


public sealed class SeriloggerOptions : SeriloggerConstants, IOptions<SeriloggerOptions>, IDeviceName
{
    private ActivitySource? _source;


    public          ActivitySource                                           ActivitySource     { get => GetActivitySource(); set => _source = value; }
    public required Guid                                                     AppID              { get;                        set; }
    public required string                                                   AppName            { get;                        set; }
    public required AppVersion                                               AppVersion         { get;                        set; }
    public          Guid                                                     DebugID            { get;                        set; } = Guid.NewGuid();
    public          Guid                                                     DeviceID           { get;                        set; } = Guid.NewGuid();
    public          string                                                   DeviceName         { get;                        set; } = string.Empty;
    public          Action<LoggerConfiguration>?                             AddNativeLogs      { get;                        set; }
    public          long                                                     DeviceIDLong       { get;                        set; }
    public          FileLifecycleHooks?                                      Hooks              { get;                        set; }
    public          IAsyncLogEventSinkMonitor?                               Monitor            { get;                        set; }
    public required IFilePaths                                               Paths              { get;                        set; }
    public          SeqData?                                                 Seq                { get;                        set; }
    public          ConsoleData?                                             Console            { get;                        set; }
    public          DebugData?                                               Debug              { get;                        set; }
    public          CultureInfo                                              CultureInfo        { get;                        set; } = CultureInfo.InvariantCulture;
    public          Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>> TakeScreenShot     { get;                        set; } = TakeEmptyScreenShot;
    public          Func<EventDetails, EventDetails>                         UpdateEventDetails { get;                        set; } = UpdateEventDetailsNoOpp;
    SeriloggerOptions IOptions<SeriloggerOptions>.                           Value              => this;


    // public RemoteLogs? RemoteLogServer { get; set; }


    private       ActivitySource                  GetActivitySource()                              => _source ??= new ActivitySource( AppName, AppVersion.ToString() );
    public static EventDetails                    UpdateEventDetailsNoOpp( EventDetails  details ) => details;
    public static ValueTask<ReadOnlyMemory<byte>> TakeEmptyScreenShot( CancellationToken token )   => new(ReadOnlyMemory<byte>.Empty);


    public Activity GetActivity( string name, Activity? parent = null, ActivityKind kind = ActivityKind.Internal )
    {
        parent ??= Activity.Current;
        ActivityContext parentContext = parent?.Context ?? new ActivityContext();

        Activity activity = ActivitySource.CreateActivity( name, kind, parentContext ) ??
                            new Activity( name )
                            {
                                DisplayName = name,
                            };

        activity.SetStartTime( DateTime.UtcNow );
        activity.SetIdFormat( ActivityIdFormat.Hierarchical );
        activity.SetStatus( ActivityStatusCode.Ok );
        activity.SetTag( nameof(AppName),    AppName );
        activity.SetTag( nameof(AppID),      AppID.ToString() );
        activity.SetTag( nameof(AppVersion), AppVersion.ToString() );
        activity.SetTag( nameof(DeviceID),   DeviceID.ToString() );
        activity.SetTag( nameof(DebugID),    DebugID.ToString() );
        activity.Start();
        return activity;
    }



    [Experimental( nameof(RemoteLogger) )]
    public readonly struct RemoteLogs( Uri serverUrl, ITextFormatter? payloadFormatter = null, LoggingLevelSwitch? controlLevelSwitch = null ) : IConfigureLogWrites
    {
        public readonly Uri                 ServerUrl          = serverUrl;
        public readonly ITextFormatter?     PayloadFormatter   = payloadFormatter;
        public readonly LoggingLevelSwitch? ControlLevelSwitch = controlLevelSwitch;
        public          void                WriteTo( LoggerSinkConfiguration configuration ) => configuration.Sink( RemoteLogger.Create( ServerUrl ) );
    }



    public readonly struct DebugData( string outputTemplate = DEFAULT_DEBUG_OUTPUT_TEMPLATE, CultureInfo? formatProvider = null, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Debug ) : IConfigureLogWrites
    {
        public readonly CultureInfo   FormatProvider           = formatProvider ?? CultureInfo.InvariantCulture;
        public readonly string        OutputTemplate           = outputTemplate;
        public readonly LogEventLevel RestrictedToMinimumLevel = restrictedToMinimumLevel;


        public void WriteTo( LoggerSinkConfiguration configuration ) => configuration.Debug( RestrictedToMinimumLevel, OutputTemplate, FormatProvider );
    }



    public readonly struct ConsoleData( string              outputTemplate               = DEFAULT_CONSOLE_OUTPUT_TEMPLATE,
                                        ConsoleTheme?       theme                        = null,
                                        object?             syncRoot                     = null,
                                        bool                applyThemeToRedirectedOutput = true,
                                        CultureInfo?        formatProvider               = null,
                                        LoggingLevelSwitch? controlLevelSwitch           = null,
                                        LogEventLevel       restrictedToMinimumLevel     = LogEventLevel.Information,
                                        LogEventLevel       standardErrorFromLevel       = LogEventLevel.Error ) : IConfigureLogWrites
    {
        public readonly CultureInfo         FormatProvider               = formatProvider ?? CultureInfo.InvariantCulture;
        public readonly LogEventLevel       RestrictedToMinimumLevel     = restrictedToMinimumLevel;
        public readonly LogEventLevel       StandardErrorFromLevel       = standardErrorFromLevel;
        public readonly LoggingLevelSwitch? ControlLevelSwitch           = controlLevelSwitch;
        public readonly string              OutputTemplate               = outputTemplate;
        public readonly bool                ApplyThemeToRedirectedOutput = applyThemeToRedirectedOutput;
        public readonly object?             SyncRoot                     = syncRoot;
        public readonly ConsoleTheme?       Theme                        = theme;


        public void WriteTo( LoggerSinkConfiguration configuration ) =>
            configuration.Console( RestrictedToMinimumLevel, OutputTemplate, FormatProvider, ControlLevelSwitch, StandardErrorFromLevel, Theme, ApplyThemeToRedirectedOutput, SyncRoot );
    }



    public readonly struct SeqData( Uri                 serverUrl,
                                    string?             apiKey,
                                    CultureInfo?        formatProvider                    = null,
                                    int                 batchPostingLimit                 = 1000,
                                    string?             bufferBaseFilename                = null,
                                    long?               bufferSizeLimitBytes              = null,
                                    LoggingLevelSwitch? controlLevelSwitch                = null,
                                    long?               eventBodyLimitBytes               = 1024 * 1024 * 10,
                                    HttpMessageHandler? messageHandler                    = null,
                                    ITextFormatter?     payloadFormatter                  = null,
                                    TimeSpan?           period                            = null,
                                    int                 queueSizeLimit                    = 100000,
                                    LogEventLevel       restrictedToMinimumLevel          = LogEventLevel.Verbose,
                                    long?               retainedInvalidPayloadsLimitBytes = null ) : IConfigureLogWrites
    {
        public readonly Uri                 ServerUrl                         = serverUrl;
        public readonly string?             APIKey                            = apiKey;
        public readonly CultureInfo         FormatProvider                    = formatProvider ?? CultureInfo.InvariantCulture;
        public readonly LogEventLevel       RestrictedToMinimumLevel          = restrictedToMinimumLevel;
        public readonly int                 BatchPostingLimit                 = batchPostingLimit;
        public readonly TimeSpan?           Period                            = period;
        public readonly string?             BufferBaseFilename                = bufferBaseFilename;
        public readonly long?               BufferSizeLimitBytes              = bufferSizeLimitBytes;
        public readonly long?               EventBodyLimitBytes               = eventBodyLimitBytes;
        public readonly LoggingLevelSwitch? ControlLevelSwitch                = controlLevelSwitch;
        public readonly HttpMessageHandler? MessageHandler                    = messageHandler;
        public readonly long?               RetainedInvalidPayloadsLimitBytes = retainedInvalidPayloadsLimitBytes;
        public readonly int                 QueueSizeLimit                    = queueSizeLimit;
        public readonly ITextFormatter?     PayloadFormatter                  = payloadFormatter;


        public void WriteTo( LoggerSinkConfiguration configuration ) =>
            configuration.Seq( ServerUrl.OriginalString,
                               RestrictedToMinimumLevel,
                               BatchPostingLimit,
                               Period,
                               APIKey,
                               BufferBaseFilename,
                               BufferSizeLimitBytes,
                               EventBodyLimitBytes,
                               ControlLevelSwitch,
                               MessageHandler,
                               RetainedInvalidPayloadsLimitBytes,
                               QueueSizeLimit,
                               PayloadFormatter,
                               FormatProvider );
    }



    public interface IConfigureLogWrites
    {
        public void WriteTo( LoggerSinkConfiguration configuration );
    }
}
