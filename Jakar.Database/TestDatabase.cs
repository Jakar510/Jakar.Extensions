// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

namespace Jakar.Database;


internal sealed class TestDatabase( IConfiguration configuration, IOptions<DbOptions> options, FusionCache cache ) : Database(configuration, options, cache), IAppID
{
    public static Guid       AppID      { get; } = Guid.NewGuid();
    public static string     AppName    => nameof(TestDatabase);
    public static AppVersion AppVersion { get; } = new(1, 0, 0, 1);


    protected override NpgsqlConnection CreateConnection( in SecuredString secure ) => new(secure);


    private static async Task InternalTestAsync()
    {
        WebApplicationBuilder builder          = WebApplication.CreateBuilder();
        SecuredString         connectionString = $"User ID=dev;Password=dev;Host=192.168.2.50;Port=5432;Database={AppName}";

        builder.AddDatabase<TestDatabase>(new DbOptions
                                          {
                                              ConnectionStringResolver = connectionString,
                                              CommandTimeout           = 30,
                                              TokenIssuer              = AppName,
                                              TokenAudience            = AppName
                                          });


        await using WebApplication app = builder.Build();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseHttpMetrics();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseTelemetry();
        app.MapGet("Ping", static () => DateTimeOffset.UtcNow);

        await app.ApplyMigrations();
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        TestDatabase                  db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
        await TestUsers(db);
        await app.RunAsync();
    }
    private static async ValueTask TestUsers( Database db, CancellationToken token = default )
    {
        UserRecord       admin   = UserRecord.Create("Admin", "Admin", UserRights<TestRight>.SA);
        UserRecord       user    = UserRecord.Create("User",  "User",  UserRights<TestRight>.Create(TestRight.Read));
        UserRecord[]     users   = [admin, user];
        List<UserRecord> results = new(users.Length);

        using ( Activity? activity = Telemetry.DbSource.StartActivity("Users.Insert") )
        {
            await foreach ( UserRecord record in db.Users.Insert(users, token) ) { results.Add(record); }

            Debug.Assert(users.Length == results.Count);
        }

        using ( Activity? activity = Telemetry.DbSource.StartActivity("Users.All") )
        {
            results.Clear();
            await foreach ( UserRecord record in db.Users.All(token) ) { results.Add(record); }

            Debug.Assert(users.Length == results.Count);
        }
    }



    public enum TestRight
    {
        Read,
        Write,
        Delete,
        Admin
    }
}
