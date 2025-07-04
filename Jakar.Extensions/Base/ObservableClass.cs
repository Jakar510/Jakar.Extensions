namespace Jakar.Extensions;


[Serializable]
public abstract class ObservableClass : BaseClass, IObservableObject
{
    public event PropertyChangedEventHandler?  PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;


#pragma warning disable CS4026 // The CallerMemberNameAttribute will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments


    [NotifyPropertyChangedInvocator] protected internal void                   OnPropertyChanged( [CallerMemberName] string property = EMPTY ) => OnPropertyChanged(property.GetPropertyChangedEventArgs());
    [NotifyPropertyChangedInvocator]                    void IObservableObject.OnPropertyChanged( [CallerMemberName] string property = EMPTY ) => OnPropertyChanged(property);
    [NotifyPropertyChangedInvocator]                    void IObservableObject.OnPropertyChanged( PropertyChangedEventArgs  e )                => OnPropertyChanged(e);
    [NotifyPropertyChangedInvocator] protected internal void                   OnPropertyChanged( PropertyChangedEventArgs  e )                => PropertyChanged?.Invoke(this, e);


    protected internal void                                 OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging(property.GetPropertyChangingEventArgs());
    void IObservableObject.                                 OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging(property);
    [NotifyPropertyChangedInvocator] void IObservableObject.OnPropertyChanging( PropertyChangingEventArgs e )                => OnPropertyChanging(e);
    protected internal               void                   OnPropertyChanging( PropertyChangingEventArgs e )                => PropertyChanging?.Invoke(this, e);


    protected internal static bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value )
    {
        backingStore = value;
        return true;
    }
    protected internal static bool SetPropertyWithoutNotify<TValue, TComparer>( ref TValue backingStore, TValue value, TComparer comparer )
        where TComparer : IEqualityComparer<TValue>
    {
        if ( comparer.Equals(backingStore, value) ) { return false; }

        backingStore = value;
        return true;
    }
    protected internal bool SetProperty<TValue>( ref TValue backingStore, TValue value, [CallerMemberName] string propertyName = EMPTY )
    {
        OnPropertyChanging(propertyName);
        backingStore = value;
        OnPropertyChanged(propertyName);

        return true;
    }
    protected internal bool SetProperty<TValue, TComparer>( ref TValue backingStore, TValue value, TComparer comparer, [CallerMemberName] string propertyName = EMPTY )
        where TComparer : IEqualityComparer<TValue>
    {
        if ( comparer.Equals(backingStore, value) ) { return false; }

        OnPropertyChanging(propertyName);
        backingStore = value;
        OnPropertyChanged(propertyName);

        return true;
    }
    protected internal bool SetProperty<TValue, TComparer>( ref TValue backingStore, TValue value, in TValue minValue, TComparer comparer, [CallerMemberName] string propertyName = EMPTY )
        where TValue : IComparisonOperators<TValue, TValue, bool>
        where TComparer : IEqualityComparer<TValue>
    {
        value = value < minValue
                    ? minValue
                    : value;

        return SetProperty(ref backingStore, value, comparer, propertyName);
    }


#pragma warning restore CS4026 // The CallerMemberNameAttribute will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning restore CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
}



[Serializable]
public abstract class ObservableClass<TClass> : BaseClass<TClass>, IObservableObject
    where TClass : ObservableClass<TClass>, IEqualComparable<TClass>
{
    public event PropertyChangedEventHandler?  PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;


#pragma warning disable CS4026 // The CallerMemberNameAttribute will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments


    [NotifyPropertyChangedInvocator] protected internal void                   OnPropertyChanged( [CallerMemberName] string property = EMPTY ) => OnPropertyChanged(property.GetPropertyChangedEventArgs());
    [NotifyPropertyChangedInvocator]                    void IObservableObject.OnPropertyChanged( [CallerMemberName] string property = EMPTY ) => OnPropertyChanged(property);
    [NotifyPropertyChangedInvocator]                    void IObservableObject.OnPropertyChanged( PropertyChangedEventArgs  e )                => OnPropertyChanged(e);
    [NotifyPropertyChangedInvocator] protected internal void                   OnPropertyChanged( PropertyChangedEventArgs  e )                => PropertyChanged?.Invoke(this, e);


    protected internal void                                 OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging(property.GetPropertyChangingEventArgs());
    void IObservableObject.                                 OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging(property);
    [NotifyPropertyChangedInvocator] void IObservableObject.OnPropertyChanging( PropertyChangingEventArgs e )                => OnPropertyChanging(e);
    protected internal               void                   OnPropertyChanging( PropertyChangingEventArgs e )                => PropertyChanging?.Invoke(this, e);


    protected internal static bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value )
    {
        backingStore = value;
        return true;
    }
    protected internal static bool SetPropertyWithoutNotify<TValue, TComparer>( ref TValue backingStore, TValue value, TComparer comparer )
        where TComparer : IEqualityComparer<TValue>
    {
        if ( comparer.Equals(backingStore, value) ) { return false; }

        backingStore = value;
        return true;
    }
    protected internal bool SetProperty<TValue>( ref TValue backingStore, TValue value, [CallerMemberName] string propertyName = EMPTY )
    {
        OnPropertyChanging(propertyName);
        backingStore = value;
        OnPropertyChanged(propertyName);

        return true;
    }
    protected internal bool SetProperty<TValue, TComparer>( ref TValue backingStore, TValue value, TComparer comparer, [CallerMemberName] string propertyName = EMPTY )
        where TComparer : IEqualityComparer<TValue>
    {
        if ( comparer.Equals(backingStore, value) ) { return false; }

        OnPropertyChanging(propertyName);
        backingStore = value;
        OnPropertyChanged(propertyName);

        return true;
    }
    protected internal bool SetProperty<TValue, TComparer>( ref TValue backingStore, TValue value, in TValue minValue, TComparer comparer, [CallerMemberName] string propertyName = EMPTY )
        where TValue : IComparisonOperators<TValue, TValue, bool>
        where TComparer : IEqualityComparer<TValue>
    {
        value = value < minValue
                    ? minValue
                    : value;

        return SetProperty(ref backingStore, value, comparer, propertyName);
    }


#pragma warning restore CS4026 // The CallerMemberNameAttribute will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning restore CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
}



public abstract class ObservableClass<TClass, TID> : ObservableClass<TClass>, IUniqueID<TID>
    where TClass : ObservableClass<TClass, TID>, IEqualComparable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    private TID _id;


    public virtual TID ID { get => _id; init => _id = value; }


    protected ObservableClass() : base() { }
    protected ObservableClass( TID id ) => ID = id;


    protected bool SetID( TClass record ) => SetID(record.ID);
    protected bool SetID( TID    id )     => SetProperty(ref _id, id, nameof(ID));
}
