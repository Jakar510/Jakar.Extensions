// Jakar.Extensions :: Jakar.Extensions.Tests
// 3/27/2024  10:39

using System.Linq;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf( typeof(ObservableCollection<>) )]

// ReSharper disable once InconsistentNaming
public class ObservableCollection_Tests : Assert
{
    [Test]
    [TestCase( 10 )]
    [TestCase( 20 )]
    [TestCase( 30 )]
    [TestCase( 40 )]
    public void Indexes( int value )
    {
        ObservableCollection<int> collection = [..Enumerable.Range( 0, 100 )];
        this.AreEqual( value - 1, collection.IndexOf( value ) );
        this.AreEqual( value - 1, collection.LastIndexOf( value ) );
        this.AreEqual( value - 1, collection.FindIndex( Match ) );
        this.AreEqual( value - 1, collection.Find( Match ) );
        this.AreEqual( value - 1, collection.FindLast( Match ) );
        this.AreEqual( 1,         collection.FindAll( Match ).Count );
        this.AreEqual( value - 1, collection.FindAll( Match )[0] );
        return;

        bool Match( int x ) => x == value;
    }


    [Test]
    public void Sort()
    {
        int[] array  = [..Enumerable.Range( 0, 100 ).Select( x => Random.Shared.Next( 1000 ) )];
        int[] sorted = [..array];
        Array.Sort( sorted, ValueSorter<int>.Default );

        ObservableCollection<int> collection = new(array, ValueSorter<int>.Default);
        collection.Sort();
        this.AreEquals<int>( sorted, collection.ToArray() );

        collection.Clear();
        collection.Add( array );
        collection.Sort( ValueSorter<int>.Default.Compare );
        this.AreEquals<int>( sorted, collection.ToArray() );
    }


    [Test]
    [TestCase( 1 )]
    [TestCase( 2 )]
    [TestCase( 3 )]
    [TestCase( 4 )]
    [TestCase( "1" )]
    [TestCase( "2" )]
    [TestCase( "3" )]
    [TestCase( "4" )]
    public void Run<T>( T value )
    {
        ObservableCollection<T> collection = [];
        this.True( collection.TryAdd( value ) );
        this.True( collection.Contains( value ) );
        this.True( collection.Remove( value ) );
        this.False( collection.Contains( value ) );
        collection.Add( value );
        this.False( collection.TryAdd( value ) );
        collection.Clear();
        this.AreEqual( collection.Count, 0 );
    }
}
