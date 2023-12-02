// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

using System.Collections.Frozen;
using Npgsql;



namespace Jakar.Database;


#if DEBUG



public sealed class TestDatabase : Database
{
    // private const string CONNECTION_STRING = "Server=localhost;Database=Experiments;User Id=tester;Password=tester;Encrypt=True;TrustServerCertificate=True";
    private const string CONNECTION_STRING = "User ID=dev;Password=jetson;Host=localhost;Port=5432;Database=Experiments";


    public TestDatabase( IConfiguration                                configuration, ISqlCacheFactory sqlCacheFactory, IOptions<DbOptions> options ) : base( configuration, sqlCacheFactory, options ) => ConnectionString = CONNECTION_STRING;
    protected override DbConnection CreateConnection( in SecuredString secure ) => new NpgsqlConnection( secure );


    public static async Task Test()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.AddDefaultLogging<TestDatabase>( true );

        builder.AddDb<TestDatabase>( dbOptions =>
                                     {
                                         SecuredString secured = CONNECTION_STRING;
                                         dbOptions.ConnectionString = secured;
                                         dbOptions.DbType           = DbInstance.Postgres;
                                         dbOptions.TokenAudience    = nameof(TestDatabase);
                                         dbOptions.TokenIssuer      = nameof(TestDatabase);
                                     },
                                     DbHostingExtensions.ConfigureMigrationsPostgres );


        await using WebApplication app = builder.Build();

        try
        {
            await using ( AsyncServiceScope scope = app.Services.CreateAsyncScope() )
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.ListMigrations();

                if ( runner.HasMigrationsToApplyUp() ) { runner.MigrateUp(); }
            }


            await using ( AsyncServiceScope scope = app.Services.CreateAsyncScope() )
            {
                var               db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
                CancellationToken token = default;
                var               admin = UserRecord.Create( "Admin", "Admin", string.Empty );
                var               user  = UserRecord.Create( "User",  "User",  string.Empty, admin );

                UserRecord[] users = {
                                         admin,
                                         user
                                     };

                var results = new List<UserRecord>( users.Length );
                await foreach ( UserRecord record in db.Users.Insert( users, token ) ) { results.Add( record ); }

                Debug.Assert( users.Length == results.Count );

                results.Clear();
                await foreach ( UserRecord record in db.Users.All( token ) ) { results.Add( record ); }

                Debug.Assert( users.Length == results.Count );
            }
        }
        finally
        {
        #if DEBUG
            if ( app.Configuration.GetValue( "DB_DOWN", true ) )
            {
                await using AsyncServiceScope scope  = app.Services.CreateAsyncScope();
                var                           runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateDown( 0 );
            }
        #endif
        }
    }
}



#endif
