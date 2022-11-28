using System;
using System.Diagnostics;
using Xamarin.Forms;
using XamarinForms.Models;



namespace XamarinForms.ViewModels
{
    [QueryProperty( nameof(ItemId), nameof(ItemId) )]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string description;
        private string itemId;
        private string text;

        public string Description
        {
            get => description;
            set => SetProperty( ref description, value );
        }
        public string Id { get; set; }

        public string ItemId
        {
            get => itemId;
            set
            {
                itemId = value;
                LoadItemId( value );
            }
        }

        public string Text
        {
            get => text;
            set => SetProperty( ref text, value );
        }

        public async void LoadItemId( string itemId )
        {
            try
            {
                Item item = await DataStore.GetItemAsync( itemId );
                Id          = item.Id;
                Text        = item.Text;
                Description = item.Description;
            }
            catch (Exception) { Debug.WriteLine( "Failed to Load Item" ); }
        }
    }
}
