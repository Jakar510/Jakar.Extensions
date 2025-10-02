// Jakar.Extensions :: Jakar.Database
// 1/14/2024  20:9

namespace Jakar.Database;


public static class Migrations
{
    public static bool HasFlagValue( this ColumnOptions type, ColumnOptions flag ) => ( type & flag ) != 0;


    public static string CreateTableSql<TClass>()
        where TClass : class, ITableRecord<TClass> => SqlTableBuilder<TClass>.Create()
                                                                             .Build();


    public static IEnumerable<MigrationRecord> BuiltIns()
    {
        yield return MigrationRecord.Create<MigrationRecord>(0,
                                                             DateTimeOffset.Now,
                                                             "create migration table",
                                                             $"""
                                                              CREATE TABLE IF NOT EXISTS {MigrationRecord.TABLE_NAME.SqlColumnName()}
                                                              (
                                                              migration_id    bigint        NOT NULL,
                                                              table_id        varchar(256)  NOT NULL,
                                                              description     varchar(4096) NOT NULL,
                                                              date_created    timestamptz   NOT NULL DEFAULT SYSUTCDATETIME(),
                                                              additional_data json          NULL
                                                              PRIMARY KEY(table_id, migration_id)
                                                              );
                                                              """);

        yield return new MigrationRecord("create set_last_modified function", MigrationRecord.TABLE_NAME, 1, DateTimeOffset.Now)
                     {
                         SQL = $"""
                                CREATE OR REPLACE FUNCTION set_last_modified()
                                RETURNS TRIGGER AS $$
                                BEGIN
                                    NEW.{nameof(ILastModified.LastModified).SqlColumnName()} = now();
                                    RETURN NEW;
                                END;
                                $$ LANGUAGE plpgsql;
                                """
                     };

        yield return MigrationRecord.FromEnum<MimeType>(2, DateTimeOffset.Now);

        yield return MigrationRecord.Create<MigrationRecord>(3,
                                                             DateTimeOffset.Now,
                                                             "create mime_types table",
                                                             $"""
                                                              CREATE TABLE IF NOT EXISTS {nameof(MimeType).SqlColumnName()}
                                                              (
                                                              id    bigint        PRIMARY KEY,
                                                              name  varchar(256)  UNIQUE NOT NULL,
                                                              );

                                                              -- Insert values if they do not exist

                                                              -- Insert values with explicit ids (enum order)
                                                              INSERT INTO {nameof(MimeType).SqlColumnName()} (id, name)
                                                              SELECT v.id, v.name
                                                              FROM (VALUES
                                                                 {string.Join(",\n", Enum.GetValues<MimeType>().Select(( v, i ) => $"    ({i}, '{v}')"))}
                                                              ) AS v(id, name)
                                                              WHERE NOT EXISTS (
                                                              SELECT 1 FROM mime_types m WHERE m.id = v.id OR m.name = v.name
                                                              );
                                                              );
                                                              """);

        yield return MigrationRecord.Create<AddressRecord>(4,
                                                           DateTimeOffset.Now,
                                                           "create address table",
                                                           $"""
                                                            CREATE TABLE IF NOT EXISTS {AddressRecord.TableName}
                                                            (
                                                            id                uuid           NOT NULL PRIMARY KEY,
                                                            line1             varchar(512)   NOT NULL,
                                                            line2             varchar(512)   NOT NULL,
                                                            city              varchar(512)   NOT NULL,
                                                            state_or_province varchar(512)   NOT NULL,
                                                            country           varchar(512)   NOT NULL,
                                                            postal_code       varchar(64)    NOT NULL,
                                                            address           varchar(3000)  NULL,
                                                            is_primary        boolean        NOT NULL DEFAULT FALSE,
                                                            created_by        uuid           NULL,
                                                            date_created      timestamptz    NOT NULL DEFAULT SYSUTCDATETIME(),
                                                            last_modified     timestamptz    NULL,
                                                            additional_data   json           NULL,
                                                            FOREIGN KEY(created_by) REFERENCES users(id) ON DELETE SET NULL
                                                            );

                                                            CREATE TRIGGER update_last_modified
                                                            BEFORE INSERT OR UPDATE ON {AddressRecord.TableName}
                                                            FOR EACH ROW
                                                            EXECUTE FUNCTION set_last_modified();
                                                            """);

        yield return MigrationRecord.Create<FileRecord>(5,
                                                        DateTimeOffset.Now,
                                                        "create file table",
                                                        $"""
                                                         CREATE TABLE IF NOT EXISTS {FileRecord.TABLE_NAME}
                                                         (
                                                         file_name        varchar(256)                NULL UNIQUE,
                                                         file_description varchar({UNICODE_CAPACITY}) NULL,
                                                         file_type        varchar(256)                NULL,
                                                         full_path        varchar({UNICODE_CAPACITY}) NULL UNIQUE,
                                                         file_size        bigint                      NOT NULL,
                                                         hash             varchar({UNICODE_CAPACITY}) NOT NULL,
                                                         mime_type        varchar(256)                NULL,
                                                         payload          text                        NOT NULL,
                                                         id               uuid                        NOT NULL PRIMARY KEY,
                                                         date_created     timestamptz                 NOT NULL DEFAULT SYSUTCDATETIME(),
                                                         last_modified    timestamptz                 NULL,
                                                         FOREIGN KEY(mime_type) REFERENCES mime_types(id) ON DELETE SET NULL
                                                         );


                                                         CREATE TRIGGER update_last_modified
                                                         BEFORE INSERT OR UPDATE ON {FileRecord.TableName}
                                                         FOR EACH ROW
                                                         EXECUTE FUNCTION set_last_modified();
                                                         """);
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
