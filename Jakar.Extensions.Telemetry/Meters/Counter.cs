// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 02/14/2025  14:02

namespace Jakar.Extensions.Telemetry.Meters;


public interface ICounter<TValue> : IMeterInstrument
    where TValue : struct, INumber<TValue>
{
    public abstract static TValue           StartValue   { get; set; }
    public                 TValue           CurrentValue { get; }
    public                 TValue           Precision    { get; set; }
    public                 ICounter<TValue> Increment();
    public                 ICounter<TValue> Decrement();
}



[SuppressMessage( "ReSharper", "ConvertToAutoPropertyWhenPossible" )]
public abstract class Counter<TInstrument, TValue>( TelemetryMeter meter, string name, string unit, params Pair[]? tags ) : Instrument<TInstrument>( meter, name, unit, tags ), ICounter<TValue>
    where TValue : struct, INumber<TValue>
    where TInstrument : Counter<TInstrument, TValue>, ICreateInstrument<TInstrument>
{
    private       TValue _current = StartValue;
    public        TValue Precision    { get; set; } = TValue.One;
    public static TValue StartValue   { get; set; } = TValue.Zero;
    public        TValue CurrentValue => _current;


    protected abstract JToken ToJToken( TValue value );
    public ICounter<TValue> Increment()
    {
        _current += Precision;
        RecordMeasurement( ToJToken( _current ) );
        return this;
    }
    public ICounter<TValue> Decrement()
    {
        _current -= Precision;
        RecordMeasurement( ToJToken( _current ) );
        return this;
    }
}



public sealed class CounterLong( TelemetryMeter meter, string name, string unit, params Pair[]? tags ) : Counter<CounterLong, long>( meter, name, unit, tags ), ICreateInstrument<CounterLong>
{
    public static      CounterLong Create( TelemetryMeter meter, string name, string unit, params Pair[]? tags ) => new(meter, name, unit, tags);
    protected override JToken      ToJToken( long         value ) => value;
}



public sealed class CounterULong( TelemetryMeter meter, string name, string unit, params Pair[]? tags ) : Counter<CounterULong, ulong>( meter, name, unit, tags ), ICreateInstrument<CounterULong>
{
    public static      CounterULong Create( TelemetryMeter meter, string name, string unit, params Pair[]? tags ) => new(meter, name, unit, tags);
    protected override JToken       ToJToken( ulong        value ) => value;
}



public sealed class CounterDouble( TelemetryMeter meter, string name, string unit, params Pair[]? tags ) : Counter<CounterDouble, double>( meter, name, unit, tags ), ICreateInstrument<CounterDouble>
{
    public static      CounterDouble Create( TelemetryMeter meter, string name, string unit, params Pair[]? tags ) => new(meter, name, unit, tags);
    protected override JToken        ToJToken( double       value ) => value;
}
