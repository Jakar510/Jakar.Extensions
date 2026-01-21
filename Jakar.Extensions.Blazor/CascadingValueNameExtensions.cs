// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/18/2024  21:06

namespace Jakar.Extensions.Blazor;


public interface ICascadingValueName
{
    public abstract static string CascadingName { get; }
}



public sealed class NotifyPropertyChangedCascadingValueSource<TValue> : CascadingValueSource<TValue>, IDisposable
    where TValue : INotifyPropertyChanged
{
    public readonly  TValue                                                     Value;
    private readonly ILogger<NotifyPropertyChangedCascadingValueSource<TValue>> __logger;


    public NotifyPropertyChangedCascadingValueSource( TValue value, ILogger<NotifyPropertyChangedCascadingValueSource<TValue>> logger ) : base(value, true)
    {
        value.PropertyChanged += OnValueOnPropertyChanged;
        Value                =  value;
        __logger               =  logger;
    }
    public void Dispose() => Value.PropertyChanged -= OnValueOnPropertyChanged;
    private void OnValueOnPropertyChanged( object? sender, PropertyChangedEventArgs args )
    {
        NotifyChangedAsync()
           .SafeFireAndForget(__logger);
    }


    public static NotifyPropertyChangedCascadingValueSource<TValue> Create( IServiceProvider provider, TValue value ) => new(value,
                                                                                                                             provider.GetRequiredService<ILoggerFactory>()
                                                                                                                                     .CreateLogger<NotifyPropertyChangedCascadingValueSource<TValue>>());
    public static NotifyPropertyChangedCascadingValueSource<TValue> Create( IServiceProvider provider ) => new(provider.GetRequiredService<TValue>(),
                                                                                                               provider.GetRequiredService<ILoggerFactory>()
                                                                                                                       .CreateLogger<NotifyPropertyChangedCascadingValueSource<TValue>>());
}



public sealed class NotifyPropertyChangedCascadingValueNamedSource<TValue> : CascadingValueSource<TValue>, IDisposable
    where TValue : INotifyPropertyChanged, ICascadingValueName
{
    public readonly  TValue                                                     Value;
    private readonly ILogger<NotifyPropertyChangedCascadingValueSource<TValue>> __logger;


    public NotifyPropertyChangedCascadingValueNamedSource( TValue value, ILogger<NotifyPropertyChangedCascadingValueSource<TValue>> logger ) : base(TValue.CascadingName, value, true)
    {
        value.PropertyChanged += OnValueOnPropertyChanged;
        Value                =  value;
        __logger               =  logger;
    }
    public void Dispose() => Value.PropertyChanged -= OnValueOnPropertyChanged;
    private void OnValueOnPropertyChanged( object? sender, PropertyChangedEventArgs args )
    {
        NotifyChangedAsync()
           .SafeFireAndForget(__logger);
    }


    public static NotifyPropertyChangedCascadingValueSource<TValue> Create( IServiceProvider provider, TValue value ) => new(value,
                                                                                                                             provider.GetRequiredService<ILoggerFactory>()
                                                                                                                                     .CreateLogger<NotifyPropertyChangedCascadingValueSource<TValue>>());
    public static NotifyPropertyChangedCascadingValueSource<TValue> Create( IServiceProvider provider ) => new(provider.GetRequiredService<TValue>(),
                                                                                                               provider.GetRequiredService<ILoggerFactory>()
                                                                                                                       .CreateLogger<NotifyPropertyChangedCascadingValueSource<TValue>>());
}



public static class CascadingValueNameExtensions
{
    extension( IServiceCollection self )
    {
        public IServiceCollection TryAddCascadingValueScoped<TValue>()
            where TValue : class, INotifyPropertyChanged
        {
            self.TryAddScoped<TValue>();
            self.TryAddScoped(NotifyPropertyChangedCascadingValueSource<TValue>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueSource<TValue>>());
            return self;
        }
        public IServiceCollection TryAddCascadingValueScoped<TInterface, TValue>()
            where TInterface : class, INotifyPropertyChanged
            where TValue : class, TInterface
        {
            self.TryAddScoped<TInterface, TValue>();
            self.TryAddScoped(NotifyPropertyChangedCascadingValueSource<TInterface>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueSource<TInterface>>());
            return self;
        }
        public IServiceCollection TryAddCascadingValueScopedNamed<TValue>()
            where TValue : class, INotifyPropertyChanged, ICascadingValueName
        {
            self.TryAddScoped<TValue>();
            self.TryAddScoped(NotifyPropertyChangedCascadingValueNamedSource<TValue>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueNamedSource<TValue>>());
            return self;
        }
        public IServiceCollection TryAddCascadingValueScopedNamed<TInterface, TValue>()
            where TInterface : class, INotifyPropertyChanged
            where TValue : class, TInterface, ICascadingValueName
        {
            self.TryAddScoped<TInterface, TValue>();
            self.TryAddScoped(NotifyPropertyChangedCascadingValueNamedSource<TValue>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueNamedSource<TValue>>());
            return self;
        }
        public IServiceCollection TryAddCascadingValueSingleton<TValue>()
            where TValue : class, INotifyPropertyChanged
        {
            self.TryAddSingleton<TValue>();
            self.TryAddScoped(NotifyPropertyChangedCascadingValueSource<TValue>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueSource<TValue>>());
            return self;
        }
        public IServiceCollection TryAddCascadingValueSingleton<TValue>( TValue instance )
            where TValue : class, INotifyPropertyChanged
        {
            self.TryAddSingleton(instance);
            self.TryAddScoped(NotifyPropertyChangedCascadingValueSource<TValue>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueSource<TValue>>());
            return self;
        }
        public IServiceCollection TryAddCascadingValueSingleton<TInterface, TValue>()
            where TInterface : class, INotifyPropertyChanged
            where TValue : class, TInterface
        {
            self.TryAddSingleton<TInterface, TValue>();
            self.TryAddScoped(NotifyPropertyChangedCascadingValueSource<TInterface>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueSource<TInterface>>());
            return self;
        }
        public IServiceCollection TryAddCascadingValueSingleton<TInterface, TValue>( TValue instance )
            where TInterface : class, INotifyPropertyChanged
            where TValue : class, TInterface
        {
            self.TryAddSingleton<TInterface>(instance);
            self.TryAddScoped(NotifyPropertyChangedCascadingValueSource<TInterface>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueSource<TInterface>>());
            return self;
        }
        public IServiceCollection TryAddCascadingValueSingletonNamed<TValue>()
            where TValue : class, INotifyPropertyChanged, ICascadingValueName
        {
            self.TryAddSingleton<TValue>();
            self.TryAddScoped(NotifyPropertyChangedCascadingValueNamedSource<TValue>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueNamedSource<TValue>>());
            return self;
        }
        public IServiceCollection TryAddCascadingValueSingletonNamed<TValue>( TValue instance )
            where TValue : class, INotifyPropertyChanged, ICascadingValueName
        {
            self.TryAddSingleton(instance);
            self.TryAddScoped(NotifyPropertyChangedCascadingValueNamedSource<TValue>.Create);
            self.TryAddCascadingValue(static provider => provider.GetRequiredService<NotifyPropertyChangedCascadingValueNamedSource<TValue>>());
            return self;
        }
    }
}
