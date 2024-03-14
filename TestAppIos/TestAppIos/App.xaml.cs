using System;
using Jakar.Extensions;
using TestAppIos.Services;
using Xamarin.Forms;



namespace TestAppIos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Console.WriteLine( new AppVersion( 1, 0, 0 ) );

            // Console.WriteLine(Jakar.Extensions.AppVersion.FromAssembly( typeof(App).Assembly ));

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
