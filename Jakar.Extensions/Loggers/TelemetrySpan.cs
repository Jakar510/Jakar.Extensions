// TrueLogic :: TrueLogic.Models
// 05/02/2025  18:56

using System.Diagnostics.Metrics;



namespace Jakar.Extensions.Loggers;


public sealed class TelemetrySource : IFuzzyEquals<AppVersion>, IDisposable
{
    public static readonly ActivityContext EmptyActivityContext = default;
    public readonly        ActivitySource  Source;
    public readonly        AppVersion      Version;
    public readonly        Guid            AppID;
    public readonly        Meter           Meter;
    public readonly        string          AppName;
    public                 string?         PackageName { get; set; }


    public static TelemetrySource? Current { get; set; }


    static TelemetrySource() => Activity.DefaultIdFormat = ActivityIdFormat.Hierarchical;
    public TelemetrySource( Guid appID, string appName, AppVersion appVersion )
    {
        ArgumentException.ThrowIfNullOrEmpty( appName );

        Activity.Current = null;
        AppName          = appName;
        Version          = appVersion;
        AppID            = appID;
        Source           = new ActivitySource( appName, appVersion.ToString() );
        Meter            = new Meter( appName, appVersion.ToString() );
    }
    public void Dispose()
    {
        Meter.Dispose();
        Source.Dispose();
    }


    public static implicit operator AppVersion( TelemetrySource     sources ) => sources.Version;
    public static implicit operator Meter( TelemetrySource          sources ) => sources.Meter;
    public static implicit operator ActivitySource( TelemetrySource sources ) => sources.Source;


    public bool Equals( AppVersion?     other ) => Version.Equals( other );
    public bool FuzzyEquals( AppVersion other ) => Version.FuzzyEquals( other );


    [Pure] public Activity? StartActivity( string name, params ActivityLink[]? links )                                                                    => StartActivity( name, ActivityKind.Internal,                   links );
    [Pure] public Activity? StartActivity( string name, ActivityKind           kind,          params ActivityLink[]? links )                              => StartActivity( name, EmptyActivityContext,                    kind,                  null, links );
    [Pure] public Activity? StartActivity( string name, Activity?              parent,        params ActivityLink[]? links )                              => StartActivity( name, parent,                                  ActivityKind.Internal, links );
    [Pure] public Activity? StartActivity( string name, Activity?              parent,        ActivityKind           kind, params ActivityLink[]? links ) => StartActivity( name, parent?.Context ?? EmptyActivityContext, kind,                  null, links );
    [Pure] public Activity? StartActivity( string name, ActivityContext        parentContext, params ActivityLink[]? links ) => StartActivity( name, parentContext, ActivityKind.Internal, null, links );


    [Pure]
    public Activity? StartActivity( string name, ActivityContext parentContext, ActivityKind kind, ActivityTagsCollection? tags, params ActivityLink[]? links )
    {
        ArgumentException.ThrowIfNullOrEmpty( name );
        if ( Source.HasListeners() is false ) { return null; }

        Activity activity = Source.StartActivity( name, kind, parentContext, tags, links, DateTimeOffset.UtcNow ) ?? throw new InvalidOperationException( $"{nameof(Source)}.{nameof(Source.CreateActivity)}" );
        activity.ActivityTraceFlags = ActivityTraceFlags.Recorded;
        activity.IsAllDataRequested = true;
        activity.TraceStateString   = null;

        activity.SetIdFormat( ActivityIdFormat.Hierarchical );
        activity.SetStatus( ActivityStatusCode.Ok );
        activity.SetTag( nameof(AppName), AppName );
        activity.SetTag( nameof(AppID),   AppID.ToString() );
        activity.SetTag( nameof(Version), Version.ToString() );
        return activity;
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
}



// [SupportedOSPlatform( "windows" )]
[NotSerializable]
public readonly struct TelemetrySpan : IDisposable
{
    public const     string    APP           = nameof(APP);
    public const     string    ON_SLEEP      = nameof(ON_SLEEP);
    public const     string    ON_RESUME     = nameof(ON_RESUME);
    public const     string    ON_START      = nameof(ON_START);
    public const     string    CREATE_WINDOW = nameof(CREATE_WINDOW);
    public const     string    ELAPSED_TIME  = nameof(ELAPSED_TIME);
    public const     string    START_STOP_ID = nameof(START_STOP_ID);
    private readonly Activity? _parent;
    private readonly Activity? _activity;
    private readonly long      _start = Stopwatch.GetTimestamp();
    private readonly string    _id    = RandomID();


    public TimeSpan        Elapsed      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Stopwatch.GetElapsedTime( _start, Stopwatch.GetTimestamp() ); }
    public DateTimeOffset? CreateWindow { set => SetBaggage( CREATE_WINDOW, value?.ToString() ); }
    public DateTimeOffset? StartTime    { set => SetBaggage( ON_START,      value?.ToString() ); }
    public DateTimeOffset? ResumeTime   { set => SetBaggage( ON_RESUME,     value?.ToString() ); }
    public DateTimeOffset? SleepTime    { set => SetBaggage( ON_SLEEP,      value?.ToString() ); }
    public bool            IsValid      => _activity is not null;
    public string?         Name         => _activity?.DisplayName;


    [Obsolete( "Use 'Name' constructor instead, as it doesn't work if 'Name' is empty" )] public TelemetrySpan() : this( EMPTY ) { }
    public TelemetrySpan( string name, Activity? parent = null )
    {
        parent ??= Activity.Current;
        TelemetrySource? source = TelemetrySource.Current;

        if ( string.IsNullOrWhiteSpace( name ) || source is null )
        {
            _parent   = null;
            _activity = null;
            return;
        }


        string key = parent is not null
                         ? $"{parent.DisplayName}.{name}"
                         : name;

        _parent   = parent;
        _activity = Activity.Current = source.StartActivity( key );
        _activity?.AddEvent( $"{_activity.DisplayName}.Start".GetEvent( new EventProperties { [START_STOP_ID] = _id } ) );
    }
    public void Dispose()
    {
        Activity.Current = _parent;
        if ( _activity is null ) { return; }

        EventProperties properties = new()
                                     {
                                         [START_STOP_ID] = _id,
                                         [ELAPSED_TIME]  = Elapsed
                                     };

        _activity.AddEvent( $"{_activity.DisplayName}.End".GetEvent( properties ) );
        _activity.Dispose();
    }


    public static string RandomID()
    {
        Span<byte> span = stackalloc byte[40];
        RandomNumberGenerator.Fill( span );
        return Convert.ToHexString( span );
    }


    public ActivityLink Link( ActivityTagsCollection? tags = null ) => _activity is null
                                                                           ? new ActivityLink()
                                                                           : new ActivityLink( _activity.Context, tags );


    [Pure, MustDisposeResource] public        TelemetrySpan ParseJson()                                                                           => new(nameof(ParseJson), _activity);
    [Pure, MustDisposeResource] public        TelemetrySpan ToJson()                                                                              => new(nameof(ToJson), _activity);
    [Pure, MustDisposeResource] public        TelemetrySpan Navigation()                                                                          => new(nameof(Navigation), _activity);
    [Pure, MustDisposeResource] public        TelemetrySpan WriteToFile()                                                                         => new(nameof(WriteToFile), _activity);
    [Pure, MustDisposeResource] public        TelemetrySpan ReadFromFile()                                                                        => new(nameof(ReadFromFile), _activity);
    [Pure, MustDisposeResource] public        TelemetrySpan SubSpan( [CallerMemberName] string name                                     = EMPTY ) => new(name, _activity);
    [Pure, MustDisposeResource] public static TelemetrySpan Create( [CallerMemberName]  string name                                     = EMPTY ) => new(name);
    [Pure, MustDisposeResource] public static TelemetrySpan Create( Activity?                  activity, [CallerMemberName] string name = EMPTY ) => new(name, activity);


    public void SetAttribute( string key, object? value )
    {
        if ( _activity is null ) { return; }

        Debug.Assert( _activity.ActivityTraceFlags is ActivityTraceFlags.Recorded );
        _activity.SetTag( key, value );
    }
    public void AddAttribute( string key, object? value )
    {
        if ( _activity is null ) { return; }

        Debug.Assert( _activity.ActivityTraceFlags is ActivityTraceFlags.Recorded );
        _activity.AddTag( key, value );
    }


    public TelemetrySpan AddTag( string key, string? value )
    {
        if ( _activity is null ) { return this; }

        _activity.AddTag( key, (object?)value );
        return this;
    }
    public TelemetrySpan AddTag( string key, object? value )
    {
        if ( _activity is null ) { return this; }

        _activity.AddTag( key, value );
        return this;
    }
    public TelemetrySpan SetTag( string key, object? value )
    {
        if ( _activity is null ) { return this; }

        _activity.SetTag( key, value );
        return this;
    }


    public TelemetrySpan AddBaggage( string key, string? value )
    {
        if ( _activity is null ) { return this; }

        _activity.AddBaggage( key, value );
        return this;
    }
    public TelemetrySpan SetBaggage( string key, string? value )
    {
        if ( _activity is null ) { return this; }

        _activity.SetBaggage( key, value );
        return this;
    }


    public TelemetrySpan AddEvent( EventProperties? properties = null, [CallerMemberName] string eventType  = EMPTY ) => AddEvent( eventType.GetEvent( properties ) );
    public TelemetrySpan AddEvent( string           eventType,         EventProperties?          properties = null )  => AddEvent( eventType.GetEvent( properties ) );
    public TelemetrySpan AddEvent( ActivityEvent e )
    {
        if ( _activity is null ) { return this; }

        _activity.AddEvent( e );
        return this;
    }
    public TelemetrySpan AddLink( ActivityLink link )
    {
        if ( _activity is null ) { return this; }

        _activity.AddLink( link );
        return this;
    }


    public TelemetrySpan AddException( Exception exception, in TagList tags = default, DateTimeOffset? timestamp = null )
    {
        if ( _activity is null ) { return this; }

        _activity.AddException( exception, in tags, timestamp ?? DateTimeOffset.UtcNow );
        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ValueTask Delay( double seconds, CancellationToken token = default ) => Delay( TimeSpan.FromSeconds( seconds ), token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ValueTask Delay( long   ms,      CancellationToken token = default ) => Delay( TimeSpan.FromMilliseconds( ms ), token );
    public async ValueTask Delay( TimeSpan delay, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = SubSpan();
        await Task.Delay( delay, token ).ConfigureAwait( false );
    }
}
