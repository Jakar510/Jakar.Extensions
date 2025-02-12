using Serilog;
using Serilog.Core;



namespace TestMauiApp;


public sealed class Serilogger( ActivitySource source, Logger logger, SeriloggerOptions options ) : Serilogger<TestMauiApp>( source, logger, options ), ICreateSerilogger<Serilogger>
{
    public static ILoggerProvider GetProvider( IServiceProvider provider )                                                    => null;
    public static Serilogger      Get( IServiceProvider         provider )                                                    => null;
    public static Serilogger      Create( IServiceProvider      provider )                                                    => null;
    public static Serilogger      Create( SeriloggerOptions     options )                                                     => null;
    public static Serilogger      Create( ActivitySource        source, SeriloggerOptions options )                           => null;
    public static Serilogger      Create( ActivitySource        source, Logger            logger, SeriloggerOptions options ) => new(source, logger, options);
}



public sealed partial class App : Application, IDisposable
{
    public static     FilePaths  Paths   { get; } = new(FileSystem.AppDataDirectory, FileSystem.CacheDirectory);
    public static     Serilogger Logger  { get; } = Serilogger.Create( Paths );
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
