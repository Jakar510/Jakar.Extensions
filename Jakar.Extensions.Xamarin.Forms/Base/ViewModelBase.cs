using Microsoft.Extensions.Logging;
using Xamarin.CommunityToolkit.ObjectModel;



namespace Jakar.Extensions.Xamarin.Forms;


public abstract class ViewModelBase : BaseViewModel
{
    public AppView View { get; }


    protected ViewModelBase( AppView view ) : base() => View = view;
    protected ViewModelBase( AppView view, string title ) : this( view ) => Title = title;


    protected virtual ValueTask HandleException( Exception   e ) => default;
}



public abstract class ViewModelBase<TView> : ViewModelBase where TView : AppView
{
    public new TView View => (TView)base.View;

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
    public ObservableCollection<TValue> Items            { get; } = new();
    public ObservableCollection<TValue> SelectedItems    { get; } = new();
    public TValue? SelectedItem
    {
        get => _selectedItem;
        set
        {
            SetProperty( ref _selectedItem, value );
            OnSelectedItem( value );
        }
    }


    protected ViewModelBase( TView view, IEnumerable<TValue> values ) : this( view ) => Items.Add( values );
    protected ViewModelBase( TView view, IEnumerable<TValue> values, TValue              value ) : this( view, values ) => SelectedItem = value;
    protected ViewModelBase( TView view, IEnumerable<TValue> values, IEnumerable<TValue> selectedItems ) : this( view, values ) => SelectedItems.Add( selectedItems );
    protected ViewModelBase( TView view, IEnumerable<TValue> values, TValue              value, IEnumerable<TValue> selectedItems ) : this( view, values, value ) => SelectedItems.Add( selectedItems );
    protected ViewModelBase( TView view ) : base( view )
    {
        RefreshCommand          =  new AsyncCommand( async () => await RefreshAsync() );
        SaveAsyncCommand        =  new AsyncCommand( async () => await SaveAsync() );
        Items.CollectionChanged += ItemsOnCollectionChanged;
    }


    protected virtual void ItemsOnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e ) => HasItems = Items.Any();


    public override void OnAppearing()
    {
        base.OnAppearing();
        RefreshCommand.Execute( null );
        SelectedItem = null;
    }


    private async void OnSelectedItem( TValue? value )
    {
        try
        {
            if ( value is not null )
            {
                if ( SelectedItems.Contains( value ) ) { SelectedItems.Add( value ); }
                else { SelectedItems.Remove( value ); }
            }

            // ReSharper disable once MethodHasAsyncOverload
            OnItemSelected( value );
            await OnItemSelectedAsync( value );
        }
        catch ( Exception e ) { await HandleException( e ); }
    }


    protected virtual void OnItemSelected( TValue?           value ) { }
    protected virtual ValueTask OnItemSelectedAsync( TValue? value ) => default;
    protected ValueTask RefreshAsync() => RefreshAsync( default );
    protected abstract ValueTask RefreshAsync( CancellationToken token );
    protected ValueTask SaveAsync() => SaveAsync( default );
    protected abstract ValueTask SaveAsync( CancellationToken token );


    public virtual void Dispose()
    {
        Items.CollectionChanged -= ItemsOnCollectionChanged;
        GC.SuppressFinalize( this );
    }
}
