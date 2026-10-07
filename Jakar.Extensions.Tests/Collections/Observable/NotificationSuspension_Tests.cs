// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/01/2026

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests.Collections;


[TestFixture]
[TestOf(typeof(NotificationSuspension))]
public sealed class NotificationSuspension_Tests : Assert
{
    private static (List<NotifyCollectionChangedEventArgs> Changes, List<string?> Properties) Record( ICollectionAlerts collection )
    {
        List<NotifyCollectionChangedEventArgs> changes    = [];
        List<string?>                          properties = [];
        collection.CollectionChanged += ( _, e ) => changes.Add(e);
        collection.PropertyChanged   += ( _, e ) => properties.Add(e.PropertyName);
        return ( changes, properties );
    }


    [Test] public void AddsInALoop_RaiseOneReset_WhenTheScopeEnds()
    {
        ObservableCollection<int> collection = new();
        ( List<NotifyCollectionChangedEventArgs> changes, List<string?> properties ) = Record(collection);

        using ( collection.SuspendNotifications() )
        {
            for ( int i = 0; i < 100; i++ ) { collection.Add(i); }

            this.IsTrue(collection.AreNotificationsSuspended);
            this.AreEqual(0, changes.Count);
            this.AreEqual(0, properties.Count);
        }

        this.IsFalse(collection.AreNotificationsSuspended);
        this.AreEqual(1,                                   changes.Count);
        this.AreEqual(NotifyCollectionChangedAction.Reset, changes[0].Action);
        That(properties, Is.EquivalentTo([nameof(collection.Count), nameof(collection.IsEmpty), nameof(collection.IsNotEmpty)]));
        this.AreEqual(100, collection.Count);
    }

    [Test] public void NoChanges_RaiseNothing()
    {
        ObservableCollection<int> collection = new(1, 2, 3);
        ( List<NotifyCollectionChangedEventArgs> changes, List<string?> properties ) = Record(collection);

        using ( collection.SuspendNotifications() ) { _ = collection.Contains(2); }

        this.AreEqual(0, changes.Count);
        this.AreEqual(0, properties.Count);
    }

    [Test] public void NestedScopes_RaiseOnceWhenTheOutermostEnds()
    {
        ObservableCollection<int> collection = new();
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(collection);

        using ( collection.SuspendNotifications() )
        {
            using ( collection.SuspendNotifications() ) { collection.Add(1); }

            this.AreEqual(0, changes.Count);
            collection.Add(2);
        }

        this.AreEqual(1, changes.Count);
    }

    [Test] public void ChangesThatKeepTheCount_StillRaiseReset_ButNotCount()
    {
        ObservableCollection<int> collection = new(3, 1, 2);
        ( List<NotifyCollectionChangedEventArgs> changes, List<string?> properties ) = Record(collection);

        using ( collection.SuspendNotifications() )
        {
            collection[0] = 7;
            collection.Sort();
        }

        this.AreEqual(NotifyCollectionChangedAction.Reset, changes.Single().Action);
        this.AreEqual(0,                                   properties.Count);
    }

    [Test] public void AfterTheScope_EventsAreRaisedNormally()
    {
        ObservableCollection<int> collection = new();
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(collection);

        using ( collection.SuspendNotifications() ) { collection.Add(1); }

        collection.Add(2);

        this.AreEqual(2,                                 changes.Count);
        this.AreEqual(NotifyCollectionChangedAction.Add, changes[1].Action);
    }

    [Test] public void DisposingTheSameScopeTwice_IsANoOp()
    {
        ObservableCollection<int> collection = new();
        NotificationSuspension    outer      = collection.SuspendNotifications();
        NotificationSuspension    inner      = collection.SuspendNotifications();

        inner.Dispose();
        inner.Dispose();
        this.IsTrue(collection.AreNotificationsSuspended); // outer is still open

        outer.Dispose();
        this.IsFalse(collection.AreNotificationsSuspended);
    }

    [Test] public void Default_IsANoOp()
    {
        NotificationSuspension scope = default;
        DoesNotThrow(scope.Dispose);
    }

    [Test] public void Dictionary_And_HashSet_AreSuspended()
    {
        ObservableDictionary<string, int> dictionary = new();
        ObservableHashSet<int>            set        = new();
        ( List<NotifyCollectionChangedEventArgs> dictionaryChanges, _ ) = Record(dictionary);
        ( List<NotifyCollectionChangedEventArgs> setChanges, _ )        = Record(set);

        using ( dictionary.SuspendNotifications() )
        using ( set.SuspendNotifications() )
        {
            for ( int i = 0; i < 10; i++ )
            {
                dictionary[i.ToString()] = i;
                set.Add(i);
            }
        }

        this.AreEqual(NotifyCollectionChangedAction.Reset, dictionaryChanges.Single().Action);
        this.AreEqual(NotifyCollectionChangedAction.Reset, setChanges.Single().Action);
    }

    [Test] public async Task Concurrent_AddsFromManyThreads_RaiseOneReset()
    {
        ConcurrentObservableCollection<int> collection = new();
        ( List<NotifyCollectionChangedEventArgs> changes, _ ) = Record(collection);

        using ( collection.SuspendNotifications() )
        {
            await Task.WhenAll(Enumerable.Range(0, 8)
                                         .Select(t => Task.Run(() =>
                                                               {
                                                                   for ( int i = 0; i < 250; i++ ) { collection.Add(t * 1000 + i); }
                                                               })));
        }

        this.AreEqual(2000, collection.Count);
        this.AreEqual(1,    changes.Count);
    }

    [Test] public async Task ScopeClosingWhileAnotherThreadChanges_NeverLosesANotification()
    {
        for ( int round = 0; round < 200; round++ )
        {
            ConcurrentObservableCollection<int> collection = new();
            int                                 events     = 0;
            collection.CollectionChanged += ( _, _ ) => Interlocked.Increment(ref events);

            NotificationSuspension scope  = collection.SuspendNotifications();
            Task                   writer = Task.Run(() => collection.Add(1));
            scope.Dispose();
            await writer;

            this.GreaterThan(0, events); // either the Reset or the Add itself was raised
        }
    }
}
