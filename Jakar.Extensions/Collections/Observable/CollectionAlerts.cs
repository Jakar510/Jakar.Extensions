// Jakar.Extensions :: Jakar.Extensions
// 04/12/2022  1:54 PM

using ZLinq;



namespace Jakar.Extensions;


public interface ICollectionAlerts : INotifyCollectionChanged, IObservableObject
{
    /// <summary> <see langword="true"/> while at least one <see cref="SuspendNotifications"/> scope is open. </summary>
    bool AreNotificationsSuspended { get; }

    /// <summary> Collects changes until the returned scope is disposed, then raises a single <see cref="NotifyCollectionChangedAction.Reset"/> (if anything changed). </summary>
    [MustDisposeResource] NotificationSuspension SuspendNotifications();
}



/// <summary> Implemented by collections whose notifications can be suspended with <see cref="ICollectionAlerts.SuspendNotifications"/>. </summary>
internal interface INotificationSuspendable
{
    void ResumeNotifications();
}



/// <summary>
///     Returned by <see cref="ICollectionAlerts.SuspendNotifications"/>: while any scope is open, the collection raises no <see cref="INotifyCollectionChanged.CollectionChanged"/> or
///     <see cref="INotifyPropertyChanged.PropertyChanged"/> events for its changes; disposing the last open scope raises a single <see cref="NotifyCollectionChangedAction.Reset"/> (and
///     <c>Count</c> / <c>IsEmpty</c> / <c>IsNotEmpty</c> when they changed) if anything changed. Scopes nest.
/// </summary>
/// <remarks> Use it with <see langword="using"/>; disposing the same variable twice is a no-op. Don't copy it: each copy would resume once. </remarks>
public struct NotificationSuspension : IDisposable
{
    private INotificationSuspendable? __owner;


    internal NotificationSuspension( INotificationSuspendable owner ) => __owner = owner;


    public void Dispose()
    {
        INotificationSuspendable? owner = __owner;
        __owner = null;
        owner?.ResumeNotifications();
    }
}



public interface ICollectionAlerts<TValue> : IReadOnlyCollection<TValue>, IValueEnumerable<ArrayBuffer<TValue>, TValue>, ICollectionAlerts;



public interface ICollectionAlerts<TSelf, TValue> : ICollectionAlerts<TValue>, IJsonModel<TSelf>, IEqualComparable<TSelf>
    where TSelf : ICollectionAlerts<TSelf, TValue>
{
    public abstract static implicit operator TSelf( List<TValue>           values );
    public abstract static implicit operator TSelf( HashSet<TValue>        values );
    public abstract static implicit operator TSelf( ConcurrentBag<TValue>  values );
    public abstract static implicit operator TSelf( Collection<TValue>     values );
    public abstract static implicit operator TSelf( TValue[]               values );
    public abstract static implicit operator TSelf( ImmutableArray<TValue> values );
    public abstract static implicit operator TSelf( ReadOnlyMemory<TValue> values );
    public abstract static implicit operator TSelf( ReadOnlySpan<TValue>   values );
}



public delegate bool FilterDelegate<TValue>( int index, ref readonly TValue? value );



/// <remarks>
///     Change notifications are only built when someone listens: with no <see cref="CollectionChanged"/> subscriber (and <see cref="OnChanged"/> not overridden) no
///     <see cref="NotifyCollectionChangedEventArgs"/> is allocated. <see cref="IsEmpty"/> / <see cref="IsNotEmpty"/> are only raised when they actually change.
///     Use <see cref="SuspendNotifications"/> to batch many single item changes into one <see cref="NotifyCollectionChangedAction.Reset"/>.
/// </remarks>
public abstract class CollectionAlerts<TSelf, TValue> : BaseClass<TSelf>, ICollectionAlerts<TValue>, INotificationSuspendable
    where TSelf : CollectionAlerts<TSelf, TValue>, ICollectionAlerts<TSelf, TValue>, IEqualComparable<TSelf>
{
// ReSharper disable StaticMemberInGenericType
    protected static readonly NotifyCollectionChangedEventArgs _resetArgs       = new(NotifyCollectionChangedAction.Reset);
    private static readonly   PropertyChangedEventArgs         __countArgs      = new(nameof(Count));
    private static readonly   PropertyChangedEventArgs         __isEmptyArgs    = new(nameof(IsEmpty));
    private static readonly   PropertyChangedEventArgs         __isNotEmptyArgs = new(nameof(IsNotEmpty));
    private static readonly   PropertyChangedEventArgs         __capacityArgs   = new(nameof(Capacity));

// ReSharper restore StaticMemberInGenericType

    private FilterDelegate<TValue>? __filter;
    private sbyte                   __filterOverridden    = -1; // -1 unknown, 0 no, 1 yes
    private sbyte                   __onChangedOverridden = -1;
    private int                     __lastCount           = -1; // -1 unknown
    private int                     __suspended;                // number of open SuspendNotifications scopes
    private int                     __pending;                  // 1 when something changed while suspended


    public abstract int                     Capacity       { get; }
    public abstract int                     Count          { get; }
    public          bool                    IsEmpty        { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Count <= 0; }
    public          bool                    IsNotEmpty     { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Count > 0; }
    public          FilterDelegate<TValue>? OverrideFilter { get; set; }

    /// <summary> <see langword="true"/> when enumeration filters items (<see cref="OverrideFilter"/> is set or <see cref="Filter"/> is overridden). </summary>
    protected bool HasFilter => OverrideFilter is not null || IsFilterOverridden;

    private bool IsFilterOverridden
    {
        get
        {
            if ( __filterOverridden < 0 )
            {
                __filterOverridden = (sbyte)( ( __filter ??= Filter ).Method.DeclaringType == typeof(CollectionAlerts<TSelf, TValue>)
                                                  ? 0
                                                  : 1 );
            }

            return __filterOverridden == 1;
        }
    }
    /// <summary> Whether change args have to be built: someone subscribed to <see cref="CollectionChanged"/>, or a subclass overrides <see cref="OnChanged"/>. </summary>
    private bool NeedsChangeArgs
    {
        get
        {
            if ( CollectionChanged is not null ) { return true; }

            if ( __onChangedOverridden < 0 )
            {
                Action<NotifyCollectionChangedEventArgs> handler = OnChanged;

                __onChangedOverridden = (sbyte)( handler.Method.DeclaringType == typeof(CollectionAlerts<TSelf, TValue>)
                                                     ? 0
                                                     : 1 );
            }

            return __onChangedOverridden == 1;
        }
    }


    public bool AreNotificationsSuspended => Volatile.Read(ref __suspended) > 0;


    public event NotifyCollectionChangedEventHandler? CollectionChanged;


    public override bool Equals( TSelf?    other ) => ReferenceEquals(this, other);
    public override int  CompareTo( TSelf? other ) => Nullable.Compare(Count, other?.Count);


    /// <inheritdoc cref="ICollectionAlerts.SuspendNotifications"/>
    /// <example>
    ///     <code>
    /// using ( collection.SuspendNotifications() )
    /// {
    ///     foreach ( Item item in items ) { collection.Add(item); }
    /// } // one Reset here
    ///     </code>
    /// </example>
    /// <remarks> Suspension applies to the whole collection, including changes made by other threads while a scope is open. </remarks>
    [MustDisposeResource] public NotificationSuspension SuspendNotifications()
    {
        Interlocked.Increment(ref __suspended);
        return new NotificationSuspension(this);
    }
    void INotificationSuspendable.ResumeNotifications()
    {
        int open = Interlocked.Decrement(ref __suspended);

        if ( open < 0 )
        {
            Interlocked.Increment(ref __suspended);
            throw new InvalidOperationException($"{nameof(SuspendNotifications)} scope disposed more often than it was opened.");
        }

        if ( open == 0 && Interlocked.Exchange(ref __pending, 0) == 1 ) { Reset(); }
    }


    /// <summary> While suspended, records that something changed and returns <see langword="true"/>, so the caller raises nothing. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private bool Deferred() => Volatile.Read(ref __suspended) != 0 && Defer();
    private bool Defer()
    {
        Volatile.Write(ref __pending, 1);
        if ( Volatile.Read(ref __suspended) != 0 ) { return true; }

        // The last scope closed meanwhile. If it already consumed the flag it raised the Reset; otherwise raise this change ourselves.
        return Interlocked.Exchange(ref __pending, 0) == 0;
    }


    public virtual void Refresh() => Reset();
    protected void Reset()
    {
        if ( Deferred() ) { return; }

        if ( NeedsChangeArgs ) { OnChanged(_resetArgs); }
        else { CountChanged(); }
    }
    protected void Added( TValue[] value, int index )
    {
        if ( Deferred() ) { return; }

        if ( NeedsChangeArgs ) { OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index)); }
        else { CountChanged(); }
    }
    protected void Added( ref readonly TValue value, int index )
    {
        if ( Deferred() ) { return; }

        if ( NeedsChangeArgs ) { OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index)); }
        else { CountChanged(); }
    }
    protected void Removed( TValue[] value, int index )
    {
        if ( Deferred() ) { return; }

        if ( NeedsChangeArgs ) { OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index)); }
        else { CountChanged(); }
    }
    protected void Removed( ref readonly TValue value, int index )
    {
        if ( Deferred() ) { return; }

        if ( NeedsChangeArgs ) { OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index)); }
        else { CountChanged(); }
    }
    /// <summary> Items were removed but are no longer available: raised as a reset. </summary>
    protected void Removed( int index ) => Reset();
    protected void Moved( TValue[] value, ref readonly int index, ref readonly int oldIndex )
    {
        if ( Deferred() ) { return; }

        if ( NeedsChangeArgs ) { OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, index, oldIndex)); }
    }
    protected void Moved( ref readonly TValue value, ref readonly int index, ref readonly int oldIndex )
    {
        if ( Deferred() ) { return; }

        if ( NeedsChangeArgs ) { OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, index, oldIndex)); }
    }
    protected void Replaced( ref readonly TValue? old, ref readonly TValue value, int index )
    {
        if ( Deferred() ) { return; }

        if ( NeedsChangeArgs ) { OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old, index)); }
    }
    protected void OnCapacityChanged() => OnPropertyChanged(__capacityArgs);
    /// <summary> Raises <see cref="Count"/>, <see cref="IsEmpty"/> and <see cref="IsNotEmpty"/>. </summary>
    protected void OnCountChanged()
    {
        __lastCount = -1;
        if ( Deferred() ) { return; }

        CountChanged();
    }
    /// <summary> Raises <see cref="Count"/> when it changed, and <see cref="IsEmpty"/> / <see cref="IsNotEmpty"/> when emptiness changed (e.g. a sort raises neither). </summary>
    private void CountChanged()
    {
        int count = Count;
        int last  = __lastCount;
        __lastCount = count;

        if ( count == last ) { return; }

        OnPropertyChanged(__countArgs);

        // IsEmpty / IsNotEmpty only change when the collection goes from empty to non-empty or back. (last < 0: the previous count is unknown, so raise them.)
        bool wasEmpty = last  == 0;
        bool isEmpty  = count == 0;
        if ( last >= 0 && wasEmpty == isEmpty ) { return; }

        OnPropertyChanged(__isEmptyArgs);
        OnPropertyChanged(__isNotEmptyArgs);
    }
    protected virtual void OnChanged( NotifyCollectionChangedEventArgs e )
    {
        CollectionChanged?.Invoke(this, e);
        if ( e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset ) { CountChanged(); }
    }


    /// <summary> The filter used for enumeration (the delegate is created once). </summary>
    protected         FilterDelegate<TValue> GetFilter()                                     => OverrideFilter ?? ( __filter ??= Filter );
    protected virtual bool                   Filter( int index, ref readonly TValue? value ) => true;


    [Pure] [MustDisposeResource] protected internal abstract ArrayBuffer<TValue>                          FilteredValues();
    [Pure]                       public                      ValueEnumerable<ArrayBuffer<TValue>, TValue> AsValueEnumerable() => new(FilteredValues());


    /// <summary> Enumerates a snapshot of the (filtered) items, so the collection may be modified while enumerating. </summary>
    public virtual IEnumerator<TValue> GetEnumerator()
    {
        using ArrayBuffer<TValue> owner = FilteredValues();
        for ( int i = 0; i < owner.Length; i++ ) { yield return owner.Values[i]; }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
