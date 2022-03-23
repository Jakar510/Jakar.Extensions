using Jakar.Extensions.Xamarin.Forms.ResourceManager;


namespace Jakar.Extensions.Xamarin.Forms;


public abstract class ApiServices<TDebug, TPrompts, TAppSettings, TFileSystem, TLanguage, TDeviceID, TViewPage> where TDebug : Debug<TDeviceID, TViewPage>, new()
                                                                                                                where TPrompts : Prompts<TDeviceID, TViewPage>, new()
                                                                                                                where TAppSettings : IAppSettings<TDeviceID, TViewPage>, new()
                                                                                                                where TFileSystem : BaseFileSystemApi, new()
                                                                                                                where TLanguage : LanguageApi, new()
{
    public TDebug                         Debug      { get; } = new();
    public TPrompts                       Prompts    { get; } = new();
    public TAppSettings                   Settings   { get; } = new();
    public TLanguage                      Language   { get; } = new();
    public TFileSystem                    FileSystem { get; } = new();
    public LocationManager                Location   { get; } = new();
    public BarometerReader                Barometer  { get; } = new();
    public Commands<TDeviceID, TViewPage> Loading    { get; }


    /// <summary>
    /// appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes)
    /// </summary>
    /// <param name="app_center_id"></param>
    /// <param name="appCenterServices"></param>
    protected ApiServices( in string app_center_id, params Type[] appCenterServices )
    {
        Prompts.Init(Debug);
        Prompts.Init(Settings);
        Debug.Init(FileSystem, Settings, app_center_id, appCenterServices);

        Loading = new Commands<TDeviceID, TViewPage>(Prompts);
    }
}



public abstract class ApiServices<TDebug, TPrompts, TAppSettings, TFileSystem, TLanguage, TResourceManager, TDeviceID, TViewPage> :
    ApiServices<TDebug, TPrompts, TAppSettings, TFileSystem, TLanguage, TDeviceID, TViewPage> where TDebug : Debug<TDeviceID, TViewPage>, new()
                                                                                              where TPrompts : Prompts<TDeviceID, TViewPage>, new()
                                                                                              where TAppSettings : IAppSettings<TDeviceID, TViewPage>, new()
                                                                                              where TFileSystem : BaseFileSystemApi, new()
                                                                                              where TLanguage : LanguageApi, new()
                                                                                              where TResourceManager : BaseResourceDictionaryManager, new()
{
    public TResourceManager Resources { get; } = new();


    /// <summary>
    /// appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes)
    /// </summary>
    /// <param name="app_center_id"></param>
    /// <param name="appCenterServices"></param>
    protected ApiServices( in string app_center_id, params Type[] appCenterServices ) : base(app_center_id, appCenterServices) { }
}
