// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/01/2026

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests.Collections;


[TestFixture]
[TestOf(typeof(ObservableCollection<>))]
public sealed class ObservableBehavior_Tests : Assert
{
    private static (List<NotifyCollectionChangedEventArgs> Changes, List<string?> Properties) Record( INotifyCollectionChanged collection )
    {
        List<NotifyCollectionChangedEventArgs> changes    = [];
        List<string?>                          properties = [];
        collection.CollectionChanged                         += ( _, e ) => changes.Add(e);
        ( (INotifyPropertyChanged)collection ).PropertyChanged += ( _, e ) => properties.Add(e.PropertyName);
        return ( changes, properties );
    }


    // ─── Constructors ────────────────────────────────────────────────────────

    [Test] public void ArrayConstructor_KeepsTheValues()
    {
        int[]                     values     = [1, 2, 3];
        ObservableCollection<int> collection = new(values);
        Assert.That(collection.ToArray(), Is.EqualTo(values));
    }

    [Test] public void ConcurrentEnumerableComparerConstructor_KeepsTheValues()
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 5), Comparer<int>.Default);
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 0, 1, 2, 3, 4 }));
    }


    // ─── Remove ──────────────────────────────────────────────────────────────

    [Test] public void RemoveRange_RemovesEveryItemInTheRange()
    {
        ObservableCollection<int> collection = new(Enumerable.Range(0, 10));
        collection.RemoveRange(2, 5);
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 0, 1, 7, 8, 9 }));
    }

    [Test] public void RemoveRange_ToTheEnd_IsAllowed()
    {
        ObservableCollection<int> collection = new(Enumerable.Range(0, 10));
        collection.RemoveRange(5, 5);
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 0, 1, 2, 3, 4 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => collection.RemoveRange(3, 3));
    }

    [Test] public void RemoveMatch_RemovesAllMatches()
    {
        ObservableCollection<int> collection = new(Enumerable.Range(0, 10));
        this.AreEqual(5, collection.Remove(static ( ref readonly int x ) => x % 2 == 0));
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 1, 3, 5, 7, 9 }));
    }

    [Test] public void RemoveMatch_SingleItem_RaisesRemoveWithTheItem()
    {
        ObservableCollection<int> collection = new(Enumerable.Range(0, 5));
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(collection);

        this.AreEqual(1, collection.Remove(static ( ref readonly int x ) => x == 3));

        this.AreEqual(1,                                    changes.Count);
        this.AreEqual(NotifyCollectionChangedAction.Remove, changes[0].Action);
        this.AreEqual(3,                                    changes[0].OldItems![0]);
        this.AreEqual(3,                                    changes[0].OldStartingIndex);
    }

    [Test] public void Remove_RaisesRemove_NotAdd()
    {
        ObservableCollection<int> collection = new(1, 2, 3);
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(collection);

        this.IsTrue(collection.Remove(2));

        this.AreEqual(NotifyCollectionChangedAction.Remove, changes.Single().Action);
        this.AreEqual(2,                                    changes[0].OldItems![0]);
        this.AreEqual(1,                                    changes[0].OldStartingIndex);
    }

    [Test] public void HashSetRemove_RaisesRemove()
    {
        ObservableHashSet<int> set = new(1, 2, 3);
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(set);

        this.IsTrue(set.Remove(2));
        this.AreEqual(NotifyCollectionChangedAction.Remove, changes.Single().Action);
    }


    // ─── Insert / Add ────────────────────────────────────────────────────────

    [Test] public void InsertSpan_InsertsInOrder()
    {
        ObservableCollection<int> collection = new(0, 1, 5);
        collection.Insert(2, [2, 3, 4]);
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 0, 1, 2, 3, 4, 5 }));
    }

    [Test] public void InsertRepeated_InsertsInOrder()
    {
        ObservableCollection<int> collection = new(0, 1, 5);
        collection.Insert(2, 9, 3);
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 0, 1, 9, 9, 9, 5 }));

        collection.Add(7, 2);
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 0, 1, 9, 9, 9, 5, 7, 7 }));
    }

    [Test] public void AddSingleItemSpan_RaisesAdd_ManyRaiseReset_NoneRaisesNothing()
    {
        ObservableCollection<int> collection = new();
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(collection);

        collection.Add([1]);
        collection.Add([2, 3]);
        collection.Add(ReadOnlySpan<int>.Empty);

        this.AreEqual(2,                                   changes.Count);
        this.AreEqual(NotifyCollectionChangedAction.Add,   changes[0].Action);
        this.AreEqual(0,                                   changes[0].NewStartingIndex);
        this.AreEqual(NotifyCollectionChangedAction.Reset, changes[1].Action);
    }

    [Test] public void AddOrUpdate_DoesNotSort()
    {
        ObservableCollection<int> collection = new(3, 1, 2);
        collection.AddOrUpdate([5, 4]);
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 3, 1, 2, 5, 4 }));
    }

    [Test] public void Replace()
    {
        ObservableCollection<int> collection = new(0, 1, 2, 3);
        collection.Replace(1, [8, 9]);
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 0, 8, 9, 3 }));

        collection.Replace(0, 7, 2);
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 7, 7, 9, 3 }));

        Assert.Throws<ArgumentOutOfRangeException>(() => collection.Replace(3, [1, 2]));
    }


    // ─── Search ──────────────────────────────────────────────────────────────

    [Test] public void Find_IncludesTheLastItem()
    {
        ObservableCollection<int> collection = new(Enumerable.Range(0, 10));

        this.AreEqual(9, collection.FindIndex(static ( ref readonly int x ) => x == 9));
        this.AreEqual(9, collection.Find(static ( ref readonly int x ) => x == 9));
        this.AreEqual(9, collection.FindLast(static ( ref readonly int x ) => x == 9));
        this.AreEqual(1, collection.FindAll(static ( ref readonly int x ) => x == 9).Length);
    }

    [Test] public void FindLastIndex_FindsTheLastMatch()
    {
        ObservableCollection<int> collection = new(1, 2, 1, 2, 1);

        this.AreEqual(4,         collection.FindLastIndex(static ( ref readonly int x ) => x == 1));
        this.AreEqual(3,         collection.FindLastIndex(static ( ref readonly int x ) => x == 2));
        this.AreEqual(2,         collection.FindLastIndex(static ( ref readonly int x ) => x == 1, 0, 3));
        this.AreEqual(NOT_FOUND, collection.FindLastIndex(static ( ref readonly int x ) => x == 7));
        this.AreEqual(1,         collection.FindLast(static ( ref readonly int x ) => x       == 1));
    }

    [Test] public void Search_OnAnEmptyCollection_ReturnsNotFound()
    {
        ObservableCollection<int> collection = new();

        this.AreEqual(NOT_FOUND, collection.FindIndex(static ( ref readonly int _ ) => true));
        this.AreEqual(NOT_FOUND, collection.FindLastIndex(static ( ref readonly int _ ) => true));
        this.AreEqual(NOT_FOUND, collection.LastIndexOf(1, 0));
        this.AreEqual(0,         collection.Find(static ( ref readonly int _ ) => true));
        this.AreEqual(0,         collection.FindAll(static ( ref readonly int _ ) => true).Length);
        this.IsFalse(collection.Exists(static ( ref readonly int _ ) => true));
    }

    [Test] public void IndexOf_WithCount_UsesListSemantics()
    {
        ObservableCollection<int> collection = new(0, 1, 2, 3, 2);

        this.AreEqual(2,         collection.IndexOf(2, 0, 5));
        this.AreEqual(4,         collection.IndexOf(2, 3, 2));
        this.AreEqual(NOT_FOUND, collection.IndexOf(2, 3, 1));
        this.AreEqual(4,         collection.LastIndexOf(2, 4, 5));
        this.AreEqual(2,         collection.LastIndexOf(2, 3, 4));
    }


    // ─── IList ───────────────────────────────────────────────────────────────

    [Test] public void NonGenericIList_RaisesEvents_AndInsertsInsteadOfOverwriting()
    {
        ObservableCollection<int> collection = new(0, 1, 2);
        IList                     list       = collection;
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(collection);

        this.AreEqual(3, list.Add(3)); // the index of the new item
        list.Insert(0, -1);
        list.Remove(1);

        Assert.That(collection.ToArray(), Is.EqualTo(new[] { -1, 0, 2, 3 }));
        this.AreEqual(3, changes.Count);
        Assert.Throws<ArgumentException>(() => list.Add("not an int"));
    }

    [Test] public void NonGenericIList_RespectsReadOnly()
    {
        ObservableCollection<int> collection = new(Enumerable.Range(0, 3)) { IsReadOnly = true };
        Assert.Throws<NotSupportedException>(() => ( (IList)collection ).Add(3));
        Assert.Throws<NotSupportedException>(() => ( (IList)collection ).Insert(0, 3));
    }

    [Test] public void CopyTo_WithSourceAndLength()
    {
        ObservableCollection<int> collection = new(0, 1, 2, 3, 4);
        int[]                     array      = new int[4];
        collection.CopyTo(array, 1, 2, 3);
        Assert.That(array, Is.EqualTo(new[] { 0, 3, 4, 0 }));
    }


    // ─── Notifications ───────────────────────────────────────────────────────

    [Test] public void IsEmpty_IsOnlyRaisedWhenItChanges()
    {
        ObservableCollection<int> collection = new();
        ( _, List<string?> properties ) = Record(collection);

        collection.Add(1); // empty -> not empty
        collection.Add(2);
        collection.Remove(1);
        collection.Remove(2); // not empty -> empty

        this.AreEqual(4, properties.Count(static x => x == nameof(ObservableCollection<int>.Count)));
        this.AreEqual(2, properties.Count(static x => x == nameof(ObservableCollection<int>.IsEmpty)));
        this.AreEqual(2, properties.Count(static x => x == nameof(ObservableCollection<int>.IsNotEmpty)));
    }

    [Test] public void WithoutSubscribers_CountIsStillRaised()
    {
        ObservableCollection<int> collection = new();
        List<string?>             properties = [];
        collection.PropertyChanged += ( _, e ) => properties.Add(e.PropertyName);

        collection.Add(1);
        collection.Add([2, 3]);
        collection.Clear();

        this.AreEqual(3, properties.Count(static x => x == nameof(ObservableCollection<int>.Count)));
        this.AreEqual(2, properties.Count(static x => x == nameof(ObservableCollection<int>.IsEmpty))); // add first, clear
    }

    [Test] public void OverriddenOnChanged_StillReceivesArgs_WithoutSubscribers()
    {
        Recording collection = new();
        collection.Add(1);
        collection.RemoveAt(0);

        Assert.That(collection.Actions, Is.EqualTo(new[] { NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Remove }));
    }

    [Test] public void Filter_OverrideAndOverrideFilter_AreApplied()
    {
        Recording collection = new();
        collection.Add([1, 2, 3, 4]);
        Assert.That(collection.ToList(), Is.EqualTo(new[] { 2, 4 })); // overridden Filter keeps evens

        ObservableCollection<int> plain = new(1, 2, 3, 4);
        Assert.That(plain.ToList(), Is.EqualTo(new[] { 1, 2, 3, 4 }));

        plain.OverrideFilter = static ( int _, ref readonly int x ) => x > 2;
        Assert.That(plain.ToList(), Is.EqualTo(new[] { 3, 4 }));
    }


    // ─── Dictionaries / sets ─────────────────────────────────────────────────

    [Test] public void Dictionary_Indexer_AddsThenReplaces()
    {
        ObservableDictionary<string, int> dictionary = new();
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(dictionary);

        dictionary["a"] = 1;
        dictionary["a"] = 2;

        this.AreEqual(2,                                     dictionary["a"]);
        this.AreEqual(NotifyCollectionChangedAction.Add,     changes[0].Action);
        this.AreEqual(NotifyCollectionChangedAction.Replace, changes[1].Action);
        this.AreEqual(new KeyValuePair<string, int>("a", 1), changes[1].OldItems![0]);
    }

    [Test] public void Dictionary_PairOperations_MatchTheValue()
    {
        ObservableDictionary<string, int> dictionary = new() { ["a"] = 1 };
        ICollection<KeyValuePair<string, int>> pairs = dictionary;

        this.IsTrue(pairs.Contains(new KeyValuePair<string, int>("a", 1)));
        this.IsFalse(pairs.Contains(new KeyValuePair<string, int>("a", 2)));
        this.IsFalse(pairs.Remove(new KeyValuePair<string, int>("a", 2)));
        this.IsTrue(pairs.Remove(new KeyValuePair<string, int>("a", 1)));
        this.AreEqual(0, dictionary.Count);
    }

    [Test] public void Dictionary_CopyTo_HonoursTheStartIndex()
    {
        ObservableDictionary<string, int> dictionary = new() { ["a"] = 1, ["b"] = 2 };
        KeyValuePair<string, int>[]       array      = new KeyValuePair<string, int>[3];
        dictionary.CopyTo(array, 1);

        this.AreEqual(default(KeyValuePair<string, int>), array[0]);
        string[] keys = [array[1].Key, array[2].Key];
        Array.Sort(keys);
        Assert.That(keys, Is.EqualTo(new[] { "a", "b" }));
    }

    [Test] public void ConcurrentDictionary_Indexer_AddsThenReplaces_AndRemoveMatchesTheValue()
    {
        ObservableConcurrentDictionary<string, int> dictionary = new();
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(dictionary);

        dictionary["a"] = 1;
        dictionary["a"] = 2;

        this.AreEqual(NotifyCollectionChangedAction.Add,     changes[0].Action);
        this.AreEqual(NotifyCollectionChangedAction.Replace, changes[1].Action);
        this.IsTrue(dictionary.ContainsValue(2));
        this.IsFalse(dictionary.Remove(new KeyValuePair<string, int>("a", 1)));
        this.IsTrue(dictionary.Remove(new KeyValuePair<string, int>("a", 2)));

        KeyValuePair<string, int>[] array = new KeyValuePair<string, int>[2];
        dictionary["b"] = 3;
        dictionary.CopyTo(array, 1);
        this.AreEqual("b", array[1].Key);
    }

    [Test] public async Task ConcurrentDictionary_Indexer_ReportsEveryReplacementExactlyOnce()
    {
        ObservableConcurrentDictionary<int, int> dictionary = new();
        ConcurrentBag<int>                       replaced   = [];
        int                                      added      = 0;

        dictionary.CollectionChanged += ( _, e ) =>
                                        {
                                            if ( e.Action == NotifyCollectionChangedAction.Add ) { Interlocked.Increment(ref added); }
                                            else if ( e.Action == NotifyCollectionChangedAction.Replace ) { replaced.Add(( (KeyValuePair<int, int>)e.OldItems![0]! ).Value); }
                                        };

        await Task.WhenAll(Enumerable.Range(1, 8).Select(t => Task.Run(() =>
                                                                       {
                                                                           for ( int i = 0; i < 1000; i++ ) { dictionary[0] = ( t * 10_000 ) + i; }
                                                                       })));

        this.AreEqual(1,        added);
        this.AreEqual(8000 - 1, replaced.Count);
        this.AreEqual(replaced.Count, replaced.Distinct().Count()); // every old value was replaced exactly once
    }

    [Test] public void ConcurrentDictionary_Enumerate_WhileAdding_DoesNotOverflow()
    {
        ObservableConcurrentDictionary<int, int> dictionary = new();
        for ( int i = 0; i < 100; i++ ) { dictionary[i] = i; }

        // a bounded writer: the dictionary grows while it is being enumerated
        Task writer = Task.Run(() =>
                               {
                                   for ( int i = 100; i < 20_000; i++ ) { dictionary[i] = i; }
                               });

        while ( !writer.IsCompleted ) { Assert.DoesNotThrow(() => _ = dictionary.ToList()); }

        writer.Wait();
        this.AreEqual(20_000, dictionary.ToList().Count);
    }

    [Test] public void HashSet_SetOperations_OnlyNotifyWhenChanged()
    {
        ObservableHashSet<int> set = new(1, 2, 3);
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(set);

        set.UnionWith([1, 2]);
        set.ExceptWith([7]);
        set.IntersectWith([1, 2, 3, 4]);
        this.AreEqual(0, changes.Count);

        set.UnionWith([4]);
        this.AreEqual(1, changes.Count);
    }


    // ─── Concurrent collection ───────────────────────────────────────────────

    [Test] public void Concurrent_TrimExcess_ReleasesTheLock()
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 10));
        collection.TrimExcess();

        bool acquired = Task.Run(() =>
                                 {
                                     if ( !collection.Lock.TryEnter() ) { return false; }

                                     collection.Lock.Exit();
                                     return true;
                                 }).Result;

        this.IsTrue(acquired);
    }

    [Test] public async Task Concurrent_ParallelMutations_AreConsistent()
    {
        ConcurrentObservableCollection<int> collection = new();

        await Task.WhenAll(Enumerable.Range(0, 8).Select(t => Task.Run(() =>
                                                                       {
                                                                           for ( int i = 0; i < 500; i++ )
                                                                           {
                                                                               collection.Add(t, 2);
                                                                               collection.Insert(0, -1, 2);
                                                                               collection.Remove(static ( ref readonly int x ) => x < 0);
                                                                           }
                                                                       })));

        this.AreEqual(8 * 500 * 2, collection.Count); // every insert of -1 was removed again
        for ( int t = 0; t < 8; t++ ) { this.AreEqual(500 * 2, collection.FindCount(( ref readonly int x ) => x == t)); }
    }



    private sealed class Recording() : ObservableCollection<Recording, int>(Comparer<int>.Default), ICollectionAlerts<Recording, int>
    {
        public readonly List<NotifyCollectionChangedAction> Actions = [];


        protected override void OnChanged( NotifyCollectionChangedEventArgs e )
        {
            Actions.Add(e.Action);
            base.OnChanged(e);
        }
        protected override bool Filter( int index, ref readonly int value ) => value % 2 == 0;


        public static implicit operator Recording( List<int>                  values ) => throw new NotSupportedException();
        public static implicit operator Recording( HashSet<int>               values ) => throw new NotSupportedException();
        public static implicit operator Recording( ConcurrentBag<int>         values ) => throw new NotSupportedException();
        public static implicit operator Recording( System.Collections.ObjectModel.Collection<int> values ) => throw new NotSupportedException();
        public static implicit operator Recording( int[]                      values ) => throw new NotSupportedException();
        public static implicit operator Recording( System.Collections.Immutable.ImmutableArray<int> values ) => throw new NotSupportedException();
        public static implicit operator Recording( ReadOnlyMemory<int>        values ) => throw new NotSupportedException();
        public static implicit operator Recording( ReadOnlySpan<int>          values ) => throw new NotSupportedException();
        public override int GetHashCode()                => RuntimeHelpers.GetHashCode(this);
        public override bool Equals( object? other )    => ReferenceEquals(this, other);
        public static bool operator ==( Recording? l, Recording? r ) => ReferenceEquals(l, r);
        public static bool operator !=( Recording? l, Recording? r ) => !ReferenceEquals(l, r);
        public static bool operator >( Recording   l, Recording  r ) => l.CompareTo(r) > 0;
        public static bool operator >=( Recording  l, Recording  r ) => l.CompareTo(r) >= 0;
        public static bool operator <( Recording   l, Recording  r ) => l.CompareTo(r) < 0;
        public static bool operator <=( Recording  l, Recording  r ) => l.CompareTo(r) <= 0;
    }
}
