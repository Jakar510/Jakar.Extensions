// Jakar.Extensions :: Jakar.Extensions
// 08/03/2022  5:29 PM

using Microsoft.Extensions.DependencyInjection;



namespace Jakar.Extensions;


public interface IHostInfo
{
    public Uri HostInfo { get; }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static IServiceCollection AddSingleton<TSelf>( IServiceCollection collection, TSelf value )
        where TSelf : class, IHostInfo => collection.AddSingleton( value ).AddTransient<IHostInfo, TSelf>( static provider => provider.GetRequiredService<TSelf>() );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static IServiceCollection AddSingleton<TSelf>( IServiceCollection collection, Func<IServiceProvider, TSelf> value )
        where TSelf : class, IHostInfo => collection.AddSingleton( value ).AddTransient<IHostInfo, TSelf>( static provider => provider.GetRequiredService<TSelf>() );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static IServiceCollection AddScoped<TSelf>( IServiceCollection collection, Func<IServiceProvider, TSelf> value )
        where TSelf : class, IHostInfo => collection.AddScoped( value ).AddTransient<IHostInfo, TSelf>( static provider => provider.GetRequiredService<TSelf>() );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static IHostInfo Get( IServiceProvider provider ) => provider.GetRequiredService<IHostInfo>();
}
