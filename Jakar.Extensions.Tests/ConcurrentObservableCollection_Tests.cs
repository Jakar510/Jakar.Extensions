using NUnit.Framework;



namespace Jakar.Extensions.Tests;


[ TestFixture ]

// ReSharper disable once InconsistentNaming
public class ConcurrentObservableCollection_Tests : Assert
{
    private readonly ConcurrentObservableCollection<int>    _integers = new();
    private readonly ConcurrentObservableCollection<string> _strings  = new();


    [ Test, TestCase( 1 ), TestCase( 2 ), TestCase( 3 ), TestCase( 4 ) ]
    public void Run( int value )
    {
        True( _integers.TryAdd( value ) );
        True( _integers.Contains( value ) );
        True( _integers.Remove( value ) );
        False( _integers.Contains( value ) );
    }


    [ Test, TestCase( "1" ), TestCase( "2" ), TestCase( "3" ), TestCase( "4" ) ]
    public void Run( string value )
    {
        True( _strings.TryAdd( value ) );
        True( _strings.Contains( value ) );
        True( _strings.Remove( value ) );
        False( _strings.Contains( value ) );
    }
}
