// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  19:06

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace Jakar.Extensions.Telemetry;


public class Meter : IDisposable
{
    public          ConcurrentDictionary<string, Counter>    Counters    { get; init; } = [];
    public          ConcurrentDictionary<string, Instrument> Instruments { get; init; } = [];
    public required string                                   Name        { get; init; }
    public required TelemetryTag[]?                          Tags        { get; init; }
    public          string?                                  Version     { get; init; }


    public event Action<Reading>? OnMeasurement;

    internal void ReportMeasurement( in Reading reading ) => OnMeasurement?.Invoke( reading );


    public Counter AddInstrument( string name, string? description = null )                       => AddInstrument( name, description, null );
    public Counter AddInstrument( string name, string? description, params TelemetryTag[]? tags ) => Counters[name] = Counter.Create( this, name, description, tags );
    public T AddInstrument<T>( string name, string? description = null )
        where T : Instrument, ICreateInstrument<T> => AddInstrument<T>( name, description, null );
    public T AddInstrument<T>( string name, string? description, params TelemetryTag[]? tags )
        where T : Instrument, ICreateInstrument<T>
    {
        T x = T.Create( this, name, description, tags );
        Instruments[name] = x;
        return x;
    }


    public void Dispose()
    {
        foreach ( Counter counter in Counters.Values ) { counter.Dispose(); }

        foreach ( Instrument instrument in Instruments.Values ) { instrument.Dispose(); }

        Counters.Clear();
        Instruments.Clear();
        GC.SuppressFinalize( this );
    }
}



public interface ICreateInstrument<out T> : IInstrument
    where T : Instrument, ICreateInstrument<T>
{
    public abstract static T Create( Meter meter, string name, string? description, params TelemetryTag[]? tags );
}



public interface IInstrument : IDisposable
{
    public string?         Description { get; init; }
    public string          Name        { get; init; }
    public TelemetryTag[]? Tags        { get; init; }
    public string?         Unit        { get; init; }
}



public class Instrument : IInstrument
{
    public required              string?             Description { get; init; }
    [JsonIgnore] public required Meter               Meter       { get; init; }
    public required              string              Name        { get; init; }
    public                       LinkedList<Reading> Readings    { get; init; } = [];
    public required              TelemetryTag[]?     Tags        { get; init; }
    public                       string?             Unit        { get; init; }


    public virtual void Dispose()                                => GC.SuppressFinalize( this );
    protected      void RecordMeasurement( JToken? measurement ) => RecordMeasurement( measurement, null );
    protected void RecordMeasurement( JToken? measurement, params TelemetryTag[]? tags )
    {
        Reading reading = new(measurement, DateTimeOffset.UtcNow, tags);
        Readings.Add( reading );
        Meter.ReportMeasurement( reading );
    }
}



public sealed class Counter : Instrument
{
    private ulong  _current;
    public  ushort Precision { get; init; } = 1;


    public static Counter Create( Meter meter, string name, string? description, TelemetryTag[]? tags, ushort precision = 1 ) =>
        new()
        {
            Meter       = meter,
            Name        = name,
            Description = description,
            Tags        = tags,
            Precision   = precision
        };


    public Counter Increment()
    {
        _current += Precision;
        RecordMeasurement( _current );
        return this;
    }
    public Counter Decrement()
    {
        _current += Precision;
        RecordMeasurement( _current );
        return this;
    }
}



[DefaultValue( nameof(Empty) )]
public readonly record struct Reading( JToken? Value, DateTimeOffset TimeStamp, params TelemetryTag[]? Tags )
{
    public static readonly Reading Empty = new(null, DateTimeOffset.UtcNow, null);
    public static Reading Create<T>( T reading, DateTimeOffset timeStamp, params TelemetryTag[]? tags )
    {
        JToken? value = reading switch
                        {
                            null             => null,
                            Guid n           => n,
                            bool n           => n,
                            char n           => n,
                            byte n           => n,
                            sbyte n          => n,
                            short n          => n,
                            ushort n         => n,
                            int n            => n,
                            uint n           => n,
                            long n           => n,
                            ulong n          => n,
                            float n          => n,
                            double n         => n,
                            decimal n        => n,
                            DateTimeOffset n => n,
                            DateTime n       => n,
                            TimeSpan n       => n,
                            TimeOnly n       => n.ToTimeSpan(),
                            DateOnly n       => n.ToDateTime( TimeOnly.MinValue ),
                            string n         => n,
                            Uri n            => n,
                            byte[] n         => n,
                            _                => JToken.FromObject( reading )
                        };

        return new Reading( value, timeStamp, tags );
    }
}



[DefaultValue( nameof(Empty) )]
public readonly record struct Reading<T>( T? Value, DateTimeOffset TimeStamp, params TelemetryTag[]? Tags )
{
    public static readonly Reading<T> Empty = new(default, DateTimeOffset.UtcNow, null);
    public static          Reading<T> Create( T value, DateTimeOffset timeStamp, params TelemetryTag[]? tags ) => new(value, timeStamp, tags);
}
