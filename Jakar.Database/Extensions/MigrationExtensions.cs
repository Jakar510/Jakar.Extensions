// Jakar.Extensions :: Jakar.Database
// 1/14/2024  20:9

namespace Jakar.Database;


public static class MigrationExtensions
{
    public static void MigrateUp( this WebApplication app )
    {
        using IServiceScope scope  = app.Services.CreateScope();
        IMigrationRunner    runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();

        if ( runner.HasMigrationsToApplyUp() ) { runner.MigrateUp(); }
    }
    public static async ValueTask MigrateUpAsync( this WebApplication app )
    {
        await using AsyncServiceScope scope  = app.Services.CreateAsyncScope();
        IMigrationRunner              runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();

        if ( runner.HasMigrationsToApplyUp() ) { runner.MigrateUp(); }
    }
    public static void MigrateDown( this WebApplication app, string key, long version = 0 )
    {
        if ( app.Environment.IsProduction() ) { return; }

        if ( app.Configuration.GetValue( key, true ) is false ) { return; }

        using IServiceScope scope  = app.Services.CreateScope();
        IMigrationRunner    runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown( version );
    }
    public static async ValueTask MigrateDownAsync( this WebApplication app, string key = "DB_DOWN", long version = 0 )
    {
        if ( app.Environment.IsProduction() ) { return; }

        if ( app.Configuration.GetValue( key, true ) is false ) { return; }

        await using AsyncServiceScope scope  = app.Services.CreateAsyncScope();
        IMigrationRunner              runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown( version );
    }
}
