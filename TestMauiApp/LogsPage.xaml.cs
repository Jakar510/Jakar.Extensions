using CommunityToolkit.Mvvm.Input;



namespace TestMauiApp;


public sealed partial class LogsPage : ContentPage
{
    private readonly ILogger                        _logger;
    public           ObservableCollection<FileLogs> Files          { get; } = new(64);
    public           AsyncRelayCommand              RefreshCommand { get; }


    public LogsPage()
    {
        InitializeComponent();
        BindingContext = this;
        RefreshCommand = new AsyncRelayCommand( RefreshAsync );
        _logger        = App.Serilogger.CreateLogger<LogsPage>();
    }
    protected override void OnAppearing()    => RefreshCommand.Execute( null );
    protected override void OnDisappearing() => Files.Clear();


    private async Task RefreshAsync( CancellationToken token )
    {
        try
        {
            IsBusy = true;
            Files.Clear();
            foreach ( LocalFile file in App.Serilogger.LogsDirectory.GetFiles() ) { Files.Add( await FileLogs.Create( file, token ) ); }
        }
        finally { IsBusy = false; }
    }
}



public sealed class FileLogs( string fileName, string[] lines ) : ObservableCollection<string>( lines )
{
    private static readonly char[] _separator = ['\n', '\r'];
    public                  string Name { get; } = fileName;


    public static async ValueTask<FileLogs> Create( LocalFile file, CancellationToken token )
    {
        string content = await file.ReadAsync().AsString( token );
        return new FileLogs( file.Name, content.Split( _separator, StringSplitOptions.RemoveEmptyEntries ) );
    }
}
