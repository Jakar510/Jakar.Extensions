// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 02/14/2025  14:02

namespace Jakar.Extensions.Telemetry.Meters;


public readonly record struct MeterInstrumentInfo( string Name, string Unit, string? Description, params Pair[]? Tags )
{
    public MeterInstrumentInfo( string Name, string Unit, string? Description ) : this( Name, Unit, Description, null ) { }
}



public interface IMeterInstrument : IDisposable
{
    public string              Type { get; }
    public MeterInstrumentInfo Info { get; }
}



public interface IMeterInstrumentReadings<TValue> : IMeterInstrument
    where TValue : IEquatable<TValue>
{
    public Reading<TValue>?      LastValue { get; }
    public List<Reading<TValue>> Readings  { get; }
}



public interface ICreateInstrument<out TInstrument, TValue> : IMeterInstrumentReadings<TValue>
    where TInstrument : ICreateInstrument<TInstrument, TValue>
    where TValue : struct, INumber<TValue>
{
    public abstract static TInstrument Create( TelemetryMeter<TValue> meter, string name, string unit, string? description, params Pair[]? tags );
}
