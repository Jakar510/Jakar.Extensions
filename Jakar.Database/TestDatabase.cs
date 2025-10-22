// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

using StackExchange.Redis;



namespace Jakar.Database;


internal sealed class TestDatabase( IConfiguration configuration, IOptions<DbOptions> options, FusionCache cache ) : Database(configuration, options, cache), IAppID
{
    public static Guid       AppID      { get; } = Guid.NewGuid();
    public static string     AppName    => nameof(TestDatabase);
    public static AppVersion AppVersion { get; } = new(1, 0, 0, 1);


    protected override NpgsqlConnection CreateConnection( in SecuredString secure ) => new(secure);


    public static async Task TestAsync()
    {
        WebApplicationBuilder        builder          = WebApplication.CreateBuilder();
        SecuredStringResolverOptions connectionString = $"User ID=dev;Password=dev;Host=localhost;Port=5432;Database={AppName}";

        DbOptions options = new()
                            {
                                ConnectionStringResolver = connectionString,
                                CommandTimeout           = 30,
                                TokenIssuer              = AppName,
                                TokenAudience            = AppName
                            };

        builder.AddDatabase<TestDatabase>(options);

        await using WebApplication app = builder.Build();

        app.UseDefaults();

        app.MapGet("/",     static () => DateTimeOffset.UtcNow);
        app.MapGet("/Ping", static () => DateTimeOffset.UtcNow);

        await app.ApplyMigrations();

        await TestAll(app);

        await app.RunAsync();
    }


    private static async ValueTask TestAll( WebApplication app, CancellationToken token = default )
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        TestDatabase                  db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
        ( UserRecord admin, UserRecord user ) = await Add_Users(db, token);
        ( RoleRecord Admin, RoleRecord User ) = await Add_Roles(db, admin, token);
    }
    private static async ValueTask<(RoleRecord Admin, RoleRecord User)> Add_Roles( Database db, UserRecord adminUser, CancellationToken token = default )
    {
        RoleRecord admin = RoleRecord.Create("Admin", Permissions<TestRight>.SA(),                   "Admins", adminUser);
        RoleRecord user  = RoleRecord.Create("User",  Permissions<TestRight>.Create(TestRight.Read), "Users",  adminUser);
        return ( await db.Roles.Insert(admin, token), await db.Roles.Insert(user, token) );
    }
    private static async ValueTask<(UserRecord Admin, UserRecord User)> Add_Users( Database db, CancellationToken token = default )
    {
        UserRecord admin = UserRecord.Create("Admin", "Admin", Permissions<TestRight>.SA());
        UserRecord user  = UserRecord.Create("User",  "User",  Permissions<TestRight>.Create(TestRight.Read));

        using ( Telemetry.DbSource.StartActivity("Users.Add.SU") ) { admin = await db.Users.Insert(admin, token); }

        using ( Telemetry.DbSource.StartActivity("Users.Add.User") ) { user = await db.Users.Insert(user, token); }

        return ( admin, user );
    }



    public enum TestRight
    {
        Admin,
        Read,
        Write,
    }
}
