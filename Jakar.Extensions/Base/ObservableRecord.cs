namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
public record ObservableRecord : BaseRecord, IObservableObject
{
    public static ref readonly DateTimeOffset SQLMinDate     { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref BaseClass.SQLMinDate; }
    public static ref readonly DateOnly       SQLMinDateOnly { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref BaseClass.SQLMinDateOnly; }


    public event PropertyChangedEventHandler?  PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;


#pragma warning disable CS4026 // The CallerMemberNameAttribute will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments


    [NotifyPropertyChangedInvocator] public void OnPropertyChanged( [CallerMemberName] string property = EMPTY ) => OnPropertyChanged(property.GetPropertyChangedEventArgs());
    [NotifyPropertyChangedInvocator] public void OnPropertyChanged( PropertyChangedEventArgs  e )                => PropertyChanged?.Invoke(this, e);


    public void OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging(property.GetPropertyChangingEventArgs());
    public void OnPropertyChanging( PropertyChangingEventArgs e )                => PropertyChanging?.Invoke(this, e);


    bool IObservableObject.SetPropertyWithoutNotify<TValue>( ref            TValue backingStore, TValue value )                                                                                                                         => SetPropertyWithoutNotify(ref backingStore, value);
    bool IObservableObject.SetPropertyWithoutNotify<TValue, TComparer>( ref TValue backingStore, TValue value, TComparer                 comparer )                                                                                     => SetPropertyWithoutNotify(ref backingStore, value, comparer);
    bool IObservableObject.SetProperty<TValue>( ref                         TValue backingStore, TValue value, [CallerMemberName] string propertyName                                                                         = EMPTY ) => SetProperty(ref backingStore, value, propertyName);
    bool IObservableObject.SetProperty<TValue, TComparer>( ref              TValue backingStore, TValue value, TComparer                 comparer, [CallerMemberName] string propertyName                                     = EMPTY ) => SetProperty(ref backingStore, value, comparer,    propertyName);
    bool IObservableObject.SetProperty<TValue, TComparer>( ref              TValue backingStore, TValue value, in TValue                 minValue, TComparer                 comparer, [CallerMemberName] string propertyName = EMPTY ) => SetProperty(ref backingStore, value, in minValue, comparer, propertyName);


    protected virtual bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value )
    {
        backingStore = value;
        return true;
    }
    protected virtual bool SetPropertyWithoutNotify<TValue, TComparer>( ref TValue backingStore, TValue value, TComparer comparer )
        where TComparer : EqualityComparer<TValue> => !comparer.Equals(backingStore, value) && SetProperty(ref backingStore, value);
    protected virtual bool SetProperty<TValue>( ref TValue backingStore, TValue value, [CallerMemberName] string propertyName = EMPTY )
    {
        OnPropertyChanging(propertyName);
        backingStore = value;
        OnPropertyChanged(propertyName);

        return true;
    }
    protected virtual bool SetProperty<TValue, TComparer>( ref TValue backingStore, TValue value, TComparer comparer, [CallerMemberName] string propertyName = EMPTY )
        where TComparer : EqualityComparer<TValue> => !comparer.Equals(backingStore, value) && SetProperty(ref backingStore, value, propertyName);
    protected virtual bool SetProperty<TValue, TComparer>( ref TValue backingStore, TValue value, in TValue minValue, TComparer comparer, [CallerMemberName] string propertyName = EMPTY )
        where TValue : IComparisonOperators<TValue, TValue, bool>
        where TComparer : EqualityComparer<TValue>
    {
        value = value < minValue
                    ? minValue
                    : value;

        return SetProperty(ref backingStore, value, comparer, propertyName);
    }


#pragma warning restore CS4026 // The CallerMemberNameAttribute will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning restore CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
}



public abstract record ObservableRecord<TClass> : ObservableRecord, IEquatable<TClass>, IComparable<TClass>, IComparable
    where TClass : ObservableRecord<TClass>, IEqualComparable<TClass>, IJsonModel<TClass>
{
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is TClass t
                   ? CompareTo(t)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(TClass));
    }
    public abstract int  CompareTo( TClass? other );
    public abstract bool Equals( TClass?    other );

    
    public static bool TryFromJson( string? json, [NotNullWhen(true)] out TClass? result )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace(json) )
            {
                result = null;
                return false;
            }

            result = FromJson(json);
            return true;
        }
        catch ( Exception e ) { SelfLogger.WriteLine("{Exception}", e.ToString()); }

        result = null;
        return false;
    }
    public static TClass FromJson( string json ) => json.FromJson<TClass>();
}



public abstract record ObservableRecord<TClass, TID> : ObservableRecord<TClass>, IUniqueID<TID>
    where TClass : ObservableRecord<TClass, TID>, IEqualComparable<TClass>, IJsonModel<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    private TID __id;


    public virtual TID ID { get => __id; init => __id = value; }


    protected ObservableRecord() : base() { }
    protected ObservableRecord( TID id ) => ID = id;


    protected bool SetID( TClass record ) => SetID(record.ID);
    protected bool SetID( TID    id )     => SetProperty(ref __id, id, nameof(ID));
}
