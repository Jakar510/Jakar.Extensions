// Jakar.Extensions :: Jakar.Database
// 1/14/2024  20:9

namespace Jakar.Database;


public static class Migrations
{
    public static bool HasFlagValue( this ColumnOptions type, ColumnOptions flag ) => ( type & flag ) != 0;


    public static string CreateTableSql<TSelf>()
        where TSelf : class, ITableRecord<TSelf> => SqlTableBuilder<TSelf>.Create()
                                                                             .Build();


    public static IEnumerable<MigrationRecord> BuiltIns()
    {
        ulong migrationID = 1;
        yield return MigrationRecord.CreateTable(migrationID++);

        yield return MigrationRecord.SetLastModified(migrationID++);

        yield return MigrationRecord.FromEnum<MimeType>(migrationID++);
        yield return MigrationRecord.FromEnum<SupportedLanguage>(migrationID++);
        yield return MigrationRecord.FromEnum<SubscriptionStatus>(migrationID++);
        yield return MigrationRecord.FromEnum<DeviceCategory>(migrationID++);
        yield return MigrationRecord.FromEnum<DevicePlatform>(migrationID++);
        yield return MigrationRecord.FromEnum<DeviceTypes>(migrationID++);
        yield return MigrationRecord.FromEnum<DistanceUnit>(migrationID++);
        yield return MigrationRecord.FromEnum<ProgrammingLanguage>(migrationID++);
        yield return MigrationRecord.FromEnum<Status>(migrationID++);

        yield return FileRecord.CreateTable(migrationID++);

        yield return UserRecord.CreateTable(migrationID++);

        yield return RecoveryCodeRecord.CreateTable(migrationID++);
        yield return UserRecoveryCodeRecord.CreateTable(migrationID++);

        yield return RoleRecord.CreateTable(migrationID++);
        yield return UserRoleRecord.CreateTable(migrationID++);

        yield return GroupRecord.CreateTable(migrationID++);
        yield return UserGroupRecord.CreateTable(migrationID++);

        yield return AddressRecord.CreateTable(migrationID++);
        yield return UserAddressRecord.CreateTable(migrationID);
    }


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

        if ( !app.Configuration.GetValue(key, true) ) { return; }

        using IServiceScope scope  = app.Services.CreateScope();
        IMigrationRunner    runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(version);
    }
    public static async ValueTask MigrateDownAsync( this WebApplication app, string key = "DB_DOWN", long version = 0 )
    {
        if ( app.Environment.IsProduction() ) { return; }

        if ( !app.Configuration.GetValue(key, true) ) { return; }

        await using AsyncServiceScope scope  = app.Services.CreateAsyncScope();
        IMigrationRunner              runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(version);
    }
}
