using Serilog;



namespace TestMauiApp;


public sealed partial class App : Application, IDisposable
{
    public static     Serilogger Logger  { get; } = Serilogger.Create<TestMauiApp, DebugSettings>( new FilePaths( FileSystem.AppDataDirectory, FileSystem.CacheDirectory ) );
    public new static App        Current => (App)(Application.Current ?? throw new NullReferenceException( nameof(Current) ));


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
