namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf( typeof(ConcurrentObservableCollection<>) )]

// ReSharper disable once InconsistentNaming
public class ConcurrentObservableCollection_Tests : Assert
{
    [Test]
    [TestCase( 1 )]
    [TestCase( 2 )]
    [TestCase( 3 )]
    [TestCase( 4 )]
    public void Run( int value )
    {
        ConcurrentObservableCollection<int> collection = [];
        this.True( collection.TryAdd( value ) );
        this.True( collection.Contains( value ) );
        this.True( collection.Remove( value ) );
        this.False( collection.Contains( value ) );
    }


    [Test]
    [TestCase( "1" )]
    [TestCase( "2" )]
    [TestCase( "3" )]
    [TestCase( "4" )]
    public void Run( string value )
    {
        ConcurrentObservableCollection<string> collection = [];
        this.True( collection.TryAdd( value ) );
        this.True( collection.Contains( value ) );
        this.True( collection.Remove( value ) );
        this.False( collection.Contains( value ) );
    }
}