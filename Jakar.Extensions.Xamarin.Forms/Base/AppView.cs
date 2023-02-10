// Jakar.Extensions :: Jakar.Extensions.Xamarin.Forms
// 01/21/2023  2:01 PM

namespace Jakar.Extensions.Xamarin.Forms;


public class AppView : ContentView
{
    public BaseContentPage Page { get; }


    public AppView( BaseContentPage page ) => Page = page;


    public virtual void OnAppearing() { }
    public virtual void OnDisappearing() { }
}



public class AppView<TViewModel> : AppView where TViewModel : BaseViewModel
{
    public new BaseContentPage<TViewModel> Page      { get; }
    public     TViewModel                  ViewModel => Page.ViewModel;


    public AppView( BaseContentPage<TViewModel> page ) : base( page ) => Page = page;


    public override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.OnAppearing();
    }
    public override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.OnDisappearing();
    }
}
