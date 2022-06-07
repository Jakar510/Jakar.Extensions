using System;
using Jakar.Extensions.Xamarin.Forms.Droid.Services;
using Jakar.Extensions.Xamarin.Forms.Interfaces;




[assembly: Xamarin.Forms.Dependency(typeof(BaseUrlAndroid))]


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.Droid.Services;


[global::Android.Runtime.Preserve(AllMembers = true)]
public class BaseUrlAndroid : IBaseUrl
{
    public BaseUrlAndroid() { }
    public string GetBaseString() => "file:///android_asset/";

    public Uri GetUri() => new(GetBaseString());
}
