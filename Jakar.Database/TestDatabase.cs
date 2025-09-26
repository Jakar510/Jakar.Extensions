// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

namespace Jakar.Database;


internal sealed class TestDatabase( IConfiguration configuration, IOptions<DbOptions> options, FusionCache cache ) : Database(configuration, options, cache), IAppID
{
    public static Guid       AppID      { get; } = Guid.NewGuid();
    public static string     AppName    => nameof(TestDatabase);
    public static AppVersion AppVersion { get; } = new(1, 0, 0, 1);


    protected override NpgsqlConnection CreateConnection( in SecuredString secure ) => new(secure);


    [Experimental("SqlTableBuilder")][Conditional("DEBUG")]
    public static async void TestAsync()
    {
        try
        {
            string sql = SqlTableBuilder<GroupRecord>.Create()
                                                     .WithColumn(ColumnMetaData.Nullable(nameof(GroupRecord.CustomerID), DbType.String, GroupRecord.MAX_SIZE))
                                                     .WithColumn(ColumnMetaData.NotNullable(nameof(GroupRecord.NameOfGroup), DbType.String,            GroupRecord.MAX_SIZE,                     $"{nameof(GroupRecord.NameOfGroup)} > 0"))
                                                     .WithColumn(ColumnMetaData.NotNullable(nameof(GroupRecord.Rights),      DbType.StringFixedLength, (uint)Enum.GetValues<TestRight>().Length, $"{nameof(GroupRecord.Rights)} > 0"))
                                                     .WithColumn<RecordID<GroupRecord>>(nameof(GroupRecord.ID))
                                                     .WithColumn<RecordID<GroupRecord>?>(nameof(GroupRecord.CreatedBy))
                                                     .WithColumn<Guid?>(nameof(GroupRecord.CreatedBy))
                                                     .WithColumn<DateTimeOffset>(nameof(GroupRecord.DateCreated))
                                                     .WithColumn<DateTimeOffset?>(nameof(GroupRecord.LastModified))
                                                     .Build();

            sql.WriteToConsole();
            await InternalTestAsync();

        #pragma warning disable RS1035
            Console.ReadKey();
        #pragma warning restore RS1035
        }
        catch ( Exception e )
        {
        #pragma warning disable RS1035
            Console.Error.WriteLine(e);
        #pragma warning restore RS1035
        }
    }
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

        try
        {
            await app.MigrateUpAsync();
            await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
            TestDatabase                  db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
            await TestUsers(db);
            await app.RunAsync();
        }
        finally { await app.MigrateDownAsync(); }
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
