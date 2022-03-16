namespace Jakar.Extensions.Models.Collections;


/// <summary>
/// <para><see href="https://stackoverflow.com/a/54733415/9530917">This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread</see></para>
/// <para><see href="https://stackoverflow.com/a/14602121/9530917">How do I update an ObservableCollection via a worker thread?</see></para>
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentObservableCollection<T> : ObservableCollection<T> // IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyCollection<T>
{
    public             object Lock => _lock;
    protected readonly object _lock = new();


    public ConcurrentObservableCollection() : base() { }
    public ConcurrentObservableCollection( IEnumerable<T> items ) : base(items) { }
    public ConcurrentObservableCollection( List<T>        items ) : base(items) { }


    public new void Add( T item )
    {
        lock ( _lock ) { base.Add(item); }
    }


    public bool TryAdd( T item )
    {
        lock ( _lock )
        {
            if ( base.Contains(item) ) { return false; }

            base.Add(item);
            return true;
        }
    }


    public new void CopyTo( T[] array, int arrayIndex )
    {
        lock ( _lock ) { base.CopyTo(array, arrayIndex); }
    }


    public new bool Remove( T item )
    {
        lock ( _lock )
        {
            if ( base.Contains(item) ) { return false; }

            int index = IndexOf(item);
            if ( index < 0 ) { return false; }

            bool result = base.Remove(item);
            return result;
        }
    }


    public new int Count
    {
        get
        {
            lock ( _lock ) { return base.Count; }
        }
    }


    public new int IndexOf( T item )
    {
        lock ( _lock ) { return base.IndexOf(item); }
    }


    public new void Insert( int index, T item )
    {
        lock ( _lock ) { base.Insert(index, item); }
    }


    public new void RemoveAt( int index )
    {
        lock ( _lock )
        {
            T removedItem = this[index];
            base.RemoveAt(index);
        }
    }


    public new void Clear()
    {
        lock ( _lock ) { base.Clear(); }
    }


    public new bool Contains( T item )
    {
        lock ( _lock ) { return base.Contains(item); }
    }


    public new T this[ int index ]
    {
        get
        {
            lock ( _lock ) { return base[index]; }
        }
        set
        {
            lock ( _lock )
            {
                if ( base.Count > index )
                {
                    T old = base[index];

                    base[index] = value;
                }
                else { base[index] = value; }
            }
        }
    }
}
