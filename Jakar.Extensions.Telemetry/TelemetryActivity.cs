// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  17:06

namespace Jakar.Extensions.Telemetry;


/// <summary> Represents an operation with context to be used for logging. </summary>
[JsonObject, SuppressMessage( "ReSharper", "ConvertToAutoPropertyWhenPossible" )]
public sealed class TelemetryActivity( string operationName, in TelemetryActivityContext context ) : IDisposable
{
    public const      string                   OPERATION_NAME = "OperationName";
    internal readonly TelemetryActivityContext context        = context;
    private           ActivityKind             _kind          = ActivityKind.Internal;
    private           bool                     _isStopped     = true;
    private           DateTimeOffset?          _endTimeUtc;
    private           DateTimeOffset?          _startTimeUtc;
    private           StatusCode?              _status;
    private           string?                  _displayName;
    private           string?                  _statusDescription;
    private           TelemetryActivity?       _child;
    internal          TelemetryActivity?       parent;
    private           TimeSpan?                _duration;


    public static TelemetryActivity? Current { get; set; }
    public TelemetryActivity? Child
    {
        get => _child;
        set
        {
            _child = value;
            if ( _child is not null ) { _child.parent = this; }
        }
    }
    public TelemetryActivityContext   Context           { get => context;                       init => context = value; }
    public string                     DisplayName       { get => _displayName ?? OperationName; set => _displayName = value; }
    public TimeSpan?                  Duration          { get => _duration;                     init => _duration = value; }
    public DateTimeOffset?            EndTimeUtc        { get => _endTimeUtc;                   init => _endTimeUtc = value; }
    public LinkedList<TelemetryEvent> Events            { get;                                  init; } = [];
    public bool                       IsStopped         { get => _isStopped;                    init => _isStopped = value; }
    public ActivityKind               Kind              { get => _kind;                         init => _kind = value; }
    public TelemetryMeters            Meters            { get;                                  init; } = new();
    public string                     OperationName     { get;                                  init; } = operationName;
    public TelemetryActivityContext?  Parent            => parent?.Context;
    public string                     SpanID            => TelemetryActivitySpanID.Collate( this );
    public DateTimeOffset?            StartTimeUtc      { get => _startTimeUtc;      init => _startTimeUtc = value; }
    public StatusCode?                Status            { get => _status;            init => _status = value; }
    public string?                    StatusDescription { get => _statusDescription; init => _statusDescription = value; }
    public TagsList                   Tags              { get;                       init; } = [];


    public void Dispose() => Stop();


    public static TelemetryActivity Create( string operationName )                                   => new(operationName, TelemetryActivityContext.Create( operationName ));
    public static TelemetryActivity Create( string operationName, TelemetryActivityContext context ) => new(operationName, context);


    public TelemetryActivity SetStatus( StatusCode? code, string? description = null )
    {
        _status = code;

        _statusDescription = code is StatusCode.Error
                                 ? description
                                 : null;

        return this;
    }
    public TelemetryActivity SetKind( ActivityKind kind )
    {
        _kind = kind;
        return this;
    }


    public TelemetryActivity Start()
    {
        _startTimeUtc ??= DateTimeOffset.UtcNow;
        _isStopped    =   false;
        return this;
    }
    public TelemetryActivity Stop()
    {
        if ( _startTimeUtc.HasValue )
        {
            _endTimeUtc ??= DateTimeOffset.UtcNow;
            _duration   ??= _endTimeUtc.Value - _startTimeUtc.Value;
        }

        _isStopped = true;
        return this;
    }


    public TelemetryActivity AddChild( string operationName ) => Child ??= Create( operationName, Context.CreateChild( operationName ) ).Start();
    public TelemetryActivity SetParent( TelemetryActivity? parentContext )
    {
        parent = parentContext;
        return this;
    }
    public TelemetryActivity AddEvent( in TelemetryEvent value )
    {
        Events.AddLast( value );
        return this;
    }
    public TelemetryActivity AddEvent( [CallerMemberName] string caller = BaseRecord.EMPTY ) => AddEvent( new TelemetryEvent( caller ) );


    public void AddTag( string                    key, string? value ) => AddTag( new Pair( key, value ) );
    public void AddTag( in     Pair               pair )  => Tags.Add( in pair );
    public void AddTag( params ReadOnlySpan<Pair> pairs ) => Tags.Add( pairs );


    public void AddException( Exception exception, in TagsList tags, DateTimeOffset utcNow ) { }


    public void AddLink( TelemetryActivityLink link ) { }
}
