#nullable enable
using System.Globalization;



namespace Jakar.Json.Serialization;


[SuppressMessage( "ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable" )]
public ref struct JWriter
{
    public const string NULL = "null";


    private readonly StringBuilder _sb = new();
    private readonly bool          _shouldIndent;
    private          int           _indentLevel = 0;


    public JWriter() : this( Formatting.None ) { }
    public JWriter( Formatting formatting ) => _shouldIndent = formatting is Formatting.Indented;


    [Pure] public JArray AddArray() => new(this, false);
    [Pure] public JObject AddObject() => new(this, false);


    public JWriter StartBlock( char start, bool shouldIndent )
    {
        Indent( shouldIndent )
           .Append( start );

        if ( _shouldIndent )
        {
            _sb.Append( '\n' );
            _indentLevel += 1;
        }

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


    public JWriter NewLine()
    {
        if ( _shouldIndent ) { _sb.Append( '\n' ); }

        return this;
    }


    public JWriter Indent() => Indent( _shouldIndent );
    public JWriter Indent( bool shouldIndent )
    {
        if ( _indentLevel < 0 ) { throw new InvalidOperationException( $"{nameof(_indentLevel)} is negative" ); }

        if ( shouldIndent ) { _sb.Append( ' ', _indentLevel * 4 ); }
        else { _sb.Append( ' ' ); }

        return this;
    }


    public JWriter Next()
    {
        _sb.Append( ',' );
        if ( _shouldIndent ) { NewLine(); }

        return this;
    }


    public JWriter Null()
    {
        _sb.Append( NULL );
        return this;
    }
    public JWriter Append( string value ) => Append( value.AsSpan() );
    public JWriter Append( ReadOnlySpan<char> value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( char value )
    {
        _sb.Append( value );
        return this;
    }


    public JWriter Append( short value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( ushort value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( int value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( uint value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( long value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( ulong value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( float value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( double value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( decimal value )
    {
        _sb.Append( value );
        return this;
    }


    public JWriter Append( ISpanFormattable value, ReadOnlySpan<char> format, CultureInfo culture, int bufferSize )
    {
        Span<char> buffer = stackalloc char[bufferSize];

        if ( !value.TryFormat( buffer, out int charsWritten, format, culture ) ) { throw new InvalidOperationException( $"Can't format value: '{value}'" ); }

        _sb.Append( buffer[..charsWritten] );
        return this;
    }
    public JWriter Append<T>( T? value, ReadOnlySpan<char> format, CultureInfo culture, int bufferSize ) where T : struct, ISpanFormattable
    {
        if ( value.HasValue )
        {
            Span<char> buffer = stackalloc char[bufferSize];

            if ( !value.Value.TryFormat( buffer, out int charsWritten, format, culture ) ) { throw new InvalidOperationException( $"Can't format value: '{value}'" ); }

            _sb.Append( buffer[..charsWritten] );
        }
        else { _sb.Append( NULL ); }

        return this;
    }


    public override string ToString() => _sb.ToString();
    public void Dispose() => _sb.Clear();
}
