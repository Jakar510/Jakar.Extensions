// TrueLogic :: TrueLogic.Models
// 05/02/2025  18:56

namespace Jakar.Extensions;


[NotSerializable, DefaultValue("Empty")]
public struct TelemetrySpan : IDisposable, IEquatable<TelemetrySpan>
{
    public const           string        APP           = nameof(APP);
    public const           string        ON_SLEEP      = nameof(ON_SLEEP);
    public const           string        ON_RESUME     = nameof(ON_RESUME);
    public const           string        ON_START      = nameof(ON_START);
    public const           string        CREATE_WINDOW = nameof(CREATE_WINDOW);
    public const           string        ELAPSED_TIME  = nameof(ELAPSED_TIME);
    public const           string        START_STOP_ID = nameof(START_STOP_ID);
    public static readonly TelemetrySpan Empty         = new(EMPTY, null);
    private                Activity?     _parent;
    private                Activity?     _activity;
    private readonly       long          _start = Stopwatch.GetTimestamp();
    private readonly       string        _id    = RandomID();
    public readonly        string        DisplayName;


    public readonly TimeSpan        Elapsed      => Stopwatch.GetElapsedTime(_start, Stopwatch.GetTimestamp());
    public readonly DateTimeOffset? CreateWindow { set => SetBaggage(CREATE_WINDOW, value?.ToString()); }
    public readonly DateTimeOffset? StartTime    { set => SetBaggage(ON_START,      value?.ToString()); }
    public readonly DateTimeOffset? ResumeTime   { set => SetBaggage(ON_RESUME,     value?.ToString()); }
    public readonly DateTimeOffset? SleepTime    { set => SetBaggage(ON_SLEEP,      value?.ToString()); }
    public          bool            IsValid      => _activity is not null;


    public TelemetrySpan( string name, Activity? parent )
    {
        TelemetrySource? source = TelemetrySource.Current;

        if ( string.IsNullOrWhiteSpace(name) || source is null )
        {
            _parent     = null;
            _activity   = null;
            DisplayName = string.Empty;
            return;
        }


        DisplayName = parent is not null
                          ? $"{parent.DisplayName}.{name}"
                          : name;

        _parent   = parent;
        _activity = Activity.Current = source.StartActivity(DisplayName);
        _activity?.AddEvent($"{DisplayName}.Start".GetEvent(new ActivityTagsCollection { [START_STOP_ID] = _id }));
    }
    public void Dispose()
    {
        TelemetrySpan self = this;
        this = default;

        if ( self._parent is not null )
        {
            Activity.Current = self._parent;
            self._parent     = null;
        }

        if ( self._activity is not null )
        {
            ActivityTagsCollection properties = new()
                                                {
                                                    [START_STOP_ID] = _id,
                                                    [ELAPSED_TIME]  = Elapsed
                                                };

            self._activity.AddEvent($"{self.DisplayName}.End".GetEvent(properties));
            self._activity.Dispose();
            self._activity = null;
        }
    }


    public static string RandomID()
    {
        Span<byte> span = stackalloc byte[40];
        RandomNumberGenerator.Fill(span);
        return Convert.ToHexString(span);
    }


    public readonly ActivityLink Link( ActivityTagsCollection? tags = null ) => _activity is null
                                                                                    ? new ActivityLink()
                                                                                    : new ActivityLink(_activity.Context, tags);


    [Pure, MustDisposeResource] public readonly TelemetrySpan ParseJson()                                                                                 => SubSpan();
    [Pure, MustDisposeResource] public readonly TelemetrySpan CreatePage()                                                                                => SubSpan();
    [Pure, MustDisposeResource] public readonly TelemetrySpan GoHome()                                                                                    => SubSpan();
    [Pure, MustDisposeResource] public readonly TelemetrySpan GoBack()                                                                                    => SubSpan();
    [Pure, MustDisposeResource] public readonly TelemetrySpan Navigation()                                                                                => SubSpan();
    [Pure, MustDisposeResource] public readonly TelemetrySpan WriteToFile()                                                                               => SubSpan();
    [Pure, MustDisposeResource] public readonly TelemetrySpan SubSpan( [CallerMemberName] string         name                                   = EMPTY ) => new(name, _activity);
    [Pure, MustDisposeResource] public static   TelemetrySpan Create( [CallerMemberName]  string         name                                   = EMPTY ) => new(name, Activity.Current);
    [Pure, MustDisposeResource] public static   TelemetrySpan Create( ref readonly        TelemetrySpan? parent, [CallerMemberName] string name = EMPTY ) => parent?.SubSpan(name) ?? Create(name);


    public readonly void SetAttribute( string key, object? value )
    {
        if ( _activity is null ) { return; }

        Debug.Assert(_activity.ActivityTraceFlags is ActivityTraceFlags.Recorded);
        _activity.SetTag(key, value);
    }
    public readonly void AddAttribute( string key, object? value )
    {
        if ( _activity is null ) { return; }

        Debug.Assert(_activity.ActivityTraceFlags is ActivityTraceFlags.Recorded);
        _activity.AddTag(key, value);
    }


    public readonly TelemetrySpan AddTag( string key, string? value )
    {
        if ( _activity is null ) { return this; }

        _activity.AddTag(key, (object?)value);
        return this;
    }
    public readonly TelemetrySpan AddTag( string key, object? value )
    {
        if ( _activity is null ) { return this; }

        _activity.AddTag(key, value);
        return this;
    }
    public readonly TelemetrySpan SetTag( string key, object? value )
    {
        if ( _activity is null ) { return this; }

        _activity.SetTag(key, value);
        return this;
    }


    public readonly TelemetrySpan AddBaggage( string key, string? value )
    {
        if ( _activity is null ) { return this; }

        _activity.AddBaggage(key, value);
        return this;
    }
    public readonly TelemetrySpan SetBaggage( string key, string? value )
    {
        if ( _activity is null ) { return this; }

        _activity.SetBaggage(key, value);
        return this;
    }


    public readonly TelemetrySpan AddEvent( ActivityTagsCollection? properties = null, [CallerMemberName] string eventType  = EMPTY ) => AddEvent(eventType.GetEvent(properties));
    public readonly TelemetrySpan AddEvent( string                  eventType,         ActivityTagsCollection?   properties = null )  => AddEvent(eventType.GetEvent(properties));
    public readonly TelemetrySpan AddEvent( in ActivityEvent e )
    {
        if ( _activity is null ) { return this; }

        _activity.AddEvent(e);
        return this;
    }
    public readonly TelemetrySpan AddLink( in ActivityLink link )
    {
        if ( _activity is null ) { return this; }

        _activity.AddLink(link);
        return this;
    }


    public readonly TelemetrySpan AddException( Exception exception, in TagList tags = default, DateTimeOffset? timestamp = null )
    {
        if ( _activity is null ) { return this; }

        _activity.AddException(exception, in tags, timestamp ?? DateTimeOffset.UtcNow);
        return this;
    }


    public readonly async ValueTask WaitForNextTickAsync( PeriodicTimer timer, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = SubSpan();
        await timer.WaitForNextTickAsync(token).ConfigureAwait(false);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly ValueTask Delay( double seconds, CancellationToken token = default ) => Delay(TimeSpan.FromSeconds(seconds), token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly ValueTask Delay( long   ms,      CancellationToken token = default ) => Delay(TimeSpan.FromMilliseconds(ms), token);
    public readonly async ValueTask Delay( TimeSpan delay, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = SubSpan();
        await Task.Delay(delay, token).ConfigureAwait(false);
    }


    public          bool Equals( TelemetrySpan other )                          => string.Equals(_id, other._id, StringComparison.Ordinal) && Equals(_activity, other._activity) && Equals(_parent, other._parent);
    public override bool Equals( object?       obj )                            => obj is TelemetrySpan other                              && Equals(other);
    public override int  GetHashCode()                                          => HashCode.Combine(_id, _activity, _parent);
    public static   bool operator ==( TelemetrySpan left, TelemetrySpan right ) => left.Equals(right);
    public static   bool operator !=( TelemetrySpan left, TelemetrySpan right ) => !left.Equals(right);
}
