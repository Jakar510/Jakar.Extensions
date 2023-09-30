namespace Jakar.AppLogger.Portal.Pages;


public partial class Logs : ControlBase
{
    public ConcurrentObservableCollection<LogRecord> Records { get; } = new();


    protected override async Task OnParametersSetAsync()
    {
        try
        {
            Records.Clear();
            await foreach ( LogRecord record in Api.GetLogs() ) { Records.Add( record ); }
        }
        catch ( SqlException e ) { e.WriteToConsole(); }
    }
}
