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
    public static IServiceCollection AddBlazorServices( this IServiceCollection services, Action<LocalStorageOptions>? configureLocal = null, Action<SessionStorageOptions>? configureSession = null ) =>
        services.AddBlazorServices<LoginUserState, ModelErrorState>( configureLocal, configureSession );
    public static IServiceCollection AddBlazorServices<TLoginState, TErrorState>( this IServiceCollection services, Action<LocalStorageOptions>? configureLocal = null, Action<SessionStorageOptions>? configureSession = null )
        where TLoginState : class, ILoginUserState
        where TErrorState : class, IModelErrorState
    {
        services.AddAuthentication();
        services.AddAuthorization();
        services.TryAddCascadingValueScopedNamed<TLoginState>();
        services.TryAddCascadingValueScopedNamed<TErrorState>();
        services.AddBlazoredModal().AddBlazoredToast().AddBlazoredLocalStorage( configureLocal ).AddBlazoredSessionStorage( configureSession );
        services.AddRadzenComponents();
        services.TryAddScoped<BlazorServices>();
        return services;
    }
}
