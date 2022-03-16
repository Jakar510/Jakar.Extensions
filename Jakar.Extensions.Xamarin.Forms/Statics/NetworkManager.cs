using System.Net.NetworkInformation;
using System.Net.Sockets;
using Jakar.Extensions.Xamarin.Forms.Interfaces;
using Xamarin.Essentials;





namespace Jakar.Extensions.Xamarin.Forms.Statics;


public static class NetworkManager
{
    private static readonly INetworkManager _manager = DependencyService.Resolve<INetworkManager>();

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

    public static bool CheckIpAddress( this IPAddress? address, params AddressFamily[] families )
    {
        if ( address is null ) { return false; }

        if ( address.ToString().StartsWith("169.254", StringComparison.OrdinalIgnoreCase) ) { return false; }

        return address.AddressFamily.IsOneOf(families);
    }

    public static string? GetIpAddress( params AddressFamily[] families )
    {
        IPAddress? address = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(x => x.CheckIpAddress(families));

        if ( address is not null ) { return address.ToString(); }


        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces() )
        {
            if ( netInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 && netInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet ) { continue; }

            foreach ( UnicastIPAddressInformation addressInfo in netInterface.GetIPProperties().UnicastAddresses )
            {
                if ( addressInfo.Address.CheckIpAddress(families) ) { return addressInfo.Address.ToString(); }
            }
        }

        return default;
    }


    public static bool IsConnected     => Connectivity.NetworkAccess == NetworkAccess.Internet;
    public static bool IsWiFiConnected => IsConnected && Connectivity.ConnectionProfiles.Any(p => p == ConnectionProfile.WiFi || p == ConnectionProfile.Ethernet);


    public static void ThrowIfNotConnected()
    {
        if ( IsConnected ) return;

        throw new SocketException(SocketError.NetworkDown.ToInt());
    }
}
