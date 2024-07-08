namespace TestMauiApp;


public partial class MainPage : ContentPage, IDisposable
{
    private readonly ObservableCollection<string> _collection = [];
    private          int                          _count;
    private readonly ILogger                      _logger;

    // private readonly System.Collections.ObjectModel.ObservableCollection<string> _collection = [];


    public MainPage() : this( App.Current ) { }
    public MainPage( App app )
    {
        InitializeComponent();
        Cv.ItemsSource                =  _collection;
        _collection.CollectionChanged += OnCollectionChanged;
        _logger                       =  App.Serilogger.CreateLogger<MainPage>();
    }
    public void Dispose()
    {
        _collection.CollectionChanged -= OnCollectionChanged;
        _collection.Dispose();
        GC.SuppressFinalize( this );
    }
    private static void OnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs args ) => args.Action.WriteToDebug(); // Cv.ScrollTo( _collection.Last(), ScrollToPosition.End );


    protected override void OnAppearing()
    {
        base.OnAppearing();
        Activity.Current?.AddEvent().SetBaggage( "UserID", Guid.NewGuid().ToString() ).SetBaggage( "SessionID", _count.ToString() ).AddTag( "X", "Y" );
        _logger.LogInformation( "{Event}", nameof(OnAppearing) );
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Activity.Current?.AddEvent();
        _logger.LogInformation( "{Event}", nameof(OnDisappearing) );
    }


    private void CounterClicked()
    {
        Activity.Current?.AddEvent();
        _count++;

        string value = CounterBtn.Text = _count == 1
                                             ? $"Clicked {_count} time"
                                             : $"Clicked {_count} times";

        Activity.Current?.SetCustomProperty( nameof(_count), _count.ToString() );
        _logger.LogInformation( "CounterClicked.{Value}", value );
        if ( _count % 5 == 0 ) { _collection.Clear(); }

        _collection.Add( value );
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
