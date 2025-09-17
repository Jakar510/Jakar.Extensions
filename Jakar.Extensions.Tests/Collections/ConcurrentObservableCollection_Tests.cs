using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests.Collections;


[TestFixture, TestOf(typeof(ConcurrentObservableCollection<>))]

// ReSharper disable once InconsistentNaming
public class ConcurrentObservableCollection_Tests : Assert
{
    internal static Comparer<int> Sorter => Comparer<int>.Default;


    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public void IndexOf( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(value, collection.IndexOf(value));
        return;
    }

    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public void LastIndexOf( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(value, collection.LastIndexOf(value));
        return;
    }

    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public void Find( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(value, collection.Find(match));
        return;

        bool match( ref readonly int x ) => x == value;
    }

    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public void FindLast( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(value, collection.FindLast(match));
        return;

        bool match( ref readonly int x ) => x == value;
    }

    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public void FindAll( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(1,     collection.FindAll(match).Length);
        this.AreEqual(value, collection.FindAll(match)[0]);
        return;

        bool match( ref readonly int x ) => x == value;
    }


    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public async Task IndexOfAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(value, await collection.IndexOfAsync(value));
    }

    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public async Task LastIndexOfAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(value, await collection.LastIndexOfAsync(value));
    }

    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public async Task FindAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(value, await collection.FindAsync(match));
        return;

        bool match( ref readonly int x ) => x == value;
    }

    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public async Task FindLastAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(value, await collection.FindLastAsync(match));
        return;

        bool match( ref readonly int x ) => x == value;
    }

    [Test, TestCase(10), TestCase(20), TestCase(30), TestCase(40)]
    public async Task FindAllAsync( int value )
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 100));
        this.AreEqual(1,     ( await collection.FindAllAsync(match) ).Length);
        this.AreEqual(value, ( await collection.FindAllAsync(match) )[0]);
        return;

        bool match( ref readonly int x ) => x == value;
    }


    [Test]
    public void Sort()
    {
        ReadOnlyMemory<int> array  = new([..Enumerable.Range(0, 100).Select(static x => Random.Shared.Next(1000))]);
        ReadOnlyMemory<int> sorted = GetSorted(array.Span);

        ConcurrentObservableCollection<int> collection = new(Sorter, array.Span);
        collection.Sort();
        this.AreEqual(sorted.Span, collection.ToArray());

        collection.Clear();
        collection.Add(array.Span);
        collection.Sort(Sorter);
        this.AreEqual(sorted.Span, collection.ToArray());
    }

    [Test]
    public async Task SortAsync()
    {
        ReadOnlyMemory<int> array  = new([..Enumerable.Range(0, 100).Select(static x => Random.Shared.Next(1000))]);
        ReadOnlyMemory<int> sorted = GetSorted(array.Span);

        ConcurrentObservableCollection<int> collection = new(Sorter, array.Span);
        await collection.SortAsync();
        this.AreEqual(sorted.Span, collection.ToArray());

        await collection.ClearAsync();
        collection.Add(array.Span);
        await collection.SortAsync(Sorter);
        this.AreEqual(sorted.Span, collection.ToArray());
    }
    private static ReadOnlyMemory<TValue> GetSorted<TValue>( scoped in ReadOnlySpan<TValue> array )
    {
        TValue[] sorted = [.. array];
        Array.Sort(sorted, Comparer<TValue>.Default);
        return sorted;
    }


    [Test, TestCase(1), TestCase(2), TestCase(3), TestCase(4), TestCase("1"), TestCase("2"), TestCase("3"), TestCase("4")]
    public void Run<TValue>( TValue value )
        where TValue : IEquatable<TValue>
    {
        ConcurrentObservableCollection<TValue> collection = [];
        collection.Add(value);
        this.IsFalse(collection.TryAdd(value));
        this.IsTrue(collection.Contains(value));
        this.IsTrue(collection.Remove(value));
        this.IsFalse(collection.Contains(value));
        collection.Add(value);
        collection.Clear();
        this.AreEqual(collection.Count, 0);
    }
    [Test, TestCase(1), TestCase(2), TestCase(3), TestCase(4), TestCase("1"), TestCase("2"), TestCase("3"), TestCase("4")]
    public async Task RunAsync<TValue>( TValue value )
        where TValue : IEquatable<TValue>
    {
        ConcurrentObservableCollection<TValue> collection = [];
        await collection.AddAsync(value);
        this.IsFalse(await collection.TryAddAsync(value));
        this.IsTrue(await collection.ContainsAsync(value));
        this.IsTrue(await collection.RemoveAsync(value));
        this.IsFalse(await collection.ContainsAsync(value));
        await collection.AddAsync(value);
        await collection.ClearAsync();
        this.AreEqual(collection.Count, 0);
    }
}
