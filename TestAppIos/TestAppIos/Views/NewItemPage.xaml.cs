using System;
using System.Collections.Generic;
using System.ComponentModel;

using TestAppIos.Models;
using TestAppIos.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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