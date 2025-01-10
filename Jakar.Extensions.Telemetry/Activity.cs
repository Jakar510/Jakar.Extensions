// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  17:06

namespace Jakar.Extensions.Telemetry;


/// <summary> Represents an operation with context to be used for logging. </summary>
[JsonObject, SuppressMessage( "ReSharper", "ConvertToAutoPropertyWhenPossible" )]
public sealed record Activity( string                      DisplayName,
                               TelemetryContext            Context,
                               ActivityKind                Kind,
                               TimeSpan?                   Duration,
                               DateTimeOffset?             EndTimeUtc,
                               bool                        IsStopped,
                               DateTimeOffset?             StartTimeUtc,
                               StatusCode?                 Status,
                               string?                     StatusDescription,
                               TelemetryTag.Collection     Tags,
                               TelemetryBaggage.Collection Baggage,
                               TelemetryEvent.Collection   Events,
                               Meter.Collection            Meters ) : IDisposable
{
    public const string          OPERATION_NAME     = "OperationName";
    private      bool            _isStopped         = IsStopped;
    private      DateTimeOffset? _endTimeUtc        = EndTimeUtc;
    private      DateTimeOffset? _startTimeUtc      = StartTimeUtc;
    private      StatusCode?     _status            = Status;
    private      string?         _statusDescription = StatusDescription;
    private      TimeSpan?       _duration          = Duration;


    public TimeSpan?       Duration          { get => _duration;          init => _duration = value; }
    public DateTimeOffset? EndTimeUtc        { get => _endTimeUtc;        init => _endTimeUtc = value; }
    public bool            IsStopped         { get => _isStopped;         init => _isStopped = value; }
    public DateTimeOffset? StartTimeUtc      { get => _startTimeUtc;      init => _startTimeUtc = value; }
    public StatusCode?     Status            { get => _status;            init => _status = value; }
    public string?         StatusDescription { get => _statusDescription; init => _statusDescription = value; }


    public static Activity Create( string displayName, TelemetryContext context, ActivityKind kind = ActivityKind.Internal ) => new(displayName,
                                                                                                                                    context,
                                                                                                                                    kind,
                                                                                                                                    null,
                                                                                                                                    null,
                                                                                                                                    true,
                                                                                                                                    null,
                                                                                                                                    null,
                                                                                                                                    null,
                                                                                                                                    TelemetryTag.Collection.Create(),
                                                                                                                                    TelemetryBaggage.Collection.Create(),
                                                                                                                                    TelemetryEvent.Collection.Create(),
                                                                                                                                    Meter.Collection.Create());
    public Activity CreateChild( string name ) => Create( name, Context.CreateChild( name ), Kind ).Start();
    public Activity SetStatus( StatusCode? code, string? description )
    {
        _status = code;

        _statusDescription = code is StatusCode.Error
                                 ? description
                                 : null;

        return this;
    }
    public Activity Start()
    {
        _startTimeUtc ??= DateTimeOffset.UtcNow;
        _isStopped    =   false;
        return this;
    }
    public Activity Stop()
    {
        _endTimeUtc ??= DateTimeOffset.UtcNow;
        _duration   ??= _endTimeUtc - StartTimeUtc;
        _isStopped  =   true;
        return this;
    }


    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize( this );
    }
}
