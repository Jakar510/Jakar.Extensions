using Microsoft.Extensions.DependencyInjection.Extensions;



namespace Jakar.Extensions.Blazor;


/// <summary>
///     <para>
///         <see href="https://blazor.radzen.com/get-started"/>
///     </para>
///     <para>
///         <see href="https://github.com/Blazored/Modal"/>
///     </para>
///     <para>
///         <see href="https://github.com/Blazored/Toast"/>
///     </para>
///     <para>
///         <see href="https://github.com/Blazored/LocalStorage"/>
///     </para>
///     <para>
///         <see href="https://github.com/Blazored/SessionStorage"/>
///     </para>
/// </summary>
public static class BlazorBuilder
{
    public static IServiceCollection AddBlazorServices( this IServiceCollection services, Action<LocalStorageOptions>? configureLocal = null, Action<SessionStorageOptions>? configureSession = null )
    {
        services.AddAuthentication();
        services.AddAuthorization();
        services.TryAddScoped<LoginState>();
        services.TryAddCascadingValue( LoginState.KEY, LoginState.Get );
        services.TryAddScoped<ErrorState>();
        services.TryAddCascadingValue( ErrorState.KEY, ErrorState.Get );
        services.AddBlazoredModal().AddBlazoredToast().AddBlazoredLocalStorage( configureLocal ).AddBlazoredSessionStorage( configureSession );
        services.AddRadzenComponents();
        services.AddScoped<BlazorServices>();
        return services;
    }
}
