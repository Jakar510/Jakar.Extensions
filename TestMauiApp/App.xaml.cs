namespace TestMauiApp;


public partial class App : Application
{
    public new static App            Current       => (App)(Application.Current ?? throw new NullReferenceException( nameof(Current) ));
    public            ILoggerFactory LoggerFactory { get; }


    // public Telemetry<TestMauiApp> Telemetry { get; }
    // public App( Telemetry<TestMauiApp> telemetry ) : base()
    public App( ILoggerFactory factory ) : base()
    {
        InitializeComponent();

        // Telemetry = telemetry;
        LoggerFactory = factory;
        MainPage      = new AppShell();
    }
}
