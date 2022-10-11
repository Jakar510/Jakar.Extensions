#nullable enable
using Acr.UserDialogs;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Platform = Xamarin.Essentials.Platform;



namespace Jakar.Extensions.Xamarin.Forms.Droid;


public abstract class BaseApplication : FormsAppCompatActivity
{
#pragma warning disable 8618
    public static BaseApplication Instance { get; protected set; }
#pragma warning restore 8618


    protected void Init( Bundle savedInstanceState, params string[] flags )
    {
        global::Xamarin.Forms.Forms.SetFlags( flags );
        global::Xamarin.Forms.Forms.Init( this, savedInstanceState );
        FormsMaterial.Init( this, savedInstanceState );
        Platform.Init( this, savedInstanceState );

        CrossCurrentActivity.Current.Init( this, savedInstanceState );
        UserDialogs.Init( this );

        Instance = this;
    }

    public override void OnRequestPermissionsResult( int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults )
    {
        Platform.OnRequestPermissionsResult( requestCode, permissions, grantResults );
        base.OnRequestPermissionsResult( requestCode, permissions, grantResults );
    }
}
