// Jakar.Extensions :: Jakar.Extensions
// 07/01/2022  12:12 PM

namespace Jakar.Extensions;


public abstract class BaseViewModel : ObservableClass
{
    private bool    _isBusy;
    private string? _title = string.Empty;


    public virtual bool IsBusy
    {
        get => _isBusy;
        set
        {
            if ( SetProperty( ref _isBusy, value ) ) { OnPropertyChanged( nameof(IsNotBusy) ); }
        }
    }

    public bool IsNotBusy => !IsBusy;


    public string? Title
    {
        get => _title;
        set => SetProperty( ref _title, value );
    }


    protected BaseViewModel() { }
    protected BaseViewModel( string title ) => Title = title;


    public virtual void OnAppearing() { }
    public virtual void OnDisappearing() { }
}
