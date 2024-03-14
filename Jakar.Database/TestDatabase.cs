// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

using Npgsql;



namespace Jakar.Database;


internal sealed class TestDatabase : Database
{
    // private const string CONNECTION_STRING = "Server=localhost;Database=Experiments;User Id=tester;Password=tester;Encrypt=True;TrustServerCertificate=True";


    internal TestDatabase( IConfiguration                              configuration, ISqlCacheFactory sqlCacheFactory, IOptions<DbOptions> options, IDistributedCache distributedCache, ITableCacheFactory tableCacheFactory ) : base( configuration, sqlCacheFactory, options, distributedCache, tableCacheFactory ) { }
    protected override DbConnection CreateConnection( in SecuredString secure ) => new NpgsqlConnection( secure );


    [Conditional( "DEBUG" )]
    public static async void TestAsync<T>()
        where T : IAppName
    {
        try { await InternalTestAsync<T>(); }
        catch ( Exception e ) { Console.WriteLine( e ); }

        Console.ReadKey();
    }
    private static async Task InternalTestAsync<T>()
        where T : IAppName
    {
        SecuredString         connectionString = $"User ID=dev;Password=jetson;Host=localhost;Port=5432;Database={typeof(T).Name}";
        WebApplicationBuilder builder          = WebApplication.CreateBuilder();

        builder.AddDefaultDbServices<T, TestDatabase>( DbInstance.Postgres,
                                                       connectionString,
                                                       redis =>
                                                       {
                                                           redis.InstanceName  = typeof(T).Name;
                                                           redis.Configuration = "localhost:6379";
                                                       } );

        await using WebApplication app = builder.Build();

        try
        {
            await app.MigrateUpAsync();

            await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
            var                           db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
            await TestUsers( db );
        }
        finally { await app.MigrateDownAsync(); }
    }
    private static async ValueTask TestUsers( Database db, CancellationToken token = default )
    {
        using var adminRights = UserRights<TestRight>.Create();
        var       admin       = UserRecord.Create( "Admin", "Admin", adminRights );
        using var userRights  = UserRights<TestRight>.Create( TestRight.Read );
        var       user        = UserRecord.Create( "User", "User", userRights, admin );

        UserRecord[] users = [admin, user];

        var results = new List<UserRecord>( users.Length );
        await foreach ( UserRecord record in db.Users.Insert( users, token ) ) { results.Add( record ); }

        Debug.Assert( users.Length == results.Count );

        results.Clear();
        await foreach ( UserRecord record in db.Users.All( token ) ) { results.Add( record ); }

        Debug.Assert( users.Length == results.Count );
    }



    public enum TestRight
    {
        Read,
        Write,
        Delete,
        Admin
    }
}
