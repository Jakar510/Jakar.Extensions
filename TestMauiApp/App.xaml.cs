using Microsoft.Extensions.Logging.Abstractions;
using Serilog;



namespace TestMauiApp;


public sealed partial class App : Application, IDisposable
{
    public static readonly ActivitySource ActivitySource = new(nameof(TestMauiApp));
    /*
    public static readonly Serilogger Logger = Serilogger.Create( new SeriloggerOptions
                                                                  {
                                                                      Paths          = Paths,
                                                                      ActivitySource = ActivitySource,
                                                                      DebugID        = TestMauiApp.AppID,
                                                                      DeviceID       = Guid.NewGuid(),
                                                                      DeviceIDLong   = 0,
                                                                      DeviceName     = "DevTest1",
                                                                      AppID          = Guid.NewGuid(),
                                                                      AppName        = TestMauiApp.AppName,
                                                                      AppVersion     = TestMauiApp.AppVersion,
                                                                  } );
    */

    public new static App       Current => (App)( Application.Current ?? throw new NullReferenceException(nameof(Current)) );
    public static     FilePaths Paths   { get; } = new(FileSystem.AppDataDirectory, FileSystem.CacheDirectory);


    public App() : base()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
    public static ILogger<T> CreateLogger<T>() => Current.Handler?.MauiContext?.Services.GetRequiredService<ILoggerFactory>().CreateLogger<T>() ?? NullLogger<T>.Instance;


    protected override void OnStart()  => base.OnStart();
    protected override void OnSleep()  => base.OnSleep();
    protected override void OnResume() => base.OnResume();


    public void Dispose() => Log.CloseAndFlush();
}
