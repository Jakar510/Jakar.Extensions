// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Linq;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Randoms))]
public class Randoms_Tests : Assert
{
    [Test] public void RandomString_IsUppercaseLatin_WithTheRequestedLength()
    {
        string value = Randoms.RandomString(4096);

        this.AreEqual(4096, value.Length);
        this.IsTrue(value.All(static c => c is >= 'A' and <= 'Z'));
        this.AreEqual(26, value.Distinct().Count()); // every letter (including 'I') is reachable
    }

    [Test] public void RandomString_WithConverter_AppliesIt()
    {
        string value = Randoms.RandomString(512, static c => c);

        this.AreEqual(512, value.Length);
        this.IsTrue(value.All(static c => c is >= 'a' and <= 'z'));
        this.IsTrue(Randoms.RandomString(64, char.ToUpperInvariant).All(static c => c is >= 'A' and <= 'Z'));
    }

    [Test] public void RandomString_Empty() => this.AreEqual("", Randoms.RandomString(0));
}
