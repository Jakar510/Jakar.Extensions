// Jakar.Extensions :: Jakar.Extensions.Tests

namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Numbers))]
public class Numbers_Tests : Assert
{
    // ─── Round ───────────────────────────────────────────────────────────────

    [Test] [TestCase(1.4,  1.0)] [TestCase(1.5,  2.0)] [TestCase(-1.5, -2.0)] [TestCase(0.0, 0.0)]
    public void Round_Double( double value, double expected ) => this.AreEqual(expected, value.Round());

    [Test] [TestCase(1.4f, 1.0f)] [TestCase(1.5f, 2.0f)] [TestCase(-1.5f, -2.0f)] [TestCase(0.0f, 0.0f)]
    public void Round_Float( float value, float expected ) => this.AreEqual(expected, value.Round());

    [Test] [TestCase(1.4, 1.0)] [TestCase(1.5, 2.0)] [TestCase(0.0, 0.0)]
    public void Round_Decimal( decimal value, decimal expected ) => this.AreEqual(expected, value.Round());


    // ─── Floor ───────────────────────────────────────────────────────────────

    [Test] [TestCase(1.9,  1.0)] [TestCase(1.1,  1.0)] [TestCase(-1.1, -2.0)] [TestCase(0.0, 0.0)]
    public void Floor_Double( double value, double expected ) => this.AreEqual(expected, value.Floor());

    [Test] [TestCase(1.9f, 1.0f)] [TestCase(1.1f, 1.0f)] [TestCase(-1.1f, -2.0f)] [TestCase(0.0f, 0.0f)]
    public void Floor_Float( float value, float expected ) => this.AreEqual(expected, value.Floor());

    [Test] [TestCase(1.9, 1.0)] [TestCase(1.1, 1.0)] [TestCase(0.0, 0.0)]
    public void Floor_Decimal( decimal value, decimal expected ) => this.AreEqual(expected, value.Floor());


    // ─── Abs ─────────────────────────────────────────────────────────────────

    [Test] [TestCase((short)5,   (short)5)]  [TestCase((short)-5,  (short)5)]  [TestCase((short)0, (short)0)]
    public void Abs_Short( short value, short expected ) => this.AreEqual(expected, value.Abs());

    [Test] [TestCase(5,   5)]  [TestCase(-5,  5)]  [TestCase(0, 0)]  [TestCase(int.MaxValue, int.MaxValue)]
    public void Abs_Int( int value, int expected ) => this.AreEqual(expected, value.Abs());

    [Test] [TestCase(5L,  5L)] [TestCase(-5L, 5L)] [TestCase(0L, 0L)]
    public void Abs_Long( long value, long expected ) => this.AreEqual(expected, value.Abs());

    [Test] [TestCase(5.0f,  5.0f)] [TestCase(-5.0f, 5.0f)] [TestCase(0.0f, 0.0f)]
    public void Abs_Float( float value, float expected ) => this.AreEqual(expected, value.Abs());

    [Test] [TestCase(5.0,  5.0)] [TestCase(-5.0, 5.0)] [TestCase(0.0, 0.0)]
    public void Abs_Double( double value, double expected ) => this.AreEqual(expected, value.Abs());

    [Test] [TestCase(5.0, 5.0)] [TestCase(0.0, 0.0)]
    public void Abs_Decimal( decimal value, decimal expected ) => this.AreEqual(expected, value.Abs());


    // ─── Clamp (concrete) ────────────────────────────────────────────────────

    [Test] [TestCase(5,   1, 10, 5)]  [TestCase(0,   1, 10, 1)]  [TestCase(11,  1, 10, 10)] [TestCase(1,  1, 10, 1)]  [TestCase(10, 1, 10, 10)]
    public void Clamp_Int( int value, int min, int max, int expected ) => this.AreEqual(expected, value.Clamp(min, max));

    [Test] [TestCase(5L,  1L, 10L, 5L)] [TestCase(0L,  1L, 10L, 1L)] [TestCase(11L, 1L, 10L, 10L)]
    public void Clamp_Long( long value, long min, long max, long expected ) => this.AreEqual(expected, value.Clamp(min, max));

    [Test] [TestCase(5.0, 1.0, 10.0, 5.0)] [TestCase(0.0, 1.0, 10.0, 1.0)] [TestCase(11.0, 1.0, 10.0, 10.0)]
    public void Clamp_Double( double value, double min, double max, double expected ) => this.AreEqual(expected, value.Clamp(min, max));


    // ─── Clamp (generic via IComparisonOperators) ─────────────────────────────

    [Test] [TestCase(5,   1, 10, 5)]  [TestCase(-1, 0, 10, 0)] [TestCase(20, 0, 10, 10)]
    public void Clamp_Generic_Int( int value, int min, int max, int expected )
    {
        int result = value.Clamp(min, max);
        this.AreEqual(expected, result);
    }

    [Test]
    public void Clamp_Generic_ThrowsWhenMinGtMax()
    {
        Throws<ArgumentException>(() => _ = 5.Clamp(10, 1));
    }


    // ─── Cast helpers ─────────────────────────────────────────────────────────

    [Test] [TestCase(1.0,  (byte)1)] [TestCase(255.0, (byte)255)] [TestCase(0.0, (byte)0)]
    public void AsByte_Double( double value, byte expected ) => this.AreEqual(expected, value.AsByte());

    [Test] [TestCase(1,   (byte)1)] [TestCase(0,   (byte)0)]
    public void AsByte_Int( int value, byte expected ) => this.AreEqual(expected, value.AsByte());

    [Test] [TestCase(1.0,  1)] [TestCase(3.9, 3)] [TestCase(0.0, 0)]
    public void AsInt_Double( double value, int expected ) => this.AreEqual(expected, value.AsInt());

    [Test] [TestCase(1.0f, 1)] [TestCase(3.9f, 3)] [TestCase(0.0f, 0)]
    public void AsInt_Float( float value, int expected ) => this.AreEqual(expected, value.AsInt());

    [Test] [TestCase(1L,  1)] [TestCase(0L,  0)]
    public void AsInt_Long( long value, int expected ) => this.AreEqual(expected, value.AsInt());

    [Test] [TestCase(1.0,  1.0f)] [TestCase(0.0, 0.0f)]
    public void AsFloat_Double( double value, float expected ) => this.AreEqual(expected, value.AsFloat());

    [Test] [TestCase(1,   (short)1)] [TestCase(0,   (short)0)]
    public void AsShort_Int( int value, short expected ) => this.AreEqual(expected, value.AsShort());


    // ─── String.As<T> parsing ────────────────────────────────────────────────

    [Test] [TestCase("42",  42)]  [TestCase("0",   0)]   [TestCase("-7",  -7)]
    public void StringAs_Int_Valid( string value, int expected )
    {
        int result = value.As<int>(0);
        this.AreEqual(expected, result);
    }

    [Test] [TestCase("abc", 99)] [TestCase("",    99)] [TestCase(null,  99)]
    public void StringAs_Int_Invalid_ReturnsDefault( string? value, int defaultValue )
    {
        int result = value.As<int>(defaultValue);
        this.AreEqual(defaultValue, result);
    }

    [Test] [TestCase("3.14", 3.14)] [TestCase("0.0",  0.0)]
    public void StringAs_Double_Valid( string value, double expected )
    {
        double result = value.As<double>(0.0);
        Assert.That(result, Is.EqualTo(expected).Within(1e-10));
    }


    // ─── Enum cast helpers ───────────────────────────────────────────────────

    [Test]
    public void Enum_AsInt_DayOfWeek()
    {
        int sunday = DayOfWeek.Sunday.AsInt();
        int monday = DayOfWeek.Monday.AsInt();
        this.AreEqual(0, sunday);
        this.AreEqual(1, monday);
    }

    [Test]
    public void Enum_AsByte_DayOfWeek()
    {
        byte sunday = DayOfWeek.Sunday.AsByte();
        this.AreEqual((byte)0, sunday);
    }

    [Test]
    public void Enum_AsLong_DayOfWeek()
    {
        long friday = DayOfWeek.Friday.AsLong();
        this.AreEqual(5L, friday);
    }


    // ─── Generic Min / Max (tests expected semantics) ─────────────────────────

    [Test]
    public void Min_Generic_ReturnsSmaller()
    {
        int smaller = 3;
        int larger  = 7;
        // Expected: 3.Min(7) == 3  (smaller of the two)
        int result = smaller.Min(larger);
        this.AreEqual(smaller, result);
    }

    [Test]
    public void Max_Generic_ReturnsLarger()
    {
        int smaller = 3;
        int larger  = 7;
        // Expected: 3.Max(7) == 7  (larger of the two)
        int result = smaller.Max(larger);
        this.AreEqual(larger, result);
    }

    [Test]
    public void Min_Generic_Equal_ReturnsSelf()
    {
        int value  = 5;
        int result = value.Min(5);
        this.AreEqual(5, result);
    }

    [Test]
    public void Max_Generic_Equal_ReturnsSelf()
    {
        int value  = 5;
        int result = value.Max(5);
        this.AreEqual(5, result);
    }

}
