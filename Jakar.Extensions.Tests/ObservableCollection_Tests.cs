// Jakar.Extensions :: Jakar.Extensions.Tests
// 3/27/2024  10:39

using System.Linq;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf( typeof(ObservableCollection<>) )]
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
        this.AreEqual( value, collection.IndexOf( value ) );
        this.AreEqual( value, collection.LastIndexOf( value ) );
        this.AreEqual( value, collection.FindIndex( Match ) );
        this.AreEqual( value, collection.Find( Match ) );
        this.AreEqual( value, collection.FindLast( Match ) );
        this.AreEqual( 1,     collection.FindAll( Match ).Length );
        this.AreEqual( value, collection.FindAll( Match )[0] );
        return;

        bool Match( int x ) => x == value;
    }


    [Test]
    public void Sort()
    {
        int[] array  = [..Enumerable.Range( 0, 100 ).Select( static x => Random.Shared.Next( 1000 ) )];
        int[] sorted = [..array];
        Array.Sort( sorted, ValueSorter<int>.Default );

        ObservableCollection<int> collection = new(array, ValueSorter<int>.Default);
        collection.Sort();
        this.AreEqual( sorted, collection.ToArray() );

        collection.Clear();
        collection.Add( array );
        collection.Sort( ValueSorter<int>.Default.Compare );
        this.AreEqual( sorted, collection.ToArray() );
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
        collection.Add( value );
        this.False( collection.TryAdd( value ) );
        this.True( collection.Contains( value ) );
        this.True( collection.Remove( value ) );
        this.False( collection.Contains( value ) );
        collection.Add( value );
        collection.Clear();
        this.AreEqual( collection.Count, 0 );
    }
}
