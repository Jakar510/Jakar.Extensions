// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/07/2026

using System.Linq;
using Jakar.Extensions.UserGuid;



namespace Jakar.Extensions.Tests;


/// <summary>
///     The user models are <c>[JsonModel]</c> types: Jakar.Json reads and writes them through generated code. Covers the shared member types:
///     observable collections, <see cref="UserRights"/> and the <see cref="JObjectNode"/> extension-data bag.
/// </summary>
[TestFixture]
public sealed class UserModelJson_Tests
{
    [Test] public void Members_RunOnJakarJson()
    {
        UserModel user = GoldenSamples.User();
        user.Rights = "rights-value";

        string    json = user.ToJson();
        UserModel back = UserModel.FromJson(json);

        Assert.That(json,              Is.EqualTo(JsonCodec.ToJson(user)));
        Assert.That(json,              Does.StartWith("{\"Addresses\" : [{"), "compact, in Jakar.Json's layout");
        Assert.That(user.ToJsonUtf8(), Is.EqualTo(System.Text.Encoding.UTF8.GetBytes(json)));
        AssertSameUser(back, user);
        Assert.That(back.Rights.Value,                                      Is.EqualTo("rights-value"));
        Assert.That(back.ToJson(),                                          Is.EqualTo(json), "idempotent");
        Assert.That(user.TryFormat(stackalloc char[8], out _, default, null), Is.False, "ISpanFormattable comes with it, and reports a short buffer");
        Assert.Throws<JsonReadException>(static () => UserModel.FromJson("{\"UserName\": }"));
    }

    [Test] public void ExtensionData_KeepsUnknownMembers()
    {
        string json = GoldenSamples.User().ToJson().TrimEnd().TrimEnd('}') + ", \"custom\": {\"a\": [1, 2.50, \"x\", null, true]}}";

        UserModel back = UserModel.FromJson(json);

        Assert.That(back.AdditionalData!.Keys,                                        Is.EqualTo(new[] { "custom" }), "unknown members are captured");
        Assert.That(back.AdditionalData["custom"]!["a"]![1]!.ToJson(),                Is.EqualTo("2.50"),             "numbers keep their text");
        Assert.That(back.ToJson(),                                                    Does.EndWith("\"custom\" : {\"a\" : [1,2.50,\"x\",null,true]}}"), "written after the declared members");
        Assert.That(UserModel.FromJson(back.ToJson()).AdditionalData!["custom"]!["a"], Is.InstanceOf<JArrayNode>());
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
