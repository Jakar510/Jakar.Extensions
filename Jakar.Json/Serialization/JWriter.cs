#nullable enable
using System.Runtime.InteropServices;



namespace Jakar.Json.Serialization;


[SuppressMessage( "ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable" )]
public ref struct JWriter
{
    public const string NULL = "null";


    // private readonly StringBuilder      _stream;
    // private readonly StringWriter       _writer;
    private readonly ValueStringBuilder _sb;
    private readonly bool               _shouldIndent;
    private          int                _indentLevel = 0;


    public JWriter() : this( 10_000, Formatting.Indented ) { }
    public JWriter( int capacity, Formatting formatting )
    {
        _shouldIndent = formatting is Formatting.Indented;
        _sb           = new ValueStringBuilder( capacity );

        // _stream       = new StringBuilder( capacity );
        // _writer       = new StringWriter( _stream );
    }


    [Pure] public readonly JArray AddArray() => new(this, _shouldIndent);
    [Pure] public readonly JObject AddObject() => new(this, _shouldIndent);


    public JWriter StartBlock( char start, bool shouldIndent )
    {
        Indent( shouldIndent )
           .Append( start );

        if ( !_shouldIndent ) { return this; }

        _sb.Append( '\n' );
        _indentLevel += 1;
        return this;
    }


    public JWriter FinishBlock( char end ) => FinishBlock()
                                             .NewLine()
                                             .Indent()
                                             .Append( end );
    public JWriter FinishBlock()
    {
        if ( _shouldIndent ) { _indentLevel -= 1; }

        return this;
    }


    public readonly JWriter NewLine()
    {
        if ( _shouldIndent ) { _sb.Append( '\n' ); }

        return this;
    }


    public readonly JWriter Indent() => Indent( _shouldIndent );
    public readonly JWriter Indent( bool shouldIndent )
    {
        if ( _indentLevel < 0 ) { throw new InvalidOperationException( $"{nameof(_indentLevel)} is negative" ); }

        if ( shouldIndent ) { _sb.Append( ' ', _indentLevel * 4 ); }
        else { _sb.Append( ' ' ); }

        return this;
    }


    public readonly JWriter Next()
    {
        _sb.Append( ',' );
        return NewLine();
    }


    public readonly JWriter Null()
    {
        _sb.Append( NULL );
        return this;
    }
    public readonly JWriter Append( string value ) => Append( value.AsSpan() );
    public readonly JWriter Append( ReadOnlySpan<char> value )
    {
        _sb.Append( value );
        return this;
    }
    public readonly JWriter Append( char value )
    {
        _sb.Append( value );
        return this;
    }


    public readonly JWriter Append<T>( T? value ) where T : ISpanFormattable => Append( value,                            CultureInfo.CurrentCulture );
    public readonly JWriter Append<T>( T? value, IFormatProvider? provider ) where T : ISpanFormattable => Append( value, GetDefaultFormat<T>(), provider );
    public readonly JWriter Append<T>( T? value, ReadOnlySpan<char> format, IFormatProvider? provider ) where T : ISpanFormattable
    {
        if ( value is null ) { return Null(); }

        _sb.AppendSpanFormattable( value, format, provider );
        return this;
    }
    public readonly JWriter Append<T>( T? value, ReadOnlySpan<char> format, int bufferSize, IFormatProvider? provider = default ) where T : struct, ISpanFormattable
    {
        if ( !value.HasValue ) { return Null(); }

        Span<char> buffer = stackalloc char[bufferSize];
        if ( !value.Value.TryFormat( buffer, out int charsWritten, format, provider ) ) { throw new InvalidOperationException( $"Can't format value: '{value}'" ); }

        buffer = buffer[..charsWritten];
        _sb.Append( MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), buffer.Length ) );
        return this;
    }

    public static string? GetDefaultFormat<T>()
    {
        Type type = typeof(T);

        if ( type == typeof(short) ||
             type == typeof(short?) ||
             type == typeof(int) ||
             type == typeof(int?) ||
             type == typeof(long) ||
             type == typeof(long?) ||
             type == typeof(float) ||
             type == typeof(float?) ||
             type == typeof(double) ||
             type == typeof(double?) ||
             type == typeof(decimal) ||
             type == typeof(decimal?) ) { return "g"; }

        if ( type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?) ) { return "o"; }

        if ( type == typeof(DateTime) || type == typeof(DateTime?) ) { return @"YYYY-MM-DDTHH:MM:SS"; }

        if ( type == typeof(TimeSpan) || type == typeof(TimeSpan?) ) { return ""; }

        if ( type == typeof(TimeOnly) || type == typeof(TimeOnly?) ) { return ""; }

        if ( type == typeof(DateOnly) || type == typeof(DateOnly?) ) { return ""; }

        return default;
    }


    public readonly override string ToString() => _sb.ToString();
    public readonly void Dispose() => _sb.Reset();
}
