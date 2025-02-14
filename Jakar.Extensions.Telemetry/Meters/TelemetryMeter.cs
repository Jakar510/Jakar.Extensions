// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  19:06

namespace Jakar.Extensions.Telemetry.Meters;


public class TelemetryMeter : IDisposable
{
    public          ConcurrentDictionary<string, Instrument> Instruments { get; init; } = [];
    public required string                                   Name        { get; init; }
    public          Pair[]?                          Tags        { get; set; }
    public          string?                                  Version     { get; init; }


    public TInstrument AddInstrument<TInstrument, TValue>( string name, string unit )
        where TInstrument : Instrument, ICreateInstrument<TInstrument> => AddInstrument<TInstrument>( name, unit, null );
    public TInstrument AddInstrument<TInstrument>( string name, string unit, params Pair[]? tags )
        where TInstrument : Instrument, ICreateInstrument<TInstrument>
    {
        TInstrument instrument = TInstrument.Create( this, name, unit, tags );
        Instruments[name] = instrument.SetMeter( this );
        return instrument;
    }


    public void Dispose()
    {
        foreach ( IMeterInstrument instrument in Instruments.Values ) { instrument.Dispose(); }

        Instruments.Clear();
        GC.SuppressFinalize( this );
    }



    public sealed class Collection() : ConcurrentDictionary<string, TelemetryMeter>( Environment.ProcessorCount, Buffers.DEFAULT_CAPACITY, StringComparer.Ordinal ), IDisposable
    {
        public static Collection Create() => new();
        public Collection( IEnumerable<KeyValuePair<string, TelemetryMeter>> values ) : this()
        {
            foreach ( (string? key, TelemetryMeter? value) in values ) { GetOrAdd( key, value ); }
        }
        public Collection( IDictionary<string, TelemetryMeter> values ) : this()
        {
            foreach ( (string? key, TelemetryMeter? value) in values ) { GetOrAdd( key, value ); }
        }
        public void Dispose()
        {
            foreach ( TelemetryMeter meter in Values ) { meter.Dispose(); }

            Clear();
        }
    }
}
