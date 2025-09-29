// Jakar.Extensions :: Jakar.Extensions
// 06/11/2025  14:09

namespace Jakar.Extensions;


public class LockFreeStack<TValue> : IReadOnlyCollection<TValue>
{
    private Node? __head;
    private int   __count;

    public int Count => Interlocked.CompareExchange( ref __count, 0, 0 );


    public void Push( TValue value )
    {
        Node node = new(value);

        do { node.next = __head; }
        while ( Interlocked.CompareExchange( ref __head, node, node.next ) != node.next );

        Interlocked.Increment( ref __count );
    }
    public TValue? TryPop() => TryPop( out TValue? result )
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
        while ( Interlocked.CompareExchange( ref __head, oldHead.next, oldHead ) != oldHead );

        Interlocked.Decrement( ref __count );
        result = oldHead.Value;
        return true;
    }



    protected sealed class Node( TValue value ) : IEqualityOperators<Node>
    {
        public readonly TValue Value = value;
        public          Node?  next;


        public          bool Equals( Node?   other )                => ReferenceEquals( this, other );
        public override bool Equals( object? obj )                  => ReferenceEquals( this, obj ) || Equals( obj as Node );
        public override int  GetHashCode()                          => HashCode.Combine( Value );
        public static   bool operator ==( Node? left, Node? right ) => left?.Equals( right ) is true;
        public static   bool operator !=( Node? left, Node? right ) => left?.Equals( right ) is not true;
    }



    public IEnumerator<TValue> GetEnumerator()
    {
        Node? current = __head;

        while ( current is not null )
        {
            yield return current.Value;
            current = current.next;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
