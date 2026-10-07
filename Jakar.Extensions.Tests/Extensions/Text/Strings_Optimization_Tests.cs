// Jakar.Extensions :: Jakar.Extensions.Tests

namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Strings))]
public class Strings_Optimization_Tests : Assert
{
    [TestCase("ab",  3, "ababab")]
    [TestCase("ab",  1, "ab")]
    [TestCase("ab",  0, "")]
    [TestCase("",    5, "")]
    [TestCase("x",   4, "xxxx")]
    [TestCase("xyz", 2, "xyzxyz")]
    public void Repeat( string value, int count, string expected ) => this.AreEqual(expected, value.Repeat(count));

    [Test] public void Repeat_Negative_Throws() => Throws<ArgumentOutOfRangeException>(static () => "ab".Repeat(-1));


    [TestCase("a-b-c",  '-', "abc")]
    [TestCase("--a--",  '-', "a")]
    [TestCase("----",   '-', "")]
    [TestCase("abc",    '-', "abc")]
    [TestCase("",       '-', "")]
    [TestCase("-",      '-', "")]
    [TestCase("ab-",    '-', "ab")]
    public void RemoveAll_Char( string value, char old, string expected ) => this.AreEqual(expected, value.RemoveAll(old));

    [Test]
    public void RemoveAll_Char_Absent_ReturnsSameInstance()
    {
        const string VALUE = "no dashes here";
        Assert.AreSame(VALUE, VALUE.RemoveAll('-'));
    }
}
