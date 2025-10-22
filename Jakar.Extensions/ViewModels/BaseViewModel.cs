// Jakar.Extensions :: Jakar.Extensions
// 07/01/2022  12:12 PM

namespace Jakar.Extensions;


public abstract class BaseViewModel : BaseClass, IChangeable
{
    private bool    __isBusy;
    private string? __title = EMPTY;


    public virtual bool IsBusy
    {
        get => __isBusy;
        set
        {
            if ( SetProperty(ref __isBusy, value) ) { OnPropertyChanged(nameof(IsNotBusy)); }
        }
    }

    public bool IsNotBusy => !IsBusy;


    public string? Title { get => __title; set => SetProperty(ref __title, value); }


    protected BaseViewModel() { }
    protected BaseViewModel( string? title ) => Title = title;


    public virtual void OnAppearing()    { }
    public virtual void OnDisappearing() { }
}
