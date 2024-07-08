using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui;



namespace TestMauiApp;


public static class MauiProgram
{
    [SuppressMessage( "ReSharper", "RedundantTypeArgumentsOfMethod" )]
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();


        builder.UseMauiApp<App>()
               .UseMauiCommunityToolkit()
               .ConfigureFonts( static fonts =>
                                {
                                    fonts.AddFont( "OpenSans-Regular.ttf",  "OpenSansRegular" );
                                    fonts.AddFont( "OpenSans-Semibold.ttf", "OpenSansSemibold" );
                                } );

        builder.Services.AddHttpClient();

        App.Serilogger.Configure( builder.Logging );
        return builder.Build();
    }
}
