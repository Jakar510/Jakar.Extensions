// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

// Shared by the golden-file generator (compiled against Jakar.Extensions 10.x / Newtonsoft with LEGACY defined) and by GoldenFile_Tests.
// Every sample is deterministic, so the JSON 10.x wrote and the JSON 11.0 writes can be compared directly.

using System.Collections.Generic;



namespace Jakar.Extensions.Tests;


public static class GoldenSamples
{
    public static readonly Guid ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");


    public static AppVersion     AppVersion()     => new(1, 2, 3, 4);
    public static AppInformation AppInformation() => new(AppVersion(), ID_1, "Jakar", "com.jakar.app");
    public static Pair           Pair()           => new("key", "value");
    public static StringTags     StringTags()     => new([new Pair("field", "notes"), new Pair("nullable", null)], ["must be 5 characters or fewer", "required"]);


    /// <summary> File name → JSON, written with the serializer of the build this is compiled into. </summary>
    public static IEnumerable<(string Name, string Json)> All()
    {
        yield return ( "AppVersion", AppVersion().ToJson() );
        yield return ( "AppInformation", AppInformation().ToJson() );
        yield return ( "Pair", Pair().ToJson() );
        yield return ( "StringTags", StringTags().ToJson() );
    }
}