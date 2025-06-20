namespace Jakar.Extensions;


/// <summary> Validator Extensions </summary>
public static partial class Validate
{
    private static volatile string _demo = "DEMO";


    public static string FormatNumber( this float value, int         maxDecimals           = 4 ) => value.FormatNumber( CultureInfo.CurrentCulture, maxDecimals );
    public static string FormatNumber( this float value, CultureInfo info, int maxDecimals = 4 ) => Regex.Replace( string.Format( info, $"{{0:n{maxDecimals}}}", value ), $"[{info.NumberFormat.NumberDecimalSeparator}]?0+$", string.Empty );


    public static string FormatNumber( this double value, int         maxDecimals           = 4 ) => value.FormatNumber( CultureInfo.CurrentCulture, maxDecimals );
    public static string FormatNumber( this double value, CultureInfo info, int maxDecimals = 4 ) => Regex.Replace( string.Format( info, $"{{0:n{maxDecimals}}}", value ), $"[{info.NumberFormat.NumberDecimalSeparator}]?0+$", string.Empty );


    public static string FormatNumber( this decimal value, int         maxDecimals           = 4 ) => value.FormatNumber( CultureInfo.CurrentCulture, maxDecimals );
    public static string FormatNumber( this decimal value, CultureInfo info, int maxDecimals = 4 ) => Regex.Replace( string.Format( info, $"{{0:n{maxDecimals}}}", value ), $"[{info.NumberFormat.NumberDecimalSeparator}]?0+$", string.Empty );


    public static bool IsDemo( this string value, params ReadOnlySpan<string> options )
    {
        ReadOnlySpan<char> span = value.AsSpan();
        span = span.Trim();
        return span.IsDemo( options );
    }
    public static bool IsDemo( this ref readonly ReadOnlySpan<char> value, params ReadOnlySpan<string> options )
    {
        if ( value.IsEmpty ) { return false; }

        if ( value.Contains( _demo, StringComparison.OrdinalIgnoreCase ) ) { return true; }

        foreach ( string option in options )
        {
            if ( value.Contains( option, StringComparison.OrdinalIgnoreCase ) ) { return true; }
        }

        return false;
    }


    public static bool IsDouble( this              string             value ) => double.TryParse( value, out double _ );
    public static bool IsDouble( this ref readonly ReadOnlySpan<char> value ) => double.TryParse( value, out double _ );


    public static bool IsInteger( this              string             value ) => int.TryParse( value, out int _ );
    public static bool IsInteger( this ref readonly ReadOnlySpan<char> value ) => int.TryParse( value, out int _ );


    public static bool IsIPAddress( this string value )
    {
        ReadOnlySpan<char> span = value;
        return span.IsIPAddress();
    }
    public static bool IsIPAddress( this ref readonly ReadOnlySpan<char> value ) => value.ParseIPAddress() is not null;


    public static bool IsEmailAddress( this              string             value ) => Re.Email.IsMatch( value );
    public static bool IsEmailAddress( this ref readonly ReadOnlySpan<char> value ) => Re.Email.IsMatch( value );


    public static bool IsValidPort( this              string             value ) => int.TryParse( value, out int n ) && n.IsValidPort();
    public static bool IsValidPort( this ref readonly ReadOnlySpan<char> value ) => int.TryParse( value, out int n ) && n.IsValidPort();
    public static bool IsValidPort( this              int                value ) => value is > IPEndPoint.MinPort and <= IPEndPoint.MaxPort;


    public static bool IsWebAddress( this string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) ) { return false; }

        Uri? uriResult = ParseWebAddress( value );
        return uriResult != null && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }


    public static IPAddress? ParseIPAddress( this string? value )
    {
        ReadOnlySpan<char> span = value;
        return span.ParseIPAddress();
    }
    public static IPAddress? ParseIPAddress( this ref readonly ReadOnlySpan<char> value ) => value.IsEmpty
                                                                                                 ? null
                                                                                                 : IPAddress.TryParse( value, out IPAddress? address )
                                                                                                     ? address
                                                                                                     : IPAddress.TryParse( value.Trim(), out IPAddress? address2 )
                                                                                                         ? address2
                                                                                                         : null;


    public static Uri? ParseWebAddress( this string value ) => Uri.TryCreate( value, UriKind.Absolute, out Uri? uriResult )
                                                                   ? uriResult
                                                                   : null;


    public static string SetDemo( string value )
    {
        if ( value is null ) { throw new ArgumentNullException( nameof(value) ); }

        return Interlocked.Exchange( ref _demo, value );
    }


    [Pure]
    public static TValue ThrowIfNull<TValue>( TValue? value, string? message = null, [CallerArgumentExpression( nameof(value) )] string? name = null, [CallerMemberName] string? caller = null ) => value ??
                                                                                                                                                                                                    throw new ArgumentNullException( name,
                                                                                                                                                                                                                                     message is null
                                                                                                                                                                                                                                         ? caller
                                                                                                                                                                                                                                         : $"{caller}: '{message}'" );

    [Pure]
    public static string ThrowIfNull( string? value, string? message = null, [CallerArgumentExpression( nameof(value) )] string? name = null, [CallerMemberName] string? caller = null ) => string.IsNullOrWhiteSpace( value )
                                                                                                                                                                                                ? throw new ArgumentNullException( name,
                                                                                                                                                                                                                                   message is null
                                                                                                                                                                                                                                       ? caller
                                                                                                                                                                                                                                       : $"{caller}: '{message}'" )
                                                                                                                                                                                                : value;
}
