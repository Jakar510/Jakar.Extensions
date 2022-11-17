// Jakar.Extensions :: Jakar.Database
// 09/02/2022  7:26 PM

namespace Jakar.Database.Caches;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
[SuppressMessage( "ReSharper", "ConvertToPrimaryConstructor" )]
public sealed record TableCacheOptions : IOptions<TableCacheOptions>
{
    public ILoggerFactory                         Factory     { get; init; }
    TableCacheOptions IOptions<TableCacheOptions>.Value       => this;
    public TimeSpan                               RefreshTime { get; init; } = TimeSpan.FromSeconds( 10 );


    public TableCacheOptions( ILoggerFactory factory ) => Factory = factory;
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
