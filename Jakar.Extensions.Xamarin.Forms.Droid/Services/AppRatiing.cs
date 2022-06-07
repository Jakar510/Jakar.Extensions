using System;
using Android.Content;
using Android.Content.PM;
using Jakar.Extensions.Xamarin.Forms.Droid.Services;
using Jakar.Extensions.Xamarin.Forms.Interfaces;
using Jakar.Extensions.Xamarin.Forms.Statics;
using AUri = Android.Net.Uri;




[assembly: Xamarin.Forms.Dependency(typeof(AppRating))]


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.Droid.Services;


public class AppRating : IAppRating
{
    public void RateApp()
    {
        Context context = BaseApplication.Instance;
        string  url     = $"market://details?id={AppDeviceInfo.PackageName}";

        try
        {
            context.PackageManager?.GetPackageInfo("com.android.vending", PackageInfoFlags.Activities);
            using var intent = new Intent(Intent.ActionView, AUri.Parse(url));

            context.StartActivity(intent);
        }
        catch ( PackageManager.NameNotFoundException ex )
        {
            // this won't happen. But catching just in case the user has downloaded the app without having Google Play installed.

            Console.WriteLine(ex.Message);
        }
        catch ( ActivityNotFoundException )
        {
            // if Google Play fails to load, open the App link on the browser 

            string playStoreUrl = $@"https://play.google.com/store/apps/details?id={AppDeviceInfo.PackageName}"; //Add here the url of your application on the store

            var browserIntent = new Intent(Intent.ActionView, AUri.Parse(playStoreUrl));
            browserIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ResetTaskIfNeeded);

            context.StartActivity(browserIntent);
        }
    }
}
