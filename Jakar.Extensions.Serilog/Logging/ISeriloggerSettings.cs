// Jakar.Extensions :: Jakar.Extensions
// 09/08/2022  11:16 AM


using System.ComponentModel;
using System.Formats.Asn1;
using OneOf;



namespace Jakar.Extensions.Serilog;


public interface ISeriloggerSettings : IDeviceName, INotifyPropertyChanged, INotifyPropertyChanging
{
    public bool               EnableAnalytics        { get; }
    public bool               EnableApi              { get; }
    public bool               EnableCrashes          { get; }
    public bool               IncludeAppStateOnError { get; }
    public LoggingLevelSwitch LoggingLevel           { get; }
    public bool               TakeScreenshotOnError  { get; }
    public Guid               DebugID                { get; }
    public bool               IsDebuggable           { get; set; }
    public FilePaths          Paths                  { get; }


    public        ISeriloggerSettings Clone();
    public        void                OnAppearing();
    public        void                SetPreferences();
    public static bool                GetDebuggerIsAttached()      => Debugger.IsAttached;
    public static ValueTask<bool>     GetDebuggerIsAttachedAsync() => new(GetDebuggerIsAttached());
}



public interface ISeriloggerSettings<out TSeriloggerSettings> : INotifyPropertyChanged, INotifyPropertyChanging
    where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>
{
    public TSeriloggerSettings DebugSettings { get; }
}



public interface ICreateSeriloggerSettings<out TSeriloggerSettings> : ISeriloggerSettings
    where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>
{
    public abstract static string                                   SharedKey { get; }
    public abstract static OneOf<Func<ValueTask<bool>>, Func<bool>> CanDebug  { get; set; }
    public new             TSeriloggerSettings                      Clone();
    public abstract static TSeriloggerSettings                      Create( FilePaths paths, ISeriloggerSettings settings );
    public abstract static TSeriloggerSettings                      Create( FilePaths paths, bool                enableApi, bool enableCrashes, bool enableAnalytics, bool includeAppStateOnError, bool takeScreenshotOnError );
}



public static class CreateSeriloggerSettings
{
    public static void SetPreferences<TSeriloggerSettings>( this TSeriloggerSettings value )
        where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>
    {
        string sharedKey = TSeriloggerSettings.SharedKey;
        sharedKey.SetPreference( nameof(ISeriloggerSettings.IncludeAppStateOnError), value.IncludeAppStateOnError );
        sharedKey.SetPreference( nameof(ISeriloggerSettings.TakeScreenshotOnError),  value.TakeScreenshotOnError );
        sharedKey.SetPreference( nameof(ISeriloggerSettings.EnableAnalytics),        value.EnableAnalytics );
        sharedKey.SetPreference( nameof(ISeriloggerSettings.EnableApi),              value.EnableApi );
        sharedKey.SetPreference( nameof(ISeriloggerSettings.EnableCrashes),          value.EnableCrashes );
    }
    public static TSeriloggerSettings FromPreferences<TSeriloggerSettings>( this FilePaths paths )
        where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>
    {
        string sharedKey              = TSeriloggerSettings.SharedKey;
        bool   enableAnalytics        = sharedKey.GetPreference( nameof(ISeriloggerSettings.EnableAnalytics),        true );
        bool   enableApi              = sharedKey.GetPreference( nameof(ISeriloggerSettings.EnableApi),              true );
        bool   enableCrashes          = sharedKey.GetPreference( nameof(ISeriloggerSettings.EnableCrashes),          true );
        bool   includeAppStateOnError = sharedKey.GetPreference( nameof(ISeriloggerSettings.IncludeAppStateOnError), true );
        bool   takeScreenshotOnError  = sharedKey.GetPreference( nameof(ISeriloggerSettings.TakeScreenshotOnError),  true );

        return TSeriloggerSettings.Create( paths, enableApi, enableCrashes, enableAnalytics, includeAppStateOnError, takeScreenshotOnError );
    }
}
