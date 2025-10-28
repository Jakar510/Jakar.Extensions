namespace Jakar.Extensions;


[Serializable]
public class BaseClass : IJsonModel, IObservableObject
{
    protected JsonObject? _additionalData;

    [JsonExtensionData] public virtual JsonObject? AdditionalData { get => _additionalData; set => _additionalData = value; }


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



[Serializable]
public abstract class BaseClass<TSelf> : BaseClass, IEquatable<TSelf>, IComparable<TSelf>, IComparable
    where TSelf : BaseClass<TSelf>, IJsonModel<TSelf>
{
    public abstract bool Equals( TSelf?    other );
    public abstract int  CompareTo( TSelf? other );


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is TSelf t
                   ? CompareTo(t)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(TSelf));
    }


    public override bool Equals( object? other ) => ReferenceEquals(this, other) || ( other is TSelf x && Equals(x) );
    public override int  GetHashCode()           => RuntimeHelpers.GetHashCode(this);


    public static bool TryFromJson( string? json, [NotNullWhen(true)] out TSelf? result )
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
    public static TSelf FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, TSelf.JsonTypeInfo));


    public TSelf WithAdditionalData( IJsonModel value ) => WithAdditionalData(value.AdditionalData);
    public virtual TSelf WithAdditionalData( JsonObject? additionalData )
    {
        if ( additionalData is null ) { return (TSelf)this; }

        JsonObject json = _additionalData ??= new JsonObject();
        foreach ( ( string key, JsonNode? jToken ) in additionalData ) { json[key] = jToken; }

        return (TSelf)this;
    }
}
