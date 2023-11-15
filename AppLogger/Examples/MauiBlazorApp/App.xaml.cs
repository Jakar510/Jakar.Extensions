using Microsoft.Maui.Controls;



namespace MauiBlazorApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
}
