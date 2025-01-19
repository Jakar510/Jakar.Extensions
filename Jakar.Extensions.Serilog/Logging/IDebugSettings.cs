// Jakar.Extensions :: Jakar.Extensions
// 09/08/2022  11:16 AM


using System.ComponentModel;



namespace Jakar.Extensions.Serilog;


public interface IDebugSettings : IDeviceName, INotifyPropertyChanged, INotifyPropertyChanging
{
    public bool               EnableAnalytics        { get; }
    public bool               EnableApi              { get; }
    public bool               EnableCrashes          { get; }
    public bool               IncludeAppStateOnError { get; }
    public LoggingLevelSwitch LoggingLevel           => Serilogger.LoggingLevel;
    public bool               TakeScreenshotOnError  { get; }
    public Guid               DebugID                { get; }


    public IDebugSettings Clone();
    public void           OnAppearing();
    public ValueTask      TryUpdateAppTrackingAsync();
    public void           SetPreferences();
}



public interface IDebugSettings<out TDebugSettings>
    where TDebugSettings : class, IDebugSettings
{
    public abstract static TDebugSettings FromPreferences();
}



public interface IDebuggerSettings<out TDebugSettings> : INotifyPropertyChanged, INotifyPropertyChanging
    where TDebugSettings : class, IDebugSettings<TDebugSettings>, IDebugSettings
{
    public TDebugSettings DebugSettings { get; }
}
