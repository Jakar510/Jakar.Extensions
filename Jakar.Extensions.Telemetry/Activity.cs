// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  17:06

namespace Jakar.Extensions.Telemetry;


/// <summary> Represents an operation with context to be used for logging. </summary>
[JsonObject]
public class Activity : IDisposable
{
    private bool           _isStopped;
    private DateTimeOffset _startTimeUtc;
    private StatusCode?    _status;
    private string?        _displayName;
    private string?        _statusDescription;
    private TimeSpan       _duration;


    public          LinkedList<TelemetryBaggage>        Baggage           { get;                         init; } = [];
    public required TelemetryContext                    Context           { get;                         init; }
    public          string                              DisplayName       { get => _displayName ?? Name; set => _displayName = value; }
    public          TimeSpan                            Duration          { get => _duration;            init => _duration = value; }
    public          LinkedList<TelemetryEvent>          Events            { get;                         init; } = [];
    public          bool                                HasRemoteParent   { get;                         init; }
    public          bool                                IsStopped         { get => _isStopped;           init => _isStopped = value; }
    public          ActivityKind                        Kind              { get;                         init; }
    public          ConcurrentDictionary<string, Meter> Meters            { get;                         init; } = new(Environment.ProcessorCount, 64);
    public          string?                             RootID            { get;                         init; }
    public required string                              Name              { get;                         init; }
    public          DateTimeOffset                      StartTimeUtc      { get => _startTimeUtc;        init => _startTimeUtc = value; }
    public          StatusCode?                         Status            { get => _status;              init => _status = value; }
    public          string?                             StatusDescription { get => _statusDescription;   init => _statusDescription = value; }
    public          LinkedList<TelemetryTag>            Tags              { get;                         init; } = [];


    public static Activity Create( string name, TelemetryContext context, ActivityKind kind = ActivityKind.Internal ) =>
        new()
        {
            Name    = name,
            Context = context,
            Kind    = kind
        };
    public Activity StartChild( string name )
    {
        Activity child = new()
                         {
                             Name    = name,
                             Context = Context,
                         };


        return child.Start();
    }
    public Activity SetStatus( StatusCode? code, string? description = null )
    {
        _status = code;

        _statusDescription = code is StatusCode.Error
                                 ? description
                                 : null;

        return this;
    }
    public Activity Start()
    {
        _startTimeUtc = DateTimeOffset.UtcNow;
        _isStopped    = false;
        return this;
    }
    public Activity Stop()
    {
        _duration  = DateTimeOffset.UtcNow - StartTimeUtc;
        _isStopped = true;
        return this;
    }


    public void Dispose()
    {
        if ( IsStopped is false ) { Stop(); }

        GC.SuppressFinalize( this );
    }
}
