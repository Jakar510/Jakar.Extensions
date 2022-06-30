// Jakar.Extensions :: Jakar.Extensions.Wpf
// 05/17/2022  4:13 PM

#nullable enable
namespace Jakar.Extensions.Wpf;


public abstract class PageBase : Page, IChangeableAsync
{
    public ListCollectionView? Items { get; init; }


    protected PageBase() : base() { }


    public virtual Task OnAppearingAsync( CancellationToken    token ) => Task.CompletedTask;
    public virtual Task OnDisappearingAsync( CancellationToken token ) => Task.CompletedTask;
}
