using System.Diagnostics.CodeAnalysis;



namespace Jakar.Extensions.Blazor;


[SuppressMessage("ReSharper", "UnusedType.Global")]
public static class BlazoredBuilder
{
    public static WebApplicationBuilder AddBlazored( this WebApplicationBuilder builder )
    {
        builder.AddBlazoredModal();
        builder.AddBlazoredToast();
        builder.AddBlazoredLocalStorage();
        builder.AddBlazoredSessionStorage();
        return builder;
    }
    public static WebApplicationBuilder AddBlazored( this WebApplicationBuilder builder, Action<SessionStorageOptions> configureSession )
    {
        builder.AddBlazoredModal();
        builder.AddBlazoredToast();
        builder.AddBlazoredLocalStorage();
        builder.AddBlazoredSessionStorage(configureSession);
        return builder;
    }
    public static WebApplicationBuilder AddBlazored( this WebApplicationBuilder builder, Action<LocalStorageOptions> configureLocal )
    {
        builder.AddBlazoredModal();
        builder.AddBlazoredToast();
        builder.AddBlazoredLocalStorage(configureLocal);
        builder.AddBlazoredSessionStorage();
        return builder;
    }
    public static WebApplicationBuilder AddBlazored( this WebApplicationBuilder builder, Action<LocalStorageOptions> configureLocal, Action<SessionStorageOptions> configureSession )
    {
        builder.AddBlazoredModal();
        builder.AddBlazoredToast();
        builder.AddBlazoredLocalStorage(configureLocal);
        builder.AddBlazoredSessionStorage(configureSession);
        return builder;
    }


    public static WebApplicationBuilder AddBlazoredToast( this WebApplicationBuilder builder )
    {
        builder.Services.AddBlazoredToast();
        return builder;
    }


    public static WebApplicationBuilder AddBlazoredModal( this WebApplicationBuilder builder )
    {
        builder.Services.AddBlazoredModal();
        return builder;
    }


    public static WebApplicationBuilder AddBlazoredLocalStorage( this WebApplicationBuilder builder )
    {
        builder.Services.AddBlazoredLocalStorage();
        return builder;
    }
    public static WebApplicationBuilder AddBlazoredLocalStorage( this WebApplicationBuilder builder, Action<LocalStorageOptions> configure )
    {
        builder.Services.AddBlazoredLocalStorage(configure);
        return builder;
    }


    public static WebApplicationBuilder AddBlazoredSessionStorage( this WebApplicationBuilder builder )
    {
        builder.Services.AddBlazoredSessionStorage();
        return builder;
    }
    public static WebApplicationBuilder AddBlazoredSessionStorage( this WebApplicationBuilder builder, Action<SessionStorageOptions> configure )
    {
        builder.Services.AddBlazoredSessionStorage(configure);
        return builder;
    }
}
