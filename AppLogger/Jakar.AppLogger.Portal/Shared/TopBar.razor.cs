namespace Jakar.AppLogger.Portal.Shared;


public partial class TopBar : ControlBase
{
    private string? _notifyCount;


    protected override void OnInitialized()
    {
        base.OnInitialized();
        _notifyCount             =  Api.Notifications.Count.ToString();
        Api.NotificationReceived += ApiOnNotificationReceived;
    }
    ~TopBar() => Api.NotificationReceived -= ApiOnNotificationReceived;


    private void Callback( MouseEventArgs args ) => Services.NavigateTo( "/Notifications" );


    private async void ApiOnNotificationReceived( object? sender, Notification e )
    {
        _notifyCount = Api.Notifications.Count.ToString();
        await StateHasChangedAsync();
    }
}
