// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/18/2024  21:06

namespace Jakar.Extensions.Blazor;


public interface ICascadingValueName
{
    public abstract static string CascadingName { get; }
}



public static class CascadingValueNameExtensions
{
    public static IServiceCollection TryAddCascadingValueScoped<TClass>( this IServiceCollection services )
        where TClass : class, INotifyPropertyChanged
    {
        services.TryAddScoped<TClass>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueScoped<TInterface, TClass>( this IServiceCollection services )
        where TInterface : class, INotifyPropertyChanged
        where TClass : class, TInterface
    {
        services.TryAddScoped<TInterface, TClass>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }


    public static IServiceCollection TryAddCascadingValueScopedNamed<TClass>( this IServiceCollection services )
        where TClass : class, INotifyPropertyChanged, ICascadingValueName
    {
        services.TryAddScoped<TClass>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(TClass.CascadingName, value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueScopedNamed<TInterface, TClass>( this IServiceCollection services )
        where TInterface : class, INotifyPropertyChanged
        where TClass : class, TInterface, ICascadingValueName
    {
        services.TryAddScoped<TInterface, TClass>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(TClass.CascadingName, value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }


    public static IServiceCollection TryAddCascadingValueSingleton<TClass>( this IServiceCollection services )
        where TClass : class, INotifyPropertyChanged
    {
        services.TryAddSingleton<TClass>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueSingleton<TClass>( this IServiceCollection services, TClass instance )
        where TClass : class, INotifyPropertyChanged
    {
        services.TryAddSingleton(instance);

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }


    public static IServiceCollection TryAddCascadingValueSingleton<TInterface, TClass>( this IServiceCollection services )
        where TInterface : class, INotifyPropertyChanged
        where TClass : class, TInterface
    {
        services.TryAddSingleton<TInterface, TClass>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueSingleton<TInterface, TClass>( this IServiceCollection services, TClass instance )
        where TInterface : class, INotifyPropertyChanged
        where TClass : class, TInterface
    {
        services.TryAddSingleton<TInterface>(instance);

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }


    public static IServiceCollection TryAddCascadingValueSingletonNamed<TClass>( this IServiceCollection services )
        where TClass : class, INotifyPropertyChanged, ICascadingValueName
    {
        services.TryAddSingleton<TClass>();

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(TClass.CascadingName, value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
    public static IServiceCollection TryAddCascadingValueSingletonNamed<TClass>( this IServiceCollection services, TClass instance )
        where TClass : class, INotifyPropertyChanged, ICascadingValueName
    {
        services.TryAddSingleton(instance);

        services.TryAddCascadingValue(static provider =>
                                      {
                                          ILogger<TClass>              logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<TClass>();
                                          TClass                       value  = provider.GetRequiredService<TClass>();
                                          CascadingValueSource<TClass> source = new(TClass.CascadingName, value, false);
                                          value.PropertyChanged += ( _, _ ) => source.NotifyChangedAsync().SafeFireAndForget(logger);
                                          return source;
                                      });

        return services;
    }
}
