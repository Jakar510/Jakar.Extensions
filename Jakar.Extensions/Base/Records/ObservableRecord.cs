namespace Jakar.Extensions;


[ SuppressMessage( "ReSharper", "VirtualMemberNeverOverridden.Global" ) ]
public record ObservableRecord : BaseRecord, INotifyPropertyChanged, INotifyPropertyChanging
{
#if NET6_0_OR_GREATER
    public static DateOnly SQLMinDateOnly { get; } = new(SQLMinDate.Date.Year, SQLMinDate.Date.Month, SQLMinDate.Date.Day);
#endif


    public ObservableRecord() { }
    public static readonly DateTime SQLMinDate = DateTime.Parse( "1/1/1753 12:00:00 AM", CultureInfo.InvariantCulture );


    /// <summary>
    ///     <para> Checks the values with <paramref name="comparer"/> . </para>
    ///     Then calls <see cref="OnPropertyChanging(string)"/> , sets the value, then calls <see cref="OnPropertyChanged(string)"/>
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="backingStore"> </param>
    /// <param name="value"> </param>
    /// <param name="comparer"> </param>
    /// <param name="propertyName"> </param>
    /// <returns>
    ///     <para> Returns <see langword="false"/> if the values are equal, and therefore the <paramref name="backingStore"/> was not changed, otherwise <see langword="true"/> </para>
    /// </returns>
    protected virtual bool SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, [ CallerMemberName ] string? propertyName = default )
    {
        if ( comparer.Equals( backingStore, value ) ) { return false; }

        OnPropertyChanging( propertyName );
        backingStore = value;
        OnPropertyChanged( propertyName );

        return true;
    }


    /// <summary>
    ///     <para> Checks the values with <see cref="EqualityComparer{T}.Default"/> . </para>
    ///     Then calls <see cref="OnPropertyChanging(string)"/> , sets the value, then calls <see cref="OnPropertyChanged(string)"/>
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="backingStore"> </param>
    /// <param name="value"> </param>
    /// <param name="propertyName"> </param>
    /// <returns>
    ///     <para> Returns <see langword="false"/> if the values are equal, and therefore the <paramref name="backingStore"/> was not changed, otherwise <see langword="true"/> </para>
    /// </returns>
    protected virtual bool SetProperty<T>( ref T backingStore, T value, [ CallerMemberName ] string? propertyName = default ) => SetProperty( ref backingStore, value, EqualityComparer<T>.Default, propertyName );


    protected virtual bool SetProperty( ref DateTime backingStore, DateTime value, [ CallerMemberName ] string? propertyName = default ) => SetProperty( ref backingStore, value, SQLMinDate, propertyName );
    protected virtual bool SetProperty( ref DateTime backingStore, DateTime value, in DateTime minDate, [ CallerMemberName ] string? propertyName = default )
    {
        value = value < minDate
                    ? minDate
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<DateTime>.Default, propertyName );
    }
    protected virtual bool SetProperty( ref DateTime? backingStore, DateTime? value, [ CallerMemberName ] string? propertyName = default ) => SetProperty( ref backingStore, value, SQLMinDate, propertyName );
    protected virtual bool SetProperty( ref DateTime? backingStore, DateTime? value, in DateTime minDate, [ CallerMemberName ] string? propertyName = default )
    {
        value = value is null || value < minDate
                    ? null
                    : value.Value;

        if ( value.HasValue ) { value = DateTime.SpecifyKind( value.Value, DateTimeKind.Utc ); }

        return SetProperty( ref backingStore, value, propertyName );
    }


    protected virtual bool SetProperty( ref DateTimeOffset  backingStore, DateTimeOffset  value, [ CallerMemberName ] string? propertyName = default ) => SetProperty( ref backingStore, value, SQLMinDate, propertyName );
    protected virtual bool SetProperty( ref DateTimeOffset? backingStore, DateTimeOffset? value, [ CallerMemberName ] string? propertyName = default ) => SetProperty( ref backingStore, value, SQLMinDate, propertyName );
    protected virtual bool SetProperty( ref DateTimeOffset backingStore, DateTimeOffset value, in DateTimeOffset minDate, [ CallerMemberName ] string? propertyName = default )
    {
        value = value < minDate
                    ? minDate
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<DateTimeOffset>.Default, propertyName );
    }
    protected virtual bool SetProperty( ref DateTimeOffset? backingStore, DateTimeOffset? value, in DateTimeOffset minDate, [ CallerMemberName ] string? propertyName = default )
    {
        value = value < minDate
                    ? null
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<DateTimeOffset>.Default, propertyName );
    }


    protected virtual bool SetProperty( ref TimeSpan backingStore, TimeSpan value, in TimeSpan minValue, [ CallerMemberName ] string? propertyName = default )
    {
        value = value < minValue
                    ? minValue
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<TimeSpan>.Default, propertyName );
    }
    protected virtual bool SetProperty( ref TimeSpan? backingStore, TimeSpan? value, in TimeSpan? minValue, [ CallerMemberName ] string? propertyName = default )
    {
        value = value < minValue
                    ? null
                    : value;


        return SetProperty( ref backingStore, value, ValueEqualizer<TimeSpan>.Default, propertyName );
    }


    /// <summary>
    ///     <para> Checks the values with <see cref="EqualityComparer{T}.Default"/> . </para>
    ///     Then calls <see cref="OnPropertyChanging(string)"/> , sets the value, then calls <see cref="OnPropertyChanged(string)"/>
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="backingStore"> </param>
    /// <param name="value"> </param>
    /// <returns>
    ///     <para> Returns <see langword="false"/> if the values are equal, and therefore the <paramref name="backingStore"/> was not changed, otherwise <see langword="true"/> </para>
    /// </returns>
    protected virtual bool SetPropertyWithoutNotify<T>( ref T backingStore, T value ) => SetPropertyWithoutNotify( ref backingStore, value, EqualityComparer<T>.Default );

    /// <summary>
    ///     <para> Checks the values with <paramref name="comparer"/> . </para>
    ///     Then calls <see cref="OnPropertyChanging(string)"/> , sets the value, then calls <see cref="OnPropertyChanged(string)"/>
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="backingStore"> </param>
    /// <param name="value"> </param>
    /// <param name="comparer"> </param>
    /// <returns>
    ///     <para> Returns <see langword="false"/> if the values are equal, and therefore the <paramref name="backingStore"/> was not changed, otherwise <see langword="true"/> </para>
    /// </returns>
    protected virtual bool SetPropertyWithoutNotify<T>( ref T backingStore, T value, IEqualityComparer<T> comparer )
    {
        if ( comparer.Equals( backingStore, value ) ) { return false; }

        backingStore = value;
        return true;
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual Task SetProperty<T>( ref T backingStore, T value, Func<Task> onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return Task.CompletedTask; }

        return onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual Task SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Func<Task> onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return Task.CompletedTask; }

        return onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual Task SetProperty<T>( ref T backingStore, T value, Func<T, Task> onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return Task.CompletedTask; }

        return onChanged( value );
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual Task SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Func<T, Task> onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return Task.CompletedTask; }

        return onChanged( value );
    }


    [ NotifyPropertyChangedInvocator ] protected virtual void OnPropertyChanged( [ CallerMemberName ] string?  property = default ) => OnPropertyChanged( new PropertyChangedEventArgs( property ?? string.Empty ) );
    [ NotifyPropertyChangedInvocator ] protected virtual void OnPropertyChanged( PropertyChangedEventArgs      e )                  => PropertyChanged?.Invoke( this, e );
    protected virtual                                    void OnPropertyChanging( [ CallerMemberName ] string? property = default ) => OnPropertyChanging( new PropertyChangingEventArgs( property ?? string.Empty ) );
    protected virtual                                    void OnPropertyChanging( PropertyChangingEventArgs    e )                  => PropertyChanging?.Invoke( this, e );


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, ICommand onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return; }

        if ( onChanged.CanExecute( value ) ) { onChanged.Execute( value ); }
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, ICommand onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return; }

        if ( onChanged.CanExecute( value ) ) { onChanged.Execute( value ); }
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, Action onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return; }

        onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Action onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return; }

        onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, Action<T> onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return; }

        onChanged( value );
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Action<T> onChanged, [ CallerMemberName ] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return; }

        onChanged( value );
    }


    public event PropertyChangedEventHandler?  PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;


#if NET6_0_OR_GREATER
    protected virtual bool SetProperty( ref TimeOnly backingStore, TimeOnly value, in TimeOnly minDate, [CallerMemberName] string? propertyName = default )
    {
        value = value < minDate
                    ? minDate
                    : value;


        return SetProperty( ref backingStore, value, EqualityComparer<TimeOnly>.Default, propertyName );
    }
    protected virtual bool SetProperty( ref TimeOnly? backingStore, TimeOnly? value, in TimeOnly minDate, [CallerMemberName] string? propertyName = default )
    {
        value = value < minDate
                    ? minDate
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<TimeOnly>.Default, propertyName );
    }


    protected virtual bool SetProperty( ref DateOnly backingStore, DateOnly value, [CallerMemberName] string? propertyName = default ) => SetProperty( ref backingStore, value, SQLMinDateOnly, propertyName );
    protected virtual bool SetProperty( ref DateOnly backingStore, DateOnly value, in DateOnly minDate, [CallerMemberName] string? propertyName = default )
    {
        value = value < minDate
                    ? minDate
                    : value;


        return SetProperty( ref backingStore, value, EqualityComparer<DateOnly>.Default, propertyName );
    }
    protected virtual bool SetProperty( ref DateOnly? backingStore, DateOnly? value, [CallerMemberName] string? propertyName = default ) => SetProperty( ref backingStore, value, SQLMinDateOnly, propertyName );
    protected virtual bool SetProperty( ref DateOnly? backingStore, DateOnly? value, in DateOnly minDate, [CallerMemberName] string? propertyName = default )
    {
        value = value < minDate
                    ? minDate
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<DateOnly>.Default, propertyName );
    }
#endif
}



public abstract record ObservableRecord<TRecord> : ObservableRecord, IEquatable<TRecord>, IComparable<TRecord>, IComparable where TRecord : ObservableRecord<TRecord>
{
    protected ObservableRecord() { }


    public static TRecord? FromJson( [ NotNullIfNotNull( "json" ) ] string? json ) => json?.FromJson<TRecord>();


    public string ToJson()       => JsonNet.ToJson( this );
    public string ToPrettyJson() => this.ToJson( Formatting.Indented );


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is TRecord value
                   ? CompareTo( value )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(TRecord) );
    }
    public abstract int  CompareTo( TRecord? other );
    public abstract bool Equals( TRecord?    other );
}



public abstract record ObservableRecord<TRecord, TID> : ObservableRecord<TRecord>, IUniqueID<TID> where TRecord : ObservableRecord<TRecord, TID>
                                                                                                  where TID : struct, IComparable<TID>, IEquatable<TID>
{
    private TID _id;


    [ Key ]
    public virtual TID ID
    {
        get => _id;
        init => _id = value;
    }


    protected ObservableRecord() : base() { }
    protected ObservableRecord( TID id ) => ID = id;


    protected bool SetID( TRecord record ) => SetID( record.ID );
    protected bool SetID( TID     id )     => SetProperty( ref _id, id, nameof(ID) );
}
