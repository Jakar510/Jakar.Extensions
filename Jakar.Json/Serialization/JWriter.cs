namespace Jakar.Json.Serialization;


[ SuppressMessage( "ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable" ) ]
public ref struct JWriter( int capacity, Formatting formatting )
{
    public const           string                             NULL           = "null";
    public const           char                               QUOTE          = '"';
    public const           char                               COLON          = ':';
    public const           char                               SPACE          = ' ';
    public static readonly ConcurrentDictionary<Type, string> DefaultFormats = new();
    private readonly       ValueStringBuilder                 _sb = new( capacity );
    private                int                                _indentLevel = 0;
    public                 bool                               ShouldIndent { get; } = formatting is Formatting.Indented;


    public JWriter() : this( 10_000, Formatting.Indented ) { }
    public readonly          void   Dispose()  => _sb.Dispose();
    public readonly override string ToString() => _sb.ToString();


    [ Pure ] public readonly JArray  AddArray()  => new(this);
    [ Pure ] public readonly JObject AddObject() => new(this);


    public JWriter StartBlock( char start )
    {
        Indent().Append( start );

        if ( !ShouldIndent ) { return this; }

        _sb.Append( '\n' );
        _indentLevel += 1;
        return this;
    }


    public JWriter FinishBlock( char end ) => FinishBlock().NewLine().Indent().Append( end );
    public JWriter FinishBlock()
    {
        if ( ShouldIndent ) { _indentLevel -= 1; }

        return this;
    }


    public readonly JWriter Indent()
    {
        if ( _indentLevel < 0 ) { throw new InvalidOperationException( $"{nameof(_indentLevel)} is negative" ); }

        if ( ShouldIndent ) { _sb.Append( ' ', _indentLevel * 4 ); }
        else { _sb.Append( ' ' ); }

        return this;
    }


    public readonly JWriter NewLine()
    {
        if ( ShouldIndent ) { _sb.Append( '\n' ); }

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


    public readonly JWriter Append<T>( T? value ) where T : ISpanFormattable                            => Append( value, CultureInfo.CurrentCulture );
    public readonly JWriter Append<T>( T? value, IFormatProvider? provider ) where T : ISpanFormattable => Append( value, GetDefaultFormat<T>(), provider );
    public readonly JWriter Append<T>( T? value, ReadOnlySpan<char> format, IFormatProvider? provider ) where T : ISpanFormattable
    {
        if ( value is null ) { return Null(); }

        _sb.AppendSpanFormattable( value, format, provider );
        return this;
    }
    public readonly JWriter Append<T>( T? value, ReadOnlySpan<char> format, int bufferSize, IFormatProvider? provider = default ) where T : ISpanFormattable
    {
        if ( value is null ) { return Null(); }

        _sb.EnsureCapacity( bufferSize );
        _sb.AppendSpanFormattable( value, format, provider );
        return this;
    }


    public readonly JWriter AppendValue<T>( T? value ) where T : struct, ISpanFormattable                            => AppendValue( value, CultureInfo.CurrentCulture );
    public readonly JWriter AppendValue<T>( T? value, IFormatProvider? provider ) where T : struct, ISpanFormattable => AppendValue( value, GetDefaultFormat<T>(), provider );
    public readonly JWriter AppendValue<T>( T? value, ReadOnlySpan<char> format, IFormatProvider? provider ) where T : struct, ISpanFormattable =>
        value is null
            ? Null()
            : AppendValue( value.Value, format, provider );
    public readonly JWriter AppendValue<T>( T? value, ReadOnlySpan<char> format, int bufferSize, IFormatProvider? provider = default ) where T : struct, ISpanFormattable =>
        value is null
            ? Null()
            : AppendValue( value.Value, format, bufferSize, provider );


    public readonly JWriter AppendValue<T>( T value ) where T : struct, ISpanFormattable                            => AppendValue( value, CultureInfo.CurrentCulture );
    public readonly JWriter AppendValue<T>( T value, IFormatProvider? provider ) where T : struct, ISpanFormattable => AppendValue( value, GetDefaultFormat<T>(), provider );
    public readonly JWriter AppendValue<T>( T value, ReadOnlySpan<char> format, IFormatProvider? provider ) where T : struct, ISpanFormattable
    {
        _sb.AppendSpanFormattable( value, format, provider );
        return this;
    }
    public readonly JWriter AppendValue<T>( T value, ReadOnlySpan<char> format, int bufferSize, IFormatProvider? provider = default ) where T : struct, ISpanFormattable
    {
        _sb.EnsureCapacity( bufferSize );
        _sb.AppendSpanFormattable( value, format, provider );
        return this;
    }


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string? GetDefaultFormat<T>() => GetDefaultFormat( typeof(T) );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static string? GetDefaultFormat( in Type type )
    {
        if ( DefaultFormats.TryGetValue( type, out string? result ) ) { return result; }

        if ( type == typeof(short)   ||
             type == typeof(short?)  ||
             type == typeof(int)     ||
             type == typeof(int?)    ||
             type == typeof(long)    ||
             type == typeof(long?)   ||
             type == typeof(float)   ||
             type == typeof(float?)  ||
             type == typeof(double)  ||
             type == typeof(double?) ||
             type == typeof(decimal) ||
             type == typeof(decimal?) ) { result = "g"; }

        else if ( type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?) ) { result = "o"; }

        else if ( type == typeof(DateTime) || type == typeof(DateTime?) ) { result = @"YYYY-MM-DDTHH:MM:SS"; }

        else if ( type == typeof(TimeSpan) || type == typeof(TimeSpan?) ) { result = "g"; }

        else if ( type == typeof(TimeOnly) || type == typeof(TimeOnly?) ) { result = "t"; }

        else if ( type == typeof(DateOnly) || type == typeof(DateOnly?) ) { result = "d"; }

        else if ( type == typeof(Guid) ) { result = "D"; }

        if ( result is not null ) { DefaultFormats[type] = result; }

        return result;
    }
}
