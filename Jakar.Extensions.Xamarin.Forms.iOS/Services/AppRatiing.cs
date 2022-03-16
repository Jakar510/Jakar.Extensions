using System;
using Foundation;
using Jakar.Extensions.Xamarin.Forms.Interfaces;
using Jakar.Extensions.Xamarin.Forms.iOS.Services;
using Jakar.Extensions.Xamarin.Forms.Statics;
using StoreKit;
using UIKit;





[assembly: Xamarin.Forms.Dependency(typeof(AppRating))]


namespace Jakar.Extensions.Xamarin.Forms.iOS.Services;


public class AppRating : IAppRating
{
    public void RateApp()
    {
        if ( UIDevice.CurrentDevice.CheckSystemVersion(10, 3) ) { SKStoreReviewController.RequestReview(); }
        else
        {
            string storeUrl = $@"itms-apps://itunes.apple.com/app/{AppDeviceInfo.PackageName}?action=write-review";

            try
            {
                using var uri = new NSUrl(storeUrl);
                UIApplication.SharedApplication.OpenUrl(uri);
            }
            catch ( Exception ex )
            {
                // Here you could show an alert to the user telling that App Store was unable to launch

                Console.WriteLine(ex.Message);
            }
        }
    }
}
