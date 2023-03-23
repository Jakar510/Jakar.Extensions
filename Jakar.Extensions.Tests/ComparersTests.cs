// Jakar.Extensions :: Jakar.Extensions.Tests
// 03/23/2023  9:35 AM

using System;
using NUnit.Framework;



namespace Jakar.Extensions.Tests;


[TestFixture]
public class ComparersTests : Assert
{
    [Test]
    public void TestMin_Integers()
    {
        int  left   = 5;
        int  right  = 3;
        int? result = Comparers.Min<int>( left, right );
        AreEqual( 3, result );
        NotNull( result );
    }

    [Test]
    public void TestMin_NullableIntegers()
    {
        int? left   = 5;
        int? right  = 3;
        int? result = left.Min( right );
        AreEqual( 3, result );
        NotNull( result );

        left   = null;
        result = left.Min( right );
        AreEqual( 3, result );

        right  = null;
        result = left.Min( right );
        AreEqual( null, result );
    }

    [Test]
    public void TestMax_Integers()
    {
        int     left   = 5;
        int     right  = 3;
        double? result = Comparers.Max<double>( left, right );
        AreEqual( 5, result );
        NotNull( result );
    }

    [Test]
    public void TestMax_NullableIntegers()
    {
        int? left   = 5;
        int? right  = 3;
        int? result = left.Max( right );
        AreEqual( 5, result );

        left   = null;
        result = left.Max( right );
        AreEqual( 3, result );

        right  = null;
        result = left.Max( right );
        AreEqual( null, result );
    }

    [Test]
    public void TestMin_Doubles()
    {
        double  left   = 5.0;
        double  right  = 3.0;
        double? result = Comparers.Min<double>( left, right );
        AreEqual( 3.0, result );
        NotNull( result );
    }

    [Test]
    public void TestMin_NullableDoubles()
    {
        double? left   = 5.0;
        double? right  = 3.0;
        double? result = left.Min( right );
        AreEqual( 3.0, result );

        left   = null;
        result = left.Min( right );
        AreEqual( 3.0, result );

        right  = null;
        result = left.Min( right );
        AreEqual( null, result );
    }

    [Test]
    public void TestMax_Doubles()
    {
        double  left   = 5.0;
        double  right  = 3.0;
        double? result = Comparers.Max<double>( left, right );
        AreEqual( 5.0, result );
    }

    [Test]
    public void TestMax_NullableDoubles()
    {
        double? left   = 5.0;
        double? right  = 3.0;
        double? result = left.Max( right );
        AreEqual( 5.0, result );
        NotNull( result );

        left   = null;
        result = left.Max( right );
        AreEqual( 3.0, result );

        right  = null;
        result = left.Max( right );
        AreEqual( null, result );
    }

    [Test]
    public void TestMin_TimeSpans()
    {
        TimeSpan  left   = TimeSpan.FromSeconds( 5 );
        TimeSpan  right  = TimeSpan.FromSeconds( 3 );
        TimeSpan? result = Comparers.Min<TimeSpan>( left, right );
        AreEqual( TimeSpan.FromSeconds( 3 ), result );
        NotNull( result );
    }

    [Test]
    public void TestMax_TimeSpans()
    {
        TimeSpan  left   = TimeSpan.FromSeconds( 5 );
        TimeSpan  right  = TimeSpan.FromSeconds( 3 );
        TimeSpan? result = Comparers.Max<TimeSpan>( left, right );
        AreEqual( TimeSpan.FromSeconds( 5 ), result );
        NotNull( result );
    }

    [Test] public void TestMin_DateTimes() => TestMin( DateTime.UtcNow );
    [Test] public void TestMax_DateTimes() => TestMax( DateTime.UtcNow );
    [Test] public void TestMin_DateTimeOffsets() => TestMin( DateTimeOffset.UtcNow );
    [Test] public void TestMax_DateTimeOffsets() => TestMax( DateTimeOffset.UtcNow );


    private static void TestMin( in DateTime value )
    {
        TimeSpan offset = TimeSpan.FromDays( 1 );

        AreEqual( value - offset, Comparers.Min<DateTime>( value, value - offset ) );
        AreEqual( value,          Comparers.Min<DateTime>( value, value + offset ) );

        AreEqual( value - offset, Comparers.Min<DateTime>( default, value - offset ) );
        AreEqual( value + offset, Comparers.Min<DateTime>( default, value + offset ) );
    }
    private static void TestMax( in DateTime value )
    {
        TimeSpan offset = TimeSpan.FromDays( 1 );

        AreEqual( value,          Comparers.Max<DateTime>( value, value - offset ) );
        AreEqual( value + offset, Comparers.Max<DateTime>( value, value + offset ) );

        AreEqual( value - offset, Comparers.Max<DateTime>( default, value - offset ) );
        AreEqual( value + offset, Comparers.Max<DateTime>( default, value + offset ) );
    }
    private static void TestMin( in DateTimeOffset value )
    {
        TimeSpan offset = TimeSpan.FromDays( 1 );

        AreEqual( value - offset, Comparers.Min<DateTimeOffset>( value, value - offset ) );
        AreEqual( value,          Comparers.Min<DateTimeOffset>( value, value + offset ) );

        AreEqual( value - offset, Comparers.Min<DateTimeOffset>( default, value - offset ) );
        AreEqual( value + offset, Comparers.Min<DateTimeOffset>( default, value + offset ) );
    }
    private static void TestMax( in DateTimeOffset value )
    {
        TimeSpan offset = TimeSpan.FromDays( 1 );

        AreEqual( value,          Comparers.Max<DateTimeOffset>( value, value - offset ) );
        AreEqual( value + offset, Comparers.Max<DateTimeOffset>( value, value + offset ) );

        AreEqual( value - offset, Comparers.Max<DateTimeOffset>( default, value - offset ) );
        AreEqual( value + offset, Comparers.Max<DateTimeOffset>( default, value + offset ) );
    }
}
