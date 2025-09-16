// Jakar.Extensions :: Jakar.Extensions.Tests
// 06/25/2025  11:29

using System.Collections.Generic;



namespace Jakar.Extensions.Tests;


[TestFixture, TestOf( typeof(EqualComparer<>) )]
public class Sorter_Tests : Assert
{
    [Test, TestCase( "false", "false" ), TestCase( "true", "true" ), TestCase( 0, 0 ), TestCase( double.Pi, double.Pi ), TestCase( 5UL, 5UL )]
    public void Comparers<T>( T expected, T actual )
        where T : IComparable<T>, IEquatable<T>
    {
        using ( EnterMultipleScope() )
        {
            this.AreEqual( expected,                                      actual );
            this.AreEqual( expected.CompareTo( actual ),                  EqualComparer<T>.Default.Compare( expected, actual ) );
            this.AreEqual( expected.CompareTo( actual ),                  Comparer<T>.Default.Compare( expected, actual ) );
            this.AreEqual( EqualComparer<T>.Default.Compare( expected, actual ), Comparer<T>.Default.Compare( expected, actual ) );
        }
    }


    [Test, TestCase( 0, 1 ), TestCase( double.Pi, double.Pi + 1 ), TestCase( 5UL, 6UL )]
    public void IsGreaterThan<T>( T expected, T actual )
        where T : IComparable<T>, IEquatable<T>
    {
        using ( EnterMultipleScope() )
        {
            this.GreaterThan( expected, actual );
            this.AreEqual( expected.CompareTo( actual ),                  EqualComparer<T>.Default.Compare( expected, actual ) );
            this.AreEqual( expected.CompareTo( actual ),                  Comparer<T>.Default.Compare( expected, actual ) );
            this.AreEqual( EqualComparer<T>.Default.Compare( expected, actual ), Comparer<T>.Default.Compare( expected, actual ) );
        }
    }


    [Test, TestCase( 1, 0 ), TestCase( double.Pi, double.Pi - 1 ), TestCase( 6UL, 5UL )]
    public void IsLessThan<T>( T expected, T actual )
        where T : IComparable<T>, IEquatable<T>
    {
        using ( EnterMultipleScope() )
        {
            this.LessThan( expected, actual );
            this.AreEqual( expected.CompareTo( actual ),                  EqualComparer<T>.Default.Compare( expected, actual ) );
            this.AreEqual( expected.CompareTo( actual ),                  Comparer<T>.Default.Compare( expected, actual ) );
            this.AreEqual( EqualComparer<T>.Default.Compare( expected, actual ), Comparer<T>.Default.Compare( expected, actual ) );
        }
    }


    [Test, TestCase( "false", "false" ), TestCase( "true", "true" ), TestCase( 0, 0 ), TestCase( double.Pi, double.Pi ), TestCase( 5UL, 5UL )]
    public void Equals<T>( T expected, T actual )
        where T : IComparable<T>, IEquatable<T>
    {
        using ( EnterMultipleScope() )
        {
            this.AreEqual( expected,                                     actual );
            this.AreEqual( expected.GetHashCode(),                       actual.GetHashCode() );
            this.AreEqual( expected.Equals( actual ),                    EqualComparer<T>.Default.Equals( expected, actual ) );
            this.AreEqual( expected.Equals( actual ),                    EqualityComparer<T>.Default.Equals( expected, actual ) );
            this.AreEqual( EqualComparer<T>.Default.Equals( expected, actual ), EqualityComparer<T>.Default.Equals( expected, actual ) );
        }
    }
}
