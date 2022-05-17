// Jakar.Extensions :: Jakar.Extensions.Wpf
// 05/17/2022  4:11 PM

namespace Jakar.Extensions.Wpf;


public class WindowBase : Window, IChangeable, IChangeableAsync
{
    public WindowBase() { }
    public WindowBase( Window owner, WindowStartupLocation startup )
    {
        Owner                 = owner;
        WindowStartupLocation = startup;
    }


    public virtual void OnAppearing() { }
    public virtual void OnDisappearing() { }
    public virtual Task OnAppearingAsync( CancellationToken    token ) => Task.CompletedTask;
    public virtual Task OnDisappearingAsync( CancellationToken token ) => Task.CompletedTask;
}