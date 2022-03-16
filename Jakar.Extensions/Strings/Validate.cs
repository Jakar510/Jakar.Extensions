using System.Net.Mail;


namespace Jakar.Extensions.Strings;


/// <summary>
/// Validator Extensions
/// </summary>
public static class Validate
{
    public static TItem NotNull<TItem>( TItem? item ) => item ?? throw new NullReferenceException(nameof(item));


    public static string Demo { get; set; } = "DEMO";

    private static readonly Regex _emailRegex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

    public static string FormatNumber<T>( this T number, int maxDecimals = 4 ) => Regex.Replace(string.Format(CultureInfo.CurrentCulture, $"{{0:n{maxDecimals}}}", number),
                                                                                                $"[{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}]?0+$",
                                                                                                "");


    public static bool IsIPv4( this string ipString )
    {
        if ( string.IsNullOrWhiteSpace(ipString) ) { return false; }

        string[] splitValues = ipString.Trim().Split('.');
        return splitValues.Length == 4 && splitValues.All(IsInteger) && ParseIPv4(ipString) != null;
    }

    public static IPAddress? ParseIPv4( this string ipString ) => IPAddress.TryParse(ipString.Trim(), out IPAddress address)
                                                                      ? address
                                                                      : null;


    public static bool IsWebAddress( this string addressString )
    {
        if ( string.IsNullOrWhiteSpace(addressString) ) return false;

        Uri? uriResult = ParseWebAddress(addressString);
        return uriResult != null && ( uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps );
    }

    public static Uri? ParseWebAddress( this string addressString ) => Uri.TryCreate(addressString, UriKind.Absolute, out Uri uriResult)
                                                                           ? uriResult
                                                                           : null;


    public static bool IsEmailAddress( this string email ) => ValidateEmail(email) && IsValidEmail(email);

    public static bool ValidateEmail( this string email )
    {
        if ( string.IsNullOrWhiteSpace(email) ) { return false; }

        return email.Contains('@') && email.Contains('.') && !email.Contains(',') && !email.Contains('~');
    }

    private static bool IsValidEmail( this string email ) => _emailRegex.IsMatch(email) && ParseEmail(email) is not null;

    public static MailAddress? ParseEmail( this string email )
    {
        try { return new MailAddress(email); }
        catch ( FormatException ) { return null; }
    }


    public static bool IsValidPort( this string s )    => int.TryParse(s, out int port) && port.IsValidPort();
    public static bool IsValidPort( this int    port ) => port is > 0 and <= IPEndPoint.MaxPort;

    public static bool IsDouble( this  string argsNewTextValue ) => double.TryParse(argsNewTextValue, out double _);
    public static bool IsInteger( this string argsNewTextValue ) => int.TryParse(argsNewTextValue, out int _);

    public static bool IsDemo( this string item )
    {
        if ( string.IsNullOrWhiteSpace(item) ) return false;

        return item == Demo || item.ToLower(CultureInfo.CurrentCulture) == "demo";
    }


    public static T ThrowIfNull<T>( this T? arg, string name, [CallerMemberName] string caller = "" ) => arg ?? throw new ArgumentNullException(name, caller);

    public static string ThrowIfNull( this string? arg, string name, [CallerMemberName] string caller = "" ) => string.IsNullOrWhiteSpace(arg)
                                                                                                                    ? throw new ArgumentNullException(name, caller)
                                                                                                                    : arg;
}
