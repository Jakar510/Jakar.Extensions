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


    [Pure] public JArray AddArray() => new(ref this, false);
    [Pure] public JObject AddObject() => new(ref this, false);


    public JWriter StartBlock( in char start, in bool shouldIndent )
    {
        Indent( shouldIndent )
           .Append( start );

        if (_shouldIndent)
        {
            _sb.Append( '\n' );
            _indentLevel += 1;
        }

        return this;
    }


    public JWriter FinishBlock( in char end ) => FinishBlock()
                                                .NewLine()
                                                .Indent()
                                                .Append( end );
    public JWriter FinishBlock()
    {
        if (_shouldIndent) { _indentLevel -= 1; }

        return this;
    }


    public JWriter NewLine()
    {
        if (_shouldIndent) { _sb.Append( '\n' ); }

        return this;
    }


    public JWriter Indent() => Indent( _shouldIndent );
    public JWriter Indent( in bool shouldIndent )
    {
        if (_indentLevel < 0) { throw new InvalidOperationException( $"{nameof(_indentLevel)} is negative" ); }

        if (shouldIndent) { _sb.Append( ' ', _indentLevel * 4 ); }
        else { _sb.Append( ' ' ); }

        return this;
    }


    public JWriter Next()
    {
        _sb.Append( ',' );
        if (_shouldIndent) { NewLine(); }

        return this;
    }


    public JWriter Null()
    {
        _sb.Append( NULL );
        return this;
    }
    public JWriter Append( in string value ) => Append( value.AsSpan() );
    public JWriter Append( in ReadOnlySpan<char> value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( in char value )
    {
        _sb.Append( value );
        return this;
    }


    public JWriter Append( in short value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( in ushort value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( in int value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( in uint value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( in long value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( in ulong value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( in float value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( in double value )
    {
        _sb.Append( value );
        return this;
    }
    public JWriter Append( in decimal value )
    {
        _sb.Append( value );
        return this;
    }


    public JWriter Append( in ISpanFormattable value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize )
    {
        Span<char> buffer = stackalloc char[bufferSize];

        if (!value.TryFormat( buffer, out int charsWritten, format, culture )) { throw new InvalidOperationException( $"Can't format value: '{value}'" ); }

        _sb.Append( buffer[..charsWritten] );
        return this;
    }
    public JWriter Append<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        if (value.HasValue)
        {
            Span<char> buffer = stackalloc char[bufferSize];

            if (!value.Value.TryFormat( buffer, out int charsWritten, format, culture )) { throw new InvalidOperationException( $"Can't format value: '{value}'" ); }

            _sb.Append( buffer[..charsWritten] );
        }
        else { _sb.Append( NULL ); }

        return this;
    }


    public override string ToString() => _sb.ToString();
    public void Dispose() => _sb.Clear();
}
