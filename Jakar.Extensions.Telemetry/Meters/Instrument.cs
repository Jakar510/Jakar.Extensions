// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 02/14/2025  14:02

namespace Jakar.Extensions.Telemetry.Meters;


public class Instrument( string name, string unit, params Pair[]? tags ) : IMeterInstrumentReadings
{
    protected bool            _isDisposed;
    private   TelemetryMeter? _meter;


    protected TelemetryMeter      _Meter      => _meter ?? throw new InvalidOperationException( $"Call {nameof(SetMeter)} First", new NullReferenceException( nameof(_meter) ) );
    public    string?             Description { get; init; }
    public    Reading?            LastValue   => Readings.Last?.Value;
    public    string              Name        { get; init; } = name;
    public    LinkedList<Reading> Readings    { get; init; } = [];
    public    Pair[]?     Tags        { get; set; }  = tags;
    public    string              Type        => GetType().Name;
    public    string              Unit        { get; init; } = unit;


    public event Action<Reading>? OnMeasurement;


    public virtual void Dispose()
    {
        _isDisposed = true;
        GC.SuppressFinalize( this );
    }


    public Instrument SetMeter( TelemetryMeter meter )
    {
        _meter = meter;
        return this;
    }
    internal void ReportMeasurement( Reading reading )
    {
        ObjectDisposedException.ThrowIf( _isDisposed, this );
        OnMeasurement?.Invoke( reading );
    }
    public void RecordMeasurement( JToken value ) => RecordMeasurement( value, null );
    public void RecordMeasurement( JToken value, params Pair[]? tags )
    {
        ObjectDisposedException.ThrowIf( _isDisposed, this );
        var reading = Reading.Create( value, tags );
        Readings.AddLast( reading );
        ReportMeasurement( reading );
    }
}



public abstract class Instrument<TInstrument> : Instrument
    where TInstrument : Instrument<TInstrument>, ICreateInstrument<TInstrument>
{
    protected Instrument( TelemetryMeter meter, string name, string unit, params Pair[]? tags ) : base( name, unit, tags ) => SetMeter( meter );
}
