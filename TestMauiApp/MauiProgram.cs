using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using CommunityToolkit.Maui;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.OpenTelemetry;
using ILogger = Serilog.ILogger;



namespace TestMauiApp;


public sealed class TestMauiApp : IAppName
{
    public static string     AppName    => nameof(TestMauiApp);
    public static AppVersion AppVersion { get; } = new(1, 0, 0);
}



public static class MauiProgram
{
    [SuppressMessage( "ReSharper", "RedundantTypeArgumentsOfMethod" )]
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();


        builder.UseMauiApp<App>()
               .UseMauiCommunityToolkit()
               .ConfigureFonts( fonts =>
                                {
                                    fonts.AddFont( "OpenSans-Regular.ttf",  "OpenSansRegular" );
                                    fonts.AddFont( "OpenSans-Semibold.ttf", "OpenSansSemibold" );
                                } );

        builder.Services.AddHttpClient();


        builder.Logging.ClearProviders();
        builder.Logging.AddProvider( CreateSerilog() );

        return builder.Build();
    }
    private static SerilogLoggerProvider CreateSerilog()
    {
        LoggerConfiguration builder = new();
        builder.MinimumLevel.Verbose();
        builder.MinimumLevel.Override( "Microsoft", LogEventLevel.Warning );
        builder.MinimumLevel.Override( "System",    LogEventLevel.Warning );
        builder.Enrich.FromLogContext();
        builder.Enrich.WithThreadName();
        builder.Enrich.WithThreadId();
        builder.WriteTo.Console();
        builder.WriteTo.Debug();
        builder.WriteTo.Async( ConfigureFiles );
        builder.WriteTo.Async( ConfigureOpenTelemetry );


        ILogger logger = Log.Logger = builder.CreateLogger();
        return new SerilogLoggerProvider( logger, true );


        static void ConfigureFiles( LoggerSinkConfiguration sink ) => sink.File( Activities.SetLogsFile( FileSystem.AppDataDirectory ).FullPath, formatProvider: CultureInfo.InvariantCulture, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, encoding: Encoding.Default );

        static void ConfigureOpenTelemetry( LoggerSinkConfiguration sink ) =>
            sink.OpenTelemetry( static options =>
                                {
                                    options.Endpoint     = "https://192.168.1.12:4317";
                                    options.Protocol     = OtlpProtocol.Grpc;
                                    options.IncludedData = IncludedData.SpanIdField | IncludedData.SourceContextAttribute | IncludedData.TraceIdField | IncludedData.SpecRequiredResourceAttributes | IncludedData.MessageTemplateMD5HashAttribute | IncludedData.MessageTemplateRenderingsAttribute | IncludedData.MessageTemplateTextAttribute | IncludedData.TemplateBody;
                                } );
    }
}
