namespace TestMauiApp;


public partial class MainPage : ContentPage, IDisposable
{
    private readonly ObservableCollection<string> _collection = [];
    private          int                          _count;

    private readonly Activity          _activity = new(nameof(MainPage));
    private readonly ILogger<MainPage> _logger;

    // private readonly System.Collections.ObjectModel.ObservableCollection<string> _collection = [];


    public MainPage() : this( App.Current ) { }
    public MainPage( App app )
    {
        InitializeComponent();
        Cv.ItemsSource                =  _collection;
        _collection.CollectionChanged += OnCollectionChanged;
        _logger                       =  app.LoggerFactory.CreateLogger<MainPage>();
    }
    public void Dispose()
    {
        _collection.Dispose();

        _activity.Dispose();
        GC.SuppressFinalize( this );
    }
    private static void OnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs args ) => args.Action.WriteToDebug(); // Cv.ScrollTo( _collection.Last(), ScrollToPosition.End );


    protected override void OnAppearing()
    {
        base.OnAppearing();

        _activity.Start();
        _activity.SetBaggage( "UserID",    Guid.NewGuid().ToString() );
        _activity.SetBaggage( "SessionID", "0" );
        _activity.AddTag( "X", "Y" );
        _activity.AddEvent( new ActivityEvent( nameof(OnAppearing), DateTimeOffset.UtcNow ) );
    }
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();

        _activity.AddEvent( new ActivityEvent( nameof(OnDisappearing), DateTimeOffset.UtcNow ) );
        _activity.Stop();
        await Save();
    }
    private async Task Save()
    {
        string json = _activity.ToPrettyJson();
        json.WriteToDebug();
        await Share.RequestAsync( json, nameof(json) );
    }


    private void CounterClicked()
    {
        _activity.AddEvent( new ActivityEvent( nameof(CounterClicked), DateTimeOffset.UtcNow ) );
        _count++;

        string value = CounterBtn.Text = _count == 1
                                             ? $"Clicked {_count} time"
                                             : $"Clicked {_count} times";

        _activity.SetCustomProperty( nameof(_count), _count.ToString() );
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
