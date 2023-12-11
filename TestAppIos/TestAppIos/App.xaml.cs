using System;
using TestAppIos.Services;
using TestAppIos.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;



namespace TestAppIos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Console.WriteLine( new Jakar.Extensions.AppVersion( 1, 0, 0 ) );

            // Console.WriteLine(Jakar.Extensions.AppVersion.FromAssembly( typeof(App).Assembly ));

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
