using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinForms.ViewModels;



namespace XamarinForms.Views
{
    [XamlCompilation( XamlCompilationOptions.Compile )]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }
    }
}
