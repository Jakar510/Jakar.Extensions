// TrueLogic :: TrueLogic.Common
// 01/05/2025  13:01

using OpenTelemetry.Trace;
using Serilog.Context;



namespace Jakar.Extensions;


/*
public interface IActivityTracer
{

}
*/



/// <summary>
///     <see cref="LogContext"/>
/// </summary>
public static class TelemetrySpans
{
    /*
        private static  volatile  IDisposable?                               _activityListener;
        private static Action<ActivityListenerConfiguration>?                _configureActivityListener;
        private static Action<HttpRequestOutActivityInstrumentationOptions>? _configureHttpRequestOutActivityInstrumentation;
        public static Action<ActivityListenerConfiguration>? ConfigureActivityListener
        {
            get => _configureActivityListener;
            set
            {
                _configureActivityListener = value;
                Interlocked.Exchange( ref _activityListener, null )?.Dispose();
            }
        }
        public static Action<HttpRequestOutActivityInstrumentationOptions>? ConfigureHttpRequestOutActivityInstrumentation
        {
            get => _configureHttpRequestOutActivityInstrumentation;
            set
            {
                _configureHttpRequestOutActivityInstrumentation = value;
                Interlocked.Exchange( ref _activityListener, null )?.Dispose();
            }
        }
        public static void DisposeListener()      => Interlocked.Exchange( ref _activityListener, null )?.Dispose();
        public static void Init( string appName ) => _activityListener ??= CreateListener( appName, _configureHttpRequestOutActivityInstrumentation, _configureActivityListener );
        private static IDisposable CreateListener( string appName, Action<HttpRequestOutActivityInstrumentationOptions>? configureHttpRequestOutActivityInstrumentation, Action<ActivityListenerConfiguration>? configureActivityListener )
        {
            configureHttpRequestOutActivityInstrumentation ??= HttpRequestOutActivityInstrumentation;
            configureActivityListener                      ??= ActivityListener;
            ActivityListenerConfiguration listener = new();
            listener.InitialLevel.Verbose();
            listener.Sample.AllTraces();
            listener.Instrument.WithDefaultInstrumentation( false );
            listener.Instrument.HttpClientRequests( configureHttpRequestOutActivityInstrumentation );
            configureActivityListener( listener );

            return listener.TraceTo( Log.Current );

            static void HttpRequestOutActivityInstrumentation( HttpRequestOutActivityInstrumentationOptions options )  { }
            static void ActivityListener( ActivityListenerConfiguration                                     listener ) { }
        }
        */

    /*
       ResourceBuilder resource = ResourceBuilder.CreateDefault().AddService( appName );

       TracerProvider tracerProvider = Sdk.CreateTracerProviderBuilder()
                                          .SetResourceBuilder( resource )
                                          .AddSource( appName ) // Optional: Your Activity source name
                                          .AddInstrumentation( listener )
                                          .Build();

       Tracer tracer = tracerProvider.GetTracer( appName );
     */


    [Pure] [MustDisposeResource] public static OpenTelemetry.Trace.TelemetrySpan StartSpan( this Activity activity, [CallerMemberName] string caller = EMPTY ) =>
        ReferenceEquals(activity, Activity.Current)
            ? Tracer.CurrentSpan
            : throw new InvalidOperationException("Activity.Current is not the same as the activity.");


    public static IEnumerable<ActivityLink> Link( this IEnumerable<Activity>      activity, ActivityTagsCollection? tags = null ) => activity.Select(x => x.Link(tags));
    public static IEnumerable<ActivityLink> Link( this IEnumerable<TelemetrySpan> activity, ActivityTagsCollection? tags = null ) => activity.Select(x => x.Link(tags));
    extension( Activity                        activity )
    {
        public ActivityLink Link( in       ActivityTagsCollection? tags = null )                                           => new(activity.Context, tags);
        public void         TrackEvent( in ActivityTagsCollection? tags = null, [CallerMemberName] string caller = EMPTY ) => activity.AddEvent(caller.GetEvent(tags));
        public void TrackEvent<T>( T value, ActivityTagsCollection? tags = null, [CallerMemberName] string caller = EMPTY )
        {
            tags                ??= new ActivityTagsCollection();
            tags[nameof(value)] =   value;
            activity.AddEvent(caller.GetEvent(tags));
        }
        public void SetAttribute( string key, object? value ) => activity.SetTag(key, value);
        public void AddAttribute( string key, object? value ) => activity.AddTag(key, value);
    }



    public static ActivityEvent   GetEvent( this     string   name,     ActivityTagsCollection? tags = null ) => new(name, DateTimeOffset.UtcNow, tags);
    public static ActivityContext RandomContext()                    => new(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded);
    public static string          GetClassName<T>( this T instance ) => ( instance?.GetType() ?? typeof(T) ).Name;
}
