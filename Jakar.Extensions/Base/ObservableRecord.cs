#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
    namespace Jakar.Extensions;


    [SuppressMessage( "ReSharper", "VirtualMemberNeverOverridden.Global" )]
    public record ObservableRecord : BaseRecord, IObservableObject
    {
        public static readonly DateTime SQLMinDate = DateTime.Parse( "1/1/1753 12:00:00 AM", CultureInfo.InvariantCulture );
        public static          DateOnly SQLMinDateOnly => SQLMinDate.AsDateOnly();


        public event PropertyChangedEventHandler?  PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;


        [NotifyPropertyChangedInvocator] protected internal void                   OnPropertyChanged( [CallerMemberName] string  property = EMPTY ) => OnPropertyChanged( property.GetPropertyChangedEventArgs() );
        [NotifyPropertyChangedInvocator]                    void IObservableObject.OnPropertyChanged( [CallerMemberName] string  property = EMPTY ) => OnPropertyChanged( property );
        [NotifyPropertyChangedInvocator]                    void IObservableObject.OnPropertyChanged( PropertyChangedEventArgs   e )                => OnPropertyChanged( e );
        [NotifyPropertyChangedInvocator] protected internal void                   OnPropertyChanged( PropertyChangedEventArgs   e )                => PropertyChanged?.Invoke( this, e );
        protected internal                                  void                   OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging( property.GetPropertyChangingEventArgs() );
        void IObservableObject.                                                    OnPropertyChanging( [CallerMemberName] string property = EMPTY ) => OnPropertyChanging( property );
        [NotifyPropertyChangedInvocator] void IObservableObject.                   OnPropertyChanging( PropertyChangingEventArgs e )                => OnPropertyChanging( e );
        protected internal               void                                      OnPropertyChanging( PropertyChangingEventArgs e )                => PropertyChanging?.Invoke( this, e );


        bool IObservableObject.SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY ) => SetProperty( ref backingStore, value, equalityComparer, propertyName );
        protected virtual bool SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
        {
            equalityComparer ??= EqualityComparer<T>.Default;
            if ( equalityComparer.Equals( backingStore, value ) ) { return false; }

            OnPropertyChanging( propertyName );

            backingStore = value;

            OnPropertyChanged( propertyName );

            return true;
        }


        bool IObservableObject.SetPropertyWithoutNotify<T>( ref T backingStore, T value, IEqualityComparer<T>? equalityComparer = null ) => SetPropertyWithoutNotify( ref backingStore, value, equalityComparer );
        protected virtual bool SetPropertyWithoutNotify<T>( ref T backingStore, T value, IEqualityComparer<T>? equalityComparer = null )
        {
            equalityComparer ??= EqualityComparer<T>.Default;
            if ( equalityComparer.Equals( backingStore, value ) ) { return false; }

            backingStore = value;
            return true;
        }
    }



    public abstract record ObservableRecord<TRecord> : ObservableRecord, IEquatable<TRecord>, IComparable<TRecord>, IComparable
        where TRecord : ObservableRecord<TRecord>
    {
        public static Equalizer<TRecord> Equalizer { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Equalizer<TRecord>.Default; }
        public static Sorter<TRecord>    Sorter    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<TRecord>.Default; }


        public static TRecord? FromJson( [NotNullIfNotNull( nameof(json) )] string? json ) => json?.FromJson<TRecord>();
        public        string   ToJson()                                                    => this.ToJson( Formatting.None );
        public        string   ToPrettyJson()                                              => this.ToJson( Formatting.Indented );


        public int CompareTo( object? other )
        {
            if ( other is null ) { return 1; }

            if ( ReferenceEquals( this, other ) ) { return 0; }

            return other is TRecord t
                       ? CompareTo( t )
                       : throw new ExpectedValueTypeException( nameof(other), other, typeof(TRecord) );
        }
        public abstract int  CompareTo( TRecord? other );
        public abstract bool Equals( TRecord?    other );
    }



    public abstract record ObservableRecord<TRecord, TID> : ObservableRecord<TRecord>, IUniqueID<TID>
        where TRecord : ObservableRecord<TRecord, TID>
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    {
        private TID _id;


        public virtual TID ID { get => _id; init => _id = value; }


        protected ObservableRecord() : base() { }
        protected ObservableRecord( TID id ) => ID = id;


        protected bool SetID( TRecord record ) => SetID( record.ID );
        protected bool SetID( TID     id )     => SetProperty( ref _id, id, ValueEqualizer<TID>.Default, nameof(ID) );
    }
