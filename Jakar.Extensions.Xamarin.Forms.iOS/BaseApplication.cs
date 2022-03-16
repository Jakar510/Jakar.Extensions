using Acr.UserDialogs.Infrastructure;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;





namespace Jakar.Extensions.Xamarin.Forms.iOS;


/// <summary>
/// The UIApplicationDelegate for the application.
/// This class is responsible for launching the User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
/// This method (FinishedLaunching) is invoked when the application has loaded and is ready to run.
/// In this method you should instantiate the window, load the UI into it and then make the window visible.
/// You have 17 seconds to return from this method, or iOS will terminate your application.
/// </summary>
public abstract class BaseApplication : FormsApplicationDelegate
{
    /// <summary>
    /// Add to the subclass: "LoadApplication(new App());"
    /// </summary>
    protected void Init( params string[] flags )
    {
        global::Xamarin.Forms.Forms.SetFlags(flags);
        global::Xamarin.Forms.Forms.Init();
        FormsMaterial.Init();

        Acr.UserDialogs.UserDialogs.Init(() => UIApplication.SharedApplication.GetTopViewController());

        // Add LoadApplication(new App()); here
    }
}
