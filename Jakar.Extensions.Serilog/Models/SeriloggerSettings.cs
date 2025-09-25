// Jakar.Extensions :: Jakar.Extensions
// 01/02/2025  12:01

using Microsoft.Extensions.Logging;
using OneOf;



namespace Jakar.Extensions.Serilog;


public sealed class SeriloggerSettings : SeriloggerSettings<SeriloggerSettings, IHeaderContext>, ICreateSeriloggerSettings<SeriloggerSettings>

{
    public SeriloggerSettings( SeriloggerOptions options, ISeriloggerSettings settings ) : base( options, settings ) { }
    public SeriloggerSettings( SeriloggerOptions options, bool                enabled ) : base( options, enabled ) { }
    public SeriloggerSettings( SeriloggerOptions options, bool                enableApi, bool    enableCrashes, bool    enableAnalytics, bool    includeAppStateOnError, bool    takeScreenshotOnError ) : base( options, enableApi, enableCrashes, enableAnalytics, includeAppStateOnError, takeScreenshotOnError ) { }
    public SeriloggerSettings( SeriloggerOptions options, Setting             enableApi, Setting crashes,       Setting analytics,       Setting appState,               Setting screenshot ) : base( options, enableApi, crashes, analytics, appState, screenshot ) { }


    public static SeriloggerSettings Create( SeriloggerOptions options, ISeriloggerSettings settings )                                                                                                     => new(options, settings);
    public static SeriloggerSettings Create( SeriloggerOptions options, bool                enableApi, bool enableCrashes, bool enableAnalytics, bool includeAppStateOnError, bool takeScreenshotOnError ) => new(options, enableApi, enableCrashes, enableAnalytics, includeAppStateOnError, takeScreenshotOnError);
}



[SuppressMessage( "ReSharper", "StaticMemberInGenericType" )]
public abstract class SeriloggerSettings<TClass, THeaderContext> : BaseClass, ISeriloggerSettings
    where THeaderContext : class, IHeaderContext
    where TClass : SeriloggerSettings<TClass, THeaderContext>, ICreateSeriloggerSettings<TClass>


{
    private bool __isDebuggable = true;


    public static OneOf<Func<ValueTask<bool>>, Func<bool>> CanDebug               { get; set; } = new Func<bool>( ISeriloggerSettings.GetDebuggerIsAttached );
    public static string                                   SharedKey              { get; }      = typeof(TClass).Name;
    public        Setting                                  Analytics              { get; }
    public        Setting                                  AppState               { get; }
    public        Setting                                  Crashes                { get; }
    bool ISeriloggerSettings.                              EnableAnalytics        => Analytics.Value;
    bool ISeriloggerSettings.                              EnableApi              => EnableApi.Value;
    public Setting                                         EnableApi              { get; }
    bool ISeriloggerSettings.                              EnableCrashes          => Crashes.Value;
    public THeaderContext?                                 Header                 { get; set; }
    bool ISeriloggerSettings.                              IncludeAppStateOnError => AppState.Value;
    public bool IsDebuggable
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )] get => __isDebuggable;
        set
        {
            __isDebuggable = value;

            LoggingLevel.MinimumLevel = value
                                            ? LogEventLevel.Verbose
                                            : LogEventLevel.Information;
        }
    }
    public LoggingLevelSwitch LoggingLevel          { get; } = new(LogEventLevel.Verbose);
    public FilePaths          Paths                 => Options.Paths;
    public Setting            Screenshot            { get; }
    bool ISeriloggerSettings. TakeScreenshotOnError => Screenshot.Value;
    public SeriloggerOptions  Options               { get; }


    protected SeriloggerSettings( SeriloggerOptions options, ISeriloggerSettings settings ) : this( options, settings.EnableApi, settings.EnableCrashes, settings.EnableAnalytics, settings.IncludeAppStateOnError, settings.TakeScreenshotOnError ) { }
    protected SeriloggerSettings( SeriloggerOptions options, bool                enabled ) : this( options, enabled, enabled, enabled, enabled, enabled ) { }
    protected SeriloggerSettings( SeriloggerOptions options, bool enableApi, bool enableCrashes, bool enableAnalytics, bool includeAppStateOnError, bool takeScreenshotOnError )
    {
        Crashes    = new Setting( enableCrashes,          GetHint );
        Analytics  = new Setting( enableAnalytics,        GetHint );
        AppState   = new Setting( includeAppStateOnError, GetHint );
        Screenshot = new Setting( takeScreenshotOnError,  GetHint );
        EnableApi  = new Setting( enableApi,              GetApiHint, SetAll );
        Options    = options;
    }
    protected SeriloggerSettings( SeriloggerOptions options, Setting enableApi, Setting crashes, Setting analytics, Setting appState, Setting screenshot )
    {
        Crashes    = crashes;
        Analytics  = analytics;
        AppState   = appState;
        Screenshot = screenshot;
        EnableApi  = enableApi;
        Options    = options;
    }
    public virtual void Dispose()
    {
        Paths.Dispose();
        GC.SuppressFinalize( this );
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
    public TClass                           Clone() => Clone( Options, this );
    public static TClass Clone( SeriloggerOptions options, ISeriloggerSettings settings )
    {
        TClass result = TClass.Create( options, settings );
        Debug.Assert( !ReferenceEquals( settings, result ) );
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
    public void SetPreferences() => ((TClass)this).SetPreferences<TClass>();


    public virtual LogLevel GetLogLevel( bool isDebuggable ) => isDebuggable
                                                                    ? LogLevel.Trace
                                                                    : LogLevel.Information;
    public virtual string GetHint( bool value ) => value
                                                       ? "Always Send"
                                                       : "Never Send";
    public virtual string GetApiHint( bool value ) => value
                                                          ? "Allow Error Reporting"
                                                          : "Deny Error Reporting";



    public sealed class Setting( bool value, Func<bool, string> hinter, Action<bool>? action = null ) : BaseClass
    {
        private readonly Action<bool>?      __action = action;
        private readonly Func<bool, string> __hinter = hinter;
        private          bool               __isTitleBold;
        private          bool               __value       = value;
        private          string             __description = string.Empty;
        private          string             __title       = string.Empty;


        public string Description { get => __description; set => SetProperty( ref __description, value ); }
        public string Hint        => __hinter( __value );
        public bool   IsTitleBold { get => __isTitleBold; set => SetProperty( ref __isTitleBold, value ); }
        public string Title       { get => __title;       set => SetProperty( ref __title,       value ); }
        public bool Value
        {
            get => __value;
            set
            {
                if ( !SetProperty( ref __value, value ) ) { return; }

                OnPropertyChanged( nameof(Hint) );
                __action?.Invoke( value );
            }
        }


        public static implicit operator bool( Setting setting ) => setting.Value;
    }
}
