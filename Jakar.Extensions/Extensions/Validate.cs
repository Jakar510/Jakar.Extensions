namespace Jakar.Extensions;


/// <summary> Validator Extensions </summary>
public static class Validate
{
    private static volatile string __demo = "DEMO";


    public static TValue? TryGetTarget<TValue>( this WeakReference<TValue> value )
        where TValue : class => value.TryGetTarget(out TValue? target)
                                    ? target
                                    : null;


    public static bool IsDemo( this string value, params ReadOnlySpan<string> options )
    {
        ReadOnlySpan<char> span = value.AsSpan();
        span = span.Trim();
        return span.IsDemo(options);
    }
    public static bool IsDemo( this ref readonly ReadOnlySpan<char> value, params ReadOnlySpan<string> options )
    {
        if ( value.IsEmpty ) { return false; }

        if ( value.Contains(__demo, StringComparison.OrdinalIgnoreCase) ) { return true; }

        foreach ( string option in options )
        {
            if ( value.Contains(option, StringComparison.OrdinalIgnoreCase) ) { return true; }
        }

        return false;
    }


    public static bool IsDouble( this              string             value ) => double.TryParse(value, out double _);
    public static bool IsDouble( this ref readonly ReadOnlySpan<char> value ) => double.TryParse(value, out double _);


    public static bool IsInteger( this              string             value ) => int.TryParse(value, out int _);
    public static bool IsInteger( this ref readonly ReadOnlySpan<char> value ) => int.TryParse(value, out int _);


    public static bool IsIPAddress( this string value )
    {
        ReadOnlySpan<char> span = value;
        return span.IsIPAddress();
    }
    public static bool IsIPAddress( this ref readonly ReadOnlySpan<char> value ) => value.ParseIPAddress() is not null;


    public static bool IsEmailAddress( this              string             value ) => Regexes.Email.IsMatch(value);
    public static bool IsEmailAddress( this ref readonly ReadOnlySpan<char> value ) => Regexes.Email.IsMatch(value);


    public static bool IsValidPort( this              string             value ) => int.TryParse(value, out int n) && n.IsValidPort();
    public static bool IsValidPort( this ref readonly ReadOnlySpan<char> value ) => int.TryParse(value, out int n) && n.IsValidPort();
    public static bool IsValidPort( this              int                value ) => value is > IPEndPoint.MinPort and <= IPEndPoint.MaxPort;


    public static bool IsWebAddress( this string value )
    {
        if ( string.IsNullOrWhiteSpace(value) ) { return false; }

        Uri? uriResult = value.ParseWebAddress();
        return uriResult != null && ( uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps );
    }


    public static IPAddress? ParseIPAddress( this string? value )
    {
        ReadOnlySpan<char> span = value;
        return span.ParseIPAddress();
    }
    public static IPAddress? ParseIPAddress( this ref readonly ReadOnlySpan<char> value ) => value.IsEmpty
                                                                                                 ? null
                                                                                                 : IPAddress.TryParse(value, out IPAddress? address)
                                                                                                     ? address
                                                                                                     : IPAddress.TryParse(value.Trim(), out IPAddress? address2)
                                                                                                         ? address2
                                                                                                         : null;


    public static Uri? ParseWebAddress( this string value ) => Uri.TryCreate(value, UriKind.Absolute, out Uri? uriResult)
                                                                   ? uriResult
                                                                   : null;


    public static string SetDemo( string value )
    {
        if ( value is null ) { throw new ArgumentNullException(nameof(value)); }

        return Interlocked.Exchange(ref __demo, value);
    }


    [Pure] public static TValue ThrowIfNull<TValue>( TValue? value, string? message = null, [CallerArgumentExpression(nameof(value))] string? name = null, [CallerMemberName] string? caller = null ) => value ??
                                                                                                                                                                                                         throw new ArgumentNullException(name,
                                                                                                                                                                                                                                         message is null
                                                                                                                                                                                                                                             ? caller
                                                                                                                                                                                                                                             : $"{caller}: '{message}'");


    [Pure] public static string ThrowIfNull( string? value, string? message = null, [CallerArgumentExpression(nameof(value))] string? name = null, [CallerMemberName] string? caller = null ) => string.IsNullOrWhiteSpace(value)
                                                                                                                                                                                                     ? throw new ArgumentNullException(name,
                                                                                                                                                                                                                                       message is null
                                                                                                                                                                                                                                           ? caller
                                                                                                                                                                                                                                           : $"{caller}: '{message}'")
                                                                                                                                                                                                     : value;


    [Pure] public static string? AssertLength( [NotNullIfNotNull(nameof(value))] string? value, int maxLength, [CallerArgumentExpression(nameof(value))] string? name = null, [CallerMemberName] string? caller = null )
    {
        if ( IsValidLength(value, null, maxLength) ) { throw new ArgumentOutOfRangeException(name, $"{caller}.{name}: length '{value?.Length}' exceeds maximum length of '{maxLength}'"); }

        return value;
    }
    [Pure] public static string? AssertLength( [NotNullIfNotNull(nameof(value))] string? value, int minLength, int maxLength, [CallerArgumentExpression(nameof(value))] string? name = null, [CallerMemberName] string? caller = null )
    {
        if ( IsValidLength(value, minLength, maxLength) ) { throw new ArgumentOutOfRangeException(name, $"{caller}.{name}: length '{value?.Length}' is not in the range of '{minLength}' to '{maxLength}'"); }

        return value;
    }
    [Pure] public static bool IsValidLength( ReadOnlySpan<char> span, int? minLength, int? maxLength )
    {
        if ( minLength.HasValue && maxLength.HasValue ) { return span.Length >= minLength.Value && span.Length <= maxLength.Value; }

        if ( minLength.HasValue ) { return span.Length >= minLength.Value; }

        if ( maxLength.HasValue ) { return span.Length <= maxLength.Value; }

        return true;
    }



    extension( WeakReference self )
    {
        public object? TryGetTarget() => self.Target;
        public TValue? TryGetTarget<TValue>()
            where TValue : class => self.Target as TValue;
    }



    /// <summary> Formats with <c>N{maxDecimals}</c> in <paramref name="info"/>, then drops trailing zeros after the decimal separator (and the separator if nothing is left). </summary>
    /// <remarks>
    ///     Formats into a stack buffer and trims in place instead of running <see cref="string.Format(IFormatProvider, string, object)"/> (which boxed the value) and an uncached <see cref="Regex.Replace(string, string, string)"/>.
    ///     Only zeros after the decimal separator are removed: the previous pattern also stripped trailing zeros of the integer part when there were no decimals (e.g. 100 with <c>maxDecimals: 0</c> became "1").
    /// </remarks>
    private static string FormatNumberCore<TNumber>( TNumber value, CultureInfo info, int maxDecimals )
        where TNumber : ISpanFormattable
    {
        Span<char> format = stackalloc char[12];
        format[0] = 'n'; // same as the previous "{0:n...}" (identical output, including for invalid precisions)
        maxDecimals.TryFormat(format[1..], out int digits, default, CultureInfo.InvariantCulture);
        format = format[..( digits + 1 )];

        Span<char> buffer = stackalloc char[128];

        if ( value.TryFormat(buffer, out int written, format, info) )
        {
            ReadOnlySpan<char> text = buffer[..written];
            return new string(text[..TrimTrailingDecimalZeros(text, info.NumberFormat.NumberDecimalSeparator, maxDecimals)]);
        }

        // Very large values (e.g. double.MaxValue with group separators) don't fit the stack buffer.
        string result = value.ToString(format.ToString(), info);
        int    length = TrimTrailingDecimalZeros(result, info.NumberFormat.NumberDecimalSeparator, maxDecimals);

        return length == result.Length
                   ? result
                   : result[..length];
    }
    private static int TrimTrailingDecimalZeros( ReadOnlySpan<char> text, string separator, int maxDecimals )
    {
        if ( maxDecimals <= 0 ) { return text.Length; }

        int decimalPoint = text.LastIndexOf(separator);
        if ( decimalPoint < 0 ) { return text.Length; } // NaN, infinity

        int start = decimalPoint + separator.Length;
        int end   = text.Length;
        while ( end > start && text[end - 1] == '0' ) { end--; }

        return end == start
                   ? decimalPoint
                   : end;
    }



    extension( float self )
    {
        public string FormatNumber( int         maxDecimals           = 4 ) => self.FormatNumber(CultureInfo.CurrentCulture, maxDecimals);
        public string FormatNumber( CultureInfo info, int maxDecimals = 4 ) => FormatNumberCore(self, info, maxDecimals);
    }



    extension( double self )
    {
        public string FormatNumber( int         maxDecimals           = 4 ) => self.FormatNumber(CultureInfo.CurrentCulture, maxDecimals);
        public string FormatNumber( CultureInfo info, int maxDecimals = 4 ) => FormatNumberCore(self, info, maxDecimals);
    }



    extension( decimal self )
    {
        public string FormatNumber( int         maxDecimals           = 4 ) => self.FormatNumber(CultureInfo.CurrentCulture, maxDecimals);
        public string FormatNumber( CultureInfo info, int maxDecimals = 4 ) => FormatNumberCore(self, info, maxDecimals);
    }



    extension<TValue>( [NotNullIfNotNull("self")] TValue? self )
        where TValue : struct, IComparable<TValue>
    {
        public TValue? Min( [NotNullIfNotNull("other")] TValue? other )
        {
            if ( self is null && other is null ) { return null; }

            return Nullable.Compare(self, other) == NOT_FOUND
                       ? self  ?? other
                       : other ?? self;
        }
        public TValue? Max( [NotNullIfNotNull("other")] TValue? other )
        {
            if ( self is null && other is null ) { return null; }

            return Nullable.Compare(self, other) == 1
                       ? self  ?? other
                       : other ?? self;
        }
    }
}
