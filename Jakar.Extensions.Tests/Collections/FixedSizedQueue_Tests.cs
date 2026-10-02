// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Threading.Tasks;



namespace Jakar.Extensions.Tests.Collections;


[TestFixture]
[TestOf(typeof(FixedSizedQueue<>))]
public class FixedSizedQueue_Tests : Assert
{
    // ─── Basic state ──────────────────────────────────────────────────────────

    [Test]
    public void IsEmpty_OnConstruction()
    {
        FixedSizedQueue<int> q = new(5);
        this.IsTrue(q.IsEmpty);
        this.AreEqual(0, q.Count);
    }

    [Test]
    public void Length_MatchesConstructorArgument()
    {
        FixedSizedQueue<int> q = new(10);
        this.AreEqual(10, q.Length);
    }


    // ─── Enqueue / Count ──────────────────────────────────────────────────────

    [Test]
    public void Enqueue_SingleItem_CountIsOne()
    {
        FixedSizedQueue<int> q = new(5);
        q.Enqueue(42);
        this.AreEqual(1, q.Count);
        this.IsFalse(q.IsEmpty);
    }

    [Test]
    public void Enqueue_UpToCapacity_CountEqualsCapacity()
    {
        FixedSizedQueue<int> q = new(3);
        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        this.AreEqual(3, q.Count);
    }

    [Test]
    public void Enqueue_BeyondCapacity_CountDoesNotExceedLength()
    {
        FixedSizedQueue<int> q = new(3);
        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        q.Enqueue(4); // exceeds capacity
        q.Enqueue(5); // exceeds capacity
        this.AreEqual(3, q.Count);
    }


    // ─── Dequeue ──────────────────────────────────────────────────────────────

    [Test]
    public void Dequeue_ReturnsAndRemovesItem()
    {
        FixedSizedQueue<int> q = new(5);
        q.Enqueue(100);
        int? value = q.Dequeue();
        this.AreEqual(100, value);
        this.AreEqual(0,   q.Count);
    }

    [Test]
    public void Dequeue_AfterManyEnqueues_CountDecreases()
    {
        FixedSizedQueue<int> q = new(5);
        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        q.Dequeue();
        this.AreEqual(2, q.Count);
    }


    // ─── Contains ─────────────────────────────────────────────────────────────

    [Test]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        FixedSizedQueue<string> q = new(5);
        q.Enqueue("hello");
        this.IsTrue(q.Contains("hello"));
    }

    [Test]
    public void Contains_MissingItem_ReturnsFalse()
    {
        FixedSizedQueue<string> q = new(5);
        q.Enqueue("hello");
        this.IsFalse(q.Contains("world"));
    }

    [Test]
    public void Contains_AfterDequeue_ReturnsFalse()
    {
        FixedSizedQueue<string> q = new(5);
        q.Enqueue("hello");
        q.Dequeue();
        this.IsFalse(q.Contains("hello"));
    }

    // ─── Async variants ───────────────────────────────────────────────────────

    [Test]
    public async Task EnqueueAsync_AddsItem()
    {
        FixedSizedQueue<int> q = new(5);
        await q.EnqueueAsync(77).ConfigureAwait(false);
        this.AreEqual(1, q.Count);
    }

    [Test]
    public async Task DequeueAsync_ReturnsItem()
    {
        FixedSizedQueue<int> q = new(5);
        await q.EnqueueAsync(99).ConfigureAwait(false);
        int? value = await q.DequeueAsync().ConfigureAwait(false);
        this.AreEqual(99, value);
    }

    [Test]
    public async Task ContainsAsync_ExistingItem_ReturnsTrue()
    {
        FixedSizedQueue<int> q = new(5);
        q.Enqueue(55);
        bool result = await q.ContainsAsync(55).ConfigureAwait(false);
        this.IsTrue(result);
    }

    [Test]
    public async Task ContainsAsync_MissingItem_ReturnsFalse()
    {
        FixedSizedQueue<int> q = new(5);
        bool result = await q.ContainsAsync(55).ConfigureAwait(false);
        this.IsFalse(result);
    }


    // ─── Edge cases ───────────────────────────────────────────────────────────

    [Test]
    public void Enqueue_CapacityOne_ReplacesOnOverflow()
    {
        FixedSizedQueue<int> q = new(1);
        q.Enqueue(1);
        q.Enqueue(2); // should drop something to stay at capacity 1
        this.AreEqual(1, q.Count);
    }

    [Test]
    public void Enqueue_NullableReferenceType_Accepted()
    {
        FixedSizedQueue<string?> q = new(3);
        q.Enqueue(null);
        this.AreEqual(1, q.Count);
    }
}
