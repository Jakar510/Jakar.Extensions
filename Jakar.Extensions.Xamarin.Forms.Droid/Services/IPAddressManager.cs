using System;
using Android.Content;
using Android.Provider;
using Jakar.Extensions.Exceptions.General;
using Jakar.Extensions.Xamarin.Forms.Droid.Services;
using Jakar.Extensions.Xamarin.Forms.Interfaces;
using Xamarin.Forms;




[assembly: Dependency(typeof(IPAddressManager))]


namespace Jakar.Extensions.Xamarin.Forms.Droid.Services;


[global::Android.Runtime.Preserve(AllMembers = true)]
public class IPAddressManager : INetworkManager
{
    public void OpenWifiSettings()
    {
        try
        {
            using var intent = new Intent(Settings.ActionWifiSettings);
            BaseApplication.Instance.StartActivity(intent);
        }
        catch ( Exception ex ) { throw new WiFiException("Opening Wifi settings was not possible", ex); }
    }

    public string? GetIdentifier() => Settings.Secure.GetString(global::Android.App.Application.Context.ContentResolver, Settings.Secure.AndroidId);
}
