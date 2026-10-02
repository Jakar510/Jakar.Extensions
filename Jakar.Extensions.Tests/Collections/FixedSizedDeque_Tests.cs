// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Threading.Tasks;



namespace Jakar.Extensions.Tests.Collections;


[TestFixture]
[TestOf(typeof(FixedSizedDeque<>))]
public class FixedSizedDeque_Tests : Assert
{
    // ─── Basic state ──────────────────────────────────────────────────────────

    [Test]
    public void IsEmpty_OnConstruction()
    {
        FixedSizedDeque<int> q = new(5);
        this.IsTrue(q.IsEmpty);
        this.AreEqual(0, q.Count);
    }

    [Test]
    public void Length_MatchesConstructorArgument()
    {
        FixedSizedDeque<int> q = new(10);
        this.AreEqual(10, q.Length);
    }


    // ─── Enqueue / Count ──────────────────────────────────────────────────────

    [Test]
    public void Enqueue_SingleItem_CountIsOne()
    {
        FixedSizedDeque<int> q = new(5);
        q.Enqueue(42);
        this.AreEqual(1, q.Count);
        this.IsFalse(q.IsEmpty);
    }

    [Test]
    public void Enqueue_UpToCapacity_CountEqualsCapacity()
    {
        FixedSizedDeque<int> q = new(3);
        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        this.AreEqual(3, q.Count);
    }

    [Test]
    public void Enqueue_BeyondCapacity_CountDoesNotExceedLength()
    {
        FixedSizedDeque<int> q = new(3);
        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        q.Enqueue(4);
        q.Enqueue(5);
        this.AreEqual(3, q.Count);
    }


    // ─── Dequeue ──────────────────────────────────────────────────────────────

    [Test]
    public void Dequeue_ReturnsItem()
    {
        FixedSizedDeque<int> q = new(5);
        q.Enqueue(100);
        int value = q.Dequeue();
        this.AreEqual(100, value);
        this.AreEqual(0,   q.Count);
    }

    [Test]
    public void Dequeue_AfterManyEnqueues_CountDecreases()
    {
        FixedSizedDeque<int> q = new(5);
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
        FixedSizedDeque<string> q = new(5);
        q.Enqueue("hello");
        this.IsTrue(q.Contains("hello"));
    }

    [Test]
    public void Contains_MissingItem_ReturnsFalse()
    {
        FixedSizedDeque<string> q = new(5);
        q.Enqueue("hello");
        this.IsFalse(q.Contains("world"));
    }

    [Test]
    public void Contains_AfterDequeue_ReturnsFalse()
    {
        FixedSizedDeque<string> q = new(5);
        q.Enqueue("hello");
        q.Dequeue();
        this.IsFalse(q.Contains("hello"));
    }


    // ─── Async variants ───────────────────────────────────────────────────────

    [Test]
    public async Task EnqueueAsync_AddsItem()
    {
        FixedSizedDeque<int> q = new(5);
        await q.EnqueueAsync(77).ConfigureAwait(false);
        this.AreEqual(1, q.Count);
    }

    [Test]
    public async Task DequeueAsync_ReturnsItem()
    {
        FixedSizedDeque<int> q = new(5);
        await q.EnqueueAsync(99).ConfigureAwait(false);
        int value = await q.DequeueAsync().ConfigureAwait(false);
        this.AreEqual(99, value);
    }

    [Test]
    public async Task ContainsAsync_ExistingItem_ReturnsTrue()
    {
        FixedSizedDeque<int> q = new(5);
        q.Enqueue(55);
        bool result = await q.ContainsAsync(55).ConfigureAwait(false);
        this.IsTrue(result);
    }


    // ─── Thread safety (smoke test) ───────────────────────────────────────────

    [Test]
    public void ConcurrentEnqueue_DoesNotExceedCapacity()
    {
        const int CAPACITY  = 5;
        const int ADDITIONS = 50;

        FixedSizedDeque<int> q = new(CAPACITY);

        System.Threading.Tasks.Parallel.For(0, ADDITIONS, i => q.Enqueue(i));

        this.IsTrue(q.Count <= CAPACITY);
    }


    // ─── Edge cases ───────────────────────────────────────────────────────────

    [Test]
    public void Enqueue_CapacityOne_ReplacesOnOverflow()
    {
        FixedSizedDeque<int> q = new(1);
        q.Enqueue(1);
        q.Enqueue(2);
        this.AreEqual(1, q.Count);
    }
}
