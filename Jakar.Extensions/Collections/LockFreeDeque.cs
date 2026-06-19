// Jakar.Extensions :: Jakar.Extensions
// 06/11/2025  14:20

namespace Jakar.Extensions;


public class LockFreeDeque<TValue> : IReadOnlyCollection<TValue>
    where TValue : class
{
    private int  __count;
    private Node __head = Node.Empty;
    private Node __tail = Node.Empty;

    // Volatile.Read gives the acquire fence needed for a correct atomic read.
    public int Count => Volatile.Read(ref __count);


    public void Enqueue( TValue value )
    {
        Node newNode = new(value);

        // Michael-Scott enqueue: link newNode at the tail, then advance the tail pointer.
        while ( true )
        {
            Node  tail = Volatile.Read(ref __tail);
            Node? next = Volatile.Read(ref tail.Next);

            if ( next is null )
            {
                // Tail is consistent — try to link newNode.
                if ( Interlocked.CompareExchange(ref tail.Next, newNode, null) is null )
                {
                    // Best-effort tail advance; if it races, the next thread will fix it.
                    Interlocked.CompareExchange(ref __tail, newNode, tail);
                    Interlocked.Increment(ref __count);
                    return;
                }
            }
            else
            {
                // Tail is lagging — help advance it before retrying.
                Interlocked.CompareExchange(ref __tail, next!, tail);
            }
        }
    }


    public TValue? TryDequeue() => TryDequeue(out TValue? result)
                                       ? result
                                       : null;
    public bool TryDequeue( out TValue? result )
    {
        // Michael-Scott dequeue: head is always the sentinel (dummy) node;
        // the real first value lives in head.Next.
        while ( true )
        {
            Node  head = Volatile.Read(ref __head);
            Node  tail = Volatile.Read(ref __tail);
            Node? next = Volatile.Read(ref head.Next);

            if ( ReferenceEquals(head, tail) )
            {
                if ( next is null )
                {
                    // Queue is empty.
                    result = null;
                    return false;
                }

                // Tail is lagging behind head — help it catch up and retry.
                Interlocked.CompareExchange(ref __tail, next!, tail);
            }
            else
            {
                Debug.Assert(next is not null, "next should not be null when head != tail");
                result = next!.Value;

                // Swing head to next (making next the new sentinel).
                // Only decrement the count after a successful CAS.
                if ( ReferenceEquals(Interlocked.CompareExchange(ref __head, next!, head), head) )
                {
                    Interlocked.Decrement(ref __count);
                    return true;
                }
            }
        }
    }


    public IEnumerator<TValue> GetEnumerator()
    {
        // __head is the sentinel (dummy) node — skip it and start from the first real node.
        Node? current = Volatile.Read(ref __head).Next;

        while ( current is not null )
        {
            yield return current.Value;
            current = Volatile.Read(ref current.Next);
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    protected sealed class Node( TValue value ) : IEqualityOperators<Node>
    {
        public static readonly Node   Empty = new(null!);
        public readonly        TValue Value = value;
        public                 Node?  Next;


        public          bool Equals( Node?   other )                => ReferenceEquals(this, other) || other is not null && Value.Equals(other.Value);
        public override bool Equals( object? obj )                  => ReferenceEquals(this, obj)   || Equals(obj as Node);
        public override int  GetHashCode()                          => HashCode.Combine(Value);
        public static   bool operator ==( Node? left, Node? right ) => left?.Equals(right) ?? right is null;
        public static   bool operator !=( Node? left, Node? right ) => !( left == right );
    }
}
