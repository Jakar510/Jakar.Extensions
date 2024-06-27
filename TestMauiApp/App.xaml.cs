using Serilog;
using Serilog.Extensions.Logging;



namespace TestMauiApp;


public sealed partial class App : Application, IDisposable
{
    public new static App             Current       => (App)(Application.Current ?? throw new NullReferenceException( nameof(Current) ));
    public            ILoggerProvider LoggerFactory { get; }


    public App( ILoggerProvider factory ) : base()
    {
        InitializeComponent();
        LoggerFactory = factory;
        MainPage      = new AppShell();
    }


    protected override void OnStart()  => base.OnStart();
    protected override void OnSleep()  => base.OnSleep();
    protected override void OnResume() => base.OnResume();


    public void Dispose()
    {
        Log.CloseAndFlush();
        LoggerFactory.Dispose();
        GC.SuppressFinalize( this );
    }
}
