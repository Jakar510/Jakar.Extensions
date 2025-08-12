// TrueLogic :: TrueLogic.Models
// 05/02/2025  18:56

namespace Jakar.Extensions;


[NotSerializable, DefaultValue(nameof(Empty))]
public readonly struct TelemetrySpan : IDisposable, IEquatable<TelemetrySpan>
{
    public const           string        APP           = nameof(APP);
    public const           string        ON_SLEEP      = nameof(ON_SLEEP);
    public const           string        ON_RESUME     = nameof(ON_RESUME);
    public const           string        ON_START      = nameof(ON_START);
    public const           string        CREATE_WINDOW = nameof(CREATE_WINDOW);
    public const           string        ELAPSED_TIME  = nameof(ELAPSED_TIME);
    public const           string        START_STOP_ID = nameof(START_STOP_ID);
    public static readonly TelemetrySpan Empty         = new(EMPTY, null);
    private readonly       Activity?     __parent;
    private readonly       Activity?     __activity;
    private readonly       long          __start;
    private readonly       string        __id;
    public readonly        string        DisplayName;


    public static int             RandomIDLength { get; set; } = 16;
    public        TimeSpan        Elapsed        => Stopwatch.GetElapsedTime(__start, Stopwatch.GetTimestamp());
    public        DateTimeOffset? CreateWindow   { set => SetBaggage(CREATE_WINDOW, value?.ToString()); }
    public        DateTimeOffset? StartTime      { set => SetBaggage(ON_START,      value?.ToString()); }
    public        DateTimeOffset? ResumeTime     { set => SetBaggage(ON_RESUME,     value?.ToString()); }
    public        DateTimeOffset? SleepTime      { set => SetBaggage(ON_SLEEP,      value?.ToString()); }
    public        bool            IsValid        => __activity is not null;


    public TelemetrySpan( string name, Activity? parent, int? randomIDLength = null )
    {
        __start    = Stopwatch.GetTimestamp();
        TelemetrySource? source = TelemetrySource.Current;

        if ( string.IsNullOrWhiteSpace(name) || source is null )
        {
            __parent     = null;
            __activity   = null;
            DisplayName = string.Empty;
            __id         = string.Empty;
            return;
        }

        DisplayName = parent is not null
                          ? $"{parent.DisplayName}.{name}"
                          : name;

        __parent   = parent;
        __id       = RandomID(randomIDLength ?? RandomIDLength);
        __activity = Activity.Current = source.StartActivity(DisplayName);
        __activity?.AddEvent(new ActivityEvent("Start", DateTimeOffset.UtcNow));
    }
    public void Dispose()
    {
        if ( __activity?.IsStopped is null or true ) { return; }

        if ( __parent is not null ) { Activity.Current = __parent; }

        __activity.AddEvent(new ActivityEvent(SpanDuration.ToString(Elapsed, "End. Duration: "), DateTimeOffset.UtcNow));
        __activity.Stop();
        __activity.Dispose();
    }


    public static string RandomID( int length )
    {
        if ( length < 1 ) { throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than 0."); }

        Span<byte> span = stackalloc byte[length];
        RandomNumberGenerator.Fill(span);
        return Convert.ToHexString(span);
    }


    public ActivityLink Link( ActivityTagsCollection? tags = null ) => __activity is null
                                                                           ? new ActivityLink()
                                                                           : new ActivityLink(__activity.Context, tags);


    [Pure, MustDisposeResource] public        TelemetrySpan SubSpan( [CallerMemberName] string         name                                   = EMPTY ) => new(name, __activity);
    [Pure, MustDisposeResource] public static TelemetrySpan Create( [CallerMemberName]  string         name                                   = EMPTY ) => new(name, Activity.Current);
    [Pure, MustDisposeResource] public static TelemetrySpan Create( ref readonly        TelemetrySpan? parent, [CallerMemberName] string name = EMPTY ) => parent?.SubSpan(name) ?? Create(name);


    public TelemetrySpan AddTag( string key, string? value )
    {
        __activity?.AddTag(key, (object?)value);
        return this;
    }
    public TelemetrySpan AddTag( string key, object? value )
    {
        __activity?.AddTag(key, value);
        return this;
    }
    public TelemetrySpan SetTag( string key, object? value )
    {
        __activity?.SetTag(key, value);
        return this;
    }


    public TelemetrySpan AddBaggage( string key, string? value )
    {
        __activity?.AddBaggage(key, value);
        return this;
    }
    public TelemetrySpan SetBaggage( string key, string? value )
    {
        __activity?.SetBaggage(key, value);
        return this;
    }


    public TelemetrySpan AddEvent( ActivityTagsCollection? properties = null, [CallerMemberName] string eventType  = EMPTY ) => AddEvent(new ActivityEvent(eventType, DateTimeOffset.UtcNow, properties));
    public TelemetrySpan AddEvent( string                  eventType,         ActivityTagsCollection?   properties = null )  => AddEvent(new ActivityEvent(eventType, DateTimeOffset.UtcNow, properties));
    public TelemetrySpan AddEvent( in ActivityEvent e )
    {
        __activity?.AddEvent(e);
        return this;
    }
    public TelemetrySpan AddEvent( params ReadOnlySpan<ActivityEvent> events )
    {
        foreach ( ref readonly ActivityEvent e in events ) { __activity?.AddEvent(e); }

        return this;
    }


    public TelemetrySpan AddLink( in ActivityLink link )
    {
        __activity?.AddLink(link);
        return this;
    }
    public TelemetrySpan AddLink( params ReadOnlySpan<ActivityLink> links )
    {
        foreach ( ref readonly ActivityLink e in links ) { __activity?.AddLink(e); }

        return this;
    }


    public TelemetrySpan AddException( Exception exception, in TagList tags = default, DateTimeOffset? timestamp = null )
    {
        __activity?.AddException(exception, in tags, timestamp ?? DateTimeOffset.UtcNow);
        return this;
    }


    public async ValueTask WaitForNextTickAsync( PeriodicTimer timer, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = SubSpan();
        await timer.WaitForNextTickAsync(token).ConfigureAwait(false);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask Delay( double seconds, CancellationToken token = default ) => Delay(TimeSpan.FromSeconds(seconds), token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask Delay( long   ms,      CancellationToken token = default ) => Delay(TimeSpan.FromMilliseconds(ms), token);
    public async ValueTask Delay( TimeSpan delay, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = SubSpan();
        await Task.Delay(delay, token).ConfigureAwait(false);
    }


    public          bool Equals( TelemetrySpan other )                          => string.Equals(__id, other.__id, StringComparison.Ordinal) && Equals(__activity, other.__activity) && Equals(__parent, other.__parent);
    public override bool Equals( object?       obj )                            => obj is TelemetrySpan other                              && Equals(other);
    public override int  GetHashCode()                                          => HashCode.Combine(__id, __activity, __parent);
    public static   bool operator ==( TelemetrySpan left, TelemetrySpan right ) => left.Equals(right);
    public static   bool operator !=( TelemetrySpan left, TelemetrySpan right ) => !left.Equals(right);
}
