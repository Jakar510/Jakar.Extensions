// Jakar.Extensions :: Jakar.Extensions
// 01/02/2025  12:01

using Microsoft.Extensions.Logging;
using OneOf;



namespace Jakar.Extensions.Serilog;


public sealed class SeriloggerSettings : SeriloggerSettings<SeriloggerSettings, IHeaderContext>, ICreateSeriloggerSettings<SeriloggerSettings>
{
    public SeriloggerSettings( FilePaths               paths, ISeriloggerSettings settings ) : base( paths, settings ) { }
    public SeriloggerSettings( FilePaths               paths, bool                enabled ) : base( paths, enabled ) { }
    public SeriloggerSettings( FilePaths               paths, bool                enableApi, bool    enableCrashes, bool    enableAnalytics, bool    includeAppStateOnError, bool    takeScreenshotOnError ) : base( paths, enableApi, enableCrashes, enableAnalytics, includeAppStateOnError, takeScreenshotOnError ) { }
    public SeriloggerSettings( FilePaths               paths, Setting             enableApi, Setting crashes,       Setting analytics,       Setting appState,               Setting screenshot ) : base( paths, enableApi, crashes, analytics, appState, screenshot ) { }
    public static SeriloggerSettings Create( FilePaths paths, ISeriloggerSettings settings )                                                                                                     => null;
    public static SeriloggerSettings Create( FilePaths paths, bool                enableApi, bool enableCrashes, bool enableAnalytics, bool includeAppStateOnError, bool takeScreenshotOnError ) => null;
}



[SuppressMessage( "ReSharper", "StaticMemberInGenericType" )]
public abstract class SeriloggerSettings<TClass, THeaderContext> : ObservableClass, ISeriloggerSettings
    where THeaderContext : class, IHeaderContext
    where TClass : SeriloggerSettings<TClass, THeaderContext>, ICreateSeriloggerSettings<TClass>

{
    private bool _isDebuggable = true;


    public static  OneOf<Func<ValueTask<bool>>, Func<bool>> CanDebug               { get; set; } = new Func<bool>( ISeriloggerSettings.GetDebuggerIsAttached );
    public static  string                                   SharedKey              { get; }      = typeof(TClass).Name;
    public         Setting                                  Analytics              { get; }
    public         Setting                                  AppState               { get; }
    public         Setting                                  Crashes                { get; }
    public virtual Guid                                     DebugID                { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Serilogger.DebugID; }
    public virtual Guid                                     DeviceID               { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Serilogger.DeviceID; }
    public virtual long                                     DeviceIDLong           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Serilogger.DeviceIDLong; }
    public virtual string                                   DeviceName             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Serilogger.DeviceName; }
    bool ISeriloggerSettings.                               EnableAnalytics        => Analytics.Value;
    bool ISeriloggerSettings.                               EnableApi              => EnableApi.Value;
    public Setting                                          EnableApi              { get; }
    bool ISeriloggerSettings.                               EnableCrashes          => Crashes.Value;
    public THeaderContext?                                  Header                 { get; set; }
    bool ISeriloggerSettings.                               IncludeAppStateOnError => AppState.Value;
    public bool IsDebuggable
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _isDebuggable;
        set
        {
            _isDebuggable = value;

            LoggingLevel.MinimumLevel = value
                                            ? LogEventLevel.Verbose
                                            : LogEventLevel.Information;
        }
    }
    public LoggingLevelSwitch LoggingLevel          { get; } = new(LogEventLevel.Verbose);
    public FilePaths          Paths                 { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public Setting            Screenshot            { get; }
    bool ISeriloggerSettings. TakeScreenshotOnError => Screenshot.Value;


    protected SeriloggerSettings( FilePaths paths, ISeriloggerSettings settings ) : this( paths, settings.EnableApi, settings.EnableCrashes, settings.EnableAnalytics, settings.IncludeAppStateOnError, settings.TakeScreenshotOnError ) { }
    protected SeriloggerSettings( FilePaths paths, bool                enabled ) : this( paths, enabled, enabled, enabled, enabled, enabled ) { }
    protected SeriloggerSettings( FilePaths paths, bool enableApi, bool enableCrashes, bool enableAnalytics, bool includeAppStateOnError, bool takeScreenshotOnError )
    {
        Crashes    = new Setting( enableCrashes,          GetHint );
        Analytics  = new Setting( enableAnalytics,        GetHint );
        AppState   = new Setting( includeAppStateOnError, GetHint );
        Screenshot = new Setting( takeScreenshotOnError,  GetHint );
        EnableApi  = new Setting( enableApi,              GetApiHint, SetAll );
        Paths      = paths;
    }
    protected SeriloggerSettings( FilePaths paths, Setting enableApi, Setting crashes, Setting analytics, Setting appState, Setting screenshot )
    {
        Crashes    = crashes;
        Analytics  = analytics;
        AppState   = appState;
        Screenshot = screenshot;
        EnableApi  = enableApi;
        Paths      = paths;
    }
    public virtual void OnAppearing()
    {
        if ( Header is not null ) { Header.Title = "Telemetry"; }

        Analytics.Description  = "Sends information that can help diagnose errors";
        Analytics.Title        = "Enable analytics";
        EnableApi.Description  = "Enables or disables all error reporting and telemetry";
        EnableApi.Title        = "Send anonymous usage data";
        Crashes.Description    = "Sends error information for debug purposes";
        Crashes.Title          = "Enable crashes";
        AppState.Description   = "When an error occurs includes detailed state information for debug purposes";
        AppState.Title         = "Include app state on error";
        Screenshot.Description = "When an error occurs, send a screenshot for debug purposes";
        Screenshot.Title       = "Take screenshot on error";
    }


    ISeriloggerSettings ISeriloggerSettings.Clone() => Clone();
    public TClass                           Clone() => Clone( Paths, this );
    public static TClass Clone( FilePaths paths, ISeriloggerSettings settings )
    {
        var result = TClass.Create( paths, settings );
        Debug.Assert( ReferenceEquals( settings, result ) is false );
        result.SetPreferences();
        return result;
    }
    public void SetAll( bool value )
    {
        EnableApi.Value  = value;
        Crashes.Value    = value;
        Analytics.Value  = value;
        AppState.Value   = value;
        Screenshot.Value = value;
    }
    public void Disabled()       => SetAll( false );
    public void Enabled()        => SetAll( true );
    public void SetPreferences() => CreateSeriloggerSettings.SetPreferences( (TClass)this );


    public static LogLevel GetLogLevel( bool isDebuggable ) => isDebuggable
                                                                   ? LogLevel.Trace
                                                                   : LogLevel.Information;
    public static string GetHint( bool value ) => value
                                                      ? "Always Send"
                                                      : "Never Send";
    public static string GetApiHint( bool value ) => value
                                                         ? "Allow Error Reporting"
                                                         : "Deny Error Reporting";



    public sealed class Setting( bool value, Func<bool, string> hinter, Action<bool>? action = null ) : ObservableClass
    {
        private readonly Action<bool>?      _action = action;
        private readonly Func<bool, string> _hinter = hinter;
        private          bool               _isTitleBold;
        private          bool               _value       = value;
        private          string             _description = string.Empty;
        private          string             _title       = string.Empty;


        public string Description { get => _description; set => SetProperty( ref _description, value ); }
        public string Hint        => _hinter( _value );
        public bool   IsTitleBold { get => _isTitleBold; set => SetProperty( ref _isTitleBold, value ); }
        public string Title       { get => _title;       set => SetProperty( ref _title,       value ); }
        public bool Value
        {
            get => _value;
            set
            {
                if ( SetProperty( ref _value, value ) is false ) { return; }

                OnPropertyChanged( nameof(Hint) );
                _action?.Invoke( value );
            }
        }


        public static implicit operator bool( Setting setting ) => setting.Value;
    }
}
