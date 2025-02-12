namespace Jakar.Extensions.Serilog;


public sealed record MessageEvent( string Message, LogEventLevel Level, TimeSpan TTL, DateTimeOffset TimeStamp )
{
    private MessageEvent( string message, LogEventLevel level, int ttl ) : this( message, level, TimeSpan.FromSeconds( ttl ), DateTimeOffset.UtcNow ) { }

    public static MessageEvent Create( in Exception e,       int              ttl            = 3 ) => Create( e.Message, LogEventLevel.Error, ttl );
    public static MessageEvent Create( in string    message, in LogEventLevel level, int ttl = 3 ) => new(message, level, ttl);


    public EventDetails ToDictionary() => new()
                                          {
                                              [nameof(Message)]   = Message,
                                              [nameof(TTL)]       = TTL.ToString(),
                                              [nameof(Level)]     = Level.ToString(),
                                              [nameof(TimeStamp)] = TimeStamp.ToString()
                                          };
    public MessageEvent TrackEvent()
    {
        ISerilogger.Instance?.TrackEvent( this, Message, ToDictionary() );
        return this;
    }
    public override string ToString() => $"[{Level}] {TimeStamp} -> {Message}";



    public sealed class Collection : ConcurrentQueue<MessageEvent>
    {
        public Collection() : base() { }
    }



    public sealed class EventComparer : IComparer<MessageEvent>, IComparer
    {
        public static EventComparer Comparer { get; } = new();
        private EventComparer() { }

        public int Compare( object? x, object? y )
        {
            if ( x is not MessageEvent left ) { throw new ExpectedValueTypeException( nameof(x), x, typeof(MessageEvent) ); }

            if ( y is not MessageEvent right ) { throw new ExpectedValueTypeException( nameof(y), y, typeof(MessageEvent) ); }

            return Compare( left, right );
        }
        public int Compare( MessageEvent? x, MessageEvent? y )
        {
            if ( x is null ) { return -1; }

            if ( y is null ) { return 1; }


            int dtComparison = x.TimeStamp.CompareTo( y.TimeStamp );
            if ( dtComparison != 0 ) { return dtComparison; }


            int levelComparison = x.Level.CompareTo( y.Level );
            if ( levelComparison != 0 ) { return levelComparison; }

            return string.Compare( x.Message, y.Message, StringComparison.Ordinal );
        }
    }



    public interface IEventHost
    {
        public event EventHandler<MessageEvent>? LogEventHandler;
    }



    public interface IHandler
    {
        public void Log( Exception e,       int           ttl );
        public void Log( string    message, LogEventLevel level, int ttl );
    }
}
