#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public abstract class BaseViewModel<TPage> : BaseViewModel where TPage : Page
{
    protected abstract IAppSettings _AppSettings { get; }

    public ICommand LaunchWebsiteCommand { get; protected set; }

    public ICommand? AboutCommand  { get; protected set; }
    public ICommand? GoHomeCommand { get; protected set; }

    public    ICommand? RefreshCommand { get; protected set; }
    protected TPage?    _SourcePage    { get; set; }


    protected BaseViewModel() => LaunchWebsiteCommand = new Command<string>( async address => await LaunchWebsite( address )
                                                                                                 .ConfigureAwait( false ) );

    // GoHomeCommand = new Command(async () => await _SourcePage.Navigation.PopToRootAsync(true).ConfigureAwait(false));
    protected BaseViewModel( TPage                 source ) : this() => SetPage( source );
    protected abstract Task LaunchWebsite( object? address = null );


    public void SetPage( TPage source ) => _SourcePage = source ?? throw new ArgumentNullException( nameof(source) );


    protected abstract Task ShareScreenShot();

    protected virtual async Task ShareScreenShot( FileSystemApi api, string shareTitle )
    {
        _AppSettings.ScreenShotAddress = await api.GetScreenShot()
                                                  .ConfigureAwait( false );

        await _AppSettings.ScreenShotAddress.ShareFile( shareTitle, MimeTypeNames.Image.JPEG )
                          .ConfigureAwait( false );
    }

    protected abstract Task StartFeedBack();
}



public abstract class BaseViewModel<TPage, TItem> : BaseViewModel<TPage> where TPage : Page
                                                                         where TItem : class
{
    private TItem?   _selectedItem;
    public  ICommand LoadItemsCommand { get; protected set; }

    public ObservableCollection<TItem> Items { get; set; } = new();

    public TItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            SetProperty( ref _selectedItem, value );
        }
    }

    protected BaseViewModel() => LoadItemsCommand = new Command( async () => await ExecuteLoadItemsCommand()
                                                                                .ConfigureAwait( false ) );
    protected BaseViewModel( TPage source ) : base( source ) => LoadItemsCommand = new Command( async () => await ExecuteLoadItemsCommand()
                                                                                                               .ConfigureAwait( false ) );
    protected abstract Task ExecuteLoadItemsCommand();


    public void LoadItems() => LoadItemsCommand.Execute( null );
    public async Task LoadItemsAsync() => await MainThread.InvokeOnMainThreadAsync( ExecuteLoadItemsCommand )
                                                          .ConfigureAwait( false );
}



public abstract class BaseViewModel<TPage, TItem, TSource> : BaseViewModel<TPage, TItem> where TPage : Page
                                                                                         where TItem : class
                                                                                         where TSource : class
{
    private TSource? _sourceItem;

    public TSource? SourceItem
    {
        get => _sourceItem;
        set
        {
            _sourceItem = value;
            SetProperty( ref _sourceItem, value );
        }
    }

    protected BaseViewModel() { }
    protected BaseViewModel( TPage source ) : base( source ) { }
}
