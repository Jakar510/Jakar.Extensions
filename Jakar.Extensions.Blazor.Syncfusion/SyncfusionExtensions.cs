using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;



namespace Jakar.Extensions.Blazor.Syncfusion;


public static class SyncfusionExtensions
{
    public static IServiceCollection AddSyncfusion( this IServiceCollection services )
    {
        services.AddScoped<SfDialogService>();
        services.AddSyncfusionBlazor();
        return services;
    }
}
