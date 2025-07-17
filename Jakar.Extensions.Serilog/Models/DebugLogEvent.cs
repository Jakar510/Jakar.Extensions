using System.Reflection;
using ZLinq;



namespace Jakar.Extensions.Serilog;


public sealed class DebugLogEvent( string source, string message, string caller, LogEventLevel level, DateTimeOffset timeStamp, EventDetails? details = null ) : BaseClass, ILastModified, IComparable<DebugLogEvent>, IEquatable<DebugLogEvent>
{
    public        DateTimeOffset        TimeStamp    { get; } = timeStamp;
    public        LogEventLevel         Level        { get; } = level;
    public        string                Caller       { get; } = caller;
    public        string                Description  { get; } = $"[{level}] {source}.{caller} -> {message}";
    public        string                Message      { get; } = message;
    public        string                Source       { get; } = source;
    public static Sorter<DebugLogEvent> Sorter       => Sorter<DebugLogEvent>.Default;
    DateTimeOffset? ILastModified.      LastModified => TimeStamp;


    private DebugLogEvent( MemberInfo source, string message, string caller, LogEventLevel level, EventDetails? details = null ) : this( source.Name, message, caller, level, details ) { }
    private DebugLogEvent( string     source, string message, string caller, LogEventLevel level, EventDetails? details = null ) : this( source, message, caller, level, DateTimeOffset.UtcNow ) { }


    public static DebugLogEvent Create<TValue>( string message, LogEventLevel level = LogEventLevel.Debug, EventDetails? details = null,                [CallerMemberName] string caller  = EMPTY )                                          => Create( typeof(TValue), message, level, details, caller );
    public static DebugLogEvent Create( Type           source,  string        message,                     LogEventLevel level   = LogEventLevel.Debug, EventDetails?             details = null, [CallerMemberName] string caller = EMPTY ) => new(source, message, caller, level, details);
    public static DebugLogEvent Create( string         source,  string        message,                     LogEventLevel level,                         EventDetails?             details = null, [CallerMemberName] string caller = EMPTY ) => new(source, message, caller, level, details);


    public static DebugLogEvent Error<TValue>( TValue _,      Exception e, EventDetails? details = null, [CallerMemberName] string caller = EMPTY ) => Create<TValue>( e.Message, LogEventLevel.Error, details, caller );
    public static DebugLogEvent Error( Type           source, Exception e, EventDetails? details = null, [CallerMemberName] string caller = EMPTY ) => Create( source, e.Message, LogEventLevel.Error, details, caller );
    public static DebugLogEvent Error( string         source, Exception e, EventDetails? details = null, [CallerMemberName] string caller = EMPTY ) => Create( source, e.Message, LogEventLevel.Error, details, caller );


    public static DebugLogEvent Fatal<TValue>( TValue _,      Exception e, EventDetails? details = null, [CallerMemberName] string caller = EMPTY ) => Create<TValue>( e.Message, LogEventLevel.Fatal, details, caller );
    public static DebugLogEvent Fatal( Type           source, Exception e, EventDetails? details = null, [CallerMemberName] string caller = EMPTY ) => Create( source, e.Message, LogEventLevel.Fatal, details, caller );
    public static DebugLogEvent Fatal( string         source, Exception e, EventDetails? details = null, [CallerMemberName] string caller = EMPTY ) => Create( source, e.Message, LogEventLevel.Fatal, details, caller );


    public override string ToString() => Description;
    public int CompareTo( DebugLogEvent? other )
    {
        if ( other is null ) { return 1; }

        int lastModifiedComparison = TimeStamp.CompareTo( other.TimeStamp );
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        int sourceComparison = string.Compare( Source, other.Source, StringComparison.Ordinal );
        if ( sourceComparison != 0 ) { return sourceComparison; }

        int callerComparison = string.Compare( Caller, other.Caller, StringComparison.Ordinal );
        if ( callerComparison != 0 ) { return callerComparison; }

        int messageComparison = string.Compare( Message, other.Message, StringComparison.Ordinal );
        if ( messageComparison != 0 ) { return messageComparison; }

        return Comparer<LogEventLevel>.Default.Compare( Level, other.Level );
    }
    public bool Equals( DebugLogEvent? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Description == other.Description && Source == other.Source && Message == other.Message && Caller == other.Caller && Level == other.Level && TimeStamp.Equals( other.TimeStamp );
    }
    public override bool Equals( object? obj )                                    => ReferenceEquals( this, obj ) || obj is DebugLogEvent other && Equals( other );
    public override int  GetHashCode()                                            => HashCode.Combine( Description, Source, Message, Caller, (int)Level, TimeStamp );
    public static   bool operator ==( DebugLogEvent? left, DebugLogEvent? right ) => Sorter.Equals( left, right );
    public static   bool operator !=( DebugLogEvent? left, DebugLogEvent? right ) => Sorter.DoesNotEqual( left, right );
    public static   bool operator <( DebugLogEvent?  left, DebugLogEvent? right ) => Sorter.Compare( left, right ) < 0;
    public static   bool operator >( DebugLogEvent?  left, DebugLogEvent? right ) => Sorter.Compare( left, right ) > 0;
    public static   bool operator <=( DebugLogEvent? left, DebugLogEvent? right ) => Sorter.Compare( left, right ) <= 0;
    public static   bool operator >=( DebugLogEvent? left, DebugLogEvent? right ) => Sorter.Compare( left, right ) >= 0;



    [SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" )]
    public sealed class Collection() : ConcurrentObservableCollection<DebugLogEvent>( Buffers.DEFAULT_CAPACITY )
    {
        private readonly ConcurrentBag<DebugLogEvent> _pending   = [];
        private readonly SynchronizedValue<bool>      _isEnabled = new(true);
        private readonly SynchronizedValue<bool>      _isPaused  = new(false);
        private          DateTimeOffset?              _end;
        private          DateTimeOffset?              _start;
        private          LogEventLevel                _level = LogEventLevel.Verbose;
        private          string?                      _source;


        public int DaysToShow
        {
            get
            {
                if ( _start.HasValue && _end.HasValue ) { return (_start.Value - _end.Value).Days; }

                if ( _start.HasValue ) { return (DateTimeOffset.UtcNow - _start.Value).Days; }

                if ( _end.HasValue ) { return (_end.Value - MinDate).Days; }

                return (DateTimeOffset.UtcNow - MinDate).Days;
            }
        }
        public bool IsEnabled { get => _isEnabled; set => _isEnabled.Value = value; }
        public bool IsPaused
        {
            get => _isPaused.Value;
            private set
            {
                _isPaused.Value = value;
                OnPropertyChanged();
            }
        }
        public DateTimeOffset MinDate
        {
            get
            {
                using ( AcquireLock() ) { return buffer.Min( static x => x.TimeStamp ); }
            }
        }
        public HashSet<string> Sources
        {
            get
            {
                using ( AcquireLock() ) { return [..buffer.AsValueEnumerable().Select( static x => x.Source )]; }
            }
        }


        public Collection( params ReadOnlySpan<DebugLogEvent> enumerable ) : this() => Add( enumerable );


        protected override bool Filter( int index, ref readonly DebugLogEvent? log )
        {
            if ( log is null ) { return false; }

            if ( log.Level < _level ) { return false; }

            if ( _start.HasValue && log.TimeStamp < _start.Value ) { return false; }

            if ( _end.HasValue && log.TimeStamp > _end.Value ) { return false; }

            return string.IsNullOrWhiteSpace( _source ) || string.Equals( _source, log.Source, StringComparison.OrdinalIgnoreCase );
        }

        protected override void InternalAdd( IEnumerable<DebugLogEvent> values )
        {
            if ( IsPaused )
            {
                _pending.Add( values );
                return;
            }

            base.InternalAdd( values );
        }
        protected override void InternalAdd( params ReadOnlySpan<DebugLogEvent> values )
        {
            if ( IsPaused )
            {
                _pending.Add( values );
                return;
            }

            base.InternalAdd( values );
        }

        protected override void InternalAdd( ref readonly DebugLogEvent value, int count )
        {
            if ( IsPaused )
            {
                for ( int i = 0; i < count; i++ ) { _pending.Add( value ); }

                return;
            }

            base.InternalAdd( in value, count );
        }
        protected override void InternalAdd( ref readonly DebugLogEvent value )
        {
            if ( IsPaused )
            {
                _pending.Add( value );
                return;
            }

            base.InternalAdd( in value );
        }


        public Collection SetFilter( LogEventLevel level )
        {
            _level = level;
            Refresh();
            return this;
        }
        public Collection SetFilter( string source )
        {
            _source = source;
            Refresh();
            return this;
        }
        public Collection SetEnd( DateTimeOffset? value )
        {
            _end = value;
            Refresh();
            return this;
        }
        public Collection SetStart( DateTimeOffset? value )
        {
            _start = value;
            Refresh();
            return this;
        }


        public Collection Pause()
        {
            IsPaused = true;
            return this;
        }
        public Collection Resume()
        {
            IsPaused = false;
            Add( _pending );
            return this;
        }
        public bool Toggle()
        {
            if ( IsPaused ) { Resume(); }
            else { Pause(); }

            return IsPaused;
        }
    }



    public interface IEventHost
    {
        public event EventHandler<DebugLogEvent>? OnDebugEvent;
    }



    public interface IHandler
    {
        public void Log( Exception e );
        public void Log( Type      type,    Exception     e );
        public void Log( string    message, LogEventLevel level );
        public void Log( Type      type,    string        message, LogEventLevel level );
    }
}
