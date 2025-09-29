using Serilog.Core;



namespace TestMauiApp;


public partial class MainPage : ContentPage, IDisposable
{
    private readonly ObservableCollection<string> __collection = [];
    private          int                          __count;
    private readonly Logger                       __logger;

    // private readonly System.Collections.ObjectModel.ObservableCollection<string> _collection = [];


    public MainPage() : this( App.Current ) { }
    public MainPage( App app )
    {
        InitializeComponent();
        Cv.ItemsSource                =  __collection;
        __collection.CollectionChanged += OnCollectionChanged;
        __logger                       =  App.Logger.CreateLogger<MainPage>();
    }
    public void Dispose()
    {
        __collection.CollectionChanged -= OnCollectionChanged;
        __collection.Dispose();
        GC.SuppressFinalize( this );
    }
    private static void OnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs args ) => args.Action.WriteToDebug(); // Cv.ScrollTo( _collection.Last(), ScrollToPosition.End );


    protected override void OnAppearing()
    {
        base.OnAppearing();
        Activity.Current?.AddEvent().SetBaggage( "UserID", Guid.CreateVersion7().ToString() ).SetBaggage( "SessionID", __count.ToString() ).AddTag( "X", "Y" );
        __logger.Information( "{Event}", nameof(OnAppearing) );
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Activity.Current?.AddEvent();
        __logger.Information( "{Event}", nameof(OnDisappearing) );
    }


    private void CounterClicked()
    {
        Activity.Current?.AddEvent();
        __count++;

        string value = CounterBtn.Text = __count == 1
                                             ? $"Clicked {__count} time"
                                             : $"Clicked {__count} times";

        Activity.Current?.SetCustomProperty( nameof(__count), __count.ToString() );
        __logger.Information( "CounterClicked.{Value}", value );
        if ( __count % 5 == 0 ) { __collection.Clear(); }

        __collection.Add( value );
    }
    private void OnCounterClicked( object sender, EventArgs e )
    {
        try { CounterClicked(); }
        catch ( Exception exception )
        {
            Console.WriteLine();
            Console.WriteLine( exception.ToString() );
            Console.WriteLine();
        }
    }
}
