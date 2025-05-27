using System.Numerics;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "VirtualMemberNeverOverridden.Global" )]
public record ObservableRecord : BaseRecord, IObservableObject
{
    public static readonly DateTime SQLMinDate = DateTime.Parse( "1/1/1753 12:00:00 AM", CultureInfo.InvariantCulture );
    public static          DateOnly SQLMinDateOnly => SQLMinDate.AsDateOnly();


    public event PropertyChangedEventHandler?  PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;


#pragma warning disable CS4026 // The CallerMemberNameAttribute will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments

    [NotifyPropertyChangedInvocator] protected internal void                   OnPropertyChanged( [CallerMemberName] string  property = EMPTY ) => OnPropertyChanged( property.GetPropertyChangedEventArgs() );
    [NotifyPropertyChangedInvocator]                    void IObservableObject.OnPropertyChanged( [CallerMemberName] string  property = EMPTY ) => OnPropertyChanged( property );
    [NotifyPropertyChangedInvocator]                    void IObservableObject.OnPropertyChanged( PropertyChangedEventArgs   e )                => OnPropertyChanged( e );
    [NotifyPropertyChangedInvocator] protected internal void                   OnPropertyChanged( PropertyChangedEventArgs   e )                => PropertyChanged?.Invoke( this, e );
    protected internal                                  void                   OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging( property.GetPropertyChangingEventArgs() );
    void IObservableObject.                                                    OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging( property );
    [NotifyPropertyChangedInvocator] void IObservableObject.                   OnPropertyChanging( PropertyChangingEventArgs e )                => OnPropertyChanging( e );
    protected internal               void                                      OnPropertyChanging( PropertyChangingEventArgs e )                => PropertyChanging?.Invoke( this, e );


    bool IObservableObject.SetProperty<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY ) => SetProperty( ref backingStore, value, equalityComparer, propertyName );
    protected virtual bool SetProperty<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY ) => this.SetPropertyNotify( ref backingStore, value, equalityComparer );


    bool IObservableObject.SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null ) => SetPropertyWithoutNotify( ref backingStore, value, equalityComparer );
    protected virtual bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null ) => ObservableObjects.SetPropertyWithoutNotify( ref backingStore, value, equalityComparer );

#pragma warning restore CS4026 // The CallerMemberNameAttribute will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning restore CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
}



public abstract record ObservableRecord<TClass> : ObservableRecord, IEquatable<TClass>, IComparable<TClass>, IComparable, IParsable<TClass>
    where TClass : ObservableRecord<TClass>, IComparisonOperators<TClass>
{
    public static Equalizer<TClass> Equalizer { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Equalizer<TClass>.Default; }
    public static Sorter<TClass>    Sorter    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<TClass>.Default; }


    public static TClass? FromJson( [NotNullIfNotNull( nameof(json) )] string? json ) => json?.FromJson<TClass>();
    public        string   ToJson()                                                    => this.ToJson( Formatting.None );
    public        string   ToPrettyJson()                                              => this.ToJson( Formatting.Indented );


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is TClass t
                   ? CompareTo( t )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(TClass) );
    }
    public abstract int  CompareTo( TClass? other );
    public abstract bool Equals( TClass?    other );


    public static TClass Parse( [NotNullIfNotNull( nameof(json) )] string? json, IFormatProvider? provider )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( json );
        return json.FromJson<TClass>();
    }
    public static bool TryParse( [NotNullWhen( true )] string? json, IFormatProvider? provider, [NotNullWhen( true )] out TClass? result )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try
        {
            result = json?.FromJson<TClass>();
            return result is not null;
        }
        catch ( Exception e )
        {
            telemetrySpan.AddException( e );
            result = null;
            return false;
        }
    }
}



public abstract record ObservableRecord<TClass, TID> : ObservableRecord<TClass>, IUniqueID<TID>
    where TClass : ObservableRecord<TClass, TID>, IComparisonOperators<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    private TID _id;


    public virtual TID ID { get => _id; init => _id = value; }


    protected ObservableRecord() : base() { }
    protected ObservableRecord( TID id ) => ID = id;


    protected bool SetID( TClass record ) => SetID( record.ID );
    protected bool SetID( TID     id )     => SetProperty( ref _id, id, ValueEqualizer<TID>.Default, nameof(ID) );
}
