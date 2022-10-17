namespace Jakar.Database.Migrations;


/// <summary>
///     Add DB implementation support to FluentMigrator, Set the connection string, and Define the assembly containing the migrations
///     <para>
///         <see href = "https://fluentmigrator.github.io/articles/migration-runners.html?tabs=vs-pkg-manager-console" />
///     </para>
///     <para>
///         <see cref = "PostgresRunnerBuilderExtensions.AddPostgres" />
///     </para>
///     <para>
///         <see cref = "SQLiteRunnerBuilderExtensions.AddSQLite" />
///     </para>
///     <para>
///         <see cref = "SqlServerRunnerBuilderExtensions.AddSqlServer2008" />
///     </para>
///     <para>
///         <see cref = "SqlServerRunnerBuilderExtensions.AddSqlServer2012" />
///     </para>
///     <para>
///         <see cref = "SqlServerRunnerBuilderExtensions.AddSqlServer2014" />
///     </para>
///     <para>
///         <see cref = "SqlServerRunnerBuilderExtensions.AddSqlServer2016" />
///     </para>
///     <para>
///         <see cref = "Db2RunnerBuilderExtensions.AddDb2" />
///     </para>
/// </summary>
public static class Migrator
{
    public static IServiceCollection AddFluentMigrator( this IServiceCollection collection, Func<IMigrationRunnerBuilder, IMigrationRunnerBuilder> addDbType, string connectionString, params Assembly[] assemblies ) =>
        collection.AddFluentMigratorCore()
                  .ConfigureRunner( configure => addDbType( configure )
                                                .WithGlobalConnectionString( connectionString )
                                                .ScanIn( assemblies )
                                                .For.Migrations() )
                  .AddLogging( builder => builder.AddFluentMigratorConsole() );
    public static void UpdateDatabase( Func<IMigrationRunnerBuilder, IMigrationRunnerBuilder> addDbType, string connectionString, params Assembly[] assemblies )
    {
        using ServiceProvider serviceProvider = new ServiceCollection().AddFluentMigrator( addDbType, connectionString, assemblies )
                                                                       .BuildServiceProvider( true );

        serviceProvider.UpdateDatabase();
    }


    /// <summary>
    ///     <para> Put the database update into a scope to ensure that all resources will be disposed. </para>
    ///     <para> Update the database </para>
    /// </summary>
    public static void UpdateDatabase( this IServiceProvider provider )
    {
        using IServiceScope scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<IMigrationRunner>()
             .UpdateDatabase();
    }


    public static void UpdateDatabase( this IMigrationRunner runner ) =>

        // #if DEBUG
        //
        //     if ( Debugger.IsAttached ) { runner.MigrateDown(0); }
        //
        // #endif
        // Execute the migrations
        runner.MigrateUp();
}
