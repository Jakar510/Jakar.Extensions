// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  17:06

namespace Jakar.Extensions.Telemetry;


/// <summary> Represents an operation with context to be used for logging. </summary>
[JsonObject, SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
public sealed class TelemetryActivity( string operationName, in TelemetryActivityContext context ) : IDisposable
{
    public const      string                   OPERATION_NAME = "OperationName";
    internal readonly TelemetryActivityContext context        = context;
    private           ActivityKind             __kind         = ActivityKind.Internal;
    private           bool                     __isStopped    = true;
    private           DateTimeOffset?          __endTimeUtc;
    private           DateTimeOffset?          __startTimeUtc;
    private           StatusCode?              __status;
    private           string?                  __displayName;
    private           string?                  __statusDescription;
    private           TelemetryActivity?       __child;
    internal          TelemetryActivity?       parent;
    private           TimeSpan?                __duration;


    public static TelemetryActivity? Current { get; set; }
    public TelemetryActivity? Child
    {
        get => __child;
        set
        {
            __child = value;
            if ( __child is not null ) { __child.parent = this; }
        }
    }
    public TelemetryActivityContext   Context           { get => context;                        init => context = value; }
    public string                     DisplayName       { get => __displayName ?? OperationName; set => __displayName = value; }
    public TimeSpan?                  Duration          { get => __duration;                     init => __duration = value; }
    public DateTimeOffset?            EndTimeUtc        { get => __endTimeUtc;                   init => __endTimeUtc = value; }
    public LinkedList<TelemetryEvent> Events            { get;                                   init; } = [];
    public bool                       IsStopped         { get => __isStopped;                    init => __isStopped = value; }
    public ActivityKind               Kind              { get => __kind;                         init => __kind = value; }
    public TelemetryMeters            Meters            { get;                                   init; } = new();
    public string                     OperationName     { get;                                   init; } = operationName;
    public TelemetryActivityContext?  Parent            => parent?.Context;
    public string                     SpanID            => TelemetryActivitySpanID.Collate(this);
    public DateTimeOffset?            StartTimeUtc      { get => __startTimeUtc;      init => __startTimeUtc = value; }
    public StatusCode?                Status            { get => __status;            init => __status = value; }
    public string?                    StatusDescription { get => __statusDescription; init => __statusDescription = value; }
    public Pairs                      Tags              { get;                        init; } = [];


    public void Dispose() => Stop();


    public static TelemetryActivity Create( string operationName )                                   => new(operationName, TelemetryActivityContext.Create(operationName));
    public static TelemetryActivity Create( string operationName, TelemetryActivityContext context ) => new(operationName, context);


    public TelemetryActivity SetStatus( StatusCode? code, string? description = null )
    {
        __status = code;

        __statusDescription = code is StatusCode.Error
                                  ? description
                                  : null;

        return this;
    }
    public TelemetryActivity SetKind( ActivityKind kind )
    {
        __kind = kind;
        return this;
    }


    public TelemetryActivity Start()
    {
        __startTimeUtc ??= DateTimeOffset.UtcNow;
        __isStopped    =   false;
        return this;
    }
    public TelemetryActivity Stop()
    {
        if ( __startTimeUtc.HasValue )
        {
            __endTimeUtc ??= DateTimeOffset.UtcNow;
            __duration   ??= __endTimeUtc.Value - __startTimeUtc.Value;
        }

        __isStopped = true;
        return this;
    }


    public TelemetryActivity AddChild( string operationName ) => Child ??= Create(operationName, context.CreateChild(operationName)).Start();
    public TelemetryActivity SetParent( TelemetryActivity? parentContext )
    {
        parent = parentContext;
        return this;
    }
    public TelemetryActivity AddEvent( in TelemetryEvent value )
    {
        Events.AddLast(value);
        return this;
    }
    public TelemetryActivity AddEvent( [CallerMemberName] string caller = BaseRecord.EMPTY ) => AddEvent(new TelemetryEvent(caller));


    public void AddTag( string                    key, string? value ) => AddTag(new Pair(key, value));
    public void AddTag( in     Pair               pair )  => Tags.Add(in pair);
    public void AddTag( params ReadOnlySpan<Pair> pairs ) => Tags.Add(pairs);


    public TelemetryActivity AddException( Exception exception )                => AddException(exception, in Pairs.Empty, DateTimeOffset.UtcNow);
    public TelemetryActivity AddException( Exception exception, in Pairs tags ) => AddException(exception, in tags,        DateTimeOffset.UtcNow);
    public TelemetryActivity AddException( Exception exception, in Pairs tags, in DateTimeOffset timestamp )
    {
        Pairs exceptionTags = tags;
        Pairs.EnsureCapacity(ref exceptionTags, 4);

        // Source.NotifyActivityAddException(this, exception, ref exceptionTags);

        const string EXCEPTION_EVENT_NAME      = "exception";
        const string EXCEPTION_MESSAGE_TAG     = "exception.message";
        const string EXCEPTION_STACK_TRACE_TAG = "exception.stacktrace";
        const string EXCEPTION_TYPE_TAG        = "exception.type";

        bool hasMessage    = false;
        bool hasStackTrace = false;
        bool hasType       = false;

        for ( int i = 0; i < exceptionTags.Length; i++ )
        {
            switch ( exceptionTags[i].Key )
            {
                case EXCEPTION_MESSAGE_TAG:
                    hasMessage = true;
                    break;

                case EXCEPTION_STACK_TRACE_TAG:
                    hasStackTrace = true;
                    break;

                case EXCEPTION_TYPE_TAG:
                    hasType = true;
                    break;
            }
        }

        if ( !hasMessage ) { exceptionTags.Add(new Pair(EXCEPTION_MESSAGE_TAG, exception.Message)); }

        if ( !hasStackTrace ) { exceptionTags.Add(new Pair(EXCEPTION_STACK_TRACE_TAG, exception.ToString())); }

        if ( !hasType ) { exceptionTags.Add(new Pair(EXCEPTION_TYPE_TAG, exception.GetType().ToString())); }

        return AddEvent(new TelemetryEvent(EXCEPTION_EVENT_NAME, timestamp, in exceptionTags));
    }


    public void AddLink( in TelemetryActivityLink link ) { }
}
