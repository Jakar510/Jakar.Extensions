// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

namespace Jakar.Database;


internal sealed class TestDatabase( IConfiguration configuration, IOptions<DbOptions> options, FusionCache cache ) : Database(configuration, options, cache), IAppID
{
    public static Guid       AppID      { get; } = Guid.NewGuid();
    public static string     AppName    => nameof(TestDatabase);
    public static AppVersion AppVersion { get; } = new(1, 0, 0, 1);


    protected override NpgsqlConnection CreateConnection( in SecuredString secure ) => new(secure);


    public static async Task TestAsync( CancellationToken token = default )
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

        await app.ApplyMigrations(token);

        await TestAll(app, token);

        await app.StartAsync(token)
                 .ConfigureAwait(false);

        await app.WaitForShutdownAsync(token)
                 .ConfigureAwait(false);
    }


    private static async ValueTask TestAll( WebApplication app, CancellationToken token = default )
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        TestDatabase                  db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
        ( UserRecord admin, UserRecord user )             = await Add_Users(db, token);
        ( RoleRecord adminRole, RoleRecord userRole )     = await Add_Roles(db, admin, token);
        ( GroupRecord adminGroup, GroupRecord userGroup ) = await Add_Group(db, admin, token);
        UserRoleRecord[]  userRoles  = await Add_Roles(db, user, [adminRole, userRole],   token);
        UserGroupRecord[] userGroups = await Add_Roles(db, user, [adminGroup, userGroup], token);
        ( AddressRecord address, UserAddressRecord userAddress ) = await Add_Address(db, user, token);
        FileRecord              file          = await Add_File(db, user, token);
        UserLoginProviderRecord loginProvider = await Add_UserLoginProvider(db, user, token);
        ( RecoveryCodeRecord[] recoveryCodes, UserRecoveryCodeRecord[] userRecoveryCodes ) = await Add_RecoveryCodes(db, user, token);
    }
    private static async ValueTask<(RecoveryCodeRecord[] records, UserRecoveryCodeRecord[] results)> Add_RecoveryCodes( Database db, UserRecord user, CancellationToken token = default )
    {
        RecoveryCodeRecord.Codes codes = RecoveryCodeRecord.Create(user, 10);

        RecoveryCodeRecord[] records = await db.RecoveryCodes.Insert(codes.Values, token)
                                               .ToArray(codes.Count, token);

        UserRecoveryCodeRecord[] memory = UserRecoveryCodeRecord.Create(user, records.AsSpan());

        UserRecoveryCodeRecord[] results = await db.UserRecoveryCodes.Insert(memory.AsMemory(), token)
                                                   .ToArray(records.Length, token);

        return ( records, results );
    }
    private static async ValueTask<UserLoginProviderRecord> Add_UserLoginProvider( Database db, UserRecord user, CancellationToken token = default )
    {
        UserLoginProviderRecord record = new("login provider", "provider display name", "provider key", "value", RecordID<UserLoginProviderRecord>.New(), user, DateTimeOffset.UtcNow);
        record = await db.UserLoginProviders.Insert(record, token);
        return record;
    }
    private static async ValueTask<FileRecord> Add_File( Database db, UserRecord user, CancellationToken token = default )
    {
        FileRecord record = new("file name", "file description", "file type", 0, "hash", MimeType.Unknown, "payload", "full file system path", RecordID<FileRecord>.New(), DateTimeOffset.UtcNow);
        record       = await db.Files.Insert(record, token);
        user.ImageID = record;
        await db.Users.Update(user, token);
        return record;
    }
    private static async ValueTask<(AddressRecord result, UserAddressRecord userAddress)> Add_Address( Database db, UserRecord user, CancellationToken token = default )
    {
        AddressRecord     record      = AddressRecord.Create("address line one", "", "city", "state or province", "postal code with optional extension", "country");
        AddressRecord     result      = await db.Addresses.Insert(record, token);
        UserAddressRecord userAddress = await db.UserAddresses.Insert(UserAddressRecord.Create(user, result), token);
        return ( result, userAddress );
    }
    private static async ValueTask<UserGroupRecord[]> Add_Roles( Database db, UserRecord user, GroupRecord[] roles, CancellationToken token = default )
    {
        ReadOnlyMemory<UserGroupRecord> records = UserGroupRecord.Create(user, roles.AsSpan());

        UserGroupRecord[] results = await db.UserGroups.Insert(records, token)
                                            .ToArray(records.Length, token);

        return results;
    }
    private static async ValueTask<UserRoleRecord[]> Add_Roles( Database db, UserRecord user, RoleRecord[] roles, CancellationToken token = default )
    {
        ReadOnlyMemory<UserRoleRecord> records = UserRoleRecord.Create(user, roles.AsSpan());

        UserRoleRecord[] results = await db.UserRoles.Insert(records, token)
                                           .ToArray(records.Length, token);

        return results;
    }
    private static async ValueTask<(RoleRecord Admin, RoleRecord User)> Add_Roles( Database db, UserRecord adminUser, CancellationToken token = default )
    {
        RoleRecord admin = RoleRecord.Create("Admin", Permissions<TestRight>.SA(),                   "Admins", adminUser);
        RoleRecord user  = RoleRecord.Create("User",  Permissions<TestRight>.Create(TestRight.Read), "Users",  adminUser);
        return ( await db.Roles.Insert(admin, token), await db.Roles.Insert(user, token) );
    }
    private static async ValueTask<(GroupRecord Admin, GroupRecord User)> Add_Group( Database db, UserRecord adminUser, CancellationToken token = default )
    {
        GroupRecord admin = GroupRecord.Create("Admin", Permissions<TestRight>.SA(),                   "Admin", adminUser);
        GroupRecord user  = GroupRecord.Create("User",  Permissions<TestRight>.Create(TestRight.Read), "User",  adminUser);
        return ( await db.Groups.Insert(admin, token), await db.Groups.Insert(user, token) );
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
        Write
    }
}
