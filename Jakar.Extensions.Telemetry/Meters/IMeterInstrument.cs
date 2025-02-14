// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 02/14/2025  14:02

namespace Jakar.Extensions.Telemetry.Meters;


public interface IMeterInstrument : IDisposable
{
    public string?         Description { get; }
    public string          Name        { get; }
    public Pair[]? Tags        { get; }
    public string          Unit        { get; }
    public string          Type        { get; }
    public Reading?        LastValue   { get; }
}



public interface IMeterInstrumentReadings : IMeterInstrument
{
    public LinkedList<Reading> Readings { get; }
}



public interface ICreateInstrument<out TInstrument> : IMeterInstrumentReadings
    where TInstrument : ICreateInstrument<TInstrument>
{
    public abstract static TInstrument Create( TelemetryMeter meter, string name, string unit, params Pair[]? tags );
}



[DefaultValue( nameof(Empty) )]
public readonly record struct Reading( JToken Value, DateTimeOffset TimeStamp, params Pair[]? Tags )
{
    public static readonly Reading Empty = new(null!, DateTimeOffset.UtcNow, null);
    public static          Reading Create( JToken value, params Pair[]? tags ) => new(value, DateTimeOffset.UtcNow, tags);
}
