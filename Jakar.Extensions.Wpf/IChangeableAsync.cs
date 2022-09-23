#nullable enable
namespace Jakar.Extensions.Wpf;


public interface IChangeableAsync
{
    public Task OnAppearingAsync( CancellationToken    token );
    public Task OnDisappearingAsync( CancellationToken token );
}
