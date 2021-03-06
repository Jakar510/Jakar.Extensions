using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Foundation;
using Jakar.Extensions.Exceptions.General;
using Jakar.Extensions.Xamarin.Forms.Interfaces;
using Jakar.Extensions.Xamarin.Forms.iOS.Services;
using UIKit;
using Xamarin.Forms;




[assembly: Dependency(typeof(IpAddressManager))]


namespace Jakar.Extensions.Xamarin.Forms.iOS.Services;


public class IpAddressManager : INetworkManager
{
    public string GetIdentifier() => UIDevice.CurrentDevice.IdentifierForVendor.ToString();

    public void OpenWifiSettings()
    {
        try
        {
            using var wifiUrl = new NSUrl(@"prefs:root=WIFI");

            if ( UIApplication.SharedApplication.CanOpenUrl(wifiUrl) )
            {
                // Pre iOS 10
                UIApplication.SharedApplication.OpenUrl(wifiUrl);
            }
            else
            {
                // iOS 10
                using var nSUrl = new NSUrl(@"App-Prefs:root=WIFI");
                UIApplication.SharedApplication.OpenUrl(nSUrl);
            }
        }
        catch ( Exception ex ) { throw new WiFiException("Could not open Wifi Settings", ex); }
    }


    // public string? GetIPAddress()
    // {
    // 	string ipAddress = "";
    //
    // 	foreach ( var netInterface in NetworkInterface.GetAllNetworkInterfaces() )
    // 	{
    // 		if ( netInterface.NetworkInterfaceType is NetworkInterfaceType.Wireless80211 or NetworkInterfaceType.Ethernet )
    // 		{
    // 			foreach ( UnicastIPAddressInformation addressInfo in netInterface.GetIPProperties().UnicastAddresses )
    // 			{
    // 				if ( addressInfo.Address.AddressFamily == AddressFamily.InterNetwork ) { ipAddress = addressInfo.Address.ToString(); }
    // 			}
    // 		}
    // 	}
    //
    // 	return ipAddress;
    // }
}
