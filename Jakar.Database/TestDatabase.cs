// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

using Npgsql;
using OpenTelemetry.Exporter;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.Memory;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;



namespace Jakar.Database;


internal sealed class TestDatabase( IConfiguration configuration, IOptions<DbOptions> options, FusionCache cache ) : Database( configuration, options, cache ), IAppName
{
    // private const string CONNECTION_STRING = "Server=localhost;Database=Experiments;User Id=tester;Password=tester;Encrypt=True;TrustServerCertificate=True";


    public static string     AppName    => nameof(TestDatabase);
    public static AppVersion AppVersion { get; } = new(1, 0, 0, 1);


    protected override DbConnection CreateConnection( in SecuredString secure ) => new NpgsqlConnection( secure );


    [Experimental( "SqlTableBuilder" )]
    [Conditional( "DEBUG" )]
    public static async void TestAsync()
    {
        try
        {
            string sql = SqlTableBuilder<GroupRecord>.Create().WithColumn( ColumnMetaData.Nullable( nameof(GroupRecord.CustomerID), DbType.String, GroupRecord.MAX_SIZE ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.NameOfGroup), DbType.String, GroupRecord.MAX_SIZE, $"{nameof(GroupRecord.NameOfGroup)} > 0" ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.Rights), DbType.StringFixedLength, (uint)Enum.GetValues<TestRight>().Length, $"{nameof(GroupRecord.Rights)} > 0" ) ).WithColumn<RecordID<GroupRecord>>( nameof(GroupRecord.ID) ).WithColumn<RecordID<GroupRecord>?>( nameof(GroupRecord.CreatedBy) ).WithColumn<Guid?>( nameof(GroupRecord.CreatedBy) ).WithColumn<DateTimeOffset>( nameof(GroupRecord.DateCreated) ).WithColumn<DateTimeOffset?>( nameof(GroupRecord.LastModified) ).Build();
            sql.WriteToConsole();
            await InternalTestAsync();
        #pragma warning disable RS1035
            Console.ReadKey();
        #pragma warning restore RS1035
        }
        catch ( Exception e )
        {
        #pragma warning disable RS1035
            Console.Error.WriteLine( e );
        #pragma warning restore RS1035
        }
    }
    private static async Task InternalTestAsync()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.AddTelemetry<TestDatabase>( OtlpExportProtocol.Grpc, new Uri( "http://192.168.2.50:6317" ) );
        ConfigureDbServices.Setup( builder );


        await using WebApplication app = builder.Build();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseHttpMetrics();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseTelemetry();
        app.MapGet( "Ping", static () => DateTimeOffset.UtcNow );

        try
        {
            await app.MigrateUpAsync();
            await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
            TestDatabase                  db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
            await TestUsers( db );
            await app.RunAsync();
        }
        finally { await app.MigrateDownAsync(); }
    }
    private static async ValueTask TestUsers( Database db, CancellationToken token = default )
    {
        UserRecord       admin   = UserRecord.Create( "Admin", "Admin", UserRights<TestRight>.SA );
        UserRecord       user    = UserRecord.Create( "User",  "User",  UserRights<TestRight>.Create( [TestRight.Read] ) );
        UserRecord[]     users   = [admin, user];
        List<UserRecord> results = new(users.Length);

        using ( Activity? activity = Telemetry.DbSource.StartActivity( "Users.Insert" ) )
        {
            await foreach ( UserRecord record in db.Users.Insert( users, token ) ) { results.Add( record ); }

            Debug.Assert( users.Length == results.Count );
        }

        using ( Activity? activity = Telemetry.DbSource.StartActivity( "Users.All" ) )
        {
            results.Clear();
            await foreach ( UserRecord record in db.Users.All( token ) ) { results.Add( record ); }

            Debug.Assert( users.Length == results.Count );
        }
    }



    public enum TestRight
    {
        Read,
        Write,
        Delete,
        Admin
    }



    internal sealed class ConfigureDbServices : ConfigureDbServices<ConfigureDbServices, TestDatabase, TestDatabase>
    {
        public override bool UseApplicationCookie => false;
        public override bool UseAuth              => false;
        public override bool UseAuthCookie        => false;
        public override bool UseExternalCookie    => false;
        public override bool UseGoogleAccount     => false;
        public override bool UseMicrosoftAccount  => false;
        public override bool UseOpenIdConnect     => false;
        public override bool UseRedis             => false;


        public ConfigureDbServices()
        {
            SecuredString connectionString = $"User ID=dev;Password=dev;Host=192.168.2.50;Port=5432;Database={AppName}";

            DbOptions = new DbOptions
                        {
                            ConnectionStringResolver = connectionString,
                            CommandTimeout           = 30,
                            TokenIssuer              = AppName,
                            TokenAudience            = AppName,
                            AppName                  = AppName,
                            Version                  = AppVersion
                        }.WithAppName<TestDatabase>();
        }
        protected override void Configure( RedisBackplaneOptions  options ) { }
        protected override void Configure( MemoryBackplaneOptions options ) { }
        protected override void Configure( IFusionCacheBuilder builder )
        {
            base.Configure( builder );

            // builder.builder.InstanceName = TestDatabase.AppName;
            // builder.Configuration        = "192.168.2.50:6379";
        }
    }
}
