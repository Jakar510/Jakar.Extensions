// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/18/2024  21:06

namespace Jakar.Extensions.Blazor;


public interface ICascadingValueName
{
    public abstract static string CascadingName { get; }
}



public static class CascadingValueNameExtensions
{
    public static IServiceCollection TryAddCascadingValueScoped<TSelf>( this IServiceCollection services )
        where TSelf : class, INotifyPropertyChanged
    {
        services.TryAddScoped<TSelf>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueScoped<TInterface, TSelf>( this IServiceCollection services )
        where TInterface : class, INotifyPropertyChanged
        where TSelf : class, TInterface
    {
        services.TryAddScoped<TInterface, TSelf>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }


    public static IServiceCollection TryAddCascadingValueScopedNamed<TSelf>( this IServiceCollection services )
        where TSelf : class, INotifyPropertyChanged, ICascadingValueName
    {
        services.TryAddScoped<TSelf>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(TSelf.CascadingName, value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueScopedNamed<TInterface, TSelf>( this IServiceCollection services )
        where TInterface : class, INotifyPropertyChanged
        where TSelf : class, TInterface, ICascadingValueName
    {
        services.TryAddScoped<TInterface, TSelf>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(TSelf.CascadingName, value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }


    public static IServiceCollection TryAddCascadingValueSingleton<TSelf>( this IServiceCollection services )
        where TSelf : class, INotifyPropertyChanged
    {
        services.TryAddSingleton<TSelf>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueSingleton<TSelf>( this IServiceCollection services, TSelf instance )
        where TSelf : class, INotifyPropertyChanged
    {
        services.TryAddSingleton(instance);

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }


    public static IServiceCollection TryAddCascadingValueSingleton<TInterface, TSelf>( this IServiceCollection services )
        where TInterface : class, INotifyPropertyChanged
        where TSelf : class, TInterface
    {
        services.TryAddSingleton<TInterface, TSelf>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueSingleton<TInterface, TSelf>( this IServiceCollection services, TSelf instance )
        where TInterface : class, INotifyPropertyChanged
        where TSelf : class, TInterface
    {
        services.TryAddSingleton<TInterface>(instance);

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }


    public static IServiceCollection TryAddCascadingValueSingletonNamed<TSelf>( this IServiceCollection services )
        where TSelf : class, INotifyPropertyChanged, ICascadingValueName
    {
        services.TryAddSingleton<TSelf>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(TSelf.CascadingName, value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueSingletonNamed<TSelf>( this IServiceCollection services, TSelf instance )
        where TSelf : class, INotifyPropertyChanged, ICascadingValueName
    {
        services.TryAddSingleton(instance);

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TSelf>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TSelf>();
                                          TSelf                       value  = provider.GetRequiredService<TSelf>();
                                          CascadingValueSource<TSelf> source = new(TSelf.CascadingName, value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
}
