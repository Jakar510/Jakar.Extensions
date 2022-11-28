using Xamarin.Forms;
using XamarinForms.Models;
using XamarinForms.ViewModels;



namespace XamarinForms.Views
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
