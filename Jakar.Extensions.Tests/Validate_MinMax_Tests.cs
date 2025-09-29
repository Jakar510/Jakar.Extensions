// Jakar.Extensions :: Jakar.Extensions.Tests
// 03/23/2023  9:35 AM

namespace Jakar.Extensions.Tests;


[TestFixture, TestOf( typeof(Validate) )]
public class Validate_MinMax_Tests : Assert
{
    [Test]
    public void TestMin_Integers()
    {
        int  left   = 5;
        int  right  = 3;
        int? result = Validate.Min<int>( left, right );
        this.AreEqual( 3, result );
        this.NotNull( result );
    }

    [Test]
    public void TestMin_NullableIntegers()
    {
        int? left   = 5;
        int? right  = 3;
        int? result = left.Min( right );
        this.AreEqual( 3, result );
        this.NotNull( result );

        left   = null;
        result = left.Min( right );
        this.AreEqual( 3, result );

        right  = null;
        result = left.Min( right );
        this.AreEqual( null, result );
    }

    [Test]
    public void TestMax_Integers()
    {
        int     left   = 5;
        int     right  = 3;
        double? result = Validate.Max<double>( left, right );
        this.AreEqual( 5, result );
        this.NotNull( result );
    }

    [Test]
    public void TestMax_NullableIntegers()
    {
        int? left   = 5;
        int? right  = 3;
        int? result = left.Max( right );
        this.AreEqual( 5, result );

        left   = null;
        result = left.Max( right );
        this.AreEqual( 3, result );

        right  = null;
        result = left.Max( right );
        this.AreEqual( null, result );
    }

    [Test]
    public void TestMin_Doubles()
    {
        double  left   = 5.0;
        double  right  = 3.0;
        double? result = Validate.Min<double>( left, right );
        this.AreEqual( 3.0, result );
        this.NotNull( result );
    }

    [Test]
    public void TestMin_NullableDoubles()
    {
        double? left   = 5.0;
        double? right  = 3.0;
        double? result = left.Min( right );
        this.AreEqual( 3.0, result );

        left   = null;
        result = left.Min( right );
        this.AreEqual( 3.0, result );

        right  = null;
        result = left.Min( right );
        this.AreEqual( null, result );
    }

    [Test]
    public void TestMax_Doubles()
    {
        double  left   = 5.0;
        double  right  = 3.0;
        double? result = Validate.Max<double>( left, right );
        this.AreEqual( 5.0, result );
    }

    [Test]
    public void TestMax_NullableDoubles()
    {
        double? left   = 5.0;
        double? right  = 3.0;
        double? result = left.Max( right );
        this.AreEqual( 5.0, result );
        this.NotNull( result );

        left   = null;
        result = left.Max( right );
        this.AreEqual( 3.0, result );

        right  = null;
        result = left.Max( right );
        this.AreEqual( null, result );
    }

    [Test]
    public void TestMin_TimeSpans()
    {
        TimeSpan  left   = TimeSpan.FromSeconds( 5 );
        TimeSpan  right  = TimeSpan.FromSeconds( 3 );
        TimeSpan? result = Validate.Min<TimeSpan>( left, right );
        this.AreEqual( TimeSpan.FromSeconds( 3 ), result );
        this.NotNull( result );
    }

    [Test]
    public void TestMax_TimeSpans()
    {
        TimeSpan  left   = TimeSpan.FromSeconds( 5 );
        TimeSpan  right  = TimeSpan.FromSeconds( 3 );
        TimeSpan? result = Validate.Max<TimeSpan>( left, right );
        this.AreEqual( TimeSpan.FromSeconds( 5 ), result );
        this.NotNull( result );
    }

    [Test] public void TestMin_DateTimes()       => TestMin( DateTime.UtcNow );
    [Test] public void TestMax_DateTimes()       => TestMax( DateTime.UtcNow );
    [Test] public void TestMin_DateTimeOffsets() => TestMin( DateTimeOffset.UtcNow );
    [Test] public void TestMax_DateTimeOffsets() => TestMax( DateTimeOffset.UtcNow );


    private void TestMin( in DateTime value )
    {
        TimeSpan offset = TimeSpan.FromDays( 1 );

        this.AreEqual( value - offset, Validate.Min<DateTime>( value, value - offset ) );
        this.AreEqual( value,          Validate.Min<DateTime>( value, value + offset ) );

        this.AreEqual( value - offset, Validate.Min<DateTime>( null, value - offset ) );
        this.AreEqual( value + offset, Validate.Min<DateTime>( null, value + offset ) );
    }
    private void TestMax( in DateTime value )
    {
        TimeSpan offset = TimeSpan.FromDays( 1 );

        this.AreEqual( value,          Validate.Max<DateTime>( value, value - offset ) );
        this.AreEqual( value + offset, Validate.Max<DateTime>( value, value + offset ) );

        this.AreEqual( value - offset, Validate.Max<DateTime>( null, value - offset ) );
        this.AreEqual( value + offset, Validate.Max<DateTime>( null, value + offset ) );
    }
    private void TestMin( in DateTimeOffset value )
    {
        TimeSpan offset = TimeSpan.FromDays( 1 );

        this.AreEqual( value - offset, Validate.Min<DateTimeOffset>( value, value - offset ) );
        this.AreEqual( value,          Validate.Min<DateTimeOffset>( value, value + offset ) );

        this.AreEqual( value - offset, Validate.Min<DateTimeOffset>( null, value - offset ) );
        this.AreEqual( value + offset, Validate.Min<DateTimeOffset>( null, value + offset ) );
    }
    private void TestMax( in DateTimeOffset value )
    {
        TimeSpan offset = TimeSpan.FromDays( 1 );

        this.AreEqual( value,          Validate.Max<DateTimeOffset>( value, value - offset ) );
        this.AreEqual( value + offset, Validate.Max<DateTimeOffset>( value, value + offset ) );

        this.AreEqual( value - offset, Validate.Max<DateTimeOffset>( null, value - offset ) );
        this.AreEqual( value + offset, Validate.Max<DateTimeOffset>( null, value + offset ) );
    }
}
