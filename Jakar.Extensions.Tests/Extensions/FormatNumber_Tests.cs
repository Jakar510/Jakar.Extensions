// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Globalization;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Validate))]
public class FormatNumber_Tests : Assert
{
    private static readonly CultureInfo __invariant = CultureInfo.InvariantCulture;
    private static readonly CultureInfo __german    = CultureInfo.GetCultureInfo("de-DE");


    [TestCase(1.5,     4, "1.5")]
    [TestCase(100.0,   4, "100")]
    [TestCase(-1.25,   4, "-1.25")]
    [TestCase(1234.5,  2, "1,234.5")]
    [TestCase(0.00001, 4, "0")]
    [TestCase(1.23456, 2, "1.23")]
    [TestCase(100.0,   0, "100")]   // previously "1": the trailing zeros of the integer part were stripped
    [TestCase(1000.0,  0, "1,000")] // previously "1,"
    [TestCase(10.0,    1, "10")]
    public void Double_Invariant( double value, int decimals, string expected ) => this.AreEqual(expected, value.FormatNumber(__invariant, decimals));

    [Test] public void Float_And_Decimal_Invariant()
    {
        this.AreEqual("2.5",  2.5f.FormatNumber(__invariant));
        this.AreEqual("2.5",  2.5m.FormatNumber(__invariant));
        this.AreEqual("300",  300m.FormatNumber(__invariant, 0));
        this.AreEqual("0.01", 0.0100m.FormatNumber(__invariant));
    }

    [Test] public void OtherCulture_UsesItsSeparators()
    {
        this.AreEqual("1.234,5", 1234.5.FormatNumber(__german));
        this.AreEqual("1.000",   1000.0.FormatNumber(__german, 0));
    }

    [Test] public void SpecialValues()
    {
        this.AreEqual(double.NaN.ToString("N4", __invariant),              double.NaN.FormatNumber(__invariant));
        this.AreEqual(double.PositiveInfinity.ToString("N4", __invariant), double.PositiveInfinity.FormatNumber(__invariant));
    }

    [Test] public void VeryLargeValue_UsesTheFallbackPath()
    {
        string expected = double.MaxValue.ToString("N4", __invariant)[..^5]; // drop ".0000"
        this.AreEqual(expected, double.MaxValue.FormatNumber(__invariant));
    }

    [Test] // "n-1" is not a standard format, so both the old and new code treat it as a custom format string and echo it
    public void NegativeDecimals_MatchesThePreviousOutput() => this.AreEqual(string.Format(__invariant, "{0:n-1}", 1.5), 1.5.FormatNumber(__invariant, -1));
}
