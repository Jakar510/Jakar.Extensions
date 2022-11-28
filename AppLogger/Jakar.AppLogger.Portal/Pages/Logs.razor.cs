namespace Jakar.AppLogger.Portal.Pages;


public partial class Logs : ControlBase
{
    public ConcurrentObservableCollection<LogRecord> Records { get; } = new();


    protected override async Task OnParametersSetAsync()
    {
        try
        {
            Records.Clear();
            foreach (LogRecord record in await Api.GetLogs()) { Records.Add( record ); }
        }
        catch (SqlException e)
        {
            e.SQL.WriteToDebug();

            e.MatchAll?.WriteToDebug();

            e.Parameters?.ToPrettyJson()
             .WriteToDebug();

            e.WriteToConsole();
        }
    }
}
