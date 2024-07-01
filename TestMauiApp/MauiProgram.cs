using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using CommunityToolkit.Maui;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using ILogger = Serilog.ILogger;



namespace TestMauiApp;


public sealed class TestMauiApp : IAppID
{
    public static Guid       AppID      { get; } = Guid.NewGuid();
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
        builder.Logging.AddSerilog( CreateSerilog() );

        return builder.Build();
    }
    private static ILogger CreateSerilog()
    {
        LoggerConfiguration builder = new();
        builder.MinimumLevel.Verbose();
        builder.MinimumLevel.Override( "Microsoft", LogEventLevel.Warning );
        builder.MinimumLevel.Override( "System",    LogEventLevel.Warning );
        builder.Enrich.FromLogContext();

        // builder.Enrich.With<AppContextEnricher<TestMauiApp>>();
        builder.WriteTo.Console( formatProvider: CultureInfo.InvariantCulture );
        builder.WriteTo.Debug( formatProvider: CultureInfo.InvariantCulture );
        builder.WriteTo.Async( ConfigureFiles );

        // builder.WriteTo.Async( ConfigureTelemetry );

        return Log.Logger = builder.CreateLogger();

        // LogContext.PushProperty(  )
        static void ConfigureFiles( LoggerSinkConfiguration sink ) =>
            sink.File( Activities.SetLogsFile( FileSystem.AppDataDirectory ).FullPath, formatProvider: CultureInfo.InvariantCulture, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, encoding: Encoding.Default );

        // static void ConfigureTelemetry( LoggerSinkConfiguration sink ) => sink.Sink<TelemetryLogger>();
    }
}
