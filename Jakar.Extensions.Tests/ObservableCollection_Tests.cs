// Jakar.Extensions :: Jakar.Extensions.Tests
// 3/27/2024  10:39

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests;


[TestFixture, TestOf( typeof(ObservableCollection<>) )]
public class ObservableCollection_Tests : Assert
{
    internal static ValueSorter<int> Sorter => ValueSorter<int>.Default;


    [Test, TestCase( 10 ), TestCase( 20 ), TestCase( 30 ), TestCase( 40 )]
    public void Indexes( int value )
    {
        ObservableCollection<int> collection = new(Enumerable.Range( 0, 100 )) { IsReadOnly = true };
        this.AreEqual( value, collection.IndexOf( value ) );
        this.AreEqual( value, collection.LastIndexOf( value ) );
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
        ReadOnlyMemory<int> array  = new([..Enumerable.Range( 0, 100 ).Select( static x => Random.Shared.Next( 1000 ) )]);
        ReadOnlyMemory<int> sorted = GetSorted( array.Span );

        ObservableCollection<int> collection = new(Sorter, array.Span);
        collection.Sort();
        this.AreEqual( sorted.Span, collection.ToArray() );

        collection.Clear();
        collection.Add( array );
        collection.Sort( Sorter );
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
        ObservableCollection<T> collection = [];
        collection.Add( value );
        this.True( collection.Contains( value ) );
        this.False( collection.TryAdd( value ) );
        this.True( collection.Remove( value ) );
        this.False( collection.Contains( value ) );
        collection.Add( value );
        collection.Clear();
        this.AreEqual( collection.Count, 0 );
    }
}
