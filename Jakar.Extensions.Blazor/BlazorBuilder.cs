namespace Jakar.Extensions.Blazor;


/// <summary>
/// <para><see href="https://blazor.radzen.com/get-started"/></para>
/// <para><see href="https://github.com/Blazored/Modal"/></para>
/// <para><see href="https://github.com/Blazored/Toast"/></para>
/// <para><see href="https://github.com/Blazored/LocalStorage"/></para>
/// <para><see href="https://github.com/Blazored/SessionStorage"/></para>
/// </summary>
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public static class BlazorBuilder
{
    public static WebApplicationBuilder AddAppServices( this WebApplicationBuilder builder ) =>
        builder.AddAuthenticationCore()
               .AddBlazored()
               .AddRadzen()
               .AddScoped<AppServices>();
    public static WebApplicationBuilder AddAppServices( this WebApplicationBuilder builder, Action<LocalStorageOptions> configureLocal ) =>
        builder.AddAuthenticationCore()
               .AddBlazored(configureLocal)
               .AddRadzen()
               .AddScoped<AppServices>();
    public static WebApplicationBuilder AddAppServices( this WebApplicationBuilder builder, Action<SessionStorageOptions> configureSession ) =>
        builder.AddAuthenticationCore()
               .AddBlazored(configureSession)
               .AddRadzen()
               .AddScoped<AppServices>();
    public static WebApplicationBuilder AddAppServices( this WebApplicationBuilder builder, Action<LocalStorageOptions> configureLocal, Action<SessionStorageOptions> configureSession ) =>
        builder.AddAuthenticationCore()
               .AddBlazored(configureLocal, configureSession)
               .AddRadzen()
               .AddScoped<AppServices>();


    public static WebApplicationBuilder AddAppServices<TServices>( this WebApplicationBuilder builder ) where TServices : AppServices =>
        builder.AddAuthenticationCore()
               .AddBlazored()
               .AddRadzen()
               .AddScoped<TServices>();
    public static WebApplicationBuilder AddAppServices<TServices>( this WebApplicationBuilder builder, Action<LocalStorageOptions> configureLocal ) where TServices : AppServices =>
        builder.AddAuthenticationCore()
               .AddBlazored(configureLocal)
               .AddRadzen()
               .AddScoped<TServices>();
    public static WebApplicationBuilder AddAppServices<TServices>( this WebApplicationBuilder builder, Action<SessionStorageOptions> configureSession ) where TServices : AppServices =>
        builder.AddAuthenticationCore()
               .AddBlazored(configureSession)
               .AddRadzen()
               .AddScoped<TServices>();
    public static WebApplicationBuilder AddAppServices<TServices>( this WebApplicationBuilder builder, Action<LocalStorageOptions> configureLocal, Action<SessionStorageOptions> configureSession ) where TServices : AppServices =>
        builder.AddAuthenticationCore()
               .AddBlazored(configureLocal, configureSession)
               .AddRadzen()
               .AddScoped<TServices>();


    /// <summary>
    /// <para><see href="https://blazor.radzen.com/get-started"/></para>
    /// </summary>
    public static WebApplicationBuilder AddRadzen( this WebApplicationBuilder builder ) => builder.AddScoped<DialogService>()
                                                                                                  .AddScoped<TooltipService>()
                                                                                                  .AddScoped<ContextMenuService>()
                                                                                                  .AddScoped<NotificationService>();


    /// <summary>
    /// <para><see href="https://github.com/Blazored/Modal"/></para>
    /// <para><see href="https://github.com/Blazored/Toast"/></para>
    /// <para><see href="https://github.com/Blazored/LocalStorage"/></para>
    /// <para><see href="https://github.com/Blazored/SessionStorage"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazored( this WebApplicationBuilder builder ) => builder.AddBlazoredModal()
                                                                                                    .AddBlazoredToast()
                                                                                                    .AddBlazoredLocalStorage()
                                                                                                    .AddBlazoredSessionStorage();
    /// <summary>
    /// <para><see href="https://github.com/Blazored/Modal"/></para>
    /// <para><see href="https://github.com/Blazored/Toast"/></para>
    /// <para><see href="https://github.com/Blazored/LocalStorage"/></para>
    /// <para><see href="https://github.com/Blazored/SessionStorage"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazored( this WebApplicationBuilder builder, Action<SessionStorageOptions> configureSession ) => builder.AddBlazoredModal()
                                                                                                                                                    .AddBlazoredToast()
                                                                                                                                                    .AddBlazoredLocalStorage()
                                                                                                                                                    .AddBlazoredSessionStorage(configureSession);
    /// <summary>
    /// <para><see href="https://github.com/Blazored/Modal"/></para>
    /// <para><see href="https://github.com/Blazored/Toast"/></para>
    /// <para><see href="https://github.com/Blazored/LocalStorage"/></para>
    /// <para><see href="https://github.com/Blazored/SessionStorage"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazored( this WebApplicationBuilder builder, Action<LocalStorageOptions> configureLocal ) => builder.AddBlazoredModal()
                                                                                                                                                .AddBlazoredToast()
                                                                                                                                                .AddBlazoredLocalStorage(configureLocal)
                                                                                                                                                .AddBlazoredSessionStorage();
    /// <summary>
    /// <para><see href="https://github.com/Blazored/Modal"/></para>
    /// <para><see href="https://github.com/Blazored/Toast"/></para>
    /// <para><see href="https://github.com/Blazored/LocalStorage"/></para>
    /// <para><see href="https://github.com/Blazored/SessionStorage"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazored( this WebApplicationBuilder builder, Action<LocalStorageOptions> configureLocal, Action<SessionStorageOptions> configureSession ) =>
        builder.AddBlazoredModal()
               .AddBlazoredToast()
               .AddBlazoredLocalStorage(configureLocal)
               .AddBlazoredSessionStorage(configureSession);


    /// <summary>
    /// <para><see href="https://github.com/Blazored/Toast"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazoredToast( this WebApplicationBuilder builder )
    {
        builder.Services.AddBlazoredToast();
        return builder;
    }


    /// <summary>
    /// <para><see href="https://github.com/Blazored/Modal"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazoredModal( this WebApplicationBuilder builder )
    {
        builder.Services.AddBlazoredModal();
        return builder;
    }


    /// <summary>
    /// <para><see href="https://github.com/Blazored/LocalStorage"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazoredLocalStorage( this WebApplicationBuilder builder )
    {
        builder.Services.AddBlazoredLocalStorage();
        return builder;
    }
    /// <summary>
    /// <para><see href="https://github.com/Blazored/LocalStorage"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazoredLocalStorage( this WebApplicationBuilder builder, Action<LocalStorageOptions> configure )
    {
        builder.Services.AddBlazoredLocalStorage(configure);
        return builder;
    }


    /// <summary>
    /// <para><see href="https://github.com/Blazored/SessionStorage"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazoredSessionStorage( this WebApplicationBuilder builder )
    {
        builder.Services.AddBlazoredSessionStorage();
        return builder;
    }
    /// <summary>
    /// <para><see href="https://github.com/Blazored/SessionStorage"/></para>
    /// </summary>
    public static WebApplicationBuilder AddBlazoredSessionStorage( this WebApplicationBuilder builder, Action<SessionStorageOptions> configure )
    {
        builder.Services.AddBlazoredSessionStorage(configure);
        return builder;
    }
}
