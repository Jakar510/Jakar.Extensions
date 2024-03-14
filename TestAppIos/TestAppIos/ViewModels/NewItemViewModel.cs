using System;
using TestAppIos.Models;
using Xamarin.Forms;



namespace TestAppIos.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string  description;
        private string  text;
        public  Command CancelCommand { get; }

        public string Description { get => description; set => SetProperty( ref description, value ); }

        public Command SaveCommand { get; }

        public string Text { get => text; set => SetProperty( ref text, value ); }

        public NewItemViewModel()
        {
            SaveCommand     =  new Command( OnSave, ValidateSave );
            CancelCommand   =  new Command( OnCancel );
            PropertyChanged += ( _, __ ) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave() =>
            !String.IsNullOrWhiteSpace( text ) && !String.IsNullOrWhiteSpace( description );

        private async void OnCancel() =>

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync( ".." );

        private async void OnSave()
        {
            var newItem = new Item
                          {
                              Id          = Guid.NewGuid().ToString(),
                              Text        = Text,
                              Description = Description
                          };

            await DataStore.AddItemAsync( newItem );

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync( ".." );
        }
    }
}
