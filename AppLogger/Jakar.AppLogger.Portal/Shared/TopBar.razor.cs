namespace Jakar.AppLogger.Portal.Shared;


public partial class TopBar : ControlBase
{
    private readonly  ObservableCollection<Notification> _notifications = new();
    private           string?                            _notifyCount;
    [ Inject ] public ILogger<TopBar>                    Logger { get; set; } = default!;


    protected override void OnInitialized()
    {
        base.OnInitialized();
        _notifyCount             =  _notifications.Count.ToString();
        Api.NotificationReceived += ApiOnNotificationReceived;
    }
    ~TopBar() => Api.NotificationReceived -= ApiOnNotificationReceived;


    private void Callback( MouseEventArgs args ) => Services.NavigateTo( "/Notifications" );


    private async void ApiOnNotificationReceived( object? sender, Notification notification )
    {
        try { await ApiOnNotificationReceivedAsync( notification ); }
        catch ( Exception e ) { Logger.LogError( e, "{Caller}", nameof(ApiOnNotificationReceived) ); }
    }
    private async ValueTask ApiOnNotificationReceivedAsync( Notification notification )
    {
        _notifyCount = _notifications.Count.ToString();
        _notifications.Add( notification );
        await StateHasChangedAsync();
    }
}
