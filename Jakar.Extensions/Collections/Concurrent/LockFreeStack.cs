// Jakar.Extensions :: Jakar.Extensions
// 06/11/2025  14:09

namespace Jakar.Extensions;


public sealed class LockFreeStack<TValue> : IReadOnlyCollection<TValue>
{
    private int   __count;
    private Node? __head;

    // Volatile.Read gives the acquire fence needed for a correct atomic read.
    public int Count => Volatile.Read(ref __count);


    public void Push( TValue value )
    {
        Node  node = new(value);
        Node? oldHead;

        // CAS retry loop: read head, link new node behind it, atomically swap only if head hasn't moved since we read it.
        do
        {
            oldHead   = Volatile.Read(ref __head);
            node.Next = oldHead;
        }
        while ( Interlocked.CompareExchange(ref __head, node, oldHead) != oldHead );

        Interlocked.Increment(ref __count);
    }

    public TValue? TryPop() => Count > 0 && TryPop(out TValue? result)
                                   ? result
                                   : default;
    public bool TryPop( out TValue? result )
    {
        Node? oldHead;

        // CAS retry loop: read head, bail if empty, atomically advance head only if it hasn't moved since we read it.
        do
        {
            oldHead = Volatile.Read(ref __head);

            if ( oldHead is null )
            {
                result = default;
                return false;
            }
        }
        while ( Interlocked.CompareExchange(ref __head, oldHead.Next, oldHead) != oldHead );

        Interlocked.Decrement(ref __count);
        result = oldHead.Value;
        return true;
    }


    public bool Contains( TValue value )
    {
        Node? current = Volatile.Read(ref __head);

        while ( current is not null )
        {
            if ( EqualityComparer<TValue>.Default.Equals(current.Value, value) ) { return true; }

            current = current.Next;
        }

        return false;
    }
    public IEnumerator<TValue> GetEnumerator()
    {
        Node? current = Volatile.Read(ref __head);

        while ( current is not null )
        {
            yield return current.Value;
            current = current.Next;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    private sealed class Node( TValue value ) : IEqualityOperators<Node>
    {
        public readonly TValue Value = value;
        public          Node?  Next;

        public          bool Equals( Node?   other )                => ReferenceEquals(this, other) || other is not null && EqualityComparer<TValue?>.Default.Equals(Value, other.Value);
        public override bool Equals( object? obj )                  => ReferenceEquals(this, obj)   || Equals(obj as Node);
        public override int  GetHashCode()                          => HashCode.Combine(Value);
        public static   bool operator ==( Node? left, Node? right ) => left?.Equals(right) ?? right is null;
        public static   bool operator !=( Node? left, Node? right ) => !( left == right );
    }
}
