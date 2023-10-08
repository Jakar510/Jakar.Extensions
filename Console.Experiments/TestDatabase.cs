// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

using System.Collections.Immutable;
using System.Data.Common;
using System.Security;
using FluentMigrator.Runner;
using Jakar.Database;
using Jakar.Database.DbMigrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;



namespace Experiments;


public sealed class TestDatabase : Database
{
    private const string CONNECTION_STRING = "Server=localhost;Database=Experiments;User Id=tester;Password=tester;Encrypt=True;TrustServerCertificate=True";

    public TestDatabase( IConfiguration                            configuration, IOptions<DbOptions> options ) : base( configuration, options ) => ConnectionString = CONNECTION_STRING.ToSecureString();
    protected override DbConnection CreateConnection( in SecureString secure ) => new SqlConnection( secure.GetValue() );


    public static async Task Test()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddDefaultLogging<TestDatabase>( true );

        builder.Services.AddOptions<DbOptions>()
               .Configure( options => { options.DbType = DbInstance.MsSql; } );

        builder.Services.AddSingleton<TestDatabase>();
        builder.Services.AddTransient<Database>( provider => provider.GetRequiredService<TestDatabase>() );


        builder.Services.AddFluentMigratorCore()
               .ConfigureRunner( configure =>
                                 {
                                     // configure.AddPostgres();
                                     configure.AddSqlServer2016();
                                     DbOptions.GetConnectionString( configure );

                                     configure.ScanIn( typeof(Database).Assembly, typeof(Program).Assembly )
                                              .For.All();
                                 } );

        await using WebApplication app = builder.Build();

        try
        {
            await using ( AsyncServiceScope scope = app.Services.CreateAsyncScope() )
            {
                IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.ListMigrations();

                if ( runner.HasMigrationsToApplyUp() ) { runner.MigrateUp(); }
            }


            await using ( AsyncServiceScope scope = app.Services.CreateAsyncScope() )
            {
                TestDatabase      db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
                CancellationToken token = default;
                var               admin = UserRecord.Create( "Admin", "Admin", string.Empty );
                var               user  = UserRecord.Create( "User",  "User",  string.Empty, admin );

                ImmutableList<UserRecord> users   = ImmutableList.Create( admin, user );
                var                       results = new List<UserRecord>( users.Count );

                await foreach ( var record in db.Users.Insert( users, token ) ) { results.Add( record ); }

                Debug.Assert( users.Count == results.Count );

                results.Clear();
                await foreach ( var record in db.Users.All( token ) ) { results.Add( record ); }

                Debug.Assert( users.Count == results.Count );
            }
        }
        finally
        {
        #if DEBUG
            if ( app.Configuration.GetValue( "DISPATCH_DOWN", true ) )
            {
                await using AsyncServiceScope scope  = app.Services.CreateAsyncScope();
                IMigrationRunner              runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateDown( 0 );
            }
        #endif
        }
    }
}
