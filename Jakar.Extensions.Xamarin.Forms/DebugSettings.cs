// Jakar.Extensions :: Jakar.Extensions.Xamarin.Forms
// 09/08/2022  11:16 AM

namespace Jakar.Extensions.Xamarin.Forms;


public interface IDebugSettings : INotifyPropertyChanged, INotifyPropertyChanging
{
    bool EnableAnalytics        { get; }
    bool EnableApi              { get; }
    bool EnableCrashes          { get; }
    bool IncludeAppStateOnError { get; }
    bool TakeScreenshotOnError  { get; }
}



public sealed class DebugSettings : ObservableClass, IDebugSettings
{
    private bool _enableAnalytics        = Preferences.Get( nameof(EnableAnalytics),        true );
    private bool _enableApi              = Preferences.Get( nameof(EnableApi),              true );
    private bool _enableCrashes          = Preferences.Get( nameof(EnableCrashes),          true );
    private bool _includeAppStateOnError = Preferences.Get( nameof(IncludeAppStateOnError), true );
    private bool _takeScreenshotOnError  = Preferences.Get( nameof(TakeScreenshotOnError),  true );


    public bool EnableAnalytics
    {
        get => _enableAnalytics && EnableApi;
        set
        {
            if ( SetProperty( ref _enableAnalytics, value ) ) { Preferences.Set( nameof(EnableAnalytics), value ); }
        }
    }
    public bool EnableApi
    {
        get => _enableApi;
        set
        {
            if ( SetProperty( ref _enableApi, value ) ) { Preferences.Set( nameof(EnableApi), value ); }
        }
    }
    public bool EnableCrashes
    {
        get => _enableCrashes && EnableApi;
        set
        {
            if ( SetProperty( ref _enableCrashes, value ) ) { Preferences.Set( nameof(EnableCrashes), value ); }
        }
    }
    public bool IncludeAppStateOnError
    {
        get => _includeAppStateOnError && EnableApi;
        set
        {
            if ( SetProperty( ref _includeAppStateOnError, value ) ) { Preferences.Set( nameof(IncludeAppStateOnError), value ); }
        }
    }
    public bool TakeScreenshotOnError
    {
        get => _takeScreenshotOnError && EnableApi;
        set
        {
            if ( SetProperty( ref _takeScreenshotOnError, value ) ) { Preferences.Set( nameof(TakeScreenshotOnError), value ); }
        }
    }


    public DebugSettings() { }
    public DebugSettings( bool enableApi, bool enableCrashes, bool enableAnalytics, bool includeAppStateOnError, bool takeScreenshotOnError )
    {
        _enableApi              = enableApi;
        _enableCrashes          = enableCrashes;
        _enableAnalytics        = enableAnalytics;
        _includeAppStateOnError = includeAppStateOnError;
        _takeScreenshotOnError  = takeScreenshotOnError;
    }
}
