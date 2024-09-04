// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

using Npgsql;



namespace Jakar.Database;


internal sealed class TestDatabase : Database, IAppName
{
    // private const string CONNECTION_STRING = "Server=localhost;Database=Experiments;User Id=tester;Password=tester;Encrypt=True;TrustServerCertificate=True";


    public static string     AppName    => nameof(TestDatabase);
    public static AppVersion AppVersion { get; } = new(1, 0, 0, 1);


    internal TestDatabase( IConfiguration configuration, ISqlCacheFactory sqlCacheFactory, ITableCache tableCache, IOptions<DbOptions> options ) : base( configuration, sqlCacheFactory, tableCache, options ) { }

    protected override DbConnection CreateConnection( in SecuredString secure ) => new NpgsqlConnection( secure );


    [Conditional( "DEBUG" )]
    public static async void TestAsync()
    {
        Console.WriteLine( SqlTableBuilder<GroupRecord>.Create().WithColumn( ColumnMetaData.Nullable( nameof(GroupRecord.CustomerID), DbType.String, GroupRecord.MAX_SIZE ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.NameOfGroup), DbType.String, GroupRecord.MAX_SIZE, $"{nameof(GroupRecord.NameOfGroup)} > 0" ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.Rights), DbType.StringFixedLength, (uint)Enum.GetValues<TestRight>().Length, $"{nameof(GroupRecord.Rights)} > 0" ) ).WithColumn<RecordID<GroupRecord>>( nameof(GroupRecord.ID) ).WithColumn<RecordID<GroupRecord>?>( nameof(GroupRecord.CreatedBy) ).WithColumn<Guid?>( nameof(GroupRecord.CreatedBy) ).WithColumn<DateTimeOffset>( nameof(GroupRecord.DateCreated) ).WithColumn<DateTimeOffset?>( nameof(GroupRecord.LastModified) ).Build( DbTypeInstance.Postgres ) );
        Console.WriteLine();

        Console.WriteLine( SqlTableBuilder<GroupRecord>.Create().WithColumn( ColumnMetaData.Nullable( nameof(GroupRecord.CustomerID), DbType.String, GroupRecord.MAX_SIZE ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.NameOfGroup), DbType.String, GroupRecord.MAX_SIZE, $"{nameof(GroupRecord.NameOfGroup)} > 0" ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.Rights), DbType.StringFixedLength, (uint)Enum.GetValues<TestRight>().Length, $"{nameof(GroupRecord.Rights)} > 0" ) ).WithColumn<RecordID<GroupRecord>>( nameof(GroupRecord.ID) ).WithColumn<RecordID<GroupRecord>?>( nameof(GroupRecord.CreatedBy) ).WithColumn<Guid?>( nameof(GroupRecord.CreatedBy) ).WithColumn<DateTimeOffset>( nameof(GroupRecord.DateCreated) ).WithColumn<DateTimeOffset?>( nameof(GroupRecord.LastModified) ).Build( DbTypeInstance.MsSql ) );
        Console.WriteLine();

        try { await InternalTestAsync(); }
        catch ( Exception e ) { Console.WriteLine( e ); }

        Console.ReadKey();
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
        app.MapGet( "Ping", () => DateTimeOffset.UtcNow );

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
            await foreach ( UserRecord record in db.Users.Insert(  users, token ) ) { results.Add( record ); }

            Debug.Assert( users.Length == results.Count );
        }

        using ( Activity? activity = Telemetry.DbSource.StartActivity( "Users.All" ) )
        {
            results.Clear();
            await foreach ( UserRecord record in db.Users.All(  token ) ) { results.Add( record ); }

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



    internal sealed class ConfigureDbServices : ConfigureDbServices<ConfigureDbServices, TestDatabase, TestDatabase, SqlCacheFactory, TableCacheFactory>
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
                            DbTypeInstance           = DbTypeInstance.Postgres,
                            CommandTimeout           = 30,
                            TokenIssuer              = AppName,
                            TokenAudience            = AppName,
                            AppName                  = AppName,
                            Version                  = AppVersion
                        }.WithAppName<TestDatabase>();
        }
        public override void Redis( RedisCacheOptions options )
        {
            options.InstanceName  = TestDatabase.AppName;
            options.Configuration = "192.168.2.50:6379";
        }
    }
}
