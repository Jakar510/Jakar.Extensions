// Jakar.Extensions :: Jakar.Extensions
// 04/12/2022  1:54 PM

namespace Jakar.Extensions;


public interface ICollectionAlerts : INotifyCollectionChanged, INotifyPropertyChanging, INotifyPropertyChanged { }



public abstract class CollectionAlerts<T> : ObservableClass, ICollectionAlerts, IReadOnlyCollection<T>
{
    public const    int DEFAULT_CAPACITY = 16;
    public abstract int Count { get; }


    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected CollectionAlerts() { }


    public virtual void Refresh()                                             => Reset();
    protected      void OnCountChanged()                                      => OnPropertyChanged( nameof(Count) );
    protected      void Reset()                                               => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
    protected      void Added( List<T> items )                                => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add,  items ) );
    protected      void Added( in   T  value )                                => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add,  value ) );
    protected      void Added( in   T  value, in int index )                  => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add,  value, index ) );
    protected      void Moved( in   T  value, in int index, in int oldIndex ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move, value, index, oldIndex ) );
    protected      void Removed( in T  value )               => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, value ) );
    protected      void Removed( in T  value, in int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, value, index ) );
    protected      void Removed( int   index )                        => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove,  index ) );
    protected      void Replaced( in T old, in T @new )               => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, @new, old ) );
    protected      void Replaced( in T old, in T @new, in int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, @new, old, index ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected virtual void OnChanged( NotifyCollectionChangedEventArgs e )
    {
        CollectionChanged?.Invoke( this, e );
        OnCountChanged();
    }


    protected virtual bool Filter( T? value ) => true;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected ReadOnlyMemory<T> FilteredValues( scoped in ReadOnlySpan<T> span ) => FilteredValues( span.Where( Filter ) );
    protected ReadOnlyMemory<T> FilteredValues( scoped in SpanEnumerable<T, T, WhereDelegateProducer<T>> span )
    {
        using Buffer<T> values = new(Count);
        foreach ( T value in span ) { values.Add( value ); }

        return values.ToArray();
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected ReadOnlyMemory<T> FilteredValues( SpanEnumerable<T, EnumerableProducer<T>> span ) => FilteredValues( span.Where( Filter ) );
    protected ReadOnlyMemory<T> FilteredValues( scoped in SpanEnumerable<T, SecondaryWhereDelegateProducer<T, EnumerableProducer<T>>> span )
    {
        using Buffer<T> values = new(Count);
        foreach ( T value in span ) { values.Add( value ); }

        return values.ToArray();
    }


    public virtual IEnumerator<T>                 GetEnumerator() => new Enumerator( this );
    IEnumerator IEnumerable.                      GetEnumerator() => GetEnumerator();
    protected internal abstract ReadOnlyMemory<T> FilteredValues();



    public sealed class Enumerator( CollectionAlerts<T> enumerator ) : IEnumerator<T>, IEnumerable<T>
    {
        private int               _index;
        private ReadOnlyMemory<T> _collection = enumerator.FilteredValues();

        public T            Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _collection.Span[_index]; }
        object? IEnumerator.Current => Current;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
        IEnumerator IEnumerable.      GetEnumerator() => this;
        public void                   Dispose()       => _collection = default;
        public bool                   MoveNext()      => ++_index < _collection.Length;
        public void                   Reset()         => _index = NOT_FOUND;
    }
}