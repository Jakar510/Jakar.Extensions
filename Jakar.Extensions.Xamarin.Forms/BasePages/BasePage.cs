#nullable enable
using Org.BouncyCastle.Crypto;



namespace Jakar.Extensions.Xamarin.Forms;


public abstract class BasePage : OrientationContentPage
{
    protected BasePage() : base() { }
}



public class BasePage<TViewModel> : BasePage where TViewModel : BaseViewModel
{
    private TViewModel? _viewModel;

    public TViewModel ViewModel
    {
        get => _viewModel ?? throw new NullReferenceException( nameof(_viewModel) );
        set
        {
            _viewModel     = value;
            BindingContext = value;
        }
    }


    public BasePage() : base() => Shell.SetNavBarIsVisible( this, false );


    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.OnAppearing();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.OnDisappearing();
    }
}



public class BasePage<TView, TViewModel> : BasePage<TViewModel> where TView : AppView
                                                                where TViewModel : ViewModelBase<TView>
{
    private TView? _view;


    public new virtual TView Content
    {
        get => _view ?? throw new NullReferenceException( nameof(_view) );
        set
        {
            _view        = value;
            base.Content = value;
        }
    }


    public BasePage() : base() { }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        Content.OnAppearing();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Content.OnDisappearing();
    }
}
