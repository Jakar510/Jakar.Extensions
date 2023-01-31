﻿#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public abstract class ApiServices<TDebug, TPrompts, TAppSettings, TFileSystem, TViewPage> where TDebug : Debug
                                                                                          where TPrompts : Prompts
                                                                                          where TAppSettings : AppSettings<TViewPage>
                                                                                          where TFileSystem : BaseFileSystemApi
                                                                                          where TViewPage : struct, Enum
{
    public BarometerReader Barometer  { get; init; } = new();
    public Commands        Loading    { get; init; }
    public LanguageApi     Language   { get; init; } = new();
    public LocationManager Location   { get; init; } = new();
    public TAppSettings    Settings   { get; init; }
    public TDebug          Debug      { get; init; }
    public TFileSystem     FileSystem { get; init; }
    public TPrompts        Prompts    { get; init; }


    /// <summary> appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes) </summary>
    protected ApiServices( TAppSettings settings, TPrompts prompts, TFileSystem fileSystem, TDebug debug, string app_center_id, params Type[] appCenterServices )
    {
        Settings   = settings ?? throw new ArgumentNullException( nameof(settings) );
        Prompts    = prompts ?? throw new ArgumentNullException( nameof(prompts) );
        FileSystem = fileSystem ?? throw new ArgumentNullException( nameof(fileSystem) );
        Debug      = debug ?? throw new ArgumentNullException( nameof(debug) );
        Loading    = new Commands( Prompts );

        Task.Run( () => Debug.InitAsync( app_center_id, appCenterServices ) );
    }
}



public abstract class ApiServices<TDebug, TPrompts, TAppSettings, TFileSystem, TLanguage, TViewPage> where TDebug : Debug
                                                                                                     where TPrompts : Prompts
                                                                                                     where TAppSettings : AppSettings<TViewPage>
                                                                                                     where TFileSystem : BaseFileSystemApi
                                                                                                     where TLanguage : LanguageApi
                                                                                                     where TViewPage : struct, Enum
{
    public BarometerReader Barometer  { get; init; } = new();
    public Commands        Loading    { get; init; }
    public LocationManager Location   { get; init; } = new();
    public TAppSettings    Settings   { get; init; }
    public TDebug          Debug      { get; init; }
    public TFileSystem     FileSystem { get; init; }
    public TLanguage       Language   { get; init; }
    public TPrompts        Prompts    { get; init; }


    /// <summary> appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes) </summary>
    protected ApiServices( TAppSettings settings, TPrompts prompts, TLanguage language, TFileSystem fileSystem, TDebug debug, string app_center_id, params Type[] appCenterServices )
    {
        Settings   = settings ?? throw new ArgumentNullException( nameof(settings) );
        Prompts    = prompts ?? throw new ArgumentNullException( nameof(prompts) );
        Language   = language ?? throw new ArgumentNullException( nameof(language) );
        FileSystem = fileSystem ?? throw new ArgumentNullException( nameof(fileSystem) );
        Debug      = debug ?? throw new ArgumentNullException( nameof(debug) );
        Loading    = new Commands( Prompts );

        Task.Run( () => Debug.InitAsync( app_center_id, appCenterServices ) );
    }
}



public abstract class ApiServices<TDebug, TPrompts, TAppSettings, TFileSystem, TLanguage, TResourceManager, TViewPage> : ApiServices<TDebug, TPrompts, TAppSettings, TFileSystem, TLanguage, TViewPage> where TDebug : Debug
                                                                                                                                                                                                        where TPrompts : Prompts
                                                                                                                                                                                                        where TAppSettings : AppSettings<TViewPage>
                                                                                                                                                                                                        where TFileSystem : BaseFileSystemApi
                                                                                                                                                                                                        where TLanguage : LanguageApi
                                                                                                                                                                                                        where TResourceManager :
                                                                                                                                                                                                        BaseResourceDictionaryManager
                                                                                                                                                                                                        where TViewPage : struct, Enum
{
    public TResourceManager Resources { get; init; }


    /// <summary> appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes) </summary>
    protected ApiServices( TAppSettings settings, TPrompts prompts, TLanguage language, TFileSystem fileSystem, TDebug debug, TResourceManager resources, in string app_center_id, params Type[] appCenterServices ) :
        base( settings, prompts, language, fileSystem, debug, app_center_id, appCenterServices ) => Resources = resources ?? throw new ArgumentNullException( nameof(resources) );
}
