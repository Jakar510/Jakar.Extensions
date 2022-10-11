using System;
using Android.Runtime;
using Jakar.Extensions.Xamarin.Forms.Droid;
using Xamarin.Forms;


[assembly: Dependency( typeof(BaseUrlAndroid) )]


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.Droid;


[Preserve( AllMembers = true )]
public class BaseUrlAndroid : IBaseUrl
{
    public BaseUrlAndroid() { }
    public string GetBaseString() => "file:///android_asset/";

    public Uri GetUri() => new(GetBaseString());
}
