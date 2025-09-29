using Blazored.LocalStorage;
using Blazored.LocalStorage.StorageOptions;
using Blazored.Modal;
using Blazored.SessionStorage;
using Blazored.SessionStorage.StorageOptions;
using Blazored.Toast;
using Microsoft.AspNetCore.Authorization;
using MudBlazor.Services;
using Radzen;



namespace Jakar.Extensions.Blazor;


/// <summary>
///     <para>
///         <see href="https://github.com/syncfusion/maui-toolkit"/>
///     </para>
///     <para>
///         <see href="https://github.com/MudBlazor/MudBlazor"/>
///     </para>
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
    public static IServiceCollection AddBlazorServices( this IServiceCollection services, Action<AuthorizationOptions>? authorization = null, Action<AuthenticationOptions>? authentication = null, Action<LocalStorageOptions>? configureLocal = null, Action<SessionStorageOptions>? configureSession = null, Action<MudServicesConfiguration>? mudConfiguration = null )
    {
        services.AddAuthentication( x =>
                                    {
                                        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                                        authentication?.Invoke( x );
                                    } );

        if ( authorization is not null ) { services.AddAuthorization( authorization ); }
        else { services.AddAuthorization(); }

        services.TryAddCascadingValueScopedNamed<LoginUserState>();
        services.TryAddCascadingValueScopedNamed<ModelErrorState>();

        if ( mudConfiguration is not null ) { services.AddMudServices( mudConfiguration ); }
        else { services.AddMudServices(); }

        services.AddBlazoredModal();
        services.AddBlazoredToast();
        services.AddBlazoredLocalStorage( configureLocal );
        services.AddBlazoredSessionStorage( configureSession );
        services.AddRadzenComponents();
        services.AddRadzenCookieThemeService();
        services.AddRadzenQueryStringThemeService();
        services.TryAddScoped( BlazorServices.Create );
        return services;
    }
}
