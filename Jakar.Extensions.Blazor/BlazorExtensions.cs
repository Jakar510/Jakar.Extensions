// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/18/2024  21:06

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;



namespace Jakar.Extensions.Blazor;


public static class BlazorExtensions
{
    public static IServiceCollection TryAddCascadingValueScoped<T>( this IServiceCollection services )
        where T : class, INotifyPropertyChanged
    {
        services.TryAddScoped<T>();

        services.TryAddCascadingValue( static provider =>
                                       {
                                           ILogger<T>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<T>();
                                           T                       value  = provider.GetRequiredService<T>();
                                           CascadingValueSource<T> source = new(value, false);
                                           value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget( logger );
                                           return source;
                                       } );

        return services;
    }


    public static IServiceCollection TryAddCascadingValueScopedNamed<T>( this IServiceCollection services )
        where T : class, INotifyPropertyChanged, ICascadingValueName
    {
        services.TryAddScoped<T>();

        services.TryAddCascadingValue( static provider =>
                                       {
                                           ILogger<T>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<T>();
                                           T                       value  = provider.GetRequiredService<T>();
                                           CascadingValueSource<T> source = new(T.CascadingName, value, false);
                                           value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget( logger );
                                           return source;
                                       } );

        return services;
    }


    public static IServiceCollection TryAddCascadingValueSingleton<T>( this IServiceCollection services )
        where T : class, INotifyPropertyChanged
    {
        services.AddSingleton<T>();

        services.TryAddCascadingValue( static provider =>
                                       {
                                           ILogger<T>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<T>();
                                           T                       value  = provider.GetRequiredService<T>();
                                           CascadingValueSource<T> source = new(value, false);
                                           value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget( logger );
                                           return source;
                                       } );

        return services;
    }
    public static IServiceCollection TryAddCascadingValueSingleton<T>( this IServiceCollection services, T instance )
        where T : class, INotifyPropertyChanged
    {
        services.AddSingleton( instance );

        services.TryAddCascadingValue( static provider =>
                                       {
                                           ILogger<T>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<T>();
                                           T                       value  = provider.GetRequiredService<T>();
                                           CascadingValueSource<T> source = new(value, false);
                                           value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget( logger );
                                           return source;
                                       } );

        return services;
    }


    public static IServiceCollection TryAddCascadingValueSingletonNamed<T>( this IServiceCollection services )
        where T : class, INotifyPropertyChanged, ICascadingValueName
    {
        services.AddSingleton<T>();

        services.TryAddCascadingValue( static provider =>
                                       {
                                           ILogger<T>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<T>();
                                           T                       value  = provider.GetRequiredService<T>();
                                           CascadingValueSource<T> source = new(T.CascadingName, value, false);
                                           value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget( logger );
                                           return source;
                                       } );

        return services;
    }
    public static IServiceCollection TryAddCascadingValueSingletonNamed<T>( this IServiceCollection services, T instance )
        where T : class, INotifyPropertyChanged, ICascadingValueName
    {
        services.AddSingleton( instance );

        services.TryAddCascadingValue( static provider =>
                                       {
                                           ILogger<T>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<T>();
                                           T                       value  = provider.GetRequiredService<T>();
                                           CascadingValueSource<T> source = new(T.CascadingName, value, false);
                                           value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget( logger );
                                           return source;
                                       } );

        return services;
    }
}
