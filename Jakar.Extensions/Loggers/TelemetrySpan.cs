// TrueLogic :: TrueLogic.Models
// 05/02/2025  18:56

using System.Diagnostics.Metrics;



namespace Jakar.Extensions;


public sealed class TelemetrySource : IFuzzyEquals<TelemetrySource>, IDisposable
{
    public static readonly ActivityContext EmptyActivityContext = default;
    public readonly        ActivitySource  Source;
    public readonly        AppVersion      Version;
    public readonly        Guid            AppID;
    public readonly        Meter           Meter;
    public readonly        string          AppName;


    public static FuzzyEqualizer<TelemetrySource> FuzzyEqualizer { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => FuzzyEqualizer<TelemetrySource>.Default; }
    public static Sorter<TelemetrySource>         Sorter         { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Sorter<TelemetrySource>.Default; }
    public static TelemetrySource?                Current        { get; set; }
    public        string?                         PackageName    { get; set; }


    static TelemetrySource() => Activity.DefaultIdFormat = ActivityIdFormat.Hierarchical;
    public TelemetrySource( Guid appID, string appName, AppVersion appVersion )
    {
        ArgumentException.ThrowIfNullOrEmpty(appName);

        Activity.Current = null;
        AppName          = appName;
        Version          = appVersion;
        AppID            = appID;
        Source           = new ActivitySource(appName, appVersion.ToString());
        Meter            = new Meter(appName, appVersion.ToString());
    }
    public void Dispose()
    {
        Meter.Dispose();
        Source.Dispose();
    }


    /*
    public void AddServices( IServiceCollection services )
    {
        services.AddOpenTelemetryTracing( b => b.AddSource( Source.Name )
                                                .SetResourceBuilder( ResourceBuilder.CreateDefault().AddService( AppName ) )
                                                .AddAspNetCoreInstrumentation() // if using any server bits
                                                .AddHttpClientInstrumentation()
                                                .AddConsoleExporter() );

        services.AddOpenTelemetryMetrics( b => b.AddMeter( Meter.Name ).SetResourceBuilder( ResourceBuilder.CreateDefault().AddService( AppName ) ) );
    }
    */

    public bool Equals( AppVersion?     other ) => Version.Equals(other);
    public bool FuzzyEquals( AppVersion other ) => Version.FuzzyEquals(other);


    [Pure] public Activity? StartActivity( string name, params ActivityLink[]? links )                                                                    => StartActivity(name, ActivityKind.Internal,                   links);
    [Pure] public Activity? StartActivity( string name, ActivityKind           kind,          params ActivityLink[]? links )                              => StartActivity(name, EmptyActivityContext,                    kind,                  null, links);
    [Pure] public Activity? StartActivity( string name, Activity?              parent,        params ActivityLink[]? links )                              => StartActivity(name, parent,                                  ActivityKind.Internal, links);
    [Pure] public Activity? StartActivity( string name, Activity?              parent,        ActivityKind           kind, params ActivityLink[]? links ) => StartActivity(name, parent?.Context ?? EmptyActivityContext, kind,                  null, links);
    [Pure] public Activity? StartActivity( string name, ActivityContext        parentContext, params ActivityLink[]? links ) => StartActivity(name, parentContext, ActivityKind.Internal, null, links);


    [Pure]
    public Activity? StartActivity( string name, ActivityContext parentContext, ActivityKind kind, ActivityTagsCollection? tags, params ActivityLink[]? links )
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        if ( Source.HasListeners() is false ) { return null; }

        Activity activity = Source.StartActivity(name, kind, parentContext, tags, links, DateTimeOffset.UtcNow) ?? throw new InvalidOperationException($"{nameof(Source)}.{nameof(Source.CreateActivity)}");
        activity.ActivityTraceFlags = ActivityTraceFlags.Recorded;
        activity.IsAllDataRequested = true;
        activity.TraceStateString   = null;

        activity.SetIdFormat(ActivityIdFormat.Hierarchical);
        activity.SetStatus(ActivityStatusCode.Ok);
        activity.SetTag(nameof(AppName), AppName);
        activity.SetTag(nameof(AppID),   AppID.ToString());
        activity.SetTag(nameof(Version), Version.ToString());
        return activity;
    }


    public static implicit operator AppVersion( TelemetrySource     sources ) => sources.Version;
    public static implicit operator Meter( TelemetrySource          sources ) => sources.Meter;
    public static implicit operator ActivitySource( TelemetrySource sources ) => sources.Source;


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is TelemetrySource source ) { return CompareTo(source); }

        throw new ExpectedValueTypeException(nameof(other), other, typeof(TelemetrySource));
    }
    public int CompareTo( TelemetrySource? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int appNameComparison = string.Compare(AppName, other.AppName, StringComparison.Ordinal);
        if ( appNameComparison != 0 ) { return appNameComparison; }

        int appIDComparison = AppID.CompareTo(other.AppID);
        if ( appIDComparison != 0 ) { return appIDComparison; }

        int versionComparison = Version.CompareTo(other.Version);
        if ( versionComparison != 0 ) { return versionComparison; }

        return string.Compare(PackageName, other.PackageName, StringComparison.Ordinal);
    }
    public bool Equals( TelemetrySource? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Version.Equals(other.Version) && AppID.Equals(other.AppID) && AppName == other.AppName && PackageName == other.PackageName;
    }
    public override bool Equals( object? obj )                => ReferenceEquals(this, obj) || obj is TelemetrySource other && Equals(other);
    public override int  GetHashCode()                        => HashCode.Combine(Version, AppID, AppName, PackageName);
    public          bool FuzzyEquals( TelemetrySource other ) => Version.FuzzyEquals(other.Version);


    public static bool operator ==( TelemetrySource? left, TelemetrySource? right ) => Sorter.Equals(left, right);
    public static bool operator !=( TelemetrySource? left, TelemetrySource? right ) => Sorter.DoesNotEqual(left, right);
    public static bool operator >( TelemetrySource   left, TelemetrySource  right ) => Sorter.GreaterThan(left, right);
    public static bool operator >=( TelemetrySource  left, TelemetrySource  right ) => Sorter.GreaterThanOrEqualTo(left, right);
    public static bool operator <( TelemetrySource   left, TelemetrySource  right ) => Sorter.LessThan(left, right);
    public static bool operator <=( TelemetrySource  left, TelemetrySource  right ) => Sorter.LessThanOrEqualTo(left, right);
}



[NotSerializable, DefaultValue("Empty")]
public struct TelemetrySpan : IDisposable
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


    public readonly TimeSpan        Elapsed      { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Stopwatch.GetElapsedTime(_start, Stopwatch.GetTimestamp()); }
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
            _parent   = null;
            _activity = null;
            return;
        }


        string key = parent is not null
                         ? $"{parent.DisplayName}.{name}"
                         : name;

        _parent   = parent;
        _activity = Activity.Current = source.StartActivity(key);
        _activity?.AddEvent($"{_activity.DisplayName}.Start".GetEvent(new EventProperties { [START_STOP_ID] = _id }));
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
            EventProperties properties = new()
                                         {
                                             [START_STOP_ID] = _id,
                                             [ELAPSED_TIME]  = Elapsed
                                         };

            self._activity.AddEvent($"{self._activity.DisplayName}.End".GetEvent(properties));
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


    public readonly TelemetrySpan AddEvent( EventProperties? properties = null, [CallerMemberName] string eventType  = EMPTY ) => AddEvent(eventType.GetEvent(properties));
    public readonly TelemetrySpan AddEvent( string           eventType,         EventProperties?          properties = null )  => AddEvent(eventType.GetEvent(properties));
    public readonly TelemetrySpan AddEvent( ActivityEvent e )
    {
        if ( _activity is null ) { return this; }

        _activity.AddEvent(e);
        return this;
    }
    public readonly TelemetrySpan AddLink( ActivityLink link )
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
}
