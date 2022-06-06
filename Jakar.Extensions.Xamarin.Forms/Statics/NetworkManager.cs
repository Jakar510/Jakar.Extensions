using System.Net.NetworkInformation;
using System.Net.Sockets;
using Jakar.Extensions.Xamarin.Forms.Interfaces;
using Xamarin.Essentials;



namespace Jakar.Extensions.Xamarin.Forms.Statics;


public static class NetworkManager
{
    private static readonly INetworkManager _manager = DependencyService.Resolve<INetworkManager>();
    public static           bool            IsConnected     => Connectivity.NetworkAccess == NetworkAccess.Internet;
    public static           bool            IsWiFiConnected => IsConnected && Connectivity.ConnectionProfiles.Any(p => p == ConnectionProfile.WiFi || p == ConnectionProfile.Ethernet);


    public static string? GetIdentifier()
    {
        if ( _manager is null ) { throw new NullReferenceException(nameof(_manager)); }

        return _manager.GetIdentifier();
    }

    public static void OpenWifiSettings()
    {
        if ( _manager is null ) { throw new NullReferenceException(nameof(_manager)); }

        _manager.OpenWifiSettings();
    }

    public static string? GetIpAddressRange()
    {
        string? ip = GetIpAddress();

        return string.IsNullOrWhiteSpace(ip)
                   ? null
                   : ip[..( ip.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase) + 1 )];
    }


    public static string? GetIpAddress() => GetIpAddress(AddressFamily.InterNetwork, AddressFamily.InterNetworkV6);

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckIpAddress( this IPAddress? address, params AddressFamily[] families )
    {
        if ( address is null ) { return false; }

        Span<char> span = stackalloc char[50];

        if ( address.TryFormat(span, out int charsWritten) )
        {
            span = span[..charsWritten];
            if ( span.StartsWith("169.254") ) { return false; }

            if ( span.StartsWith("127.") ) { return false; }
        }

        return address.AddressFamily.IsOneOf(families);
    }

    public static string? GetIpAddress( params AddressFamily[] families ) => GetIpAddresses(families).FirstOrDefault()?.ToString();
    public static IEnumerable<IPAddress> GetIpAddresses( params AddressFamily[] families )
    {
        IEnumerable<IPAddress>? result = default;

        try { result = Dns.GetHostAddresses(Dns.GetHostName()).Where(x => x.CheckIpAddress(families)); }
        catch ( SocketException ) { }


        result ??= from netInterface in NetworkInterface.GetAllNetworkInterfaces()
                   where netInterface.NetworkInterfaceType is NetworkInterfaceType.Wireless80211 or NetworkInterfaceType.Ethernet
                   from addressInfo in netInterface.GetIPProperties().UnicastAddresses
                   where addressInfo.Address.CheckIpAddress(families)
                   select addressInfo.Address;

        foreach ( IPAddress value in result ) { yield return value; }
    }


    public static void ThrowIfNotConnected()
    {
        if ( IsConnected ) { return; }

        throw new SocketException(SocketError.NetworkDown.AsInt());
    }
}
