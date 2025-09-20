using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Serilog;



namespace TestMauiApp;


public static class MauiProgram
{
    [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();


        builder.UseMauiApp<App>()
               .UseMauiCommunityToolkit()
               .UseMauiCommunityToolkitMediaElement()
               .UseMauiCommunityToolkitCore()
               .UseMauiCompatibility()
               .ConfigureFonts(static fonts =>
                               {
                                   fonts.AddFont("OpenSans-Regular.ttf",  "OpenSansRegular");
                                   fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                               });

        builder.Services.AddHttpClient();

        builder.Logging.AddSerilog(App.Logger, true);
        return builder.Build();
    }
}
