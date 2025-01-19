using System.Collections.Immutable;
using System.Reflection;



namespace Jakar.Extensions.Serilog;


public readonly record struct DebugLogEvent( string Source, string Message, string Caller, LogEventLevel Level, DateTimeOffset TimeStamp, EventDetails? Details = null ) : ILastModified, IComparable<DebugLogEvent?>, IComparable<DebugLogEvent>
{
    private readonly string                        _description = $"[{Level}] {Source}.{Caller} -> {Message}";
    public static    ValueEqualizer<DebugLogEvent> Equalizer    => ValueEqualizer<DebugLogEvent>.Default;
    public static    ValueSorter<DebugLogEvent>    Sorter       => ValueSorter<DebugLogEvent>.Default;
    DateTimeOffset? ILastModified.                 LastModified => TimeStamp;


    private DebugLogEvent( MemberInfo source, string message, string caller, LogEventLevel level, EventDetails? details = null ) : this( source.Name, message, caller, level, details ) { }
    private DebugLogEvent( string     source, string message, string caller, LogEventLevel level, EventDetails? details = null ) : this( source, message, caller, level, DateTimeOffset.UtcNow ) { }


    public static DebugLogEvent Create<T>( string message, LogEventLevel level = LogEventLevel.Debug, EventDetails? details = null,                [CallerMemberName] string? caller  = null )                                           => Create( typeof(T), message, level, details, caller );
    public static DebugLogEvent Create( Type      source,  string        message,                     LogEventLevel level   = LogEventLevel.Debug, EventDetails?              details = null, [CallerMemberName] string? caller = null ) => new(source, message, caller ?? string.Empty, level, details);
    public static DebugLogEvent Create( string    source,  string        message,                     LogEventLevel level,                         EventDetails?              details = null, [CallerMemberName] string? caller = null ) => new(source, message, caller ?? string.Empty, level, details);


    public static DebugLogEvent Error<T>( T   _,      Exception e, EventDetails? details = null, [CallerMemberName] string? caller = null ) => Create<T>( e.Message, LogEventLevel.Error, details, caller );
    public static DebugLogEvent Error( Type   source, Exception e, EventDetails? details = null, [CallerMemberName] string? caller = null ) => Create( source, e.Message, LogEventLevel.Error, details, caller );
    public static DebugLogEvent Error( string source, Exception e, EventDetails? details = null, [CallerMemberName] string? caller = null ) => Create( source, e.Message, LogEventLevel.Error, details, caller );


    public static DebugLogEvent Fatal<T>( T   _,      Exception e, EventDetails? details = null, [CallerMemberName] string? caller = null ) => Create<T>( e.Message, LogEventLevel.Fatal, details, caller );
    public static DebugLogEvent Fatal( Type   source, Exception e, EventDetails? details = null, [CallerMemberName] string? caller = null ) => Create( source, e.Message, LogEventLevel.Fatal, details, caller );
    public static DebugLogEvent Fatal( string source, Exception e, EventDetails? details = null, [CallerMemberName] string? caller = null ) => Create( source, e.Message, LogEventLevel.Fatal, details, caller );


    public override string ToString() => _description;
    public int CompareTo( DebugLogEvent? other )
    {
        if ( other is null ) { return 1; }

        return CompareTo( other.Value );
    }
    public int CompareTo( DebugLogEvent other )
    {
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


    public static bool operator <( DebugLogEvent?  left, DebugLogEvent? right ) => Sorter.Compare( left, right ) < 0;
    public static bool operator >( DebugLogEvent?  left, DebugLogEvent? right ) => Sorter.Compare( left, right ) > 0;
    public static bool operator <=( DebugLogEvent? left, DebugLogEvent? right ) => Sorter.Compare( left, right ) <= 0;
    public static bool operator >=( DebugLogEvent? left, DebugLogEvent? right ) => Sorter.Compare( left, right ) >= 0;



    [SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" )]
    public sealed class Collection() : ConcurrentObservableCollection<DebugLogEvent>( Buffers.DEFAULT_CAPACITY )
    {
        private readonly ConcurrentBag<DebugLogEvent> _pending  = [];
        private readonly Synchronized<bool>           _isPaused = new(false);
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
        public bool IsPaused
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _isPaused.Value;
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
                using ( AcquireLock() ) { return [..buffer.Select( static x => x.Source )]; }
            }
        }


        public Collection( IEnumerable<DebugLogEvent> enumerable ) : this() => Add( enumerable );


        protected override bool Filter( DebugLogEvent log )
        {
            if ( log.Level < _level ) { return false; }

            if ( _start.HasValue && log.TimeStamp < _start.Value ) { return false; }

            if ( _end.HasValue && log.TimeStamp > _end.Value ) { return false; }

            return string.IsNullOrWhiteSpace( _source ) || string.Equals( _source, log.Source, StringComparison.OrdinalIgnoreCase );
        }


        public override void Add( IEnumerable<DebugLogEvent> logs )
        {
            if ( IsPaused )
            {
                _pending.Add( logs );
                return;
            }

            base.Add( logs );
            Refresh();
        }
        public override void Add( params ReadOnlySpan<DebugLogEvent> logs )
        {
            if ( IsPaused )
            {
                _pending.Add( logs );
                return;
            }

            base.Add( logs );
            Refresh();
        }
        public override void Add( DebugLogEvent log )
        {
            if ( IsPaused )
            {
                _pending.Add( log );
                return;
            }

            base.Add( log );
        }
        public override async ValueTask AddAsync( ReadOnlyMemory<DebugLogEvent> values, CancellationToken token = default )
        {
            if ( IsPaused )
            {
                _pending.Add( values.Span );
                return;
            }

            await base.AddAsync( values, token );
        }
        public override async ValueTask AddAsync( ImmutableArray<DebugLogEvent> values, CancellationToken token = default )
        {
            if ( IsPaused )
            {
                _pending.Add( values.AsSpan() );
                return;
            }

            await base.AddAsync( values, token );
        }
        public override async ValueTask AddAsync( IEnumerable<DebugLogEvent> values, CancellationToken token = default )
        {
            if ( IsPaused )
            {
                _pending.Add( values );
                return;
            }

            await base.AddAsync( values, token );
        }
        public override async ValueTask AddAsync( IAsyncEnumerable<DebugLogEvent> values, CancellationToken token = default )
        {
            if ( IsPaused )
            {
                await _pending.Add( values, token );
                return;
            }

            await base.AddAsync( values, token );
        }
        public override async ValueTask AddAsync( DebugLogEvent values, CancellationToken token = default )
        {
            if ( IsPaused )
            {
                _pending.Add( values );
                return;
            }

            await base.AddAsync( values, token );
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
