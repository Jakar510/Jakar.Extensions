using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinForms.Models;
using XamarinForms.Views;



namespace XamarinForms.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Item          _selectedItem;
        public  Command       AddItemCommand   { get; }
        public  Command       LoadItemsCommand { get; }
        public  Command<Item> ItemTapped       { get; }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty( ref _selectedItem, value );
                OnItemSelected( value );
            }
        }

        public ObservableCollection<Item> Items { get; }

        public ItemsViewModel()
        {
            Title            = "Browse";
            Items            = new ObservableCollection<Item>();
            LoadItemsCommand = new Command( async () => await ExecuteLoadItemsCommand() );

            ItemTapped = new Command<Item>( OnItemSelected );

            AddItemCommand = new Command( OnAddItem );
        }

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                IEnumerable<Item> items = await DataStore.GetItemsAsync( true );
                foreach (Item item in items) { Items.Add( item ); }
            }
            catch (Exception ex) { Debug.WriteLine( ex ); }
            finally { IsBusy = false; }
        }

        public void OnAppearing()
        {
            IsBusy       = true;
            SelectedItem = null;
        }

        private async void OnAddItem( object obj ) => await Shell.Current.GoToAsync( nameof(NewItemPage) );

        private async void OnItemSelected( Item item )
        {
            if (item == null) { return; }

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync( $"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}" );
        }
    }
}
