// Jakar.Extensions :: Jakar.AppLogger.Common
// 08/24/2023  9:54 PM

namespace Jakar.AppLogger.Common;


/// <summary> LogValues to enable formatting options supported by <see cref="string.Format(IFormatProvider, string, object?)"/>. This also enables using {NamedformatItem} in the format string. </summary>
internal readonly record struct FormattedLogValues : IReadOnlyList<KeyValuePair<string, object?>>
{
    private static          int                                              _count;
    private static readonly ConcurrentDictionary<string, LogValuesFormatter> _formatters           = new();
    internal const          int                                              MAX_CACHED_FORMATTERS = 1024;
    private const           string                                           NULL_FORMAT           = "[null]";
    private readonly        LogValuesFormatter?                              _formatter;
    private readonly        object?[]?                                       _values;
    private readonly        string                                           _originalMessage;

    public int Count => _formatter is null
                            ? 1
                            : _formatter?.ValueNames.Count + 1 ?? 0;

    public KeyValuePair<string, object?> this[ int index ]
    {
        get
        {
            if ( index < 0 || index >= Count ) { throw new IndexOutOfRangeException( nameof(index) ); }

            return index == Count - 1
                       ? new KeyValuePair<string, object?>( "{OriginalFormat}", _originalMessage )
                       : _formatter?.GetValue( _values!, index ) ?? default;
        }
    }

    public FormattedLogValues( string? format, params object?[]? values )
    {
        _formatter = values != null && values.Length != 0 && format != null
                         ? _count >= MAX_CACHED_FORMATTERS
                               ? _formatters.TryGetValue( format, out LogValuesFormatter result )
                                     ? result
                                     : new LogValuesFormatter( format )
                               : _formatters.GetOrAdd( format, ValueFactory )
                         : null;

        _originalMessage = format ?? NULL_FORMAT;
        _values          = values;
        return;

        static LogValuesFormatter ValueFactory( string value )
        {
            Interlocked.Increment( ref _count );
            return new LogValuesFormatter( value );
        }
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        for ( int i = 0; i < Count; ++i ) { yield return this[i]; }
    }

    public override string ToString()
    {
        if ( _formatter == null ) { return _originalMessage; }

        return _formatter?.Format( _values ) ?? string.Empty;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}



/// <summary> Formatter to convert the named format items like {NamedFormatItem} to <see cref="string.Format(IFormatProvider, string, object)"/> format. </summary>
public readonly record struct LogValuesFormatter
{
    private static readonly char[] _formatDelimiters =
    {
        ',',
        ':'
    };
    private const string NULL_VALUE = "(null)";


    private readonly string       _format;
    public           string       OriginalFormat { get; }
    public           List<string> ValueNames     { get; } = new();


    public LogValuesFormatter( ReadOnlySpan<char> format )
    {
        OriginalFormat = format.ToString();

        using var vsb       = new ValueStringBuilder( 256 );
        int       scanIndex = 0;
        int       endIndex  = format.Length;

        while ( scanIndex < endIndex )
        {
            int openBraceIndex = FindBraceIndex( format, '{', scanIndex, endIndex );

            if ( scanIndex == 0 && openBraceIndex == endIndex )
            {
                // No holes found.
                _format = format.ToString();
                return;
            }

            int closeBraceIndex = FindBraceIndex( format, '}', openBraceIndex, endIndex );

            if ( closeBraceIndex == endIndex )
            {
                vsb.Append( format[scanIndex..endIndex] );
                scanIndex = endIndex;
            }
            else
            {
                // Format item syntax : { index[,alignment][ :formatString] }.
                int formatDelimiterIndex = format[openBraceIndex..closeBraceIndex].IndexOfAny( _formatDelimiters );

                if ( formatDelimiterIndex < 0 ) { formatDelimiterIndex = closeBraceIndex; }

                vsb.Append( format.Slice( scanIndex, openBraceIndex - scanIndex + 1 ) );
                vsb.Append( ValueNames.Count.ToString() );

                ValueNames.Add( format.Slice( openBraceIndex + 1, formatDelimiterIndex - openBraceIndex - 1 ).ToString() );

                vsb.Append( format.Slice( formatDelimiterIndex, closeBraceIndex - formatDelimiterIndex + 1 ) );

                scanIndex = closeBraceIndex + 1;
            }
        }

        _format = vsb.ToString();
    }


    private static int FindBraceIndex( ReadOnlySpan<char> format, in char brace, in int startIndex, in int endIndex )
    {
        // Example: {{prefix{{{Argument}}}suffix}}.
        int braceIndex           = endIndex;
        int scanIndex            = startIndex;
        int braceOccurrenceCount = 0;

        while ( scanIndex < endIndex )
        {
            if ( braceOccurrenceCount > 0 && format[scanIndex] != brace )
            {
                if ( braceOccurrenceCount % 2 == 0 )
                {
                    // Even number of '{' or '}' found. Proceed search with next occurrence of '{' or '}'.
                    braceOccurrenceCount = 0;
                    braceIndex           = endIndex;
                }
                else
                {
                    // An unescaped '{' or '}' found.
                    break;
                }
            }
            else if ( format[scanIndex] == brace )
            {
                if ( brace == '}' )
                {
                    if ( braceOccurrenceCount == 0 )
                    {
                        // For '}' pick the first occurrence.
                        braceIndex = scanIndex;
                    }
                }
                else
                {
                    // For '{' pick the last occurrence.
                    braceIndex = scanIndex;
                }

                braceOccurrenceCount++;
            }

            scanIndex++;
        }

        return braceIndex;
    }


    public string Format( object?[]? values )
    {
        object?[]? formattedValues = values;

        if ( values != null )
        {
            for ( int i = 0; i < values.Length; i++ )
            {
                object formattedValue = FormatArgument( values[i] );

                // If the formatted value is changed, we allocate and copy items to a new array to avoid mutating the array passed in to this method
                if ( !ReferenceEquals( formattedValue, values[i] ) )
                {
                    formattedValues = new object[values.Length];
                    Array.Copy( values, formattedValues, i );
                    formattedValues[i++] = formattedValue;
                    for ( ; i < values.Length; i++ ) { formattedValues[i] = FormatArgument( values[i] ); }

                    break;
                }
            }
        }

        return string.Format( CultureInfo.InvariantCulture, _format, formattedValues ?? Array.Empty<object>() );
    }

    // NOTE: This method mutates the items in the array if needed to avoid extra allocations, and should only be used when caller expects this to happen
    internal string FormatWithOverwrite( object?[]? values )
    {
        if ( values != null )
        {
            for ( int i = 0; i < values.Length; i++ ) { values[i] = FormatArgument( values[i] ); }
        }

        return string.Format( CultureInfo.InvariantCulture, _format, values ?? Array.Empty<object>() );
    }

    internal string Format() => _format;

    internal string Format( object? arg0 ) => string.Format( CultureInfo.InvariantCulture, _format, FormatArgument( arg0 ) );

    internal string Format( object? arg0, object? arg1 ) => string.Format( CultureInfo.InvariantCulture, _format, FormatArgument( arg0 ), FormatArgument( arg1 ) );

    internal string Format( object? arg0, object? arg1, object? arg2 ) => string.Format( CultureInfo.InvariantCulture, _format, FormatArgument( arg0 ), FormatArgument( arg1 ), FormatArgument( arg2 ) );

    public KeyValuePair<string, object?> GetValue( object?[] values, int index )
    {
        if ( index < 0 || index > ValueNames.Count ) { throw new IndexOutOfRangeException( nameof(index) ); }

        if ( ValueNames.Count > index ) { return new KeyValuePair<string, object?>( ValueNames[index], values[index] ); }

        return new KeyValuePair<string, object?>( "{OriginalFormat}", OriginalFormat );
    }

    public IEnumerable<KeyValuePair<string, object?>> GetValues( object[] values )
    {
        var valueArray = new KeyValuePair<string, object?>[values.Length + 1];
        for ( int index = 0; index != ValueNames.Count; ++index ) { valueArray[index] = new KeyValuePair<string, object?>( ValueNames[index], values[index] ); }

        valueArray[valueArray.Length - 1] = new KeyValuePair<string, object?>( "{OriginalFormat}", OriginalFormat );
        return valueArray;
    }

    private object FormatArgument( object? value )
    {
        if ( value is null ) { return NULL_VALUE; }

        // since 'string' implements IEnumerable, special case it
        if ( value is string ) { return value; }

        // if the value implements IEnumerable, build a comma separated string.
        if ( value is not IEnumerable enumerable ) { return value; }

        using var vsb   = new ValueStringBuilder( 256 );
        bool      first = true;

        foreach ( object? e in enumerable )
        {
            if ( !first ) { vsb.Append( ", " ); }

            vsb.Append( e != null
                            ? e.ToString()
                            : NULL_VALUE );

            first = false;
        }

        return vsb.ToString();
    }
}
