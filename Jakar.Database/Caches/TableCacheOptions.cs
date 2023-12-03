// Jakar.Extensions :: Jakar.Database
// 09/02/2022  7:26 PM

namespace Jakar.Database.Caches;


[ SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" ), SuppressMessage( "ReSharper", "ConvertToPrimaryConstructor" ) ]
public sealed record TableCacheOptions : IOptions<TableCacheOptions>
{
    public TimeSpan                               ExpireTime  { get; set; } = TimeSpan.FromMinutes( 1 );
    public TimeSpan                               RefreshTime { get; set; } = TimeSpan.FromSeconds( 10 );
    TableCacheOptions IOptions<TableCacheOptions>.Value       => this;


    public static IServiceCollection Register( IServiceCollection collection )
    {
        collection.AddSingleton<IOptions<TableCacheOptions>, TableCacheOptions>();
        return collection;
    }
    public static WebApplicationBuilder Register( WebApplicationBuilder builder )
    {
        Register( builder.Services );
        return builder;
    }
}
