using Xamarin.CommunityToolkit.ObjectModel;



namespace Jakar.Extensions.Xamarin.Forms;


public abstract class ViewModelBase : BaseViewModel
{
    public AppView View { get; }

    protected ViewModelBase( AppView view ) : base() => View = view;
    protected ViewModelBase( AppView view, string title ) : this( view ) => Title = title;
}



public abstract class ViewModelBase<TView> : ViewModelBase where TView : AppView
{
    public new TView View => base.View as TView ?? throw new ExpectedValueTypeException( base.View, typeof(TView) );

    protected ViewModelBase( TView view ) : base( view ) { }
}



public abstract class ViewModelBase<TView, TValue> : ViewModelBase<TView>, IDisposable where TValue : class
                                                                                       where TView : AppView
{
    private bool    _hasItems;
    private TValue? _selectedItem;


    public bool HasItems
    {
        get => _hasItems;
        set
        {
            if ( SetProperty( ref _hasItems, value ) ) { OnPropertyChanged( nameof(HasNoItems) ); }
        }
    }
    public bool                         HasNoItems       => !HasItems;
    public ICommand                     RefreshCommand   { get; }
    public ICommand                     SaveAsyncCommand { get; }
    public ObservableCollection<TValue> Items            { get; }       = new();
    public ObservableCollection<TValue> SelectedItems    { get; init; } = new();
    public TValue? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if ( SetProperty( ref _selectedItem, value ) ) { OnItemSelected( value ); }
        }
    }


    protected ViewModelBase( TView view, IEnumerable<TValue> values ) : this( view ) => Items.Add( values );
    protected ViewModelBase( TView view, IEnumerable<TValue> values, TValue value ) : this( view, values ) => SelectedItem = value;
    protected ViewModelBase( TView view ) : base( view )
    {
        RefreshCommand          =  new AsyncCommand( RefreshAsync );
        SaveAsyncCommand        =  new AsyncCommand( SaveAsync );
        Items.CollectionChanged += ItemsOnCollectionChanged;
    }


    protected virtual void ItemsOnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e ) => HasItems = Items.Any();


    protected virtual void OnItemSelected( TValue?           value ) { }
    protected virtual ValueTask OnItemSelectedAsync( TValue? value ) => default;


    public override void OnAppearing()
    {
        base.OnAppearing();
        RefreshCommand.Execute( null );
        SelectedItem = null;
    }
    protected Task RefreshAsync() => RefreshAsync( default );
    protected abstract Task RefreshAsync( CancellationToken token );
    protected Task SaveAsync() => SaveAsync( default );
    protected abstract Task SaveAsync( CancellationToken token );


    public virtual void Dispose()
    {
        Items.CollectionChanged -= ItemsOnCollectionChanged;
        GC.SuppressFinalize( this );
    }
}
