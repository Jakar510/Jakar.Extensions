// Jakar.Extensions :: Jakar.Extensions
// 08/03/2022  5:29 PM

using Microsoft.Extensions.DependencyInjection;



namespace Jakar.Extensions;


public interface IHostInfo
{
    public Uri HostInfo { get; }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static IServiceCollection AddSingleton<TClass>( IServiceCollection collection, TClass value )
        where TClass : class, IHostInfo => collection.AddSingleton( value ).AddTransient<IHostInfo, TClass>( static provider => provider.GetRequiredService<TClass>() );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static IServiceCollection AddSingleton<TClass>( IServiceCollection collection, Func<IServiceProvider, TClass> value )
        where TClass : class, IHostInfo => collection.AddSingleton( value ).AddTransient<IHostInfo, TClass>( static provider => provider.GetRequiredService<TClass>() );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static IServiceCollection AddScoped<TClass>( IServiceCollection collection, Func<IServiceProvider, TClass> value )
        where TClass : class, IHostInfo => collection.AddScoped( value ).AddTransient<IHostInfo, TClass>( static provider => provider.GetRequiredService<TClass>() );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static IHostInfo Get( IServiceProvider provider ) => provider.GetRequiredService<IHostInfo>();
}
