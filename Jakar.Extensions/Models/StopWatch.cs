// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  2:26 PM

namespace Jakar.Extensions;


public readonly struct StopWatch( string caller, TextWriter? writer = null ) : IDisposable
{
    private readonly TextWriter? __writer = writer;
    private readonly string      __caller = caller;
    private readonly long        __start  = Stopwatch.GetTimestamp();


    public TimeSpan Elapsed { get => Stopwatch.GetElapsedTime(__start, Stopwatch.GetTimestamp()); }


    public void Dispose()
    {
        if ( __writer is not null ) { __writer.WriteLine(ToString()); }
        else { Debug.WriteLine(ToString()); }
    }
    public override string ToString() => SpanDuration.ToString(Elapsed, $"[{__caller}] ");


    public static StopWatch Start( [CallerMemberName] string caller                                   = EMPTY ) => Start(Console.Out, caller);
    public static StopWatch Start( TextWriter                writer, [CallerMemberName] string caller = EMPTY ) => new(caller, writer);
}



public readonly record struct SpanDuration( double Value, SpanDuration.Range Unit ) : ISpanFormattable
{
    public readonly double Value = Value;
    public readonly Range  Unit  = Unit;


    public string ToString( string? format, IFormatProvider? formatProvider = null )
    {
        if ( string.IsNullOrWhiteSpace(format) ) { return ToString(); }

        FormattableString result = $"{format} {Value} {Unit}";
        return result.ToString(formatProvider);
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider = null )
    {
        return format.IsNullOrWhiteSpace()
                   ? destination.TryWrite(provider, $"{Value} {Unit}",          out charsWritten)
                   : destination.TryWrite(provider, $"{format} {Value} {Unit}", out charsWritten);
    }
    public override                 string ToString()                                    => $"{Value} {Unit}";
    public static implicit operator SpanDuration( (double value, Range unit)  tuple )    => new(tuple.value, tuple.unit);
    public static implicit operator SpanDuration( KeyValuePair<double, Range> tuple )    => new(tuple.Key, tuple.Value);
    public static implicit operator TimeSpan( SpanDuration                    duration ) => duration.AsTimeSpan();
    public static implicit operator SpanDuration( TimeSpan                    duration ) => Create(in duration);


    public TimeSpan AsTimeSpan()
    {
        return Unit switch
               {
                   Range.Days         => TimeSpan.FromDays(Value),
                   Range.Hours        => TimeSpan.FromHours(Value),
                   Range.Minutes      => TimeSpan.FromMinutes(Value),
                   Range.Seconds      => TimeSpan.FromSeconds(Value),
                   Range.Milliseconds => TimeSpan.FromMilliseconds(Value),
                   Range.Microseconds => TimeSpan.FromMicroseconds(Value),
                   _                  => throw new ArgumentOutOfRangeException()
               };
    }


    public static string ToString( in TimeSpan span, string? format = null ) => Create(in span)
       .ToString(format);
    public static SpanDuration Create( in TimeSpan span )
    {
        if ( span.Days != 0 ) { return new SpanDuration(span.TotalDays, Range.Days); }

        if ( span.Hours != 0 ) { return new SpanDuration(span.TotalHours, Range.Hours); }

        if ( span.Minutes != 0 ) { return new SpanDuration(span.TotalMinutes, Range.Minutes); }

        if ( span.Seconds != 0 ) { return new SpanDuration(span.TotalSeconds, Range.Seconds); }

        if ( span.Milliseconds != 0 ) { return new SpanDuration(span.TotalMilliseconds, Range.Milliseconds); }

        return new SpanDuration(span.TotalMicroseconds, Range.Microseconds);
    }



    public enum Range
    {
        Days,
        Hours,
        Minutes,
        Seconds,
        Milliseconds,
        Microseconds,
    }
}
