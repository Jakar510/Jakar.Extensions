﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Platform = Xamarin.Essentials.Platform;



namespace XamarinForms.Droid
{
    [Activity( Label = "XamarinForms",
               Icon = "@mipmap/icon",
               Theme = "@style/MainTheme",
               MainLauncher = true,
               ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate( Bundle savedInstanceState )
        {
            base.OnCreate( savedInstanceState );

            Platform.Init( this, savedInstanceState );
            Forms.Init( this, savedInstanceState );
            LoadApplication( new App() );
        }
        public override void OnRequestPermissionsResult( int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults )
        {
            Platform.OnRequestPermissionsResult( requestCode, permissions, grantResults );

            base.OnRequestPermissionsResult( requestCode, permissions, grantResults );
        }
    }
}
