#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
    namespace Jakar.Extensions;


    [Serializable]
    public abstract class ObservableClass : BaseClass, IObservableObject
    {
        public static DateTime SQLMinDate     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ObservableRecord.SQLMinDate; }
        public static DateOnly SQLMinDateOnly { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ObservableRecord.SQLMinDateOnly; }


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


        bool IObservableObject.SetProperty<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY ) => SetProperty( ref backingStore, value, equalityComparer, propertyName );
        protected virtual bool SetProperty<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
        {
            equalityComparer ??= EqualityComparer<TValue>.Default;
            if ( equalityComparer.Equals( backingStore, value ) ) { return false; }

            OnPropertyChanging( propertyName );

            backingStore = value;

            OnPropertyChanged( propertyName );

            return true;
        }


        bool IObservableObject.SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null ) => SetPropertyWithoutNotify( ref backingStore, value, equalityComparer );
        protected virtual bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null )
        {
            equalityComparer ??= EqualityComparer<TValue>.Default;
            if ( equalityComparer.Equals( backingStore, value ) ) { return false; }

            backingStore = value;
            return true;
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
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    {
        private TID _id;


        public virtual TID ID { get => _id; init => _id = value; }


        protected ObservableClass() : base() { }
        protected ObservableClass( TID id ) => ID = id;


        protected bool SetID( TRecord record ) => SetID( record.ID );
        protected bool SetID( TID     id )     => SetProperty( ref _id, id, ValueEqualizer<TID>.Default, nameof(ID) );
    }
