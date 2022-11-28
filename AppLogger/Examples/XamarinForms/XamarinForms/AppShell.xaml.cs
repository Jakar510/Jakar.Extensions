using System;
using Xamarin.Forms;
using XamarinForms.Views;



namespace XamarinForms
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute( nameof(ItemDetailPage), typeof(ItemDetailPage) );
            Routing.RegisterRoute( nameof(NewItemPage),    typeof(NewItemPage) );
        }

        private async void OnMenuItemClicked( object sender, EventArgs e ) => await Current.GoToAsync( "//LoginPage" );
    }
}
