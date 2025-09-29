namespace Jakar.Json.Serialization;


[SuppressMessage( "ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable" )]
public ref struct JWriter( int capacity, Formatting formatting )
{
    public const           string                             NULL           = "null";
    public const           char                               QUOTE          = '"';
    public const           char                               COLON          = ':';
    public const           char                               SPACE          = ' ';
    public static readonly ConcurrentDictionary<Type, string> DefaultFormats = new();
    private readonly       ValueStringBuilder                 __sb            = new(capacity);
    private                int                                __indentLevel   = 0;
    public                 bool                               ShouldIndent { get; } = formatting is Formatting.Indented;


    public JWriter() : this( 10_000, Formatting.Indented ) { }
    public readonly          void   Dispose()  => __sb.Dispose();
    public readonly override string ToString() => __sb.ToString();


    [Pure] public readonly JArray  AddArray()  => new(this);
    [Pure] public readonly JObject AddObject() => new(this);


    public JWriter StartBlock( char start )
    {
        Indent().Append( start );

        if ( !ShouldIndent ) { return this; }

        __sb.Append( '\n' );
        __indentLevel += 1;
        return this;
    }


    public JWriter FinishBlock( char end ) => FinishBlock().NewLine().Indent().Append( end );
    public JWriter FinishBlock()
    {
        if ( ShouldIndent ) { __indentLevel -= 1; }

        return this;
    }


    public readonly JWriter Indent()
    {
        if ( __indentLevel < 0 ) { throw new InvalidOperationException( $"{nameof(__indentLevel)} is negative" ); }

        if ( ShouldIndent ) { __sb.Append( ' ', __indentLevel * 4 ); }
        else { __sb.Append( ' ' ); }

        return this;
    }


    public readonly JWriter NewLine()
    {
        if ( ShouldIndent ) { __sb.Append( '\n' ); }

        return this;
    }
    public readonly JWriter Next()
    {
        __sb.Append( ',' );
        return NewLine();
    }
    public readonly JWriter Null()
    {
        __sb.Append( NULL );
        return this;
    }


    public readonly JWriter Append( ReadOnlySpan<char> value )
    {
        __sb.Append( value );
        return this;
    }
    public readonly JWriter Append( char value )
    {
        __sb.Append( value );
        return this;
    }


    public readonly JWriter Append<TValue>( TValue? value )
        where TValue : ISpanFormattable => Append( value, CultureInfo.CurrentCulture );
    public readonly JWriter Append<TValue>( TValue? value, IFormatProvider? provider )
        where TValue : ISpanFormattable => Append( value, GetDefaultFormat<TValue>(), provider );
    public readonly JWriter Append<TValue>( TValue? value, ReadOnlySpan<char> format, IFormatProvider? provider )
        where TValue : ISpanFormattable
    {
        if ( value is null ) { return Null(); }

        __sb.AppendSpanFormattable( value, format, provider );
        return this;
    }
    public readonly JWriter Append<TValue>( TValue? value, ReadOnlySpan<char> format, int bufferSize, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        if ( value is null ) { return Null(); }

        __sb.EnsureCapacity( bufferSize );
        __sb.AppendSpanFormattable( value, format, provider );
        return this;
    }


    public readonly JWriter AppendValue<TValue>( TValue? value )
        where TValue : struct, ISpanFormattable => AppendValue( value, CultureInfo.CurrentCulture );
    public readonly JWriter AppendValue<TValue>( TValue? value, IFormatProvider? provider )
        where TValue : struct, ISpanFormattable => AppendValue( value, GetDefaultFormat<TValue>(), provider );
    public readonly JWriter AppendValue<TValue>( TValue? value, ReadOnlySpan<char> format, IFormatProvider? provider )
        where TValue : struct, ISpanFormattable =>
        value is null
            ? Null()
            : AppendValue( value.Value, format, provider );
    public readonly JWriter AppendValue<TValue>( TValue? value, ReadOnlySpan<char> format, int bufferSize, IFormatProvider? provider = null )
        where TValue : struct, ISpanFormattable =>
        value is null
            ? Null()
            : AppendValue( value.Value, format, bufferSize, provider );


    public readonly JWriter AppendValue<TValue>( TValue value )
        where TValue : struct, ISpanFormattable => AppendValue( value, CultureInfo.CurrentCulture );
    public readonly JWriter AppendValue<TValue>( TValue value, IFormatProvider? provider )
        where TValue : struct, ISpanFormattable => AppendValue( value, GetDefaultFormat<TValue>(), provider );
    public readonly JWriter AppendValue<TValue>( TValue value, ReadOnlySpan<char> format, IFormatProvider? provider )
        where TValue : struct, ISpanFormattable
    {
        __sb.AppendSpanFormattable( value, format, provider );
        return this;
    }
    public readonly JWriter AppendValue<TValue>( TValue value, ReadOnlySpan<char> format, int bufferSize, IFormatProvider? provider = null )
        where TValue : struct, ISpanFormattable
    {
        __sb.EnsureCapacity( bufferSize );
        __sb.AppendSpanFormattable( value, format, provider );
        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string? GetDefaultFormat<TValue>() => GetDefaultFormat( typeof(TValue) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string? GetDefaultFormat( in Type type )
    {
        if ( DefaultFormats.TryGetValue( type, out string? result ) ) { return result; }

        if ( type == typeof(short) || type == typeof(short?) || type == typeof(int) || type == typeof(int?) || type == typeof(long) || type == typeof(long?) || type == typeof(float) || type == typeof(float?) || type == typeof(double) || type == typeof(double?) || type == typeof(decimal) || type == typeof(decimal?) ) { result = "g"; }

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
