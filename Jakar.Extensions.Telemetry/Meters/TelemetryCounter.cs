// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 03/13/2025  14:03

using OneOf;



namespace Jakar.Extensions.Telemetry.Meters;


[SuppressMessage( "ReSharper", "ConvertToAutoPropertyWhenPossible" )]
public sealed class TelemetryCounter<TValue> : TelemetryMeter<TValue>.MeterInstrument
    where TValue : struct, INumber<TValue>
{
    private readonly OneOf<Func<Reading<TValue>[]>, Func<Reading<TValue>>, None> _func;


    public TelemetryCounter() { }
    [SetsRequiredMembers] public TelemetryCounter( TelemetryMeter<TValue> meter, ref readonly MeterInstrumentInfo info ) : this( meter, in info, new None() ) { }
    [SetsRequiredMembers] public TelemetryCounter( TelemetryMeter<TValue> meter, ref readonly MeterInstrumentInfo info, OneOf<Func<Reading<TValue>[]>, Func<Reading<TValue>>, None> func ) : base( meter, in info ) => _func = func;


    public TelemetryCounter<TValue> TryMeasurements()
    {
        if ( _func.IsT0 )
        {
            Reading<TValue>[] readings = _func.AsT0();
            RecordMeasurement( readings );
        }
        else if ( _func.IsT1 )
        {
            Reading<TValue> reading = _func.AsT1();
            RecordMeasurement( in reading );
        }

        return this;
    }
}
