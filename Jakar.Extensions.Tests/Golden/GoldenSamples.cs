// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

// Shared by the golden-file generator (compiled against Jakar.Extensions 10.x / Newtonsoft with LEGACY defined) and by GoldenFile_Tests.
// Every sample is deterministic, so the JSON 10.x wrote and the JSON 11.0 writes can be compared directly.

using System.Collections.Generic;
using System.Globalization;
using Jakar.Extensions.UserGuid;



namespace Jakar.Extensions.Tests.Golden;


public static class GoldenSamples
{
    public static readonly Guid ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ID_2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid ID_3 = Guid.Parse("33333333-3333-3333-3333-333333333333");


    public static AppVersion     AppVersion()     => new(1, 2, 3, 4);
    public static AppInformation AppInformation() => new(AppVersion(), ID_1, "Jakar", "com.jakar.app");
    public static Pair           Pair()           => new("key", "value");
    public static StringTags     StringTags()     => new([new Pair("field", "notes"), new Pair("nullable", null)], ["must be 5 characters or fewer", "required"]);
    public static Error          Error()          => Extensions.Error.Validation(StringTags(), "/items/1", "must be 5 characters or fewer", "Validation");
    public static Errors         Errors()         => Extensions.Errors.Create(new Alert("Heads up", "Something happened", TimeSpan.FromSeconds(30)), Error(), Extensions.Error.Validation(description: "second"));
    public static Alert          Alert()          => new("Heads up", "Something happened", TimeSpan.FromSeconds(30));
    public static ErrorResponse  ErrorResponse()  => new("something went wrong");
    public static FileMetaData   FileMetaData()   => new("report.txt", "text/plain", null, "a report");
    public static LoginRequest   LoginRequest()   => new("tyler", "hunter2");

    public static LoginRequestVersion LoginRequestVersion() => new("tyler", "hunter2", AppVersion());


    public static UserAddress Address() => new("1 Main St", "Apt 2", "Springfield", "IL", "62701", "US", ID_1);
    public static GroupModel  Group()   => new("Admins", ID_1, ID_2, ID_3, "rights");
    public static RoleModel   Role()    => new("Admin", "rights", ID_2);

    public static UserModel User()
    {
        UserModel user = new("Tyler", "Stegmaier")
                         {
                             UserName    = "tyler",
                             Email       = "tyler@example.test",
                             PhoneNumber = "555-0100",
                             Company     = "Jakar",
                             Department  = "R&D",
                             Title       = "Engineer",
                             Website     = "https://example.test"
                         };

        user.Addresses.Add(Address());
        user.Groups.Add(Group());
        user.Roles.Add(Role());
        return user;
    }


    /// <summary> File name → JSON, written with the serializer of the build this is compiled into. </summary>
    public static IEnumerable<(string Name, string Json)> All()
    {
        yield return ( "AppVersion",          AppVersion().ToJson() );
        yield return ( "AppInformation",      AppInformation().ToJson() );
        yield return ( "Pair",                Pair().ToJson() );
        yield return ( "StringTags",          StringTags().ToJson() );
        yield return ( "Errors",              Errors().ToJson() );
        yield return ( "Alert",               Alert().ToJson() );
        yield return ( "ErrorResponse",       ErrorResponse().ToJson() );
        yield return ( "FileMetaData",        FileMetaData().ToJson() );
        yield return ( "LoginRequest",        LoginRequest().ToJson() );
        yield return ( "LoginRequestVersion", LoginRequestVersion().ToJson() );
        yield return ( "UserAddress",         Address().ToJson() );
        yield return ( "GroupModel",          Group().ToJson() );
        yield return ( "RoleModel",           Role().ToJson() );
        yield return ( "UserModel",           User().ToJson() );
#if LEGACY
        yield return ( "Error", Error().ToJson() );
#else
        yield return ( "Error", Json.Serialize(Error()) );
#endif
    }
}
