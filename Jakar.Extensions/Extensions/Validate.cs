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


    public static bool IsDemo( this string value ) => value.AsSpan().IsDemo();
    public static bool IsDemo( this ReadOnlySpan<char> value )
    {
        if ( value.IsEmpty ) { return false; }

        return value.CompareTo( _demo, StringComparison.OrdinalIgnoreCase ) == 0;
    }


    public static bool IsDouble( this string             value ) => double.TryParse( value, out double _ );
    public static bool IsDouble( this ReadOnlySpan<char> value ) => double.TryParse( value, out double _ );


    public static bool IsInteger( this string             value ) => int.TryParse( value, out int _ );
    public static bool IsInteger( this ReadOnlySpan<char> value ) => int.TryParse( value, out int _ );


    public static bool IsIPAddress( this string             value ) => value.AsSpan().IsIPAddress();
    public static bool IsIPAddress( this ReadOnlySpan<char> value ) => value.ParseIPAddress() is not null;


    public static bool IsEmailAddress( this string value ) => Re.Email.IsMatch( value );
#if NET7_0_OR_GREATER
    public static bool IsEmailAddress( this ReadOnlySpan<char> value ) => Re.Email.IsMatch( value );
#endif


    public static bool IsValidPort( this string             value ) => int.TryParse( value, out int n ) && n.IsValidPort();
    public static bool IsValidPort( this ReadOnlySpan<char> value ) => int.TryParse( value, out int n ) && n.IsValidPort();
    public static bool IsValidPort( this int                value ) => value is > 0 and <= IPEndPoint.MaxPort;


    public static bool IsWebAddress( this string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) ) { return false; }

        Uri? uriResult = ParseWebAddress( value );
        return uriResult != null && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }


    public static IPAddress? ParseIPAddress( this string? value ) => value?.AsSpan().ParseIPAddress();
    public static IPAddress? ParseIPAddress( this ReadOnlySpan<char> value )
    {
        if ( value.IsEmpty ) { return default; }

        if ( IPAddress.TryParse( value, out IPAddress? address ) ) { return address; }


        Span<byte> ip = stackalloc byte[16];
        int        i  = 0;

        if ( value.Contains( ':' ) )
        {
            foreach ( ReadOnlySpan<char> line in value.SplitOn( ':' ) )
            {
                if ( i >= 16 ) { return default; }

                if ( !byte.TryParse( line, out byte n ) ) { return default; }

                ip[i++] = n;
            }
        }
        else
        {
            foreach ( ReadOnlySpan<char> line in value.SplitOn( '.' ) )
            {
                if ( i >= 16 ) { return default; }

                if ( !byte.TryParse( line, out byte n ) ) { return default; }

                ip[i++] = n;
            }
        }

        return new IPAddress( ip[..i] );
    }


    public static Uri? ParseWebAddress( this string value ) => Uri.TryCreate( value, UriKind.Absolute, out Uri? uriResult )
                                                                   ? uriResult
                                                                   : default;


    public static string SetDemo( string value )
    {
        if ( value is null ) { throw new ArgumentNullException( nameof(value) ); }

        return Interlocked.Exchange( ref _demo, value );
    }


    // TODO: CallerArgumentExpression : https://stackoverflow.com/questions/70034586/how-can-i-use-callerargumentexpression-with-visual-studio-2022-and-net-standard
    public static T ThrowIfNull<T>( T? value, [CallerArgumentExpression( "value" )] string? name = default, [CallerMemberName] string? caller = default ) => value ?? throw new ArgumentNullException( name, caller );


    public static string ThrowIfNull( string? value, [CallerArgumentExpression( "value" )] string? name = default, [CallerMemberName] string? caller = default ) => string.IsNullOrWhiteSpace( value )
                                                                                                                                                                        ? throw new ArgumentNullException( name, caller )
                                                                                                                                                                        : value;
}
