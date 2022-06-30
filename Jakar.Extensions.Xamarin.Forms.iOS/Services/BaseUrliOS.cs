using System;
using Foundation;
using Jakar.Extensions.Xamarin.Forms.iOS;


[assembly: Xamarin.Forms.Dependency(typeof(BaseUrlIos))]


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.iOS;


public class BaseUrlIos : IBaseUrl
{
    public BaseUrlIos() { }

    public string GetBaseString() => NSBundle.MainBundle.BundlePath;

    public Uri GetUri() => new(GetBaseString());
}
