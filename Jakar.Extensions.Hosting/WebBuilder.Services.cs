#nullable enable
namespace Jakar.Extensions.Hosting;


/// <summary>
/// <para><see href="https://stackoverflow.com/a/61726193/9530917">AddTransient, AddScoped and AddSingleton Services Differences</see></para>
/// </summary>
public static partial class WebBuilder
{
    /// <summary>
    /// Singleton objects are the same for every object and every request. Are memory efficient as they are created once reused everywhere.
    /// </summary>
    public static WebApplicationBuilder AddSingleton<T>( this WebApplicationBuilder builder, T instance ) where T : class
    {
        builder.Services.AddSingleton(instance);
        return builder;
    }
    /// <summary>
    /// Singleton objects are the same for every object and every request. Are memory efficient as they are created once reused everywhere.
    /// </summary>
    public static WebApplicationBuilder AddSingleton<T>( this WebApplicationBuilder builder ) where T : class
    {
        builder.Services.AddSingleton<T>();
        return builder;
    }
    /// <summary>
    /// Singleton objects are the same for every object and every request. Are memory efficient as they are created once reused everywhere.
    /// </summary>
    public static WebApplicationBuilder AddSingleton<T>( this WebApplicationBuilder builder, Func<IServiceProvider, T> factory ) where T : class
    {
        builder.Services.AddSingleton(factory);
        return builder;
    }


    /// <summary>
    /// since they are created every time they will use more memory and Resources and can have the negative impact on performance. use this for the lightweight service with little or no state.
    /// </summary>
    public static WebApplicationBuilder AddTransient<T>( this WebApplicationBuilder builder ) where T : class
    {
        builder.Services.AddScoped<T>();
        return builder;
    }
    /// <summary>
    /// since they are created every time they will use more memory and Resources and can have the negative impact on performance. use this for the lightweight service with little or no state.
    /// </summary>
    public static WebApplicationBuilder AddTransient<T>( this WebApplicationBuilder builder, Func<IServiceProvider, T> factory ) where T : class
    {
        builder.Services.AddScoped(factory);
        return builder;
    }


    /// <summary>
    /// better option when you want to maintain state within a request.
    /// </summary>
    public static WebApplicationBuilder AddScoped<T>( this WebApplicationBuilder builder ) where T : class
    {
        builder.Services.AddScoped<T>();
        return builder;
    }
    /// <summary>
    /// better option when you want to maintain state within a request.
    /// </summary>
    public static WebApplicationBuilder AddScoped<T>( this WebApplicationBuilder builder, Func<IServiceProvider, T> factory ) where T : class
    {
        builder.Services.AddScoped(factory);
        return builder;
    }


    public static WebApplicationBuilder AddHostedService<THostedService>( this WebApplicationBuilder builder ) where THostedService : class, IHostedService
    {
        builder.Services.AddHostedService<THostedService>();
        return builder;
    }
    public static WebApplicationBuilder AddHostedService<THostedService>( this WebApplicationBuilder builder, Func<IServiceProvider, THostedService> func ) where THostedService : class, IHostedService
    {
        builder.Services.AddHostedService(func);
        return builder;
    }
}
