// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

using Npgsql;



namespace Jakar.Database;


internal sealed class TestDatabase<TApp> : Database
    where TApp : IAppName
{
    // private const string CONNECTION_STRING = "Server=localhost;Database=Experiments;User Id=tester;Password=tester;Encrypt=True;TrustServerCertificate=True";


    internal TestDatabase( IConfiguration configuration, ISqlCacheFactory sqlCacheFactory, ITableCache tableCache, IOptions<DbOptions> options ) : base( configuration, sqlCacheFactory, tableCache, options ) { }

    protected override DbConnection CreateConnection( in SecuredString secure ) => new NpgsqlConnection( secure );


    [Conditional( "DEBUG" )]
    public static async void TestAsync( string user, string password )
    {
        Console.WriteLine( SqlTableBuilder<GroupRecord>.Create().WithColumn( ColumnMetaData.Nullable( nameof(GroupRecord.CustomerID), DbType.String, GroupRecord.MAX_SIZE ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.NameOfGroup), DbType.String, GroupRecord.MAX_SIZE, $"{nameof(GroupRecord.NameOfGroup)} > 0" ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.Rights), DbType.StringFixedLength, (uint)Enum.GetValues<TestRight>().Length, $"{nameof(GroupRecord.Rights)} > 0" ) ).WithColumn<RecordID<GroupRecord>>( nameof(GroupRecord.ID) ).WithColumn<RecordID<GroupRecord>?>( nameof(GroupRecord.OwnerUserID) ).WithColumn<Guid?>( nameof(GroupRecord.OwnerUserID) ).WithColumn<DateTimeOffset>( nameof(GroupRecord.DateCreated) ).WithColumn<DateTimeOffset?>( nameof(GroupRecord.LastModified) ).Build( DbTypeInstance.Postgres ) );
        Console.WriteLine();

        Console.WriteLine( SqlTableBuilder<GroupRecord>.Create().WithColumn( ColumnMetaData.Nullable( nameof(GroupRecord.CustomerID), DbType.String, GroupRecord.MAX_SIZE ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.NameOfGroup), DbType.String, GroupRecord.MAX_SIZE, $"{nameof(GroupRecord.NameOfGroup)} > 0" ) ).WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.Rights), DbType.StringFixedLength, (uint)Enum.GetValues<TestRight>().Length, $"{nameof(GroupRecord.Rights)} > 0" ) ).WithColumn<RecordID<GroupRecord>>( nameof(GroupRecord.ID) ).WithColumn<RecordID<GroupRecord>?>( nameof(GroupRecord.OwnerUserID) ).WithColumn<Guid?>( nameof(GroupRecord.OwnerUserID) ).WithColumn<DateTimeOffset>( nameof(GroupRecord.DateCreated) ).WithColumn<DateTimeOffset?>( nameof(GroupRecord.LastModified) ).Build( DbTypeInstance.MsSql ) );
        Console.WriteLine();

        try { await InternalTestAsync( user, password ); }
        catch ( Exception e ) { Console.WriteLine( e ); }

        Console.ReadKey();
    }
    private static async Task InternalTestAsync( string user, string password )
    {
        WebApplicationBuilder builder  = WebApplication.CreateBuilder();
        builder.AddDbServices( new ConfigureDbServices(user, password) );

        await using WebApplication app = builder.Build();

        try
        {
            await app.MigrateUpAsync();

            await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
            TestDatabase<TApp>            db    = scope.ServiceProvider.GetRequiredService<TestDatabase<TApp>>();
            await TestUsers( Activity.Current, db );
        }
        finally { await app.MigrateDownAsync(); }
    }
    private static async ValueTask TestUsers( Activity? activity, Database db, CancellationToken token = default )
    {
        UserRecord admin = UserRecord.Create( "Admin", "Admin", UserRights<TestRight>.SA );
        UserRecord user  = UserRecord.Create( "User",  "User",  UserRights<TestRight>.Create( [TestRight.Read] ) );

        UserRecord[]     users   = [admin, user];
        List<UserRecord> results = new(users.Length);
        await foreach ( UserRecord record in db.Users.Insert( activity, users, token ) ) { results.Add( record ); }

        Debug.Assert( users.Length == results.Count );

        results.Clear();
        await foreach ( UserRecord record in db.Users.All( activity, token ) ) { results.Add( record ); }

        Debug.Assert( users.Length == results.Count );
    }



    public enum TestRight
    {
        Read,
        Write,
        Delete,
        Admin
    }



    internal sealed class ConfigureDbServices : ConfigureDbServices<TApp, TestDatabase<TApp>, SqlCacheFactory, TableCacheFactory>
    {
        public override bool UseApplicationCookie => false;
        public override bool UseAuth              => false;
        public override bool UseAuthCookie        => false;
        public override bool UseExternalCookie    => false;
        public override bool UseGoogleAccount     => false;
        public override bool UseMicrosoftAccount  => false;
        public override bool UseOpenIdConnect     => false;
        public override bool UseRedis             => false;


        public ConfigureDbServices( string user, string password )
        {
            SecuredString connectionString = $"User ID={user};Password={password};Host=localhost;Port=5432;Database={AppName}";

            DbOptions = new DbOptions
                        {
                            ConnectionStringResolver = connectionString,
                            DbTypeInstance           = DbTypeInstance.Postgres,
                            CommandTimeout           = 30,
                            TokenIssuer              = AppName,
                            TokenAudience            = AppName,
                            AppName                  = AppName
                        };
        }
        public override void Redis( RedisCacheOptions options )
        {
            options.InstanceName  = typeof(TApp).Name;
            options.Configuration = "localhost:6379";
        }
    }
}
