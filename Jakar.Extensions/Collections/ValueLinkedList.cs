// Jakar.Extensions :: Jakar.Extensions
// 08/12/2025  08:20

using ZLinq;



namespace Jakar.Extensions;


public sealed class ValueLinkedList<T> : IReadOnlyCollection<T>, IValueEnumerable<ValueLinkedList<T>.ValueEnumerator, T>
{
    private readonly Lock  __lock = new();
    private          int   __count;
    private          Node? __last;


    public int Count
    {
        get
        {
            lock ( __lock ) { return __count; }
        }
    }
    public bool            IsReadOnly => false;
    public Node?           First      { get; private set; }
    public ValueEnumerator Values     => new(this);


    public ValueLinkedList() { }
    public ValueLinkedList( T firstValue ) => __last = First = new Node(firstValue);
    public ValueLinkedList( IEnumerator<T> enumerator )
    {
        Debug.Assert(enumerator is not null);
        __last = First = new Node(enumerator.Current);

        while ( enumerator.MoveNext() )
        {
            __last.Next = new Node(enumerator.Current);
            __last      = __last.Next;
        }
    }


    public void Clear()
    {
        lock ( __lock )
        {
            First   = __last = null;
            __count = 0;
        }
    }
    public bool Contains( T value )
    {
        lock ( __lock )
        {
            foreach ( ref readonly T x in Values )
            {
                if ( EqualityComparer<T>.Default.Equals(value, x) ) { return true; }
            }

            return false;
        }
    }


    public bool TryCopyTo( scoped Span<T> array, int startIndex = 0 )
    {
        if ( array.Length < Count ) { return false; }

        lock ( __lock )
        {
            int i = startIndex;
            foreach ( ref readonly T x in Values ) { array[i++] = x; }

            return true;
        }
    }


    private void UnsafeAdd( ref readonly Node newNode )
    {
        if ( First is null )
        {
            First   = __last = newNode;
            __count = 1;
            return;
        }

        Debug.Assert(First is not null);
        Debug.Assert(__last is not null);

        __last.Next = newNode;
        __last      = newNode;
        __count++;
    }


    public void AddLast( T value ) => Add(value);
    public void Add( T value )
    {
        Node newNode = new(value);
        lock ( __lock ) { UnsafeAdd(in newNode); }
    }
    public void AddFront( T value )
    {
        Node newNode = new(value);

        lock ( __lock )
        {
            newNode.Next = First;
            First        = newNode;
            __count++;
        }
    }


    public bool AddIfNotExist( T value )
    {
        lock ( __lock )
        {
            Node? current = First;

            while ( current is not null )
            {
                if ( EqualityComparer<T>.Default.Equals(value, current.Value) ) { return false; }

                current = current.Next;
            }

            Node newNode = new(value);
            UnsafeAdd(in newNode);

            return true;
        }
    }
    public bool AddIfNotExist( T value, Func<T, T, bool> compare )
    {
        lock ( __lock )
        {
            Node? current = First;

            while ( current is not null )
            {
                if ( compare(value, current.Value) ) { return false; }

                current = current.Next;
            }

            Node newNode = new(value);
            UnsafeAdd(in newNode);

            return true;
        }
    }


    public bool Remove( T value )
    {
        lock ( __lock )
        {
            Node? previous = First;
            if ( previous is null ) { return false; }

            if ( EqualityComparer<T>.Default.Equals(previous.Value, value) )
            {
                First = previous.Next;
                if ( First is null ) { __last = null; }

                __count--;
                return true;
            }

            Node? current = previous.Next;

            while ( current is not null )
            {
                if ( EqualityComparer<T>.Default.Equals(current.Value, value) )
                {
                    previous.Next = current.Next;
                    if ( ReferenceEquals(__last, current) ) { __last = previous; }

                    __count--;
                    return true;
                }

                previous = current;
                current  = current.Next;
            }

            return false;
        }
    }
    public bool Remove( T value, Func<T, T, bool> compare )
    {
        lock ( __lock )
        {
            Node? previous = First;
            if ( previous is null ) { return false; }

            if ( compare(previous.Value, value) )
            {
                First = previous.Next;
                if ( First is null ) { __last = null; }

                __count--;
                return true;
            }

            Node? current = previous.Next;

            while ( current is not null )
            {
                if ( compare(current.Value, value) )
                {
                    previous.Next = current.Next;
                    if ( ReferenceEquals(__last, current) ) { __last = previous; }

                    __count--;
                    return true;
                }

                previous = current;
                current  = current.Next;
            }

            return false;
        }
    }


    public Enumerator                          GetEnumerator()     => new(this);
    IEnumerator<T> IEnumerable<T>.             GetEnumerator()     => GetEnumerator();
    IEnumerator IEnumerable.                   GetEnumerator()     => GetEnumerator();
    public ValueEnumerable<ValueEnumerator, T> AsValueEnumerable() => new(Values);



    public sealed class Enumerator( ValueLinkedList<T> list ) : IEnumerator<T>
    {
        private readonly ValueLinkedList<T> __list     = list;
        private          Node?              __nextNode = list.First;
        private          Node?              __currentNode;
        private          bool               __isDisposed;


        public ref T Current => ref __currentNode is null
                                        ? ref Unsafe.NullRef<T>()
                                        : ref __currentNode.Value;


        T IEnumerator<T>.   Current => Current;
        object? IEnumerator.Current => Current;


        public bool MoveNext()
        {
            ObjectDisposedException.ThrowIf(__isDisposed, this);

            if ( __nextNode == null )
            {
                __currentNode = null;
                return false;
            }

            __currentNode = __nextNode;
            __nextNode    = __nextNode.Next;
            return true;
        }


        public void Reset()
        {
            ObjectDisposedException.ThrowIf(__isDisposed, this);
            __currentNode = null;
            __nextNode    = __list.First;
        }
        public void Dispose()
        {
            __nextNode    = null;
            __currentNode = null;
            __isDisposed  = true;
        }
    }



    public sealed class Node( T value )
    {
        public Node? Next;
        public T     Value = value;
    }



    public struct ValueEnumerator( ValueLinkedList<T> list ) : IValueEnumerator<T>
    {
        private readonly ValueLinkedList<T> __list     = list;
        private          Node?              __nextNode = list.First;
        private          Node?              __currentNode;


        public readonly ref T Current => ref __currentNode is null
                                                 ? ref Unsafe.NullRef<T>()
                                                 : ref __currentNode.Value;

        public void Dispose()
        {
            __nextNode    = null;
            __currentNode = null;
        }
        [EditorBrowsable(EditorBrowsableState.Never)] public readonly ValueEnumerator GetEnumerator() => this;


        public bool MoveNext()
        {
            if ( __nextNode == null )
            {
                __currentNode = __list.First;
                return false;
            }

            __currentNode = __nextNode;
            __nextNode    = __currentNode.Next;
            return true;
        }


        public bool TryGetNext( out T current )
        {
            current = MoveNext()
                          ? Current
                          : default!;

            return current is not null;
        }
        public bool TryGetNonEnumeratedCount( out int count )
        {
            count = __list.Count;
            return false;
        }
        public bool TryGetSpan( out ReadOnlySpan<T> span )
        {
            span = ReadOnlySpan<T>.Empty;
            return false;
        }
        public bool TryCopyTo( scoped Span<T> destination, Index offset ) => __list.TryCopyTo(destination, offset.Value);
    }
}
