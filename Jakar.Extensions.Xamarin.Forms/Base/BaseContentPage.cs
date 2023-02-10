namespace Jakar.Extensions.Xamarin.Forms;


public abstract class BaseContentPage : OrientationContentPage
{
    protected BaseContentPage() : base()
    {
        NavigationPage.SetHasNavigationBar( this, false );
        Shell.SetNavBarIsVisible( this, false );
    }
}



public class BaseContentPage<TViewModel> : BaseContentPage where TViewModel : BaseViewModel
{
    private TViewModel? _viewModel;

    public TViewModel ViewModel
    {
        get => _viewModel ?? throw new NullReferenceException( nameof(_viewModel) );
        init => BindingContext = _viewModel = value;
    }


    public BaseContentPage() : base() { }


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



public class BaseContentPage<TView, TViewModel> : BaseContentPage<TViewModel> where TView : AppView<TViewModel>
                                                                              where TViewModel : ViewModelBase<TView>
{
    private TView? _view;


    public new virtual TView Content
    {
        get => _view ?? throw new NullReferenceException( nameof(_view) );
        init => base.Content = _view = value;
    }


    public BaseContentPage() : base() { }


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
