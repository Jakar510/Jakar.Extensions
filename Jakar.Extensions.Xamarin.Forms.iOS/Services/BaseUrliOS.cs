using System;
using Foundation;
using Jakar.Extensions.Xamarin.Forms.Interfaces;
using Jakar.Extensions.Xamarin.Forms.iOS.Services;





[assembly: Xamarin.Forms.Dependency(typeof(BaseUrlIos))]


namespace Jakar.Extensions.Xamarin.Forms.iOS.Services;


public class BaseUrlIos : IBaseUrl
{
    public BaseUrlIos() { }

    public string GetBaseString() => NSBundle.MainBundle.BundlePath;

    public Uri GetUri() => new(GetBaseString());
}
