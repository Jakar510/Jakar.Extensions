// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  19:06


using Serilog.Core;
using Serilog.Events;
using System.Threading;
using ZLinq;



namespace Jakar.Extensions.Telemetry.Meters;


public sealed class Meters( TelemetrySource source ) : ILogEventEnricher, IDisposable
{
    private readonly TelemetrySource                                  _source      = source;
    private readonly Dictionary<Type, Dictionary<string, Instrument>> _instruments = new(Buffers.DEFAULT_CAPACITY);


    public void Dispose()
    {
        foreach ( Dictionary<string, Instrument> instruments in _instruments.Values )
        {
            foreach ( Instrument instrument in instruments.Values ) { instrument.Dispose(); }

            instruments.Clear();
        }

        _instruments.Clear();
    }

    public void Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory )
    {
        IEnumerable<LogEventProperty> properties = Save();
        foreach ( LogEventProperty property in properties ) { logEvent.AddPropertyIfAbsent(property); }
    }

    public Instrument<TValue> CreateMeter<TValue>( Type type, string name )
        where TValue : IEquatable<TValue>
    {
        ref Dictionary<string, Instrument>? instruments = ref CollectionsMarshal.GetValueRefOrAddDefault(_instruments, type, out bool _);
        instruments ??= new Dictionary<string, Instrument>();

        if ( instruments.TryGetValue(name, out Instrument? instrument) ) { return (Instrument<TValue>)instrument; }

        instrument        = new Instrument<TValue>(this, name);
        instruments[name] = instrument;
        return (Instrument<TValue>)instrument;
    }
    public IEnumerable<LogEventProperty> Save() => _instruments.Values.SelectMany(static x => x.Values).Select(static x => x.Save());



    public enum Type
    {
        Save,
        Refresh,
        DBCall,
        NetworkCall,
        Navigation,
    }



    public abstract class Instrument( Meters meters, string name ) : IDisposable
    {
        protected readonly Lock                          _lock = new();
        protected readonly string                        _name = $"{meters._source.Info.AppName}.{nameof(Meters)}.{name}";
        public             LogEventProperty              Save() => new(_name, new StructureValue(GetValues()));
        protected abstract IEnumerable<LogEventProperty> GetValues();
        public virtual     void                          Dispose() => GC.SuppressFinalize(this);
    }



    public sealed class Instrument<TValue>( Meters meters, string name ) : Instrument(meters, name)
        where TValue : IEquatable<TValue>
    {
        private readonly ValueLinkedList<Reading<TValue>> __values = [];


        public override void Dispose()
        {
            __values.Clear();
            base.Dispose();
        }
        public void RecordMeasurement( TValue value )
        {
            lock ( _lock ) { __values.Add(new Reading<TValue>(value, DateTimeOffset.UtcNow)); }
        }
        protected override IEnumerable<LogEventProperty> GetValues()
        {
            lock ( _lock )
            {
                LogEventProperty[] array = __values.AsValueEnumerable().Select(GetPair).ToArray();
                __values.Clear();
                return array;
            }
        }
        private LogEventProperty GetPair( Reading<TValue> x )
        {
            string      timeStamp = $"{x.TimeStamp:yyyy-MM-dd HH:mm:ss.fff}";
            ScalarValue value     = new(x.Value);

            // return new LogEventProperty( _name, new DictionaryValue( [new KeyValuePair<ScalarValue, LogEventPropertyValue>( timeStamp, value )] ) );
            return new LogEventProperty(_name, new StructureValue([new LogEventProperty(timeStamp, value)]));
        }
    }
}



public class TelemetryMeters : IDisposable
{
    public TelemetryMeter<double>.Collection Doubles { get; init; } = [];
    public TelemetryMeter<long>.Collection   Longs   { get; init; } = [];
    public void Dispose()
    {
        Doubles.Dispose();
        Longs.Dispose();
    }
}



public class TelemetryMeter<TValue> : IDisposable
    where TValue : struct, INumber<TValue>
{
    public required TValue                                        DefaultCurrentValue { get; init; }
    public required TValue                                        DefaultPrecision    { get; init; }
    public          ConcurrentDictionary<string, MeterInstrument> Instruments         { get; init; } = [];
    public required string                                        Name                { get; init; }
    public          Pair[]?                                       Tags                { get; set; }
    public          string?                                       Version             { get; init; }


    public void Dispose()
    {
        foreach ( MeterInstrument instrument in Instruments.Values ) { instrument.Dispose(); }

        Instruments.Clear();
        GC.SuppressFinalize(this);
    }
    public TInstrument AddInstrument<TInstrument>( string name, string unit, string? description )
        where TInstrument : MeterInstrument, ICreateInstrument<TInstrument, TValue> => AddInstrument<TInstrument>(name, unit, description, null);
    public TInstrument AddInstrument<TInstrument>( string name, string unit, string? description, params Pair[]? tags )
        where TInstrument : MeterInstrument, ICreateInstrument<TInstrument, TValue> => AddInstrument(TInstrument.Create(this, name, unit, description, tags));
    public TInstrument AddInstrument<TInstrument>( TInstrument instrument )
        where TInstrument : MeterInstrument
    {
        Instruments[instrument.Info.Name] = instrument.SetMeter(this);
        return instrument;
    }


    [Pure]
    public TelemetryCounter<TValue> CreateCounter( string name, string unit, string description, params Pair[]? tags )
    {
        MeterInstrumentInfo info = new(name, unit, description, tags);
        return CreateCounter(in info);
    }


    [Pure]
    public TelemetryCounter<TValue> CreateCounter( ref readonly MeterInstrumentInfo info )
    {
        TelemetryCounter<TValue> instrument = new(this, in info);
        return AddInstrument(instrument);
    }


    [Pure]
    public TelemetryCounter<TValue> CreateCounter( Func<Reading<TValue>> func, string name, string unit, string description, params Pair[]? tags )
    {
        MeterInstrumentInfo info = new(name, unit, description, tags);
        return CreateCounter(func, in info);
    }


    [Pure]
    public TelemetryCounter<TValue> CreateCounter( Func<Reading<TValue>[]> func, string name, string unit, string description, params Pair[]? tags )
    {
        MeterInstrumentInfo info = new(name, unit, description, tags);
        return CreateCounter(func, in info);
    }


    [Pure]
    public TelemetryCounter<TValue> CreateCounter( Func<Reading<TValue>> func, ref readonly MeterInstrumentInfo info )
    {
        TelemetryCounter<TValue> instrument = new(this, in info, func);
        return AddInstrument(instrument);
    }

    [Pure]
    public TelemetryCounter<TValue> CreateCounter( Func<Reading<TValue>[]> func, ref readonly MeterInstrumentInfo info )
    {
        TelemetryCounter<TValue> instrument = new(this, in info, func);
        return AddInstrument(instrument);
    }


    private SavedMeterValues SaveValues()
    {
        Dictionary<string, MeterInstrument.SavedValues> instruments = new(Instruments.Count);
        foreach ( ( string? name, MeterInstrument? instrument ) in Instruments ) { instruments[name] = instrument.SaveValues(); }

        return new SavedMeterValues(Name, Version, Tags, instruments);
    }



    public sealed class Collection() : ConcurrentDictionary<string, TelemetryMeter<TValue>>(Environment.ProcessorCount, Buffers.DEFAULT_CAPACITY, StringComparer.Ordinal), IDisposable
    {
        public static Collection Create() => new();
        public Collection( IEnumerable<KeyValuePair<string, TelemetryMeter<TValue>>> values ) : this()
        {
            foreach ( ( string? key, TelemetryMeter<TValue>? value ) in values ) { GetOrAdd(key, value); }
        }
        public Collection( IDictionary<string, TelemetryMeter<TValue>> values ) : this()
        {
            foreach ( ( string? key, TelemetryMeter<TValue>? value ) in values ) { GetOrAdd(key, value); }
        }
        public void Dispose()
        {
            foreach ( TelemetryMeter<TValue> meter in Values ) { meter.Dispose(); }

            Clear();
        }

        public SavedState SaveState()
        {
            SavedState state = new(Count);

            foreach ( ( string? name, TelemetryMeter<TValue> meter ) in this ) { state[name] = meter.SaveValues(); }

            return state;
        }



        public sealed class SavedState( int capacity ) : Dictionary<string, SavedMeterValues>(capacity);
    }



    public class MeterInstrument : IMeterInstrumentReadings<TValue>
    {
        protected bool                    _isDisposed;
        protected TelemetryMeter<TValue>? _meter;
        private   TValue?                 _current;
        private   TValue?                 _precision;


        public             TValue                 CurrentValue { get => _current ??= _meter?.DefaultCurrentValue ?? TValue.Zero; [MemberNotNull(nameof(_current))] protected internal set => _current = value; }
        public required    MeterInstrumentInfo    Info         { get;                                                            init; }
        public             Reading<TValue>?       LastValue    => Reading<TValue>.TryGetLastValue(CollectionsMarshal.AsSpan(Readings));
        protected internal TelemetryMeter<TValue> Meter        { get => GetMeter();                                            init => _meter = value; }
        public             TValue                 Precision    { get => _precision ??= _meter?.DefaultPrecision ?? TValue.One; set => _precision = value; }
        public             List<Reading<TValue>>  Readings     { get;                                                          init; } = [];
        public             string                 Type         => GetType().Name;


        public event OnMeterMeasurement<TValue>? OnMeasurement;

        protected MeterInstrument() { }

        [SetsRequiredMembers]
        public MeterInstrument( TelemetryMeter<TValue> meter, ref readonly MeterInstrumentInfo info )
        {
            _meter = meter;
            Info   = info;
        }
        public virtual void Dispose()
        {
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }


        public    SavedValues            SaveValues() => new(Info, Readings.ToArray());
        protected TelemetryMeter<TValue> GetMeter()   => _meter ?? throw new InvalidOperationException($"Call {nameof(SetMeter)} First", new NullReferenceException(nameof(_meter)));
        public MeterInstrument SetMeter( TelemetryMeter<TValue> meter )
        {
            _meter = meter;
            return this;
        }
        internal void ReportMeasurement( ref readonly Reading<TValue> reading )
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            _current = reading.Value;
            OnMeasurement?.Invoke(in reading);
        }
        public void RecordMeasurement( TValue value ) => RecordMeasurement(value, null);
        public void RecordMeasurement( TValue value, params Pair[]? tags )
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            Reading<TValue> reading = new(value, tags);
            RecordMeasurement(in reading);
        }
        public void RecordMeasurement( ref readonly Reading<TValue> reading )
        {
            Readings.Add(reading);
            ReportMeasurement(in reading);
        }
        public void RecordMeasurement( params ReadOnlySpan<Reading<TValue>> readings )
        {
            foreach ( Reading<TValue> reading in readings ) { RecordMeasurement(in reading); }
        }


        public MeterInstrument Increment()
        {
            CurrentValue += Precision;
            RecordMeasurement(CurrentValue);
            return this;
        }
        public MeterInstrument Decrement()
        {
            CurrentValue -= Precision;
            RecordMeasurement(CurrentValue);
            return this;
        }



        public sealed record SavedValues( MeterInstrumentInfo Info, params Reading<TValue>[] Readings );
    }



    public sealed record SavedMeterValues( string Name, string? Version, Pair[]? Tags, Dictionary<string, MeterInstrument.SavedValues> Instruments );
}
