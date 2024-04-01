using System.Linq;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf( typeof(ConcurrentObservableCollection<>) )]

// ReSharper disable once InconsistentNaming
public class ConcurrentObservableCollection_Tests : Assert
{
    [Test]
    [TestCase( 10 )]
    [TestCase( 20 )]
    [TestCase( 30 )]
    [TestCase( 40 )]
    public void Indexes( int value )
    {
        ConcurrentObservableCollection<int> collection = [..Enumerable.Range( 0, 100 )];
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
    [TestCase( 10 )]
    [TestCase( 20 )]
    [TestCase( 30 )]
    [TestCase( 40 )]
    public async Task IndexesAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = [..Enumerable.Range( 0, 100 )];
        this.AreEqual( value, await collection.IndexOfAsync( value ) );
        this.AreEqual( value, await collection.LastIndexOfAsync( value ) );
        this.AreEqual( value, await collection.FindIndexAsync( Match ) );
        this.AreEqual( value, await collection.FindAsync( Match ) );
        this.AreEqual( value, await collection.FindLastAsync( Match ) );
        this.AreEqual( 1,     (await collection.FindAllAsync( Match )).Length );
        this.AreEqual( value, (await collection.FindAllAsync( Match ))[0] );
        return;

        bool Match( int x ) => x == value;
    }


    [Test]
    public void Sort()
    {
        int[] array  = [..Enumerable.Range( 0, 100 ).Select( static x => Random.Shared.Next( 1000 ) )];
        int[] sorted = [..array];
        Array.Sort( sorted, ValueSorter<int>.Default );

        ConcurrentObservableCollection<int> collection = new(array, ValueSorter<int>.Default);
        collection.Sort();
        this.AreEqual( sorted, collection.ToArray() );

        collection.Clear();
        collection.Add( array );
        collection.Sort( ValueSorter<int>.Default.Compare );
        this.AreEqual( sorted, collection.ToArray() );
    }

    [Test]
    public async Task SortAsync()
    {
        int[] array  = [..Enumerable.Range( 0, 100 ).Select( static x => Random.Shared.Next( 1000 ) )];
        int[] sorted = [..array];
        Array.Sort( sorted, ValueSorter<int>.Default );

        ConcurrentObservableCollection<int> collection = new(array, ValueSorter<int>.Default);
        collection.Sort();
        this.AreEqual( sorted, collection.ToArray() );

        await collection.ClearAsync();
        collection.Add( array );
        await collection.SortAsync( ValueSorter<int>.Default.Compare );
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
        ConcurrentObservableCollection<T> collection = [];
        collection.Add( value );
        this.False( collection.TryAdd( value ) );
        this.True( collection.Contains( value ) );
        this.True( collection.Remove( value ) );
        this.False( collection.Contains( value ) );
        collection.Add( value );
        collection.Clear();
        this.AreEqual( collection.Count, 0 );
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
    public async Task RunAsync<T>( T value )
    {
        ConcurrentObservableCollection<T> collection = [];
        await collection.AddAsync( value );
        this.False( await collection.TryAddAsync( value ) );
        this.True( await collection.ContainsAsync( value ) );
        this.True( await collection.RemoveAsync( value ) );
        this.False( await collection.ContainsAsync( value ) );
        await collection.AddAsync( value );
        await collection.ClearAsync();
        this.AreEqual( collection.Count, 0 );
    }
}
