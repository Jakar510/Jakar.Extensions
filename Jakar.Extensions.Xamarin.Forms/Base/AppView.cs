// Jakar.Extensions :: Jakar.Extensions.Xamarin.Forms
// 01/21/2023  2:01 PM

namespace Jakar.Extensions.Xamarin.Forms;


public class AppView : ContentView
{
    public BasePage Page { get; }


    public AppView( BasePage page ) => Page = page;


    public virtual void OnAppearing() { }
    public virtual void OnDisappearing() { }
}



public class AppView<TViewModel> : AppView where TViewModel : BaseViewModel
{
    public new BasePage<TViewModel> Page      { get; }
    public     TViewModel           ViewModel => Page.ViewModel;


    public AppView( BasePage<TViewModel> page ) : base( page ) => Page = page;
}
