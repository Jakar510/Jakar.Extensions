// Jakar.Extensions :: Jakar.Extensions.Wpf
// 05/17/2022  4:02 PM

using System.Windows.Controls.Primitives;



#nullable enable
namespace Jakar.Extensions.Wpf;


public abstract class PopupBase : Popup, IChangeable
{
    public new bool IsOpen
    {
        get => base.IsOpen;
        set
        {
            base.IsOpen = value;

            if ( value ) { OnAppearing(); }
            else { OnDisappearing(); }
        }
    }


    public virtual void OnAppearing() { }
    public virtual void OnDisappearing() { }
}