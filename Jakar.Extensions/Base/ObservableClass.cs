namespace Jakar.Extensions;


[Serializable]
public abstract class ObservableClass : BaseClass, INotifyPropertyChanged, INotifyPropertyChanging // IDataBaseID
{
    public static readonly DateTime sqlMinDate = DateTime.Parse( "1/1/1753 12:00:00 AM", CultureInfo.InvariantCulture );

    public event PropertyChangedEventHandler?  PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;


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
    protected virtual bool SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = default )
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
    protected virtual bool SetProperty<T>( ref T backingStore, T value, [CallerMemberName] string? propertyName = default ) => SetProperty( ref backingStore, value, EqualityComparer<T>.Default, propertyName );


    protected virtual bool SetProperty( ref DateTime backingStore, DateTime value, in DateTime minDate, [CallerMemberName] string? caller = default )
    {
        value = value < minDate
                    ? minDate
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<DateTime>.Default, caller );
    }
    protected virtual bool SetProperty( ref DateTime? backingStore, DateTime? value, in DateTime minDate, [CallerMemberName] string? caller = default )
    {
        value = value < minDate
                    ? null
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<DateTime>.Default, caller );
    }


    protected virtual bool SetProperty( ref DateTimeOffset backingStore, DateTimeOffset value, in DateTimeOffset minDate, [CallerMemberName] string? caller = default )
    {
        value = value < minDate
                    ? minDate
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<DateTimeOffset>.Default, caller );
    }
    protected virtual bool SetProperty( ref DateTimeOffset? backingStore, DateTimeOffset? value, in DateTimeOffset minDate, [CallerMemberName] string? caller = default )
    {
        value = value < minDate
                    ? null
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<DateTimeOffset>.Default, caller );
    }


    protected virtual bool SetProperty( ref TimeSpan backingStore, TimeSpan value, in TimeSpan minValue, [CallerMemberName] string? caller = default )
    {
        value = value < minValue
                    ? minValue
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<TimeSpan>.Default, caller );
    }
    protected virtual bool SetProperty( ref TimeSpan? backingStore, TimeSpan? value, in TimeSpan? minValue, [CallerMemberName] string? caller = default )
    {
        value = value < minValue
                    ? null
                    : value;

        return SetProperty( ref backingStore, value, ValueEqualizer<TimeSpan>.Default, caller );
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
    protected virtual Task SetProperty<T>( ref T backingStore, T value, Func<Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return Task.CompletedTask; }

        return onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual Task SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Func<Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return Task.CompletedTask; }

        return onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual Task SetProperty<T>( ref T backingStore, T value, Func<T, Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return Task.CompletedTask; }

        return onChanged( value );
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual Task SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Func<T, Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return Task.CompletedTask; }

        return onChanged( value );
    }


    [NotifyPropertyChangedInvocator] protected virtual void OnPropertyChanged( [CallerMemberName] string? property = default ) => OnPropertyChanged( new PropertyChangedEventArgs( property ?? string.Empty ) );

    [NotifyPropertyChangedInvocator] protected virtual void OnPropertyChanged( PropertyChangedEventArgs    e )                  => PropertyChanged?.Invoke( this, e );
    protected virtual                                  void OnPropertyChanging( [CallerMemberName] string? property = default ) => OnPropertyChanging( new PropertyChangingEventArgs( property ?? string.Empty ) );
    protected virtual                                  void OnPropertyChanging( PropertyChangingEventArgs  e )                  => PropertyChanging?.Invoke( this, e );


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, ICommand onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return; }

        if ( onChanged.CanExecute( value ) ) { onChanged.Execute( value ); }
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, ICommand onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return; }

        if ( onChanged.CanExecute( value ) ) { onChanged.Execute( value ); }
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, Action onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return; }

        onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Action onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return; }

        onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, Action<T> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, propertyName ) ) { return; }

        onChanged( value );
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    protected virtual void SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Action<T> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty( ref backingStore, value, comparer, propertyName ) ) { return; }

        onChanged( value );
    }
}



[Serializable]
public abstract class ObservableClass<TClass> : ObservableClass, IEquatable<TClass>, IComparable<TClass>, IComparable
    where TClass : ObservableClass<TClass>
{
    public static Equalizer<TClass> Equalizer { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Equalizer<TClass>.Default; }
    public static Sorter<TClass>    Sorter    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<TClass>.Default; }


    public static TClass? FromJson( [NotNullIfNotNull( nameof(json) )] string? json ) => json?.FromJson<TClass>();
    public        string  ToJson()                                                    => this.ToJson( Formatting.None );
    public        string  ToPrettyJson()                                              => this.ToJson( Formatting.Indented );


    public abstract bool Equals( TClass?    other );
    public abstract int  CompareTo( TClass? other );


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is TClass t
                   ? CompareTo( t )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(TClass) );
    }
    public sealed override bool Equals( object? other ) => ReferenceEquals( this, other ) || other is TClass file && Equals( file );
    public override        int  GetHashCode()           => RuntimeHelpers.GetHashCode( this );
}



public abstract class ObservableClass<TRecord, TID> : ObservableClass<TRecord>, IUniqueID<TID>
    where TRecord : ObservableClass<TRecord, TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    private TID _id;


    public virtual TID ID { get => _id; init => _id = value; }


    protected ObservableClass() : base() { }
    protected ObservableClass( TID id ) => ID = id;


    protected bool SetID( TRecord record ) => SetID( record.ID );
    protected bool SetID( TID     id )     => SetProperty( ref _id, id, nameof(ID) );
}
