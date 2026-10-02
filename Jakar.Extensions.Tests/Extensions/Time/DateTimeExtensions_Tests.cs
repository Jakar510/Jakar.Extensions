// Jakar.Extensions :: Jakar.Extensions.Tests



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(DateTimeExtensions))]
public class DateTimeExtensions_Tests : Assert
{
    // ─── DateTime → DateOnly ──────────────────────────────────────────────────

    [Test]
    public void AsDateOnly_DateTime_PreservesDate()
    {
        DateTime dt     = new(2024, 6, 15, 10, 30, 45);
        DateOnly result = dt.AsDateOnly();
        this.AreEqual(2024, result.Year);
        this.AreEqual(6,    result.Month);
        this.AreEqual(15,   result.Day);
    }

    [Test]
    public void AsDateOnly_DateTime_IgnoresTime()
    {
        DateTime dt1 = new(2024, 1, 1, 0,  0,  0);
        DateTime dt2 = new(2024, 1, 1, 23, 59, 59);
        this.AreEqual(dt1.AsDateOnly(), dt2.AsDateOnly());
    }

    [Test]
    public void AsDateOnly_DateTime_MinValue()
    {
        DateTime dt     = DateTime.MinValue;
        DateOnly result = dt.AsDateOnly();
        this.AreEqual(dt.Year,  result.Year);
        this.AreEqual(dt.Month, result.Month);
        this.AreEqual(dt.Day,   result.Day);
    }

    [Test]
    public void AsDateOnly_DateTime_MaxValue()
    {
        DateTime dt     = DateTime.MaxValue;
        DateOnly result = dt.AsDateOnly();
        this.AreEqual(dt.Year,  result.Year);
        this.AreEqual(dt.Month, result.Month);
        this.AreEqual(dt.Day,   result.Day);
    }


    // ─── DateTimeOffset → DateOnly ────────────────────────────────────────────

    [Test]
    public void AsDateOnly_DateTimeOffset_PreservesDate()
    {
        DateTimeOffset dto    = new(2024, 6, 15, 10, 30, 45, TimeSpan.FromHours(5));
        DateOnly       result = dto.AsDateOnly();
        this.AreEqual(2024, result.Year);
        this.AreEqual(6,    result.Month);
        this.AreEqual(15,   result.Day);
    }

    [Test]
    public void AsDateOnly_DateTimeOffset_IgnoresTime()
    {
        DateTimeOffset dto1 = new(2024, 3, 20, 0,  0,  0,  TimeSpan.Zero);
        DateTimeOffset dto2 = new(2024, 3, 20, 23, 59, 59, TimeSpan.Zero);
        this.AreEqual(dto1.AsDateOnly(), dto2.AsDateOnly());
    }


    // ─── DateTime → TimeOnly ──────────────────────────────────────────────────

    [Test]
    public void AsTimeOnly_DateTime_PreservesHourMinuteSecond()
    {
        DateTime dt     = new(2024, 6, 15, 10, 30, 45);
        TimeOnly result = dt.AsTimeOnly();
        this.AreEqual(10, result.Hour);
        this.AreEqual(30, result.Minute);
        this.AreEqual(45, result.Second);
    }

    [Test]
    public void AsTimeOnly_DateTime_Midnight()
    {
        DateTime dt     = new(2024, 1, 1, 0, 0, 0);
        TimeOnly result = dt.AsTimeOnly();
        this.AreEqual(0, result.Hour);
        this.AreEqual(0, result.Minute);
        this.AreEqual(0, result.Second);
    }

    [Test]
    public void AsTimeOnly_DateTime_EndOfDay()
    {
        DateTime dt     = new(2024, 1, 1, 23, 59, 59);
        TimeOnly result = dt.AsTimeOnly();
        this.AreEqual(23, result.Hour);
        this.AreEqual(59, result.Minute);
        this.AreEqual(59, result.Second);
    }

    [Test]
    public void AsTimeOnly_DateTime_IgnoresDate()
    {
        DateTime dt1 = new(2024, 1,  1,  10, 30, 45);
        DateTime dt2 = new(2025, 12, 31, 10, 30, 45);
        this.AreEqual(dt1.AsTimeOnly(), dt2.AsTimeOnly());
    }


    // ─── DateTimeOffset → TimeOnly ────────────────────────────────────────────

    [Test]
    public void AsTimeOnly_DateTimeOffset_PreservesHourMinuteSecond()
    {
        DateTimeOffset dto    = new(2024, 6, 15, 10, 30, 45, TimeSpan.FromHours(-7));
        TimeOnly       result = dto.AsTimeOnly();
        this.AreEqual(10, result.Hour);
        this.AreEqual(30, result.Minute);
        this.AreEqual(45, result.Second);
    }
}
