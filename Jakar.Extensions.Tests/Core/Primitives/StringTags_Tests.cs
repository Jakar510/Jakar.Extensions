// Jakar.Extensions :: Jakar.Extensions.Tests

namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(StringTags))]
public class StringTags_Tests : Assert
{
    private const string DESCRIPTION = "must be 5 characters or fewer";
    private const string FIELD       = "notes";


    // ─── StringTags / Pair carry their payload in public fields ───────────────

    private static StringTags CreateTags() => new([new Pair(FIELD, "notes"), new Pair("nullable", null)], [DESCRIPTION, "required"]);


    [Test] public void StringTags_JakarJsonRoundTrip()
    {
        StringTags tags = CreateTags();
        string     json = tags.ToJson();
        this.IsTrue(json.StartsWith("{\"Tags\" : [{\"Key\" : \"" + FIELD, StringComparison.Ordinal));
        this.IsFalse(json.Contains(nameof(StringTags.IsEmpty), StringComparison.Ordinal)); // computed members aren't written

        StringTags back = StringTags.FromJson(json);
        this.AreEqual(2,       back.Tags.Length);
        this.AreEqual(2,       back.Entries.Length);
        this.AreEqual(FIELD,   back.Tags[0].Key);
        this.AreEqual("notes", back.Tags[0].Value);
        this.IsNull(back.Tags[1].Value);
        this.AreEqual(DESCRIPTION, back.Entries[0]);
        this.IsFalse(back.IsEmpty);
    }

    [Test] public void StringTags_ToleratesMissingAndNullMembers()
    {
        this.IsTrue(StringTags.FromJson("{}").IsEmpty);
        this.IsTrue(StringTags.FromJson("""{"Tags":null,"Entries":null}""").IsEmpty);
        this.AreEqual(1, StringTags.FromJson("""{"Unknown":{"nested":[1,2]},"entries":["x"]}""").Entries.Length);
    }
}