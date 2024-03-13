using TestAppIos.Models;
using TestAppIos.ViewModels;
using Xamarin.Forms;



namespace TestAppIos.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}
