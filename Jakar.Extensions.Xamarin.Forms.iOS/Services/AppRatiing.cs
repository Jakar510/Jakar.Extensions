using System;
using Foundation;
using Jakar.Extensions.Xamarin.Forms.iOS;
using StoreKit;
using UIKit;
using Xamarin.Forms;


[assembly: Dependency( typeof(AppRating) )]


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.iOS;


public class AppRating : IAppRating
{
    public void RateApp()
    {
        if ( UIDevice.CurrentDevice.CheckSystemVersion( 10, 3 ) ) { SKStoreReviewController.RequestReview(); }
        else
        {
            string storeUrl = $@"itms-apps://itunes.apple.com/app/{AppDeviceInfo.PackageName}?action=write-review";

            try
            {
                using var uri = new NSUrl( storeUrl );
                UIApplication.SharedApplication.OpenUrl( uri );
            }
            catch ( Exception ex )
            {
                // Here you could show an alert to the user telling that App Store was unable to launch

                Console.WriteLine( ex.Message );
            }
        }
    }
}
