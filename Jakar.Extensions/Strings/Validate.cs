using System.Net.Mail;



namespace Jakar.Extensions.Strings;


/// <summary>
/// Validator Extensions
/// </summary>
public static partial class Validate
{
    private static volatile string _demo = "DEMO";

    public static string SetDemo( string value )
    {
        if ( value is null ) { throw new ArgumentNullException(nameof(value)); }

        return Interlocked.Exchange(ref _demo, value);
    }


    public static string FormatNumber( this float value, int            maxDecimals              = 4 ) => value.FormatNumber(CultureInfo.CurrentCulture, maxDecimals);
    public static string FormatNumber( this float value, in CultureInfo info, in int maxDecimals = 4 ) => Regex.Replace(string.Format(info, $"{{0:n{maxDecimals}}}", value), $"[{info.NumberFormat.NumberDecimalSeparator}]?0+$", string.Empty);


    public static string FormatNumber( this double value, int            maxDecimals              = 4 ) => value.FormatNumber(CultureInfo.CurrentCulture, maxDecimals);
    public static string FormatNumber( this double value, in CultureInfo info, in int maxDecimals = 4 ) => Regex.Replace(string.Format(info, $"{{0:n{maxDecimals}}}", value), $"[{info.NumberFormat.NumberDecimalSeparator}]?0+$", string.Empty);


    public static string FormatNumber( this decimal value, int            maxDecimals              = 4 ) => value.FormatNumber(CultureInfo.CurrentCulture, maxDecimals);
    public static string FormatNumber( this decimal value, in CultureInfo info, in int maxDecimals = 4 ) => Regex.Replace(string.Format(info, $"{{0:n{maxDecimals}}}", value), $"[{info.NumberFormat.NumberDecimalSeparator}]?0+$", string.Empty);


    public static bool IsIPv4( this string value ) => value.AsSpan().IsIPv4();
    public static bool IsIPv4( this ReadOnlySpan<char> value )
    {
        value = value.Trim();
        if ( value.IsEmpty ) { return false; }


        foreach ( ( ReadOnlySpan<char> line, ReadOnlySpan<char> separator ) in value.SplitOn('.') )
        {
            if ( !line.IsInteger() ) { return false; }
        }

        return ParseIPv4(value) != null;
    }

    public static IPAddress? ParseIPv4( this string? value ) => value?.AsSpan().ParseIPv4();
    public static IPAddress? ParseIPv4( this ReadOnlySpan<char> value ) => IPAddress.TryParse(value, out IPAddress address)
                                                                               ? address
                                                                               : null;


    public static bool IsWebAddress( this string value )
    {
        if ( string.IsNullOrWhiteSpace(value) ) { return false; }

        Uri? uriResult = ParseWebAddress(value);
        return uriResult != null && ( uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps );
    }

    public static Uri? ParseWebAddress( this string value ) => Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult)
                                                                   ? uriResult
                                                                   : null;


    public static bool IsEmailAddress( this string value ) => ValidateEmail(value) && IsValidEmail(value);

    public static bool ValidateEmail( this string value )
    {
        if ( string.IsNullOrWhiteSpace(value) ) { return false; }

        return value.Contains('@') && value.Contains('.') && !value.Contains(',') && !value.Contains('~');
    }

    private static bool IsValidEmail( this string value ) => Re.Email.IsMatch(value) && ParseEmail(value) is not null;

    public static MailAddress? ParseEmail( this string value )
    {
        try { return new MailAddress(value); }
        catch ( FormatException ) { return null; }
    }


    public static bool IsValidPort( this string             value ) => int.TryParse(value, out int n) && n.IsValidPort();
    public static bool IsValidPort( this ReadOnlySpan<char> value ) => int.TryParse(value, out int n) && n.IsValidPort();
    public static bool IsValidPort( this int                value ) => value is > 0 and <= IPEndPoint.MaxPort;

    public static bool IsDouble( this  string             value ) => double.TryParse(value, out double _);
    public static bool IsDouble( this  ReadOnlySpan<char> value ) => double.TryParse(value, out double _);
    public static bool IsInteger( this string             value ) => int.TryParse(value, out int _);
    public static bool IsInteger( this ReadOnlySpan<char> value ) => int.TryParse(value, out int _);

    public static bool IsDemo( this string value ) => value.IsDemo(CultureInfo.CurrentCulture);
    public static bool IsDemo( this string value, in CultureInfo info )
    {
        if ( string.IsNullOrWhiteSpace(value) ) { return false; }

        return value == _demo || value.ToLower(info) == "demo";
    }
    public static bool IsDemo( this ReadOnlySpan<char> value ) => value.IsDemo(CultureInfo.CurrentCulture);
    public static bool IsDemo( this ReadOnlySpan<char> value, in CultureInfo info )
    {
        if ( value.IsEmpty ) { return false; }

        var temp = new Span<char>();
        value.ToLower(temp, info);
        ReadOnlySpan<char> defaultDemo = "demo";
        ReadOnlySpan<char> globalDemo  = _demo;

        return temp == defaultDemo || value == globalDemo;
    }


    public static T ThrowIfNull<T>( T? value, string name, [CallerMemberName] string? caller = default ) =>
        value ?? throw new ArgumentNullException(name, caller); // TODO: CallerArgumentExpression : https://stackoverflow.com/questions/70034586/how-can-i-use-callerargumentexpression-with-visual-studio-2022-and-net-standard
    public static string ThrowIfNull( string? value, string name, [CallerMemberName] string? caller = default ) => string.IsNullOrWhiteSpace(value)
                                                                                                                       ? throw new ArgumentNullException(name, caller)
                                                                                                                       : value;
}
