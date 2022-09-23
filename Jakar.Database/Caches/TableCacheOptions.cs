// Jakar.Extensions :: Jakar.Database
// 09/02/2022  7:26 PM

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;



namespace Jakar.Database;


[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public class TableCacheOptions : IOptions<TableCacheOptions>
{
    TableCacheOptions IOptions<TableCacheOptions>.Value => this;

    public TimeSpan       RefreshTime { get; init; } = TimeSpan.FromSeconds(10);
    public ILoggerFactory Factory     { get; init; }


    public TableCacheOptions( ILoggerFactory factory ) => Factory = factory;


    public static WebApplicationBuilder Register( WebApplicationBuilder builder )
    {
        Register(builder.Services);
        return builder;
    }
    public static IServiceCollection Register( IServiceCollection collection )
    {
        collection.AddSingleton<IOptions<TableCacheOptions>, TableCacheOptions>();
        return collection;
    }
}
