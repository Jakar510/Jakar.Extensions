// Jakar.Extensions :: Jakar.Extensions
// 06/11/2025  14:20

namespace Jakar.Extensions;


public class LockFreeDeque<TValue> : IReadOnlyCollection<TValue>
    where TValue : class
{
    private int  _count;
    private Node _head = Node.Empty;
    private Node _tail = Node.Empty;
    public  int  Count => Interlocked.CompareExchange( ref _count, 0, 0 );


    public void Enqueue( TValue value )
    {
        Node newNode = new(value);

        while ( true )
        {
            Node  tail = _tail;
            Node? next = tail.next;

            if ( next is null )
            {
                if ( Interlocked.CompareExchange( ref tail.next, newNode, null ) is null )
                {
                    Interlocked.CompareExchange( ref _tail, newNode, tail );
                    Interlocked.Increment( ref _count );
                    return;
                }
            }
            else { Interlocked.CompareExchange( ref _tail, next, tail ); }
        }
    }


    public TValue? TryDequeue() => TryDequeue( out TValue? result )
                                       ? result
                                       : null;
    public bool TryDequeue( out TValue? result )
    {
        while ( true )
        {
            Interlocked.Decrement( ref _count );
            Node  head = _head;
            Node  tail = _tail;
            Node? next = head.next;

            if ( head == tail )
            {
                if ( next is null )
                {
                    result = null;
                    return false;
                }

                Interlocked.CompareExchange( ref _tail, next, tail );
            }
            else
            {
                Debug.Assert( next is not null, "next should not be null when head != tail" );
                result = next.Value;
                if ( Interlocked.CompareExchange( ref _head, next, head ) == head ) { return true; }
            }
        }
    }


    public IEnumerator<TValue> GetEnumerator()
    {
        Node? current = _head;

        while ( current is not null )
        {
            yield return current.Value;
            current = current.next;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    protected sealed class Node( TValue value ) : IEqualityOperators<Node>
    {
        public static readonly Node   Empty = new(null!);
        public readonly        TValue Value = value;
        public                 Node?  next;


        public          bool Equals( Node?   other )                => ReferenceEquals( this, other );
        public override bool Equals( object? obj )                  => ReferenceEquals( this, obj ) || Equals( obj as Node );
        public override int  GetHashCode()                          => HashCode.Combine( Value );
        public static   bool operator ==( Node? left, Node? right ) => left?.Equals( right ) is true;
        public static   bool operator !=( Node? left, Node? right ) => left?.Equals( right ) is not true;
    }
}
