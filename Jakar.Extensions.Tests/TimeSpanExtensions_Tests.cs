// Jakar.Extensions :: Jakar.Extensions.Tests



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(TimeSpanExtensions))]
public class TimeSpanExtensions_Tests : Assert
{
    // ─── FromDays ─────────────────────────────────────────────────────────────

    [Test] [TestCase(0)] [TestCase(1)] [TestCase(7)] [TestCase(365)]
    public void FromDays_Int( int value ) => this.AreEqual(TimeSpan.FromDays(value), value.FromDays());

    [Test] [TestCase(0L)] [TestCase(1L)] [TestCase(30L)]
    public void FromDays_Long( long value ) => this.AreEqual(TimeSpan.FromDays(value), value.FromDays());

    [Test] [TestCase(0.0)] [TestCase(1.5)] [TestCase(0.25)]
    public void FromDays_Double( double value ) => this.AreEqual(TimeSpan.FromDays(value), value.FromDays());


    // ─── FromHours ────────────────────────────────────────────────────────────

    [Test] [TestCase(0)] [TestCase(1)] [TestCase(24)] [TestCase(100)]
    public void FromHours_Int( int value ) => this.AreEqual(TimeSpan.FromHours(value), value.FromHours());

    [Test] [TestCase(0L)] [TestCase(1L)] [TestCase(48L)]
    public void FromHours_Long( long value ) => this.AreEqual(TimeSpan.FromHours(value), value.FromHours());

    [Test] [TestCase(0.0)] [TestCase(1.5)] [TestCase(0.5)]
    public void FromHours_Double( double value ) => this.AreEqual(TimeSpan.FromHours(value), value.FromHours());


    // ─── FromMinutes ──────────────────────────────────────────────────────────

    [Test] [TestCase(0)] [TestCase(1)] [TestCase(60)] [TestCase(90)]
    public void FromMinutes_Int( int value ) => this.AreEqual(TimeSpan.FromMinutes(value), value.FromMinutes());

    [Test] [TestCase(0L)] [TestCase(30L)] [TestCase(120L)]
    public void FromMinutes_Long( long value ) => this.AreEqual(TimeSpan.FromMinutes(value), value.FromMinutes());

    [Test] [TestCase(0.0)] [TestCase(1.5)] [TestCase(90.0)]
    public void FromMinutes_Double( double value ) => this.AreEqual(TimeSpan.FromMinutes(value), value.FromMinutes());


    // ─── FromSeconds ──────────────────────────────────────────────────────────

    [Test] [TestCase(0)] [TestCase(1)] [TestCase(60)] [TestCase(3600)]
    public void FromSeconds_Int( int value ) => this.AreEqual(TimeSpan.FromSeconds(value), value.FromSeconds());

    [Test] [TestCase(0L)] [TestCase(30L)] [TestCase(86400L)]
    public void FromSeconds_Long( long value ) => this.AreEqual(TimeSpan.FromSeconds(value), value.FromSeconds());

    [Test] [TestCase(0.0)] [TestCase(1.5)] [TestCase(0.001)]
    public void FromSeconds_Double( double value ) => this.AreEqual(TimeSpan.FromSeconds(value), value.FromSeconds());


    // ─── FromMilliseconds ─────────────────────────────────────────────────────

    [Test] [TestCase(0)] [TestCase(1)] [TestCase(500)] [TestCase(1000)]
    public void FromMilliseconds_Int( int value ) => this.AreEqual(TimeSpan.FromMilliseconds(value), value.FromMilliseconds());

    [Test] [TestCase(0L)] [TestCase(250L)] [TestCase(5000L)]
    public void FromMilliseconds_Long( long value ) => this.AreEqual(TimeSpan.FromMilliseconds(value), value.FromMilliseconds());

    [Test] [TestCase(0.0)] [TestCase(1.5)] [TestCase(999.9)]
    public void FromMilliseconds_Double( double value ) => this.AreEqual(TimeSpan.FromMilliseconds(value), value.FromMilliseconds());


    // ─── FromTicks ────────────────────────────────────────────────────────────

    [Test] [TestCase(0)] [TestCase(1)] [TestCase(10_000)] [TestCase(10_000_000)]
    public void FromTicks_Int( int value ) => this.AreEqual(TimeSpan.FromTicks(value), value.FromTicks());

    [Test] [TestCase(0L)] [TestCase(10_000L)] [TestCase(TimeSpan.TicksPerSecond)] [TestCase(TimeSpan.TicksPerDay)]
    public void FromTicks_Long( long value ) => this.AreEqual(TimeSpan.FromTicks(value), value.FromTicks());


    // ─── Relationships / correctness ──────────────────────────────────────────

    [Test]
    public void OneDayEquals24Hours() => this.AreEqual(1.FromDays(), 24.FromHours());

    [Test]
    public void OneHourEquals60Minutes() => this.AreEqual(1.FromHours(), 60.FromMinutes());

    [Test]
    public void OneMinuteEquals60Seconds() => this.AreEqual(1.FromMinutes(), 60.FromSeconds());

    [Test]
    public void OneSecondEquals1000Milliseconds() => this.AreEqual(1.FromSeconds(), 1000.FromMilliseconds());

    [Test]
    public void OneSecondEqualsTicksPerSecond() => this.AreEqual(1.FromSeconds(), TimeSpan.TicksPerSecond.FromTicks());
}
