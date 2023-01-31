// Jakar.Extensions :: Jakar.Extensions.Xamarin.Forms
// 01/31/2023  3:02 PM

using Microsoft.Extensions.Logging;



namespace Jakar.Extensions.Xamarin.Forms;


public class App<T> : Application where T : App<T>
{
    public new static T              Current => (T)Application.Current;
    public            LoggerWrapper? Logger  { get; init; }


    public App() : base() { }
    public App( ILogger        logger ) : base() => Logger = LoggerWrapper.Create( logger );
    public App( ILoggerFactory factory ) : base() => Logger = LoggerWrapper.Create<T>( factory );
}
