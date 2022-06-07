


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.BasePages;


public abstract class BaseContentPage : OrientationContentPage
{
    protected BaseContentPage() : base() { }
}



public abstract class BaseContentPage<TViewModel> : OrientationContentPage where TViewModel : BaseViewModel<BaseContentPage>, new()
{
    public TViewModel ViewModel { get; protected set; }


    protected BaseContentPage() : base() => ViewModel = InstanceCreator<TViewModel>.Create(this);
}
