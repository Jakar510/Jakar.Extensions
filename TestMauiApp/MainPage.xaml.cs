using System.Collections.Specialized;
using Jakar.Extensions;



namespace TestMauiApp;


public partial class MainPage : ContentPage
{
    private int _count;

    private readonly ObservableCollection<string> _collection = [];

    // private readonly System.Collections.ObjectModel.ObservableCollection<string> _collection = [];


    public MainPage()
    {
        InitializeComponent();
        Cv.ItemsSource                =  _collection;
        _collection.CollectionChanged += OnCollectionChanged;
    }
    private void OnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs args )
    {
        // Cv.ScrollTo( _collection.Last(), ScrollToPosition.End );
        args.Action.WriteToDebug();
    }


    private void CounterClicked()
    {
        _count++;

        string value = CounterBtn.Text = _count == 1
                                             ? $"Clicked {_count} time"
                                             : $"Clicked {_count} times";

        if ( _count % 5 == 0 ) { _collection.Clear(); }

        _collection.Add( value );
    }
    private void OnCounterClicked( object sender, EventArgs e )
    {
        try { CounterClicked(); }
        catch ( Exception exception )
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine( exception.ToString() );
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
