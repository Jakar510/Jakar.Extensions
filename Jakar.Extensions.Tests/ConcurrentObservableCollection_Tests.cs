using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests;


[TestFixture, TestOf( typeof(ConcurrentObservableCollection<>) )]

// ReSharper disable once InconsistentNaming
public class ConcurrentObservableCollection_Tests : Assert
{
    internal static ValueSorter<int> Sorter => ValueSorter<int>.Default;


    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public void IndexOf( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( value, collection.IndexOf( value ) );
        return;

        bool Match( int x ) => x == value;
    }

    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public void LastIndexOf( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( value, collection.LastIndexOf( value ) );
        return;

        bool Match( int x ) => x == value;
    }

    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public void Find( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( value, collection.Find( Match ) );
        return;

        bool Match( int x ) => x == value;
    }

    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public void FindLast( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( value, collection.FindLast( Match ) );
        return;

        bool Match( int x ) => x == value;
    }

    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public void FindAll( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( 1,     collection.FindAll( Match ).Length );
        this.AreEqual( value, collection.FindAll( Match )[0] );
        return;

        bool Match( int x ) => x == value;
    }


    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public async Task IndexOfAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( value, await collection.IndexOfAsync( value ) );
        return;

        bool Match( int x ) => x == value;
    }

    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public async Task LastIndexOfAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( value, await collection.LastIndexOfAsync( value ) );
    }

    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public async Task FindAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( value, await collection.FindAsync( Match ) );
        return;

        bool Match( int x ) => x == value;
    }

    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public async Task FindLastAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( value, await collection.FindLastAsync( Match ) );
        return;

        bool Match( int x ) => x == value;
    }

    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public async Task FindAllAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range( 0, 100 ));
        this.AreEqual( 1,     (await collection.FindAllAsync( Match )).Length );
        this.AreEqual( value, (await collection.FindAllAsync( Match ))[0] );
        return;

        bool Match( int x ) => x == value;
    }


    [Test]
    public void Sort()
    {
        ReadOnlyMemory<int> array  = new([..Enumerable.Range( 0, 100 ).Select( static x => Random.Shared.Next( 1000 ) )]);
        ReadOnlyMemory<int> sorted = GetSorted( array.Span );

        ConcurrentObservableCollection<int> collection = new(array.Span, Sorter);
        collection.Sort();
        this.AreEqual( sorted.Span, collection.ToArray() );

        collection.Clear();
        collection.Add( array );
        collection.Sort( Sorter );
        this.AreEqual( sorted.Span, collection.ToArray() );
    }

    [Test]
    public async Task SortAsync()
    {
        ReadOnlyMemory<int> array  = new([..Enumerable.Range( 0, 100 ).Select( static x => Random.Shared.Next( 1000 ) )]);
        ReadOnlyMemory<int> sorted = GetSorted( array.Span );

        ConcurrentObservableCollection<int> collection = new(array.Span, Sorter);
        await collection.SortAsync();
        this.AreEqual( sorted.Span, collection.ToArray() );

        await collection.ClearAsync();
        collection.Add( array.Span );
        await collection.SortAsync( Sorter );
        this.AreEqual( sorted.Span, collection.ToArray() );
    }
    private static ReadOnlyMemory<T> GetSorted<T>( scoped in ReadOnlySpan<T> array )
    {
        T[] sorted = [.. array];
        Array.Sort( sorted, Comparer<T>.Default );
        return sorted;
    }


    [Test, TestCase( 1 ), TestCase( 2 ), TestCase( 3 ), TestCase( 4 ), TestCase( "1" ), TestCase( "2" ), TestCase( "3" ), TestCase( "4" )]
    public void Run<T>( T value )
        where T : IEquatable<T>
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
    [Test, TestCase( 1 ), TestCase( 2 ), TestCase( 3 ), TestCase( 4 ), TestCase( "1" ), TestCase( "2" ), TestCase( "3" ), TestCase( "4" )]
    public async Task RunAsync<T>( T value )
        where T : IEquatable<T>
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
