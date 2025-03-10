using Serilog;



namespace TestMauiApp;


public sealed class Serilogger( SeriloggerOptions<TestMauiApp> options ) : Serilogger<Serilogger, SeriloggerSettings, TestMauiApp>( options ), ICreateSerilogger<Serilogger, TestMauiApp>
{
    public static Serilogger Create( SeriloggerOptions<TestMauiApp> options ) => new(options);
}



public sealed partial class App : Application, IDisposable
{
    public static readonly ActivitySource ActivitySource = new(nameof(TestMauiApp));
    public static          FilePaths      Paths { get; } = new(FileSystem.AppDataDirectory, FileSystem.CacheDirectory);

    public static Serilogger Logger { get; } = Serilogger.Create( new SeriloggerConstants
                                                                  {
                                                                      Paths          = Paths,
                                                                      ActivitySource = ActivitySource,
                                                                      DebugID        = TestMauiApp.AppID,
                                                                      DeviceID       = Guid.NewGuid(),
                                                                      DeviceIDLong   = 0,
                                                                      DeviceName     = "DevTest1",
                                                                      AppID          = Guid.NewGuid(),
                                                                      AppName        = TestMauiApp.AppName,
                                                                      AppVersion     = TestMauiApp.AppVersion
                                                                  } );

    public new static App Current => (App)(Application.Current ?? throw new NullReferenceException( nameof(Current) ));


    public App() : base()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }


    protected override void OnStart()  => base.OnStart();
    protected override void OnSleep()  => base.OnSleep();
    protected override void OnResume() => base.OnResume();


    public void Dispose()
    {
        Log.CloseAndFlush();
        Logger.Dispose();
    }
}
