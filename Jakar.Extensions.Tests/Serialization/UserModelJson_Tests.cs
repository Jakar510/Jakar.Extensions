// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/07/2026

using System.Linq;
using System.Text.Json;
using Jakar.Extensions.UserGuid;
using JJ = Jakar.Json;



namespace Jakar.Extensions.Tests;


/// <summary>
///     The user models are <c>[JsonModel(typeof(UserGuidJsonContext))]</c> types Jakar.Json supports, so their <see cref="IJsonModel{TSelf}"/> members run on Jakar.Json (hybrid, SPEC.md §7.7)
///     with the context's options mirrored, while <see cref="IJsonModel{TSelf}.JsonTypeInfo"/> stays System.Text.Json's. Covers both serializers on one type and the shared member types:
///     observable collections, <see cref="UserRights"/> and the <c>Dictionary&lt;string, JsonElement&gt;</c> extension-data bag.
/// </summary>
[TestFixture]
public sealed class UserModelJson_Tests
{
    [Test] public void IJsonModelMembers_RunOnJakarJson()
    {
        UserModel user = GoldenSamples.User();
        user.Rights = "rights-value";

        string    json = user.ToJson();
        UserModel back = UserModel.FromJson(json);

        Assert.That(json,                Is.EqualTo(JJ.JsonCodec.ToJson(user)), "ToJson() is Jakar.Json's");
        Assert.That(json,                Does.StartWith("{\n  \"Addresses\" : [\n    {"), "the context's WriteIndented, as System.Text.Json lays it out (two spaces)");
        Assert.That(user.ToUtf8Json(),   Is.EqualTo(System.Text.Encoding.UTF8.GetBytes(json)));
        AssertSameUser(back, user);
        Assert.That(back.Rights.Value,   Is.EqualTo("rights-value"));
        Assert.That(back.ToJson(),       Is.EqualTo(json), "idempotent");
        Assert.That(user.TryFormat(stackalloc char[8], out _, default, null), Is.False, "ISpanFormattable comes with it, and reports a short buffer");
        Assert.Throws<JsonException>(static () => UserModel.FromJson("{\"UserName\": }"), "IJsonModel's contract: JsonException");
    }

    [Test] public void JsonTypeInfo_StaysSystemTextJson_AndBothReadEachOther()
    {
        UserModel user = GoldenSamples.User();
        string    stj  = JsonSerializer.Serialize(user, UserModel.JsonTypeInfo);

        Assert.That(stj, Does.Contain("\"Addresses\": ["), "System.Text.Json's layout");
        AssertSameUser(UserModel.FromJson(stj),                                  user);
        AssertSameUser(JsonSerializer.Deserialize(user.ToJson(), UserModel.JsonTypeInfo)!, user);
    }

    [Test] public void ExtensionData_IsOneBag_ForBothSerializers()
    {
        string json = GoldenSamples.User().ToJson().TrimEnd().TrimEnd('}') + ", \"custom\": {\"a\": [1, 2.50, \"x\", null, true]}}";

        UserModel viaJakar = UserModel.FromJson(json);
        UserModel viaStj   = JsonSerializer.Deserialize(json, UserModel.JsonTypeInfo)!;

        Assert.That(viaStj.AdditionalData!["custom"].GetRawText(),                      Is.EqualTo("{\"a\": [1, 2.50, \"x\", null, true]}"));
        Assert.That(viaJakar.AdditionalData!.Keys,                                      Is.EqualTo(new[] { "custom" }), "Jakar.Json captures unknown members into the same Dictionary<string, JsonElement>");
        Assert.That(viaJakar.AdditionalData["custom"].GetProperty("a")[1].GetRawText(), Is.EqualTo("2.50"),             "numbers keep their text");

        Assert.That(viaJakar.ToJson(indented: false), Does.EndWith("\"custom\" : {\"a\" : [1,2.50,\"x\",null,true]}}"), "written after the declared members");
        Assert.That(JsonSerializer.Deserialize(viaStj.ToJson(), UserModel.JsonTypeInfo)!.AdditionalData!["custom"].GetProperty("a").GetArrayLength(), Is.EqualTo(5), "filled by System.Text.Json, written by Jakar.Json, read back by System.Text.Json");
    }

    [Test] public void ObservableCollections_WriteUnfilteredContents_AndReadInBulk()
    {
        UserModel user = GoldenSamples.User();
        user.Addresses.OverrideFilter = static ( int _, ref readonly UserAddress? _ ) => false; // a view that hides everything

        Assert.That(user.Addresses.Where(static _ => true).Count(), Is.Zero, "enumeration applies the filter");

        UserModel back = UserModel.FromJson(user.ToJson());
        Assert.That(back.Addresses.Single().City, Is.EqualTo("Springfield"), "JSON holds the contents, not the view");

        Assert.That(ObservableCollection<int>.FromJsonElements([3, 1, 2]).ToArray(), Is.EqualTo(new[] { 3, 1, 2 }), "IJsonCollection.FromJsonElements keeps the order");
    }


    private static void AssertSameUser( UserModel actual, UserModel expected )
    {
        Assert.That(actual.UserName,                     Is.EqualTo(expected.UserName));
        Assert.That(actual.FirstName,                    Is.EqualTo(expected.FirstName));
        Assert.That(actual.Email,                        Is.EqualTo(expected.Email));
        Assert.That(actual.Website,                      Is.EqualTo(expected.Website));
        Assert.That(actual.Addresses.Single().City,      Is.EqualTo(expected.Addresses.Single().City));
        Assert.That(actual.Addresses.Single().ID,        Is.EqualTo(expected.Addresses.Single().ID));
        Assert.That(actual.Groups.Single().NameOfGroup,  Is.EqualTo(expected.Groups.Single().NameOfGroup));
        Assert.That(actual.Groups.Single().Rights.Value, Is.EqualTo(expected.Groups.Single().Rights.Value));
        Assert.That(actual.Roles.Single().NameOfRole,    Is.EqualTo(expected.Roles.Single().NameOfRole));
        Assert.That(actual.Rights.Value,                 Is.EqualTo(expected.Rights.Value));
    }
}
