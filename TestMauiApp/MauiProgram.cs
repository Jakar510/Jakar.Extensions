using System.Diagnostics.CodeAnalysis;



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


    #if DEBUG
        builder.Logging.AddDebug();
    #endif

        return builder.Build();
    }
}
