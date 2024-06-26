using System.Diagnostics.CodeAnalysis;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;



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
               .ConfigureFonts( fonts =>
                                {
                                    fonts.AddFont( "OpenSans-Regular.ttf",  "OpenSansRegular" );
                                    fonts.AddFont( "OpenSans-Semibold.ttf", "OpenSansSemibold" );
                                } );

        builder.Services.AddHttpClient();


        builder.Logging.ClearProviders();

    #if DEBUG
        builder.Logging.AddDebug();
    #endif

        builder.Logging.AddOpenTelemetry( static options =>
                                          {
                                              options.IncludeScopes           = true;
                                              options.IncludeFormattedMessage = true;
                                              options.ParseStateValues        = true;

                                              options.AddConsoleExporter();

                                              options.AddOtlpExporter( static exporter =>
                                                                       {
                                                                           exporter.Protocol = OtlpExportProtocol.Grpc;
                                                                           exporter.Endpoint = new Uri( "https://192.168.1.12:4317" );
                                                                       } );
                                          } );

        return builder.Build();
    }
}
