namespace Jakar.Extensions;


[Serializable]
public class BaseClass : IJsonModel, IObservableObject, IDisposable
{
    protected bool                             _disposed;
    protected Dictionary<string, JsonElement>? _additionalData;


    /// <summary> JSON members this type doesn't declare, kept so they survive a round trip. </summary>
    [JsonExtensionData] public virtual Dictionary<string, JsonElement>? AdditionalData { get => _additionalData; set => _additionalData = value; }


    public event PropertyChangedEventHandler?  PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;


    protected virtual void Dispose( bool disposing ) => _disposed |= disposing;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~BaseClass() => Dispose(false);
    [StackTraceHidden] protected void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);


    [NotifyPropertyChangedInvocator] public void OnPropertyChanged( [CallerMemberName] string property = EMPTY ) => OnPropertyChanged(property.AsPropertyChangedEventArgs());
    [NotifyPropertyChangedInvocator] public void OnPropertyChanged( PropertyChangedEventArgs  e )                => PropertyChanged?.Invoke(this, e);


    public void OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging(property.AsPropertyChangingEventArgs());
    public void OnPropertyChanging( PropertyChangingEventArgs e )                => PropertyChanging?.Invoke(this, e);


    protected virtual bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value )
    {
        if ( EqualityComparer<TValue>.Default.Equals(backingStore, value) ) { return false; }

        backingStore = value;
        return true;
    }
    protected virtual bool SetProperty<TValue>( ref TValue backingStore, TValue value, [CallerMemberName] string propertyName = EMPTY )
    {
        if ( EqualityComparer<TValue>.Default.Equals(backingStore, value) ) { return false; }

        OnPropertyChanging(propertyName);
        backingStore = value;
        OnPropertyChanged(propertyName);

        return true;
    }
    protected virtual bool SetProperty<TValue>( ref TValue backingStore, TValue value, in TValue minValue, [CallerMemberName] string propertyName = EMPTY )
        where TValue : IComparisonOperators<TValue, TValue, bool>
    {
        value = value < minValue
                    ? minValue
                    : value;

        return SetProperty(ref backingStore, value, propertyName);
    }


#pragma warning disable CS4026 // The CallerMemberNameAttribute will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
    bool IObservableObject.SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value )                                                 => SetPropertyWithoutNotify(ref backingStore, value);
    bool IObservableObject.SetProperty<TValue>( ref              TValue backingStore, TValue value, [CallerMemberName] string propertyName = EMPTY ) => SetProperty(ref backingStore, value, propertyName);
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


    public static TSelf            FromJson( string                         json )                                                     => JsonModel.FromJson(json,     TSelf.JsonTypeInfo);
    public static TSelf            FromJson( ReadOnlySpan<byte>             utf8Json )                                                 => JsonModel.FromJson(utf8Json, TSelf.JsonTypeInfo);
    public static bool             TryFromJson( [NotNullWhen(true)] string? json,     [NotNullWhen(true)] out TSelf? result )          => JsonModel.TryFromJson(json,     TSelf.JsonTypeInfo, out result);
    public static bool             TryFromJson( ReadOnlySpan<byte>          utf8Json, [NotNullWhen(true)] out TSelf? result )          => JsonModel.TryFromJson(utf8Json, TSelf.JsonTypeInfo, out result);
    public static ValueTask<TSelf> FromJsonAsync( Stream                    stream,   CancellationToken              token = default ) => JsonModel.FromJsonAsync(stream, TSelf.JsonTypeInfo, token);


    /// <summary> The JSON of this instance (source-generated metadata, no reflection), or the type name when no metadata is registered for it. </summary>
    public override string ToString()
    {
        try { return ( (TSelf)this ).ToJson(); }
        catch ( NotSupportedException ) { return GetType().Name; }
    }


    public TSelf WithAdditionalData( IJsonModel value ) => WithAdditionalData(value.AdditionalData);
    public virtual TSelf WithAdditionalData( IReadOnlyDictionary<string, JsonElement>? additionalData )
    {
        _additionalData = Json.Merge(_additionalData, additionalData);
        return (TSelf)this;
    }
}
