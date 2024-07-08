using Serilog;



namespace TestMauiApp;


public sealed partial class App : Application, IDisposable
{
    public static     Serilogger<TestMauiApp> Serilogger { get; } = new(FileSystem.AppDataDirectory);
    public new static App                     Current    => (App)(Application.Current ?? throw new NullReferenceException( nameof(Current) ));


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
        Serilogger.Dispose();
        GC.SuppressFinalize( this );
    }
}
