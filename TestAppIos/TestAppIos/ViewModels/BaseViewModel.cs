using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TestAppIos.Models;
using TestAppIos.Services;
using Xamarin.Forms;



namespace TestAppIos.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool isBusy;

        private string           title = string.Empty;
        public  IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();
        public  bool             IsBusy    { get => isBusy; set => SetProperty( ref isBusy, value ); }
        public  string           Title     { get => title;  set => SetProperty( ref title,  value ); }

        protected bool SetProperty<T>( ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null )
        {
            if ( EqualityComparer<T>.Default.Equals( backingStore, value ) ) { return false; }

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged( propertyName );
            return true;
        }



        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged( [CallerMemberName] string propertyName = "" )
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if ( changed == null ) { return; }

            changed.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        #endregion
    }
}
