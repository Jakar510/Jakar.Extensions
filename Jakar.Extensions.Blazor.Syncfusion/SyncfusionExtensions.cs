using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;



namespace Jakar.Extensions.Blazor.Syncfusion;


public static class SyncfusionExtensions
{
    public static IServiceCollection AddSyncfusion( this IServiceCollection services )
    {
        services.TryAddScoped<SfDialogService>();
        services.AddSyncfusionBlazor();
        return services;
    }
}
