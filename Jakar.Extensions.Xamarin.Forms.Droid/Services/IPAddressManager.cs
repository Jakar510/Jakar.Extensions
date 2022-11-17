using System;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using Jakar.Extensions.Xamarin.Forms.Droid;
using Xamarin.Forms;
using Application = Android.App.Application;


[assembly: Dependency( typeof(IPAddressManager) )]


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.Droid;


[Preserve( AllMembers = true )]
public class IPAddressManager : INetworkManager
{
    public void OpenWifiSettings()
    {
        try
        {
            using var intent = new Intent( Settings.ActionWifiSettings );
            BaseApplication.Instance.StartActivity( intent );
        }
        catch ( Exception ex ) { throw new WiFiException( "Opening Wifi settings was not possible", ex ); }
    }

    public string? GetIdentifier() => Settings.Secure.GetString( Application.Context.ContentResolver, Settings.Secure.AndroidId );
}
