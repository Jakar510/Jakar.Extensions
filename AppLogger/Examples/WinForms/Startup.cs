using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



namespace WinForms;


public static class Startup
{
    public static IServiceProvider? Services { get; private set; }

    public static void Init()
    {
        IHost? host = Host.CreateDefaultBuilder().ConfigureServices( WireupServices ).Build();

        Services = host.Services;
    }

    private static void WireupServices( HostBuilderContext context, IServiceCollection services )
    {
        services.AddWindowsFormsBlazorWebView();

    #if DEBUG
        services.AddBlazorWebViewDeveloperTools();
    #endif
    }
}
