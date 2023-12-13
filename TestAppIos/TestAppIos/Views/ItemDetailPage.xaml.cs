using System.ComponentModel;

using TestAppIos.ViewModels;

using Xamarin.Forms;

namespace TestAppIos.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}