// Jakar.Extensions :: Jakar.Extensions
// 07/01/2022  12:12 PM

namespace Jakar.Extensions;


public abstract class BaseViewModel : BaseClass, IChangeable
{
    public virtual bool IsBusy
    {
        get;
        set
        {
            if ( SetProperty(ref field, value) ) { OnPropertyChanged(nameof(IsNotBusy)); }
        }
    }

    public bool IsNotBusy => !IsBusy;


    public string? Title { get; set => SetProperty(ref field, value); } = EMPTY;


    protected BaseViewModel() { }
    protected BaseViewModel( string? title ) => Title = title;


    public virtual void OnAppearing()    { }
    public virtual void OnDisappearing() { }
}
