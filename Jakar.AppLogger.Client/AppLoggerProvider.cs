// using Jakar.Extensions.AppCenter.Api;

namespace Jakar.AppLogger.Client;


public class AppLoggerProvider : Service
{
    private readonly AppLoggerApi _api;


    public AppLoggerProvider( string apiToken ) : this(apiToken, new AppLoggerConfig()) { }
    public AppLoggerProvider( string apiToken, LocalDirectory   directory ) : this(apiToken, new AppLoggerIni(directory)) { }
    public AppLoggerProvider( string apiToken, IAppLoggerConfig config ) => _api = new AppLoggerApi(apiToken, config);


    protected override void Dispose( bool disposing ) => _api.Dispose();
    public override async ValueTask DisposeAsync()
    {
        await _api.DisposeAsync();
        GC.SuppressFinalize(this);
    }


    public Task StartAsync( CancellationToken token ) => _api.StartAsync(token);
    public Task StopAsync( CancellationToken  token ) => _api.StopAsync(token);

    


    public static void Test()
    {
        // Microsoft.AppCenter.Crashes.ErrorReport
        // Microsoft.AppCenter.Crashes.ErrorAttachmentLog
        // Microsoft.AppCenter.Crashes.AndroidErrorDetails
        // Microsoft.AppCenter.Crashes.iOSErrorDetails
        // Microsoft.AppCenter.Crashes.MacOSErrorDetails
        // Microsoft.AppCenter.Crashes.ErrorReportEventArgs
        // Microsoft.AppCenter.Device
        // Microsoft.AppCenter.LogLevel
        // Microsoft.AppCenter.Ingestion.Http.IngestionDecorator
        // Microsoft.AppCenter.Ingestion.Http.HttpNetworkAdapter
        // Microsoft.AppCenter.Ingestion.Models.Device
        // Microsoft.AppCenter.Ingestion.Models.Log
        // Microsoft.AppCenter.Ingestion.Models.LogContainer
        // Microsoft.AppCenter.Ingestion.Models.LogWithProperties
        // Microsoft.AppCenter.Ingestion.Models.StartServiceLog
        // Microsoft.AppCenter.Ingestion.Models.ValidationException
        // Microsoft.AppCenter.Ingestion.Models.Serialization.LogJsonConverter
        // Microsoft.AppCenter.Ingestion.Models.Serialization.LogSerializer
        // Microsoft.AppCenter.Analytics.Analytics
        // Microsoft.AppCenter.Analytics.Ingestion.Models.EventLog
        // Microsoft.AppCenter.Analytics.Ingestion.Models.PageLog
        // Microsoft.AppCenter.Analytics.Ingestion.Models.StartSessionLog
    }
}
