using Xamarin.CommunityToolkit.UI.Views;



namespace Jakar.Extensions.Xamarin.Forms;


public abstract class BaseLoadingPopup : Popup<bool> { }



[XamlCompilation( XamlCompilationOptions.Compile )]
public sealed partial class LoadingPopup : BaseLoadingPopup, IDisposable, IAsyncDisposable
{
    private          CancellationTokenSource?      _source;
    private readonly CancellationTokenRegistration _registration;


    private LoadingPopup() : base() { }
    public LoadingPopup( CancellationToken token ) : this()
    {
        InitializeComponent();
        _registration = token.Register( Complete );
    }
    public LoadingPopup( CancellationTokenSource source ) : this()
    {
        InitializeComponent();
        _source = source;
    }


    private void Complete() => Dismiss( true );
    private void Cancel()
    {
        _source?.Cancel();
        Dismiss( false );
    }
    protected override void LightDismiss()
    {
        _source?.Cancel();
        base.LightDismiss();
    }
    private void Cancel_OnClicked( object sender, EventArgs e ) => Cancel();


    public void Dispose()
    {
        _source = default;
        _registration.Dispose();
        GC.SuppressFinalize( this );
    }
    public async ValueTask DisposeAsync()
    {
        _source = default;
        await _registration.DisposeAsync();
        GC.SuppressFinalize( this );
    }
}
