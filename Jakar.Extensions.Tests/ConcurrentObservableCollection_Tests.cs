namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf( typeof(ConcurrentObservableCollection<>) )]

// ReSharper disable once InconsistentNaming
public class ConcurrentObservableCollection_Tests : Assert
{
    private readonly ConcurrentObservableCollection<int>    _integers = new();
    private readonly ConcurrentObservableCollection<string> _strings  = new();


    [Test]
    [TestCase( 1 )]
    [TestCase( 2 )]
    [TestCase( 3 )]
    [TestCase( 4 )]
    public void Run( int value )
    {
        this.True( _integers.TryAdd( value ) );
        this.True( _integers.Contains( value ) );
        this.True( _integers.Remove( value ) );
        this.False( _integers.Contains( value ) );
    }


    [Test]
    [TestCase( "1" )]
    [TestCase( "2" )]
    [TestCase( "3" )]
    [TestCase( "4" )]
    public void Run( string value )
    {
        this.True( _strings.TryAdd( value ) );
        this.True( _strings.Contains( value ) );
        this.True( _strings.Remove( value ) );
        this.False( _strings.Contains( value ) );
    }
}
