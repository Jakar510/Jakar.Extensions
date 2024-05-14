// Jakar.Extensions :: Jakar.Extensions
// 04/12/2022  1:54 PM

namespace Jakar.Extensions;


public interface ICollectionAlerts : INotifyCollectionChanged, INotifyPropertyChanging, INotifyPropertyChanged;



public abstract class CollectionAlerts<T> : ObservableClass, ICollectionAlerts, IReadOnlyCollection<T>
{
    public abstract int Count { get; }


    public event NotifyCollectionChangedEventHandler? CollectionChanged;


    public virtual void Refresh()                                   => Reset();
    protected      void Reset()                                     => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
    protected      void Added( List<T>   value, in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Added( T         value, in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Removed( List<T> value, in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Removed( T       value, in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Removed( int     index )                                  => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove,  index ) );
    protected      void Moved( List<T>   value, in int index, in int oldIndex )   => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move,    value, index, oldIndex ) );
    protected      void Moved( T         value, in int index, in int oldIndex )   => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move,    value, index, oldIndex ) );
    protected      void Replaced( in T?  old,   in T   @new,  in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, @new,  old,   index ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected void OnCountChanged() => OnPropertyChanged( nameof(Count) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected virtual void OnChanged( NotifyCollectionChangedEventArgs e )
    {
        CollectionChanged?.Invoke( this, e );
        OnCountChanged();
    }


    protected virtual bool Filter( T? value ) => true;


    public virtual IEnumerator<T> GetEnumerator()
    {
        ReadOnlyMemory<T> memory = FilteredValues();
        for ( int i = 0; i < memory.Length; i++ ) { yield return memory.Span[i]; }
    }
    IEnumerator IEnumerable.                      GetEnumerator() => GetEnumerator();
    protected internal abstract ReadOnlyMemory<T> FilteredValues();


    /*
    public sealed class Enumerator( CollectionAlerts<T> collection ) : IEnumerator<T>, IEnumerable<T>
    {
        private const    int                 START       = 0;
        private readonly CollectionAlerts<T> _collection = collection;
        private          int                 _index      = START;
        private          ReadOnlyMemory<T>   _buffer;
        private          bool                _isDisposed;


        public ref readonly T Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _buffer.Span[_index]; }
        T IEnumerator<T>.     Current => Current;
        object? IEnumerator.  Current => Current;


        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
        IEnumerator IEnumerable.      GetEnumerator() => this;
        public void Dispose()
        {
            _isDisposed = true;
            _buffer     = ReadOnlyMemory<T>.Empty;
        }
        public bool MoveNext()
        {
            ThrowIfDisposed();
            if ( _buffer.IsEmpty ) { _buffer = _collection.FilteredValues(); }

            if ( _index >= _buffer.Length ) { return false; }

            _index++;
            return true;
        }
        public void Reset()
        {
            ThrowIfDisposed();
            _index  = START;
            _buffer = _collection.FilteredValues();
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private void ThrowIfDisposed()
        {
        #if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf( _isDisposed, this );
        #else
            if ( _isDisposed ) { throw new ObjectDisposedException( nameof(Enumerator) ); }
        #endif
        }
    }
    */
}
