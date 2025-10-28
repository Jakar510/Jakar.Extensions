// Jakar.Extensions :: Jakar.Extensions
// 06/11/2025  14:09

namespace Jakar.Extensions;


public class LockFreeStack<TValue> : IReadOnlyCollection<TValue>
{
    private int   __count;
    private Node? __head;

    public int Count => Interlocked.CompareExchange(ref __count, 0, 0);


    public void Push( TValue value )
    {
        Node node = new(value);

        do { node.Next = __head; }
        while ( Interlocked.CompareExchange(ref __head, node, node.Next) != node.Next );

        Interlocked.Increment(ref __count);
    }
    public TValue? TryPop() => TryPop(out TValue? result)
                                   ? result
                                   : default;
    public bool TryPop( out TValue? result )
    {
        Node? oldHead;

        do
        {
            oldHead = __head;
            if ( oldHead is not null ) { continue; }

            result = default;
            return false;
        }
        while ( Interlocked.CompareExchange(ref __head, oldHead.Next, oldHead) != oldHead );

        Interlocked.Decrement(ref __count);
        result = oldHead.Value;
        return true;
    }


    public IEnumerator<TValue> GetEnumerator()
    {
        Node? current = __head;

        while ( current is not null )
        {
            yield return current.Value;
            current = current.Next;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    protected sealed class Node( TValue value ) : IEqualityOperators<Node>
    {
        public readonly TValue Value = value;
        public          Node?  Next;


        public          bool Equals( Node?   other )                => ReferenceEquals(this, other);
        public override bool Equals( object? obj )                  => ReferenceEquals(this, obj) || Equals(obj as Node);
        public override int  GetHashCode()                          => HashCode.Combine(Value);
        public static   bool operator ==( Node? left, Node? right ) => left?.Equals(right) is true;
        public static   bool operator !=( Node? left, Node? right ) => left?.Equals(right) is not true;
    }
}
