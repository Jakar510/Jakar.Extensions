// Jakar.Extensions :: Jakar.Extensions.Tests
// 3/27/2024  10:39

using System.Collections.Generic;
using System.Linq;



namespace Jakar.Extensions.Tests.Collections;


[TestFixture]
[TestOf(typeof(ObservableCollection<>))]
public class ObservableCollection_Tests : Assert
{
    internal static Comparer<int> Sorter => Comparer<int>.Default;


    [Test] [TestCase(10)] [TestCase(20)] [TestCase(30)] [TestCase(40)] public void Indexes( int value )
    {
        ObservableCollection<int> collection = new(Enumerable.Range(0, 100)) { IsReadOnly = true };
        this.AreEqual(value, collection.IndexOf(value));
        this.AreEqual(value, collection.LastIndexOf(value));
        this.AreEqual(value, collection.Find(match));
        this.AreEqual(value, collection.FindLast(match));

        this.AreEqual(1,
                      collection.FindAll(match)
                                .Length);

        this.AreEqual(value, collection.FindAll(match)[0]);
        return;

        bool match( ref readonly int x ) => x == value;
    }


    [Test] public void Sort()
    {
        ReadOnlyMemory<int> array = new([
                                            ..Enumerable.Range(0, 100)
                                                        .Select(static x => Random.Shared.Next(1000))
                                        ]);

        ReadOnlyMemory<int> sorted = GetSorted(array.Span);

        ObservableCollection<int> collection = new(Sorter, array.Span);
        collection.Sort();
        this.AreEqual(sorted.Span, collection.ToArray());

        collection.Clear();
        collection.Add(array.Span);
        collection.Sort(Sorter);
        this.AreEqual(sorted.Span, collection.ToArray());
    }
    private static ReadOnlyMemory<TValue> GetSorted<TValue>( scoped in ReadOnlySpan<TValue> array )
    {
        TValue[] sorted = [.. array];
        Array.Sort(sorted, Comparer<TValue>.Default);
        return sorted;
    }


    [Test] [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)] [TestCase("1")] [TestCase("2")] [TestCase("3")] [TestCase("4")]
    public void Run<TValue>( TValue value )
        where TValue : IEquatable<TValue>
    {
        ObservableCollection<TValue> collection = [];
        collection.Add(value);
        this.IsTrue(collection.Contains(value));
        this.IsFalse(collection.TryAdd(value));
        this.IsTrue(collection.Remove(value));
        this.IsFalse(collection.Contains(value));
        collection.Add(value);
        collection.Clear();
        this.AreEqual(collection.Count, 0);
    }
}
