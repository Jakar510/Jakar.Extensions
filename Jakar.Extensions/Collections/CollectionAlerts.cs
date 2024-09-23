// Jakar.Extensions :: Jakar.Extensions
// 04/12/2022  1:54 PM

namespace Jakar.Extensions;


public interface ICollectionAlerts : INotifyCollectionChanged, IObservableObject;



public abstract class CollectionAlerts<TValue> : ObservableClass, IReadOnlyCollection<TValue>, ICollectionAlerts
{
    public abstract int Count { get; }


    public event NotifyCollectionChangedEventHandler? CollectionChanged;


    public virtual void Refresh()                                        => Reset();
    protected      void Reset()                                          => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
    protected      void Added( List<TValue>   value, in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Added( TValue         value, in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Removed( List<TValue> value, in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Removed( TValue       value, in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Removed( int          index )                                     => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove,  index ) );
    protected      void Moved( List<TValue>   value, in int    index, in int oldIndex )   => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move,    value, index, oldIndex ) );
    protected      void Moved( TValue         value, in int    index, in int oldIndex )   => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move,    value, index, oldIndex ) );
    protected      void Replaced( in TValue?  old,   in TValue @new,  in int index = -1 ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, @new,  old,   index ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected void OnCountChanged() => OnPropertyChanged( nameof(Count) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected virtual void OnChanged( NotifyCollectionChangedEventArgs e )
    {
        CollectionChanged?.Invoke( this, e );
        OnCountChanged();
    }


    protected virtual bool Filter( TValue? value ) => true;


    public virtual IEnumerator<TValue> GetEnumerator()
    {
        using FilterBuffer<TValue> owner  = FilteredValues();
        ReadOnlyMemory<TValue>     memory = owner.Memory;
        for ( int i = 0; i < memory.Length; i++ ) { yield return memory.Span[i]; }
    }
    IEnumerator IEnumerable.                         GetEnumerator() => GetEnumerator();
    protected internal abstract FilterBuffer<TValue> FilteredValues();
}



[SuppressMessage( "ReSharper", "ConvertToAutoPropertyWhenPossible" )]
public record struct FilterBuffer<TValue>( int Capacity ) : IDisposable
{
    private readonly IMemoryOwner<TValue>   _owner = MemoryPool<TValue>.Shared.Rent( Capacity );
    private          int                    _length;
    public           int                    Capacity               { get; } = Capacity;
    public readonly  int                    Length                 => _length;
    public readonly  ReadOnlyMemory<TValue> Memory                 => _owner.Memory[.._length];
    public           void                   Dispose()              => _owner.Dispose();
    public           void                   Add( in TValue value ) => _owner.Memory.Span[_length++] = value;
}
