#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.Interfaces;


/// <summary>
/// 
/// <para>
/// Ip Address <br/>
/// https://theconfuzedsourcecode.wordpress.com/2015/05/16/how-to-easily-get-device-ip-address-in-xamarin-forms-using-dependencyservice/ <br/>
/// </para>
/// <br/>
/// <para>
/// Open WiFi Settings <br/>
/// https://gist.github.com/zuckerthoben/ee097b816b88491f5874bb327ec6819c <br/>
/// </para>
/// <br/>
/// <para>
/// Xamarin Android Get Device Mac Address <br/>
/// https://gist.github.com/tomcurran/099c8e74bc094f5d285867dcea0b63d4 <br/>
/// </para>
/// <br/>
/// <para>
/// Mac Address <br/>
/// https://www.tutorialspoint.com/how-to-get-the-mac-address-of-an-ios-iphone-programmatically <br/>
/// https://stackoverflow.com/questions/50232847/how-to-get-device-id-in-xamarin-forms <br/>
/// </para>
/// <br/>
/// <para>
/// connect to WiFi <br/>
/// https://spin.atomicobject.com/2018/02/15/connecting-wifi-xamarin-forms/ <br/>
/// https://c-sharx.net/programmatically-connecting-your-xamarin-ios-app-to-a-wifi-network-using-nehotspotconfigurationmanager <br/>
/// https://github.com/Krumelur/WifiDemo <br/>
/// https://stackoverflow.com/questions/8818290/how-do-i-connect-to-a-specific-wi-fi-network-in-android-programmatically <br/>
/// https://gist.github.com/kraigspear/2c3de568cc7ae3c5c360bcac7e9db92a <br/>
/// </para>
/// 
/// </summary>
public interface INetworkManager
{
    public string? GetIdentifier();
    public void OpenWifiSettings();

    // public string? GetIPAddress();

    //public void ConnectToWifi(string ssid, string password) => ConnectToWifi(new WifiConfig(ssid, password));
    //public void ConnectToWifi(WifiConfig configuration);
}