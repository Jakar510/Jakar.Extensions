// Jakar.Extensions :: Jakar.Database
// 11/02/2023  10:25 AM

namespace Jakar.Database;


public static class DbHostingExtensions
{
    public static void ConfigureMigrationsMsSql( this IMigrationRunnerBuilder migration )
    {
        migration.AddSqlServer2016();
        DbOptions.GetConnectionString( migration );
        migration.ScanIn( typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }
    public static void ConfigureMigrationsPostgres( this IMigrationRunnerBuilder migration )
    {
        migration.AddPostgres();
        DbOptions.GetConnectionString( migration );
        migration.ScanIn( typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }


    public static WebApplicationBuilder AddDb<TDatabase>( this WebApplicationBuilder builder, Action<DbOptions> dbOptions, Action<IMigrationRunnerBuilder> migration )
        where TDatabase : Database => builder.AddDb<TDatabase, SqlCacheFactory>( dbOptions, migration );


    public static WebApplicationBuilder AddDb<TDatabase, TSqlCacheFactory>( this WebApplicationBuilder builder, Action<DbOptions> dbOptions, Action<IMigrationRunnerBuilder> migration )
        where TDatabase : Database
        where TSqlCacheFactory : class, ISqlCacheFactory
    {
        builder.Services.AddOptions<DbOptions>().Configure( dbOptions );

        builder.Services.AddSingleton<ISqlCacheFactory, TSqlCacheFactory>();
        builder.Services.AddSingleton<TDatabase>();
        builder.Services.AddTransient<Database>( provider => provider.GetRequiredService<TDatabase>() );

        builder.Services.AddFluentMigratorCore().ConfigureRunner( migration );
        return builder;
    }
}
