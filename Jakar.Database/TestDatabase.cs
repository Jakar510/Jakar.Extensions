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
        Console.WriteLine( SqlTableBuilder<GroupRecord>.Create()
                                                       .WithColumn( ColumnMetaData.Nullable( nameof(GroupRecord.CustomerID), DbType.String, GroupRecord.MAX_SIZE ) )
                                                       .WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.NameOfGroup), DbType.String,            GroupRecord.MAX_SIZE,                     $"{nameof(GroupRecord.NameOfGroup)} > 0" ) )
                                                       .WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.Rights),      DbType.StringFixedLength, (uint)Enum.GetValues<TestRight>().Length, $"{nameof(GroupRecord.Rights)} > 0" ) )
                                                       .WithColumn<RecordID<GroupRecord>>( nameof(GroupRecord.ID) )
                                                       .WithColumn<RecordID<GroupRecord>?>( nameof(GroupRecord.CreatedBy) )
                                                       .WithColumn<Guid?>( nameof(GroupRecord.OwnerUserID) )
                                                       .WithColumn<DateTimeOffset>( nameof(GroupRecord.DateCreated) )
                                                       .WithColumn<DateTimeOffset?>( nameof(GroupRecord.LastModified) )
                                                       .Build( DbTypeInstance.Postgres ) );

        Console.WriteLine();

        Console.WriteLine( SqlTableBuilder<GroupRecord>.Create()
                                                       .WithColumn( ColumnMetaData.Nullable( nameof(GroupRecord.CustomerID), DbType.String, GroupRecord.MAX_SIZE ) )
                                                       .WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.NameOfGroup), DbType.String,            GroupRecord.MAX_SIZE,                     $"{nameof(GroupRecord.NameOfGroup)} > 0" ) )
                                                       .WithColumn( ColumnMetaData.NotNullable( nameof(GroupRecord.Rights),      DbType.StringFixedLength, (uint)Enum.GetValues<TestRight>().Length, $"{nameof(GroupRecord.Rights)} > 0" ) )
                                                       .WithColumn<RecordID<GroupRecord>>( nameof(GroupRecord.ID) )
                                                       .WithColumn<RecordID<GroupRecord>?>( nameof(GroupRecord.CreatedBy) )
                                                       .WithColumn<Guid?>( nameof(GroupRecord.OwnerUserID) )
                                                       .WithColumn<DateTimeOffset>( nameof(GroupRecord.DateCreated) )
                                                       .WithColumn<DateTimeOffset?>( nameof(GroupRecord.LastModified) )
                                                       .Build( DbTypeInstance.MsSql ) );

        Console.WriteLine();

        // try { await InternalTestAsync<T>(); }
        // catch ( Exception e ) { Console.WriteLine( e ); }

        Console.ReadKey();
    }
    private static async Task InternalTestAsync<T>()
        where T : IAppName
    {
        SecuredString         connectionString = $"User ID=dev;Password=jetson;Host=localhost;Port=5432;Database={typeof(T).Name}";
        WebApplicationBuilder builder          = WebApplication.CreateBuilder();

        builder.AddDefaultDbServices<T, TestDatabase>( DbTypeInstance.Postgres,
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
            TestDatabase                           db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
            await TestUsers( db );
        }
        finally { await app.MigrateDownAsync(); }
    }
    private static async ValueTask TestUsers( Database db, CancellationToken token = default )
    {
        UserRecord admin = UserRecord.Create( "Admin", "Admin", UserRights<TestRight>.SA );
        UserRecord user  = UserRecord.Create( "User",  "User",  UserRights<TestRight>.Create( [TestRight.Read] ) );

        UserRecord[]     users   = [admin, user];
        List<UserRecord> results = new(users.Length);
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
